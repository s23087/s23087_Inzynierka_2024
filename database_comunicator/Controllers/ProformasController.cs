using database_communicator.FilterClass;
using database_communicator.Models.DTOs.Create;
using database_communicator.Models.DTOs.Modify;
using database_communicator.Services;
using Microsoft.AspNetCore.Mvc;

namespace database_communicator.Controllers
{
    /// <summary>
    /// This controller works on Proforma table and few other related tables. Used for receiving, modifying and creating data 
    /// that holds users proforma information. Use db_name parameter to pass the name of database that you want ot connect.
    /// </summary>
    [Route("{db_name}/[controller]")]
    [ApiController]
    public class ProformasController(IProformaServices proformaServices, IUserServices userServices, ILogServices logServices, INotificationServices notificationServices) : ControllerBase
    {
        private const string userNotFoundMessage = "User not found.";
        private const string proformaNotFoundMessage = "Proforma not found.";
        private readonly IProformaServices _proformaServices = proformaServices;
        private readonly IUserServices _userServices = userServices;
        private readonly ILogServices _logServices = logServices;
        private readonly INotificationServices _notificationServices = notificationServices;

        /// <summary>
        /// Add new proforma to database using given data. This action will also create new log entry and notification if other user then bounded to proforma activated this action.
        /// </summary>
        /// <param name="data">New proforma data wrapped in <see cref="Models.DTOs.Create.AddProforma"/> object.</param>
        /// <param name="userId"></param>
        /// <returns>200 when success, 500 when failure, 404 when user do not exist or 400 when this proforma already exist.</returns>
        [HttpPost]
        [Route("add/{userId}")]
        public async Task<IActionResult> AddProforma(AddProforma data, int userId)
        {
            var exist = await _userServices.UserExist(userId);
            if (!exist) return NotFound(userNotFoundMessage);
            var proUser = await _userServices.UserExist(data.UserId);
            if (!proUser) return NotFound("Chosen user not found.");
            var proExist = await _proformaServices.ProformaExist(data.ProformaNumber, data.SellerId, data.BuyerId);
            if (proExist) return BadRequest("This proforma already exist.");
            var proformaId = await _proformaServices.AddProforma(data);
            if (proformaId == 0) return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            var logId = await _logServices.getLogTypeId("Create");
            var logDescription = $"User with id {data.UserId} has created the proforma with number {data.ProformaNumber} and id {proformaId}.";
            await _logServices.CreateActionLog(logDescription, userId, logId);
            if (userId != data.UserId)
            {
                var userFull = await _userServices.GetUserFullName(userId);
                await _notificationServices.CreateNotification(new CreateNotification
                {
                    UserId = data.UserId,
                    Info = $"{userFull} has created a new proforma for you with number {data.ProformaNumber}.",
                    ObjectType = data.IsYourProforma ? "Yours Proformas" : "Clients Proformas",
                    Referance = $"{proformaId}"
                });
            }
            return Ok();
        }
        /// <summary>
        /// Tries to receive basic information about user proformas (user is recipient) from database. Can be filtered using parameters.
        /// </summary>
        /// <param name="userId">User id that data is specially filtered on.</param>
        /// <param name="search">The phrase searched in proforma information. It will check if phrase exist in proforma number.</param>
        /// <param name="sort">Contains parameter that object will be sorted by. Must start with D or A to determine ascending order. Then is follow by name of property.</param>
        /// <param name="qtyL">Filter that search for qty that is lower then given value</param>
        /// <param name="qtyG">Filter that search for qty that is greater then given value</param>
        /// <param name="totalL">Filter that search for total that is lower then given value</param>
        /// <param name="totalG">Filter that search for total that is greater then given value</param>
        /// <param name="dateL">Filter that search for date that is lower then given value</param>
        /// <param name="dateG">Filter that search for date that is greater then given value</param>
        /// <param name="recipient">Filter that search for recipient with given value</param>
        /// <param name="currency">Filter that search for currency with given value</param>
        /// <returns>200 code wit list of <see cref="Models.DTOs.Get.GetProforma"/>, 400 when sort if given sort is incorrect or 404 code when user is not found.</returns>
        [HttpGet]
        [Route("get/yours/{userId}")]
        public async Task<IActionResult> GetYoursProformas(int userId, string? search, string? sort,
            int? qtyL, int? qtyG, int? totalL, int? totalG, string? dateL, string? dateG, int? recipient, string? currency)
        {
            if (sort != null)
            {
                bool isSortOk = sort.StartsWith('A') || sort.StartsWith('D');
                if (!isSortOk) return BadRequest("Sort value is incorrect.");
            }
            var exist = await _userServices.UserExist(userId);
            if (!exist) return NotFound(userNotFoundMessage);
            var filters = new ProformaFiltersTemplate
            {
                DateL = dateL,
                DateG = dateG,
                Recipient = recipient,
                Currency = currency,
                QtyL = qtyL,
                QtyG = qtyG,
                TotalL = totalL,
                TotalG = totalG,
            };
            if (search != null)
            {
                var sResult = await _proformaServices.GetProformas(true, userId, search, sort: sort, filters);
                return Ok(sResult);
            }
            var result = await _proformaServices.GetProformas(true, userId, sort: sort, filters);
            return Ok(result);
        }
        /// <summary>
        /// Tries to receive basic information about proformas where organization users are recipients from database. Can be filtered using parameters.
        /// </summary>
        /// <param name="search">The phrase searched in proforma information. It will check if phrase exist in proforma number.</param>
        /// <param name="sort">Contains parameter that object will be sorted by. Must start with D or A to determine ascending order. Then is follow by name of property.</param>
        /// <param name="qtyL">Filter that search for qty that is lower then given value</param>
        /// <param name="qtyG">Filter that search for qty that is greater then given value</param>
        /// <param name="totalL">Filter that search for total that is lower then given value</param>
        /// <param name="totalG">Filter that search for total that is greater then given value</param>
        /// <param name="dateL">Filter that search for date that is lower then given value</param>
        /// <param name="dateG">Filter that search for date that is greater then given value</param>
        /// <param name="recipient">Filter that search for recipient with given value</param>
        /// <param name="currency">Filter that search for currency with given value</param>
        /// <returns>200 code wit list of <see cref="Models.DTOs.Get.GetProforma"/> or 400 when sort if given sort is incorrect.</returns>
        [HttpGet]
        [Route("get/yours")]
        public async Task<IActionResult> GetYoursProformas(string? search, string? sort,
            int? qtyL, int? qtyG, int? totalL, int? totalG, string? dateL, string? dateG, int? recipient, string? currency)
        {
            if (sort != null)
            {
                bool isSortOk = sort.StartsWith('A') || sort.StartsWith('D');
                if (!isSortOk) return BadRequest("Sort value is incorrect.");
            }
            var filters = new ProformaFiltersTemplate
            {
                QtyL = qtyL,
                QtyG = qtyG,
                TotalL = totalL,
                TotalG = totalG,
                DateL = dateL,
                DateG = dateG,
                Recipient = recipient,
                Currency = currency
            };
            if (search != null)
            {
                var sResult = await _proformaServices.GetProformas(true, search, sort: sort, filters);
                return Ok(sResult);
            }
            var result = await _proformaServices.GetProformas(true, sort: sort, filters);
            return Ok(result);
        }
        /// <summary>
        /// Tries to receive basic information about user proformas where clients are recipients from database. Can be filtered using parameters.
        /// </summary>
        /// <param name="userId">User id that data is specially filtered on.</param>
        /// <param name="search">The phrase searched in proforma information. It will check if phrase exist in proforma number.</param>
        /// <param name="sort">Contains parameter that object will be sorted by. Must start with D or A to determine ascending order. Then is follow by name of property.</param>
        /// <param name="qtyL">Filter that search for qty that is lower then given value</param>
        /// <param name="qtyG">Filter that search for qty that is greater then given value</param>
        /// <param name="totalL">Filter that search for total that is lower then given value</param>
        /// <param name="totalG">Filter that search for total that is greater then given value</param>
        /// <param name="dateL">Filter that search for date that is lower then given value</param>
        /// <param name="dateG">Filter that search for date that is greater then given value</param>
        /// <param name="recipient">Filter that search for recipient with given value</param>
        /// <param name="currency">Filter that search for currency with given value</param>
        /// <returns>200 code wit list of <see cref="Models.DTOs.Get.GetProforma"/>, 400 when sort if given sort is incorrect or 404 code when user is not found.</returns>
        [HttpGet]
        [Route("get/clients/{userId}")]
        public async Task<IActionResult> GetClientProformas(int userId, string? search, string? sort,
            int? qtyL, int? qtyG, int? totalL, int? totalG, string? dateL, string? dateG, int? recipient, string? currency)
        {
            if (sort != null)
            {
                bool isSortOk = sort.StartsWith('A') || sort.StartsWith('D');
                if (!isSortOk) return BadRequest("Sort value is incorrect.");
            }
            var filters = new ProformaFiltersTemplate
            {
                QtyL = qtyL,
                QtyG = qtyG,
                TotalL = totalL,
                DateG = dateG,
                Recipient = recipient,
                Currency = currency,
                TotalG = totalG,
                DateL = dateL,
            };
            var exist = await _userServices.UserExist(userId);
            if (!exist) return NotFound(userNotFoundMessage);
            if (search != null)
            {
                var sResult = await _proformaServices.GetProformas(false, userId, search, sort: sort, filters);
                return Ok(sResult);
            }
            var result = await _proformaServices.GetProformas(false, userId, sort: sort, filters);
            return Ok(result);
        }
        /// <summary>
        /// Tries to receive basic information about proformas where clients are recipients from database. Can be filtered using parameters.
        /// </summary>
        /// <param name="search">The phrase searched in proforma information. It will check if phrase exist in proforma number.</param>
        /// <param name="sort">Contains parameter that object will be sorted by. Must start with D or A to determine ascending order. Then is follow by name of property.</param>
        /// <param name="qtyL">Filter that search for qty that is lower then given value</param>
        /// <param name="qtyG">Filter that search for qty that is greater then given value</param>
        /// <param name="totalL">Filter that search for total that is lower then given value</param>
        /// <param name="totalG">Filter that search for total that is greater then given value</param>
        /// <param name="dateL">Filter that search for date that is lower then given value</param>
        /// <param name="dateG">Filter that search for date that is greater then given value</param>
        /// <param name="recipient">Filter that search for recipient with given value</param>
        /// <param name="currency">Filter that search for currency with given value</param>
        /// <returns>200 code wit list of <see cref="Models.DTOs.Get.GetProforma"/> or 400 when sort if given sort is incorrect.</returns>
        [HttpGet]
        [Route("get/clients")]
        public async Task<IActionResult> GetClientProformas(string? search, string? sort,
            int? qtyL, int? qtyG, int? totalL, int? totalG, string? dateL, string? dateG, int? recipient, string? currency)
        {
            if (sort != null)
            {
                bool isSortOk = sort.StartsWith('A') || sort.StartsWith('D');
                if (!isSortOk) return BadRequest("Sort value is incorrect.");
            }
            var filters = new ProformaFiltersTemplate
            {
                QtyL = qtyL,
                QtyG = qtyG,
                TotalL = totalL,
                TotalG = totalG,
                DateL = dateL,
                DateG = dateG,
                Recipient = recipient,
                Currency = currency
            };
            if (search != null)
            {
                var sResult = await _proformaServices.GetProformas(false, search, sort: sort, filters);
                return Ok(sResult);
            }
            var result = await _proformaServices.GetProformas(false, sort: sort, filters);
            return Ok(result);
        }
        /// <summary>
        /// Tries to receive information that was not passed as basic information in <see cref="GetClientProformas"/> 
        /// function and are needed to showcase object for user.
        /// </summary>
        /// <param name="proformaId">Id of proforma that will be shown</param>
        /// <returns>200 code with list of <see cref="Models.DTOs.Get.GetRestProforma"/> or 404 when proforma do not exist.</returns>
        [HttpGet]
        [Route("get/clients/rest/{proformaId}")]
        public async Task<IActionResult> GetRestClientProforma(int proformaId)
        {
            var exist = await _proformaServices.ProformaExist(proformaId);
            if (!exist) return NotFound(proformaNotFoundMessage);
            var result = await _proformaServices.GetRestProforma(false, proformaId);
            return Ok(result);
        }
        /// <summary>
        /// Tries to receive information that was not passed as basic information in <see cref="GetYoursProformas"/> 
        /// function and are needed to showcase object for user.
        /// </summary>
        /// <param name="proformaId">Id of proforma that will be shown</param>
        /// <returns>200 code with list of <see cref="Models.DTOs.Get.GetRestProforma"/> or 404 when proforma do not exist.</returns>
        [HttpGet]
        [Route("get/yours/rest/{proformaId}")]
        public async Task<IActionResult> GetRestYourProforma(int proformaId)
        {
            var exist = await _proformaServices.ProformaExist(proformaId);
            if (!exist) return NotFound(proformaNotFoundMessage);
            var result = await _proformaServices.GetRestProforma(true, proformaId);
            return Ok(result);
        }
        /// <summary>
        /// Delete chosen proforma where organization users are recipient. This action will also create new log entry and notification if user is not the owner of proforma.
        /// </summary>
        /// <param name="proformaId">Id of proforma to delete</param>
        /// <param name="userId">Id of user that's activating this action.</param>
        /// <returns>200 code when success, 500 when failure, 404 when user or proforma do not exist or 400 when There is existing delivery for this proforma.</returns>
        [HttpDelete]
        [Route("delete/yours/{proformaId}/user/{userId}")]
        public async Task<IActionResult> DeleteYourProforma(int proformaId, int userId)
        {
            var userExist = await _userServices.UserExist(userId);
            if (!userExist) return NotFound(userNotFoundMessage);
            var exist = await _proformaServices.ProformaExist(proformaId);
            if (!exist) return NotFound(proformaNotFoundMessage);
            var deliveryExist = await _proformaServices.DoesDeliveryExist(proformaId);
            if (deliveryExist) return BadRequest("There is existing delivery for this proforma.");
            var owner = await _proformaServices.GetProformaUserId(proformaId);
            var proformaNumber = await _proformaServices.GetProformaNumber(proformaId);
            var result = await _proformaServices.DeleteProforma(true, proformaId);
            if (!result) return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            var logId = await _logServices.getLogTypeId("Delete");
            var logDescription = $"User with id {userId} has deleted the proforma with number {proformaNumber}.";
            await _logServices.CreateActionLog(logDescription, userId, logId);
            if (userId != owner)
            {
                var userFull = await _userServices.GetUserFullName(userId);
                await _notificationServices.CreateNotification(new CreateNotification
                {
                    UserId = owner,
                    Info = $"{userFull} has deleted a proforma with id {proformaId}.",
                    ObjectType = "Yours Proformas",
                    Referance = $"{proformaId}"
                });
            }
            return Ok();
        }
        /// <summary>
        /// Delete chosen proforma where organization clients are recipient. This action will also create new log entry and notification if user is not the owner of proforma.
        /// </summary>
        /// <param name="proformaId">Id of proforma to delete</param>
        /// <param name="userId">Id of user that's activating this action.</param>
        /// <returns>200 code when success, 500 when failure, 404 when user or proforma do not exist or 400 when There is existing delivery for this proforma.</returns>
        [HttpDelete]
        [Route("delete/clients/{proformaId}/user/{userId}")]
        public async Task<IActionResult> DeleteClientProforma(int proformaId, int userId)
        {
            var userExist = await _userServices.UserExist(userId);
            if (!userExist) return NotFound(userNotFoundMessage);
            var exist = await _proformaServices.ProformaExist(proformaId);
            if (!exist) return NotFound(proformaNotFoundMessage);
            var deliveryExist = await _proformaServices.DoesDeliveryExist(proformaId);
            if (deliveryExist) return BadRequest("There is existing delivery for this proforma.");
            var owner = await _proformaServices.GetProformaUserId(proformaId);
            var proformaNumber = await _proformaServices.GetProformaNumber(proformaId);
            var result = await _proformaServices.DeleteProforma(false, proformaId);
            if (!result) return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            var logId = await _logServices.getLogTypeId("Delete");
            var logDescription = $"User with id {userId} has deleted the proforma with number {proformaNumber}.";
            await _logServices.CreateActionLog(logDescription, userId, logId);
            if (userId != owner)
            {
                var userFull = await _userServices.GetUserFullName(userId);
                await _notificationServices.CreateNotification(new CreateNotification
                {
                    UserId = owner,
                    Info = $"{userFull} has deleted a proforma with id {proformaId}.",
                    ObjectType = "Clients Proformas",
                    Referance = $"{proformaId}"
                });
            }
            return Ok();
        }
        /// <summary>
        /// Trie to receive proforma path string from database.
        /// </summary>
        /// <param name="proformaId"></param>
        /// <returns>200 code with string or 404 when proforma is not found.</returns>
        [HttpGet]
        [Route("get/path/{proformaId}")]
        public async Task<IActionResult> GetProformaPath(int proformaId)
        {
            var exist = await _proformaServices.ProformaExist(proformaId);
            if (!exist) return NotFound(proformaNotFoundMessage);
            var result = await _proformaServices.GetProformaPath(proformaId) ?? "";
            return Ok(result);
        }
        /// <summary>
        /// Tries to receive information that was not passed as basic information in <see cref="GetYoursProformas"/> or <see cref="GetClientProformas"/>
        /// function and are needed to modify object.
        /// </summary>
        /// <param name="proformaId">Id of proforma that will be modified</param>
        /// <returns>200 code with list of <see cref="Models.DTOs.Get.GetRestModifyProforma"/> or 404 when proforma do not exist.</returns>
        [HttpGet]
        [Route("get/rest/modify/{proformaId}")]
        public async Task<IActionResult> GetRestModifyProforma(int proformaId)
        {
            var exist = await _proformaServices.ProformaExist(proformaId);
            if (!exist) return NotFound(proformaNotFoundMessage);
            var result = await _proformaServices.GetRestModifyProforma(proformaId);
            return Ok(result);
        }
        /// <summary>
        /// Overwrite chosen proforma with new given data. This action will also create new log entry and notification. Notification will not be create if user is the owner of proforma.
        /// </summary>
        /// <param name="data">New proforma data wrapped in <see cref="Models.DTOs.Modify.ModifyProforma"/> object.</param>
        /// <param name="userId">Id of user that's activating this action.</param>
        /// <returns>200 code when success, 500 when failure or 404 when user not found.</returns>
        [HttpPost]
        [Route("modify/{userId}")]
        public async Task<IActionResult> ModifyProforma(ModifyProforma data, int userId)
        {
            var exist = await _userServices.UserExist(userId);
            if (!exist) return NotFound(userNotFoundMessage);
            if (data.UserId != null)
            {
                var recExist = await _userServices.UserExist((int)data.UserId);
                if (!recExist) return NotFound("Chosen user not found.");
            }
            var proformaOldNumber = await _proformaServices.GetProformaNumber(data.ProformaId);
            var result = await _proformaServices.ModifyProforma(data);
            if (!result) return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            var logId = await _logServices.getLogTypeId("Modify");
            var logDescription = $"User with id {userId} has modified the proforma with id {data.ProformaId}.";
            await _logServices.CreateActionLog(logDescription, userId, logId);
            if (userId != data.UserId && data.UserId != null)
            {
                var userFull = await _userServices.GetUserFullName(userId);
                await _notificationServices.CreateNotification(new CreateNotification
                {
                    UserId = (int)data.UserId,
                    Info = $"{userFull} has modified a proforma with number {data.ProformaNumber ?? proformaOldNumber}.",
                    ObjectType = "Requests",
                    Referance = $"{data.ProformaId}"
                });
            }
            return Ok();
        }
    }
}
