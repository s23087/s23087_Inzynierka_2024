using database_communicator.FilterClass;
using database_communicator.Models.DTOs.Create;
using database_communicator.Models.DTOs.Modify;
using database_communicator.Services;
using Microsoft.AspNetCore.Mvc;

namespace database_communicator.Controllers
{
    /// <summary>
    /// This controller works on Delivery table and few other related tables. Used for receiving, modifying and creating data 
    /// that holds delivery information. Use db_name parameter to pass the name of database that you want ot connect.
    /// </summary>
    [Route("{db_name}/[controller]")]
    [ApiController]
    public class DeliveryController(IDeliveryServices deliveryServices, IUserServices userServices, ILogServices logServices, INotificationServices notificationServices) : ControllerBase
    {
        private const string userNotFoundMessage = "User not found.";
        private const string sortErrorMessage = "Sort value is incorrect.";
        private readonly IDeliveryServices _deliveryService = deliveryServices;
        private readonly IUserServices _userServices = userServices;
        private readonly ILogServices _logServices = logServices;
        private readonly INotificationServices _notificationServices = notificationServices;

        /// <summary>
        /// Tries to receive information about deliveries that coming to user form his client.
        /// </summary>
        /// <param name="userId">User id that data is specially filtered on.</param>
        /// <param name="search">The phrase searched in deliveries information. It will check if phrase exist in proforma number or delivery id.</param>
        ///  <param name="sort">Contains parameter that object will be sorted by. Must start with D or A to determine ascending order. Then is follow by name of property.</param>
        /// <param name="estimatedL">Filter that search lower estimated dates then given</param>
        /// <param name="estimatedG">Filter that search greater estimated dates then given</param>
        /// <param name="deliveredL">Filter that search lower delivered dates then given</param>
        /// <param name="deliveredG">Filter that search greater delivered dates then given</param>
        /// <param name="recipient">Value of recipient filter</param>
        /// <param name="status">Value of status filter</param>
        /// <param name="company">Value of company filter</param>
        /// <param name="waybill">Value of waybill filter</param>
        /// <returns>200 code with list of <see cref="Models.DTOs.Get.GetDelivery"./>, 400 when sort if given sort is incorrect or 404 whe user is not found</returns>
        [HttpGet]
        [Route("get/user/{userId}")]
        public async Task<IActionResult> GetToUserDeliveries(int userId, string? search, string? sort,
            string? estimatedL, string? estimatedG, string? deliveredL, string? deliveredG, int? recipient, int? status, int? company, string? waybill)
        {
            if (sort != null)
            {
                bool isSortOk = sort.StartsWith('A') || sort.StartsWith('D');
                if (!isSortOk) return BadRequest(sortErrorMessage);
            }
            var filters = new DeliveryFiltersTemplate
            {
                EstimatedL = estimatedL,
                EstimatedG = estimatedG,
                DeliveredL = deliveredL,
                DeliveredG = deliveredG,
                Recipient = recipient,
                Status = status,
                Company = company,
                Waybill = waybill
            };
            var exist = await _userServices.UserExist(userId);
            if (!exist) return NotFound(userNotFoundMessage);
            if (search != null)
            {
                var sResult = await _deliveryService.GetDeliveries(true, userId, search, sort: sort, filters);
                return Ok(sResult);
            }
            var result = await _deliveryService.GetDeliveries(true, userId, sort: sort, filters);
            return Ok(result);
        }
        /// <summary>
        /// Tries to receive information about deliveries that coming to all users.
        /// </summary>
        /// <param name="search">The phrase searched in deliveries information. It will check if phrase exist in proforma number or delivery id.</param>
        ///  <param name="sort">Contains parameter that object will be sorted by. Must start with D or A to determine ascending order. Then is follow by name of property.</param>
        /// <param name="estimatedL">Filter that search lower estimated dates then given</param>
        /// <param name="estimatedG">Filter that search greater estimated dates then given</param>
        /// <param name="deliveredL">Filter that search lower delivered dates then given</param>
        /// <param name="deliveredG">Filter that search greater delivered dates then given</param>
        /// <param name="recipient">Value of recipient filter</param>
        /// <param name="status">Value of status filter</param>
        /// <param name="company">Value of company filter</param>
        /// <param name="waybill">Value of waybill filter</param>
        /// <returns>200 code with list of <see cref="Models.DTOs.Get.GetDelivery"/> or 400 when sort if given sort is incorrect.</returns>
        [HttpGet]
        [Route("get/user")]
        public async Task<IActionResult> GetToUserDeliveries(string? search, string? sort,
            string? estimatedL, string? estimatedG, string? deliveredL, string? deliveredG, int? recipient, int? status, int? company, string? waybill)
        {
            if (sort != null)
            {
                bool isSortOk = sort.StartsWith('A') || sort.StartsWith('D');
                if (!isSortOk) return BadRequest(sortErrorMessage);
            }
            var filters = new DeliveryFiltersTemplate
            {
                EstimatedL = estimatedL,
                EstimatedG = estimatedG,
                DeliveredL = deliveredL,
                DeliveredG = deliveredG,
                Recipient = recipient,
                Status = status,
                Company = company,
                Waybill = waybill
            };
            if (search != null)
            {
                var sResult = await _deliveryService.GetDeliveries(true, search, sort: sort, filters);
                return Ok(sResult);
            }
            var result = await _deliveryService.GetDeliveries(true, sort: sort, filters);
            return Ok(result);
        }
        /// <summary>
        /// Tries to receive information about deliveries that coming from user to his clients.
        /// </summary>
        /// <param name="userId">User id that data is specially filtered on.</param>
        /// <param name="search">The phrase searched in deliveries information. It will check if phrase exist in proforma number or delivery id.</param>
        ///  <param name="sort">Contains parameter that object will be sorted by. Must start with D or A to determine ascending order. Then is follow by name of property.</param>
        /// <param name="estimatedL">Filter that search lower estimated dates then given</param>
        /// <param name="estimatedG">Filter that search greater estimated dates then given</param>
        /// <param name="deliveredL">Filter that search lower delivered dates then given</param>
        /// <param name="deliveredG">Filter that search greater delivered dates then given</param>
        /// <param name="recipient">Value of recipient filter</param>
        /// <param name="status">Value of status filter</param>
        /// <param name="company">Value of company filter</param>
        /// <param name="waybill">Value of waybill filter</param>
        /// <returns>200 code with list of <see cref="Models.DTOs.Get.GetDelivery"./>, 400 when sort if given sort is incorrect or 404 whe user is not found</returns>
        [HttpGet]
        [Route("get/client/{userId}")]
        public async Task<IActionResult> GetClientDeliveries(int userId, string? search, string? sort,
            string? estimatedL, string? estimatedG, string? deliveredL, string? deliveredG, int? recipient, int? status, int? company, string? waybill)
        {
            if (sort != null)
            {
                bool isSortOk = sort.StartsWith('A') || sort.StartsWith('D');
                if (!isSortOk) return BadRequest(sortErrorMessage);
            }
            var filters = new DeliveryFiltersTemplate
            {
                EstimatedL = estimatedL,
                EstimatedG = estimatedG,
                DeliveredL = deliveredL,
                DeliveredG = deliveredG,
                Recipient = recipient,
                Status = status,
                Company = company,
                Waybill = waybill
            };
            var exist = await _userServices.UserExist(userId);
            if (!exist) return NotFound(userNotFoundMessage);
            if (search != null)
            {
                var sResult = await _deliveryService.GetDeliveries(false, userId, search, sort: sort, filters);
                return Ok(sResult);
            }
            var result = await _deliveryService.GetDeliveries(false, userId, sort: sort, filters);
            return Ok(result);
        }
        /// <summary>
        /// Tries to receive information about deliveries that going out to clients.
        /// </summary>
        /// <param name="search">The phrase searched in deliveries information. It will check if phrase exist in proforma number or delivery id.</param>
        ///  <param name="sort">Contains parameter that object will be sorted by. Must start with D or A to determine ascending order. Then is follow by name of property.</param>
        /// <param name="estimatedL">Filter that search lower estimated dates then given</param>
        /// <param name="estimatedG">Filter that search greater estimated dates then given</param>
        /// <param name="deliveredL">Filter that search lower delivered dates then given</param>
        /// <param name="deliveredG">Filter that search greater delivered dates then given</param>
        /// <param name="recipient">Value of recipient filter</param>
        /// <param name="status">Value of status filter</param>
        /// <param name="company">Value of company filter</param>
        /// <param name="waybill">Value of waybill filter</param>
        /// <returns>200 code with list of <see cref="Models.DTOs.Get.GetDelivery"./> or 400 when sort if given sort is incorrect</returns>
        [HttpGet]
        [Route("get/client")]
        public async Task<IActionResult> GetClientDeliveries(string? search, string? sort,
            string? estimatedL, string? estimatedG, string? deliveredL, string? deliveredG, int? recipient, int? status, int? company, string? waybill)
        {
            if (sort != null)
            {
                bool isSortOk = sort.StartsWith('A') || sort.StartsWith('D');
                if (!isSortOk) return BadRequest(sortErrorMessage);
            }
            var filters = new DeliveryFiltersTemplate
            {
                EstimatedL = estimatedL,
                EstimatedG = estimatedG,
                DeliveredL = deliveredL,
                DeliveredG = deliveredG,
                Recipient = recipient,
                Status = status,
                Company = company,
                Waybill = waybill
            };
            if (search != null)
            {
                var sResult = await _deliveryService.GetDeliveries(false, search, sort: sort, filters);
                return Ok(sResult);
            }
            var result = await _deliveryService.GetDeliveries(false, sort: sort, filters);
            return Ok(result);
        }
        /// <summary>
        /// Add new delivery to database. This action will also create new log entry and notification.
        /// </summary>
        /// <param name="data">New delivery information wrapped in <see cref="Models.DTOs.Create.AddDelivery"/> object.</param>
        /// <returns>200 when success, 500 when failure, 404 when user was not found or 400 when delivery for chosen proforma already exist.</returns>
        [HttpPost]
        [Route("add")]
        public async Task<IActionResult> AddDelivery(AddDelivery data)
        {
            var exist = await _userServices.UserExist(data.UserId);
            if (!exist) return NotFound(userNotFoundMessage);
            var proExist = await _deliveryService.DeliveryProformaExist(data.ProformaId);
            if (proExist) return BadRequest("Delivery for this proforma already exist.");
            var deliveryId = await _deliveryService.AddDelivery(data);
            if (deliveryId == 0) return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            var logId = await _logServices.getLogTypeId("Create");
            var desc = $"User with id {data.UserId} has created the delivery with id {deliveryId}.";
            await _logServices.CreateActionLog(desc, data.UserId, logId);
            var warehouseManagersId = await _deliveryService.GetWarehouseManagerIds();
            foreach (var id in warehouseManagersId)
            {
                var userFull = await _userServices.GetUserFullName(data.UserId);
                await _notificationServices.CreateNotification(new CreateNotification
                {
                    UserId = id,
                    Info = $"{userFull} has created a delivery with id {deliveryId}.",
                    ObjectType = data.IsDeliveryToUser ? "To user delivery" : "To client delivery",
                    Referance = $"{deliveryId}"
                });
            }
            return Ok();
        }
        /// <summary>
        /// Add new delivery company to database. This action will also create new log entry.
        /// </summary>
        /// <param name="data">New delivery company information wrapped in <see cref="AddDeliveryComapny"/> object.</param>
        /// <param name="userId">Id of user that's activating this action.</param>
        /// <returns>200 code when success, 500 when failure, 400 when company already exist or 404 when user does not exist.</returns>
        [HttpPost]
        [Route("add/company/{userId}")]
        public async Task<IActionResult> AddDeliveryCompany(AddDeliveryComapny data, int userId)
        {
            var exist = await _userServices.UserExist(userId);
            if (!exist) return NotFound(userNotFoundMessage);
            var companyExist = await _deliveryService.DoesDeliveryCompanyExist(data.CompanyName);
            if (companyExist) return BadRequest("This company already exists.");
            var result = await _deliveryService.AddDeliveryCompany(data.CompanyName);
            if (!result) return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            var logId = await _logServices.getLogTypeId("Create");
            var desc = $"User with id {userId} has created the delivery company with name {data.CompanyName}.";
            await _logServices.CreateActionLog(desc, userId, logId);
            return Ok();
        }
        /// <summary>
        /// Tries to receive information about delivery companies from database.
        /// </summary>
        /// <returns>200 code with list of objects of <see cref="Models.DTOs.Get.GetDeliveryCompany"/>.</returns>
        [HttpGet]
        [Route("get/delivery_companies")]
        public async Task<IActionResult> GetDeliveryCompanies()
        {
            var result = await _deliveryService.GetDeliveryCompanies();
            return Ok(result);
        }
        /// <summary>
        /// Tries to receive information about delivery statuses from database.
        /// </summary>
        /// <returns>200 code with list of objects of <see cref="Models.DTOs.Get.GetDeliveryStatus"/>.</returns>
        [HttpGet]
        [Route("get/delivery_statuses")]
        public async Task<IActionResult> GetDeliveryStatuses()
        {
            var result = await _deliveryService.GetDeliveryStatuses();
            return Ok(result);
        }
        /// <summary>
        /// Tries to receive information about proformas that user is recipient from database.
        /// </summary>
        /// <param name="userId">User id.</param>
        /// <returns>200 code with list of <see cref="Models.DTOs.Get.GetProformaList"/> or 404 when user is not found.</returns>
        [HttpGet]
        [Route("get/user/proformas/{userId}")]
        public async Task<IActionResult> GetUserProformas(int userId)
        {
            var exist = await _userServices.UserExist(userId);
            if (!exist) return NotFound(userNotFoundMessage);
            var result = await _deliveryService.GetProformaListWithoutDelivery(true, userId);
            return Ok(result);
        }
        /// <summary>
        /// Tries to receive information about proformas that user client is recipient from database.
        /// </summary>
        /// <param name="userId">User id.</param>
        /// <returns>200 code with list of <see cref="Models.DTOs.Get.GetProformaList"/> or 404 when user is not found.</returns>
        [HttpGet]
        [Route("get/client/proformas/{userId}")]
        public async Task<IActionResult> GetClientProformas(int userId)
        {
            var exist = await _userServices.UserExist(userId);
            if (!exist) return NotFound(userNotFoundMessage);
            var result = await _deliveryService.GetProformaListWithoutDelivery(false, userId);
            return Ok(result);
        }
        /// <summary>
        /// Delete chosen delivery from database. This action will also create new log entry.
        /// </summary>
        /// <param name="deliveryId">Delivery id to delete.</param>
        /// <param name="userId">Id of user that's activating this action.</param>
        /// <param name="isDeliveryToUser">True if user is recipient of thi delivery, false if client is.</param>
        /// <returns>200 when success, 500 when failure or 404 when delivery or user is not found</returns>
        [HttpDelete]
        [Route("delete/{isDeliveryToUser}/{deliveryId}/user/{userId}")]
        public async Task<IActionResult> DeleteDelivery(int deliveryId, int userId, bool isDeliveryToUser)
        {
            var userExist = await _userServices.UserExist(userId);
            if (!userExist) return NotFound(userNotFoundMessage);
            var exist = await _deliveryService.DeliveryExist(deliveryId);
            if (!exist) return NotFound("Delivery not found.");
            var owner = await _deliveryService.GetDeliveryOwnerId(deliveryId);
            var result = await _deliveryService.DeleteDelivery(deliveryId);
            if (!result) return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            var logId = await _logServices.getLogTypeId("Delete");
            var desc = $"User with id {userId} has deleted the delivery with number {deliveryId}.";
            await _logServices.CreateActionLog(desc, userId, logId);
            if (userId != owner)
            {
                var userFull = await _userServices.GetUserFullName(userId);
                await _notificationServices.CreateNotification(new CreateNotification
                {
                    UserId = owner,
                    Info = $"{userFull} has deleted a delivery with id {deliveryId}.",
                    ObjectType = isDeliveryToUser ? "To user delivery" : "To client delivery",
                    Referance = $"{deliveryId}"
                });
            }
            return Ok();
        }
        /// <summary>
        ///  Tries to receive information that was not passed as basic information in <see cref="GetToUserDeliveries"/> or <see cref="GetClientDeliveries"/> 
        /// function and are needed to showcase object for user.
        /// </summary>
        /// <param name="deliveryId">Delivery id.</param>
        /// <returns>200 with object of <see cref="Models.DTOs.Get.GetRestDelivery"/> or 404 when delivery is not found.</returns>
        [HttpGet]
        [Route("get/rest/{deliveryId}")]
        public async Task<IActionResult> GetRestOfDelivery(int deliveryId)
        {
            var exist = await _deliveryService.DeliveryExist(deliveryId);
            if (!exist) return NotFound("Delivery not found.");
            var result = await _deliveryService.GetRestDelivery(deliveryId);
            return Ok(result);
        }
        /// <summary>
        /// Add new delivery not to database.
        /// </summary>
        /// <param name="data">New note data</param>
        /// <returns>200 when success, 400 when delivery do not exist or 404 when user is not found.</returns>
        [HttpPost]
        [Route("add/note")]
        public async Task<IActionResult> AddNote(AddNote data)
        {
            var exist = await _userServices.UserExist(data.UserId);
            if (!exist) return NotFound(userNotFoundMessage);
            var deliveryExist = await _deliveryService.DeliveryExist(data.DeliveryId);
            if (!deliveryExist) return BadRequest("This delivery do not exists.");
            var result = await _deliveryService.AddNote(data);
            if (!result) return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            return Ok();
        }
        /// <summary>
        /// Modify delivery status by given value.
        /// </summary>
        /// <param name="data">New status data</param>
        /// <returns>200 code when success, 500 code when failure or 404 code when delivery do not exists.</returns>
        [HttpPost]
        [Route("modify/status")]
        public async Task<IActionResult> SetDeliveryStatus(SetDeliveryStatus data)
        {
            var deliveryExist = await _deliveryService.DeliveryExist(data.DeliveryId);
            if (!deliveryExist) return NotFound("This delivery do not exists.");
            var result = await _deliveryService.SetDeliveryStatus(data);
            if (!result) return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            return Ok();
        }
        /// <summary>
        /// Modify delivery by given values. This action will also create new log entry.
        /// </summary>
        /// <param name="data">Values that will be changed.</param>
        /// <param name="userId">Id of user that's activating this action</param>
        ///  <param name="isDeliveryToUser">True if user is recipient of thi delivery, false if client is.</param>
        /// <returns>200 code when success, 500 code when failure or 404 code when user or delivery do not exists.</returns>
        [HttpPost]
        [Route("modify/{isDeliveryToUser}/{userId}")]
        public async Task<IActionResult> ModifyDelivery(ModifyDelivery data, int userId, bool isDeliveryToUser)
        {
            var exist = await _userServices.UserExist(userId);
            if (!exist) return NotFound(userNotFoundMessage);
            var deliveryExist = await _deliveryService.DeliveryExist(data.DeliveryId);
            if (!deliveryExist) return NotFound("This delivery do not exists.");
            var owner = await _deliveryService.GetDeliveryOwnerId(data.DeliveryId);
            var result = await _deliveryService.ModifyDelivery(data);
            if (!result) return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            var logId = await _logServices.getLogTypeId("Modify");
            var desc = $"User with id {userId} has modified the delivery with id {data.DeliveryId}.";
            await _logServices.CreateActionLog(desc, userId, logId);
            if (userId != owner)
            {
                var userFull = await _userServices.GetUserFullName(userId);
                await _notificationServices.CreateNotification(new CreateNotification
                {
                    UserId = owner,
                    Info = $"{userFull} has modified a delivery with id {data.DeliveryId}.",
                    ObjectType = isDeliveryToUser ? "To user delivery" : "To client delivery",
                    Referance = $"{data.DeliveryId}"
                });
            }
            return Ok();
        }
    }
}
