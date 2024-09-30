using database_comunicator.Models.DTOs;
using database_comunicator.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace database_comunicator.Controllers
{
    [Route("{db_name}/[controller]")]
    [ApiController]
    public class DeliveryController : ControllerBase
    {
        private readonly IDeliveryServices _deliveryService;
        private readonly IUserServices _userServices;
        private readonly ILogServices _logServices;
        private readonly INotificationServices _notificationServices;
        public DeliveryController(IDeliveryServices deliveryServices, IUserServices userServices, ILogServices logServices, INotificationServices notificationServices)
        {
            _deliveryService = deliveryServices;
            _userServices = userServices;
            _logServices = logServices;
            _notificationServices = notificationServices;
        }
        [HttpGet]
        [Route("get/user/{userId}")]
        public async Task<IActionResult> GetToUserDeliveries(int userId, string? search)
        {
            var exist = await _userServices.UserExist(userId);
            if (!exist) return NotFound("User not found.");
            if (search != null)
            {
                var sResult = await _deliveryService.GetDeliveries(true, userId, search);
                return Ok(sResult);
            }
            var result = await _deliveryService.GetDeliveries(true, userId);
            return Ok(result);
        }
        [HttpGet]
        [Route("get/user")]
        public async Task<IActionResult> GetToUserDeliveries(string? search)
        {
            if (search != null)
            {
                var sResult = await _deliveryService.GetDeliveries(true, search);
                return Ok(sResult);
            }
            var result = await _deliveryService.GetDeliveries(true);
            return Ok(result);
        }
        [HttpGet]
        [Route("get/client/{userId}")]
        public async Task<IActionResult> GetClientProformas(int userId, string? search)
        {
            var exist = await _userServices.UserExist(userId);
            if (!exist) return NotFound("User not found.");
            if (search != null)
            {
                var sResult = await _deliveryService.GetDeliveries(false, userId, search);
                return Ok(sResult);
            }
            var result = await _deliveryService.GetDeliveries(false, userId);
            return Ok(result);
        }
        [HttpGet]
        [Route("get/client")]
        public async Task<IActionResult> GetClientProformas(string? search)
        {
            if (search != null)
            {
                var sResult = await _deliveryService.GetDeliveries(false, search);
                return Ok(sResult);
            }
            var result = await _deliveryService.GetDeliveries(false);
            return Ok(result);
        }
        [HttpPost]
        [Route("add")]
        public async Task<IActionResult> AddDelivery(AddDelivery data)
        {
            var exist = await _userServices.UserExist(data.UserId);
            if (!exist) return NotFound("User not found.");
            var proExist = await _deliveryService.DeliveryExist(data.ProformaId);
            if (proExist) return BadRequest("Delivery for this proforma already exist.");
            var deliveryId = await _deliveryService.AddDelivery(data);
            if (deliveryId == 0) return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            var logId = await _logServices.getLogTypeId("Create");
            var desc = $"User with id {data.UserId} has created the delivery with id {deliveryId}.";
            await _logServices.CreateActionLog(desc, data.UserId, logId);
            return Ok();
        }
        [HttpPost]
        [Route("company/add")]
        public async Task<IActionResult> AddDeliveryCompany(AddDeliveryComapny data, int userId)
        {
            var exist = await _userServices.UserExist(userId);
            if (!exist) return NotFound("User not found.");
            var comapnyExist = await _deliveryService.DoesDeliveryCompanyExist(data.CompanyName);
            if (comapnyExist) return BadRequest("This company already exists.");
            await _deliveryService.AddDeliveryCompany(data.CompanyName);
            var logId = await _logServices.getLogTypeId("Create");
            var desc = $"User with id {userId} has created the delivery company with name {data.CompanyName}.";
            await _logServices.CreateActionLog(desc, userId, logId);
            return Ok();
        }
        [HttpGet]
        [Route("get/delivery_companies")]
        public async Task<IActionResult> GetDeliveryCompanies()
        {
            var result = await _deliveryService.GetDeliveryCompanies();
            return Ok(result);
        }
        [HttpGet]
        [Route("get/user/proformas/{userId}")]
        public async Task<IActionResult> GetUserProformas(int userId)
        {
            var exist = await _userServices.UserExist(userId);
            if (!exist) return NotFound("User not found.");
            var result = await _deliveryService.GetProformaListWithoutDelivery(true, userId);
            return Ok(result);
        }
        [HttpGet]
        [Route("get/client/proformas/{userId}")]
        public async Task<IActionResult> GetClientProformas(int userId)
        {
            var exist = await _userServices.UserExist(userId);
            if (!exist) return NotFound("User not found.");
            var result = await _deliveryService.GetProformaListWithoutDelivery(false, userId);
            return Ok(result);
        }
    }
}
