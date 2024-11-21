using database_communicator.FilterClass;
using database_communicator.Models.DTOs.Create;
using database_communicator.Models.DTOs.Modify;
using database_communicator.Services;
using Microsoft.AspNetCore.Mvc;

namespace database_communicator.Controllers
{
    /// <summary>
    /// This controller works on Offer table, Offer items table and few other related tables. Used for receiving, modifying and creating data 
    /// that holds users pricelist information. Use db_name parameter to pass the name of database that you want ot connect.
    /// </summary>
    [Route("{db_name}/[controller]")]
    [ApiController]
    public class OfferController : ControllerBase
    {
        private const string userNotFoundMessage = "User not found.";
        private const string pricelistNotFoundMessage = "Pricelist not found.";
        private readonly IOfferServices _offerServices;
        private readonly IUserServices _userServices;
        private readonly ILogServices _logServices;
        public OfferController(IOfferServices offerServices, IUserServices userServices, ILogServices logServices)
        {
            _logServices = logServices;
            _offerServices = offerServices;
            _userServices = userServices;
        }
        /// <summary>
        /// Create new pricelist in database. The action will also create new log entry.
        /// </summary>
        /// <param name="data">Object containing new pricelist information</param>
        /// <returns>200 code when success, 500 code when failure, 404 when user is not found or 400 when pricelist name and user match the already existing pricelist.</returns>
        [HttpPost]
        [Route("add")]
        public async Task<IActionResult> AddOffer(AddOffer data)
        {
            var exist = await _userServices.UserExist(data.UserId);
            if (!exist) return NotFound(userNotFoundMessage);
            var proExist = await _offerServices.DoesPricelistExist(data.OfferName, data.UserId);
            if (proExist) return BadRequest("This pricelist already exist.");
            var offerId = await _offerServices.CreatePricelist(data);
            if (offerId == 0) return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            var deactivateId = await _offerServices.GetDeactivatedStatusId();
            bool fileCreated;
            if (data.OfferStatusId != deactivateId)
            {
                if (data.Path.EndsWith("csv"))
                {
                    fileCreated = await _offerServices.CreateCsvFile(offerId, data.UserId, data.Path, data.MaxQty);
                }
                else
                {
                    fileCreated = await _offerServices.CreateXmlFile(offerId, data.UserId, data.Path, data.MaxQty);
                }
            }
            else
            {
                fileCreated = true;
            }
            var logId = await _logServices.getLogTypeId("Create");
            var desc = $"User with id {data.UserId} has created the pricelist with name {data.OfferName} and id {offerId}.";
            await _logServices.CreateActionLog(desc, data.UserId, logId);
            if (!fileCreated) return Ok("With errors");
            return Ok();
        }
        /// <summary>
        /// Tries to receive basic information about clients information.
        /// </summary>
        /// <param name="userId">User id that data is specially filtered on.</param>
        /// <param name="search">The phrase searched in pricelist information. It will check if phrase exist in pricelist name.</param>
        /// <param name="sort">Contains parameter that object will be sorted by. Must start with D or A to determine ascending order. Then is follow by name of property.</param>
        /// <param name="status">Filter that search for status with given value</param>
        /// <param name="currency">Filter that search for currency with given value</param>
        /// <param name="type">Filter that search for type with given value</param>
        /// <param name="totalL">Filter that search for total that is lower then given value</param>
        /// <param name="totalG">Filter that search for total that is greater then given value</param>
        /// <param name="createdL">Filter that search for created date that is lower then given value</param>
        /// <param name="createdG">Filter that search for created date that is greater then given value</param>
        /// <param name="modifiedL">Filter that search for modified date that is lower then given value</param>
        /// <param name="modifiedG">Filter that search for modified date that is greater then given value</param>
        /// <returns>200 code with list of <see cref="Models.DTOs.Get.GetPriceList"/>, 400 when sort if given sort is incorrect or 404 code when user is not found.</returns>
        [HttpGet]
        [Route("get/{userId}")]
        public async Task<IActionResult> GetPricelist(int userId, string? search, string? sort, string? status, string? currency, string? type, int? totalL, int? totalG,
            string? createdL, string? createdG, string? modifiedL, string? modifiedG)
        {
            if (sort != null)
            {
                bool isSortOk = sort.StartsWith('A') || sort.StartsWith('D');
                if (!isSortOk) return BadRequest("Sort value is incorrect.");
            }
            var filters = new OfferFiltersTemplate
            {
                Status = status,
                Currency = currency,
                Type = type,
                TotalL = totalL,
                TotalG = totalG,
                CreatedL = createdL,
                CreatedG = createdG,
                ModifiedL = modifiedL,
                ModifiedG = modifiedG
            };
            var exist = await _userServices.UserExist(userId);
            if (!exist) return NotFound(userNotFoundMessage);
            if (search != null)
            {
                var sResult = await _offerServices.GetPriceLists(userId, search, sort: sort, filters);
                return Ok(sResult);
            }
            var result = await _offerServices.GetPriceLists(userId, sort: sort, filters);
            return Ok(result);
        }
        /// <summary>
        /// Delete pricelist from database. The action will also create new log entry.
        /// </summary>
        /// <param name="offerId">Id of pricelist to delete</param>
        /// <param name="userId">Id of user that's activating this action.</param>
        /// <returns>200 code when success, 500 when failure or 404 when user or pricelist is not found.</returns>
        [HttpDelete]
        [Route("delete/{offerId}/user/{userId}")]
        public async Task<IActionResult> DeleteOffer(int offerId, int userId)
        {
            var userExist = await _userServices.UserExist(userId);
            if (!userExist) return NotFound(userNotFoundMessage);
            var exist = await _offerServices.DoesPricelistExist(offerId);
            if (!exist) return NotFound(pricelistNotFoundMessage);
            var result = await _offerServices.DeletePricelist(offerId);
            if (!result) return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            var logId = await _logServices.getLogTypeId("Delete");
            var desc = $"User with id {userId} has deleted the pricelist with id {offerId}.";
            await _logServices.CreateActionLog(desc, userId, logId);
            return Ok();
        }
        /// <summary>
        /// Tries to receive pricelist statuses from database.
        /// </summary>
        /// <returns>200 code with list of <see cref="database_communicator.Models.DTOs.Get.GetOfferStatus"/></returns>
        [HttpGet]
        [Route("get/statues")]
        public async Task<IActionResult> GetStatuses()
        {
            var result = await _offerServices.GetPricelistStatuses();
            return Ok(result);
        }
        /// <summary>
        /// Tries to receive items that available to add to a pricelist from database.
        /// </summary>
        /// <param name="userId">Id of user that item are for.</param>
        /// <param name="currency">Shortcut name of currency that items price will be shown</param>
        /// <returns>200 code with list of <see cref="Models.DTOs.Get.GetItemsForOffer"/></returns>
        [HttpGet]
        [Route("get/items/{currency}/{userId}")]
        public async Task<IActionResult> GetItems(int userId, string currency)
        {
            var userExist = await _userServices.UserExist(userId);
            if (!userExist) return NotFound(userNotFoundMessage);
            var result = await _offerServices.GetItemsForPricelist(userId, currency);
            return Ok(result);
        }
        /// <summary>
        /// Tries to receive pricelist items from database.
        /// </summary>
        /// <param name="pricelistId">Pricelist id that items will be received</param>
        /// <param name="userId">Id of user that owns the pricelist.</param>
        /// <returns>200 code with a list of <see cref="Models.DTOs.Get.GetCredtItemForTable"/> or 404 whe user or pricelist is not found.</returns>
        [HttpGet]
        [Route("get/items/{pricelistId}/user/{userId}")]
        public async Task<IActionResult> GetOfferItems(int pricelistId, int userId)
        {
            var userExist = await _userServices.UserExist(userId);
            if (!userExist) return NotFound(userNotFoundMessage);
            var exist = await _offerServices.DoesPricelistExist(pricelistId);
            if (!exist) return NotFound(pricelistNotFoundMessage);
            var result = await _offerServices.GetPricelistItems(pricelistId, userId);
            return Ok(result);
        }
        /// <summary>
        /// Tries to receive information that was not passed as basic information in <see cref="GetPricelist"/> 
        /// function and are needed to showcase object for user.
        /// </summary>
        /// <param name="pricelistId">Id of pricelist that will be showcased.</param>
        /// <param name="userId">Id of user that's activating this action.</param>
        /// <returns>200 code ith list of <see cref="Models.DTOs.Get.GetRestModifyOffer"/> or 404 when pricelist or user is not found.</returns>
        [HttpGet]
        [Route("get/rest/{pricelistId}/user/{userId}")]
        public async Task<IActionResult> GetRestPricelist(int pricelistId, int userId)
        {
            var userExist = await _userServices.UserExist(userId);
            if (!userExist) return NotFound(userNotFoundMessage);
            var exist = await _offerServices.DoesPricelistExist(pricelistId);
            if (!exist) return NotFound(pricelistNotFoundMessage);
            var result = await _offerServices.GetRestModifyPricelist(pricelistId, userId);
            return Ok(result);
        }
        /// <summary>
        /// Overwrite chosen pricelist properties that are given in data variable. This action will also create new log entry.
        /// </summary>
        /// <param name="data">New pricelist data wrapped in <see cref="Models.DTOs.Modify.ModifyPricelist"/> object.</param>
        /// <returns>200 code when success, 500 when failure, 404 when user or pricelist not found or 400 when pricelist with given new name and user id already exist.</returns>
        [HttpPost]
        [Route("modify")]
        public async Task<IActionResult> ModifyPricelist(ModifyPricelist data)
        {
            var exist = await _userServices.UserExist(data.UserId);
            if (!exist) return NotFound(userNotFoundMessage);
            var offerExist = await _offerServices.DoesPricelistExist(data.OfferId);
            if (!offerExist) return NotFound(pricelistNotFoundMessage);
            if (data.OfferName != null)
            {
                var sameExist = await _offerServices.DoesPricelistExist(data.OfferName, data.UserId);
                if (sameExist) return BadRequest("Pricelist with that name already exist.");
            }
            var result = await _offerServices.ModifyPricelist(data);
            var deactivatedStatus = await _offerServices.GetDeactivatedStatusId();
            if ((data.OfferStatusId ?? -1) != deactivatedStatus)
            {
                var maxQty = await _offerServices.GetOfferMaxQty(data.OfferId);
                var path = await _offerServices.GetPricelistPath(data.OfferId);
                if (path.EndsWith("csv"))
                {
                    await _offerServices.CreateCsvFile(data.OfferId, data.UserId, path, maxQty);
                }
                else
                {
                    await _offerServices.CreateXmlFile(data.OfferId, data.UserId, path, maxQty);
                }
            }
            if (!result) return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            var logId = await _logServices.getLogTypeId("Modify");
            var desc = $"User with id {data.UserId} has modified the offer with id {data.OfferId}.";
            await _logServices.CreateActionLog(desc, data.UserId, logId);
            return Ok();
        }
        /// <summary>
        /// Tries to receive deactivated from database. 
        /// </summary>
        /// <returns>200 code with status id.</returns>
        [HttpGet]
        [Route("get/status/deactivated")]
        public async Task<IActionResult> GetDeactivatedStatusId()
        {
            var result = await _offerServices.GetDeactivatedStatusId();
            return Ok(result);
        }
        /// <summary>
        /// Update all pricelist files with item qty.
        /// </summary>
        /// <returns>200 code with list of pricelist ids that was not updated.</returns>
        [HttpPost]
        [Route("start/update")]
        public async Task<IActionResult> StartUpdate()
        {
            var csvIds = await _offerServices.GetAllActiveCsvOfferId();
            var xmlIds = await _offerServices.GetAllActiveXmlOfferId();
            var rejectedIds = new List<int>();
            foreach (var csvId in csvIds)
            {
                var isUpdated = await _offerServices.UpdateCsvFile(csvId);
                if (!isUpdated) rejectedIds.Add(csvId);
            }
            foreach (var xmlId in xmlIds)
            {
                var isUpdated = await _offerServices.UpdateXmlFile(xmlId);
                if (!isUpdated) rejectedIds.Add(xmlId);
            }
            return Ok(rejectedIds);
        }
    }
}
