using database_comunicator.Models;
using database_comunicator.Models.DTOs;
using database_comunicator.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace database_comunicator.Controllers
{
    [Route("{db_name}/[controller]")]
    [ApiController]
    public class OfferController : ControllerBase
    {
        private readonly IOfferServices _offerServices;
        private readonly IUserServices _userServices;
        private readonly ILogServices _logServices;
        public OfferController(IOfferServices offerServices, IUserServices userServices, ILogServices logServices)
        {
            _logServices = logServices;
            _offerServices = offerServices;
            _userServices = userServices;
        }
        [HttpPost]
        [Route("add")]
        public async Task<IActionResult> AddOffer(AddOffer data)
        {
            var exist = await _userServices.UserExist(data.UserId);
            if (!exist) return NotFound("User not found.");
            var proExist = await _offerServices.DoesPricelistExist(data.OfferName, data.UserId);
            if (proExist) return BadRequest("This pricelist already exist.");
            var offerId = await _offerServices.CreateOffer(data);
            if (offerId == 0) return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            var deactivedId = await _offerServices.GetDeactivatedStatusId();
            bool fileCreated;
            if (data.OfferStatusId != deactivedId)
            {
                if (data.Path.EndsWith("csv"))
                {
                    fileCreated = await _offerServices.CreateCsvFile(offerId, data.UserId, data.Path, data.MaxQty);
                }
                else
                {
                    fileCreated = await _offerServices.CreateXmlFile(offerId, data.UserId, data.Path, data.MaxQty);
                }
            } else
            {
                fileCreated = true;
            }
            var logId = await _logServices.getLogTypeId("Create");
            var desc = $"User with id {data.UserId} has created the pricelist with name {data.OfferName} and id {offerId}.";
            await _logServices.CreateActionLog(desc, data.UserId, logId);
            if (!fileCreated) return Ok("With errors");
            return Ok();
        }
        [HttpGet]
        [Route("get/{userId}")]
        public async Task<IActionResult> GetYoursProformas(int userId, string? search, string? sort, string? status, string? currency, string? type, int? totalL, int? totalG, 
            string? createdL, string? createdG, string? modifiedL, string? modifiedG)
        {
            var exist = await _userServices.UserExist(userId);
            if (!exist) return NotFound("User not found.");
            if (search != null)
            {
                var sResult = await _offerServices.GetPriceLists(userId, search, sort, status: status, currency: currency, type: type, totalL, totalG, 
                    createdL, createdG, modifiedL, modifiedG);
                return Ok(sResult);
            }
            var result = await _offerServices.GetPriceLists(userId, sort: sort, status: status, currency: currency, type: type, totalL, totalG, 
                createdL, createdG, modifiedL, modifiedG);
            return Ok(result);
        }
        [HttpDelete]
        [Route("delete/{offerId}")]
        public async Task<IActionResult> DeleteOffer(int offerId, int userId)
        {
            var userExist = await _userServices.UserExist(userId);
            if (!userExist) return NotFound("User not found.");
            var exist = await _offerServices.DoesPricelistExist(offerId);
            if (!exist) return NotFound("Pricelist not found.");
            var result = await _offerServices.DeletePricelist(offerId);
            if (!result) return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            var logId = await _logServices.getLogTypeId("Delete");
            var desc = $"User with id {userId} has deleted the pricelist with id {offerId}.";
            await _logServices.CreateActionLog(desc, userId, logId);
            return Ok();
        }
        [HttpGet]
        [Route("get/statues")]
        public async Task<IActionResult> GetStatuses()
        {
            var result = await _offerServices.GetOfferStatuses();
            return Ok(result);
        }
        [HttpGet]
        [Route("get/items/{currency}/{userId}")]
        public async Task<IActionResult> GetItems(int userId, string currency)
        {
            var userExist = await _userServices.UserExist(userId);
            if (!userExist) return NotFound("User not found.");
            var result = await _offerServices.GetItemsForOffers(userId, currency);
            return Ok(result);
        }
        [HttpGet]
        [Route("get/items/pricelist/{pricelistId}/user/{userId}")]
        public async Task<IActionResult> GetOfferItems(int pricelistId, int userId)
        {
            var userExist = await _userServices.UserExist(userId);
            if (!userExist) return NotFound("User not found.");
            var exist = await _offerServices.DoesPricelistExist(pricelistId);
            if (!exist) return NotFound("Pricelist not found.");
            var result = await _offerServices.GetOfferItems(pricelistId, userId);
            return Ok(result);
        }
        [HttpGet]
        [Route("get/rest/pricelist/{pricelistId}/user/{userId}")]
        public async Task<IActionResult> GetRestPricelist(int pricelistId, int userId)
        {
            var userExist = await _userServices.UserExist(userId);
            if (!userExist) return NotFound("User not found.");
            var exist = await _offerServices.DoesPricelistExist(pricelistId);
            if (!exist) return NotFound("Pricelist not found.");
            var result = await _offerServices.GetRestModifyOffer(pricelistId, userId);
            return Ok(result);
        }
        [HttpPost]
        [Route("modify")]
        public async Task<IActionResult> ModifyRequest(ModifyPricelist data)
        {
            var exist = await _userServices.UserExist(data.UserId);
            if (!exist) return NotFound("User not found.");
            var offerExist = await _offerServices.DoesPricelistExist(data.OfferId);
            if (!offerExist) return NotFound("Pricelist not found.");
            if (data.OfferName != null)
            {
                var sameExist = await _offerServices.DoesPricelistExist(data.OfferName, data.UserId);
                if (sameExist) return BadRequest("Pricelist with that name already exist.");
            }
            var result = await _offerServices.ModifyOffer(data);
            var deactivatedStatus = await _offerServices.GetDeactivatedStatusId();
            if ((data.OfferStatusId ?? -1) != deactivatedStatus)
            {
                var maxQty = await _offerServices.GetOfferMaxQty(data.OfferId);
                var path = await _offerServices.GetOfferPath(data.OfferId);
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
        [HttpGet]
        [Route("get/status/deactivated")]
        public async Task<IActionResult> GetDeactivatedStatusId()
        {
            var result = await _offerServices.GetDeactivatedStatusId();
            return Ok(result);
        }
        [HttpPost]
        [Route("start/update")]
        public async Task<IActionResult> StartUpdate()
        {
            var csvIds = await _offerServices.GetAllActiveCsvOfferId();
            var xmlIds = await _offerServices.GetAllActiveXmlOfferId();
            var rejectedIds = new List<int>();
            foreach (var csvId in csvIds)
            {
                var isUpdated = await _offerServices.CreateCsvFile(csvId);
                if (!isUpdated) rejectedIds.Add(csvId);
            }
            foreach (var xmlId in xmlIds)
            {
                var isUpdated = await _offerServices.CreateXmlFile(xmlId);
                if (!isUpdated) rejectedIds.Add(xmlId);
            }
            return Ok(rejectedIds);
        }
    }
}
