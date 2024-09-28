using database_comunicator.Models.DTOs;
using database_comunicator.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace database_comunicator.Controllers
{
    [Route("{db_name}/[controller]")]
    [ApiController]
    public class ProformasController : ControllerBase
    {
        private readonly IProformaServices _proformaServices;
        private readonly IUserServices _userServices;
        private readonly ILogServices _logServices;
        private readonly INotificationServices _notificationServices;
        public ProformasController(IProformaServices proformaServices, IUserServices userServices, ILogServices logServices, INotificationServices notificationServices)
        {
            _logServices = logServices;
            _userServices = userServices;
            _notificationServices = notificationServices;
            _proformaServices = proformaServices;
        }
        [HttpPost]
        [Route("add")]
        public async Task<IActionResult> AddProforma(AddProforma data, int userId)
        {
            var exist = await _userServices.UserExist(userId);
            if (!exist) return NotFound("User not found.");
            var proUser = await _userServices.UserExist(data.UserId);
            if (!proUser) return NotFound("Choosen user not found.");
            var proExist = await _proformaServices.ProformaExist(data.ProformaNumber, data.SellerId, data.BuyerId);
            if (!proExist) return BadRequest("This proforma already exist.");
            var proformaId = await _proformaServices.AddProforma(data);
            if (proformaId == 0) return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            var logId = await _logServices.getLogTypeId("Create");
            var desc = $"User with id {data.UserId} has created the proforma with number {data.ProformaNumber} and id {proformaId}.";
            await _logServices.CreateActionLog(desc, userId, logId);
            if (userId != data.UserId)
            {
                await _notificationServices.CreateNotification(new CreateNotification
                {
                    UserId = data.UserId,
                    Info = $"User has created a new proforma for you with number {data.ProformaNumber}.",
                    ObjectType = data.IsYourProforma ? "Yours Proformas" : "Clients Proformas",
                    Referance = $"{proformaId}"
                });
            }
            return Ok();
        }
        [HttpGet]
        [Route("get/yours/{userId}")]
        public async Task<IActionResult> GetYoursProformas(int userId, string? search)
        {
            var exist = await _userServices.UserExist(userId);
            if (!exist) return NotFound("User not found.");
            if (search != null)
            {
                var sResult = await _proformaServices.GetProformas(true, userId, search);
                return Ok(sResult);
            }
            var result = await _proformaServices.GetProformas(true, userId);
            return Ok(result);
        }
        [HttpGet]
        [Route("get/yours")]
        public async Task<IActionResult> GetYoursProformas(string? search)
        {
            if (search != null)
            {
                var sResult = await _proformaServices.GetProformas(true, search);
                return Ok(sResult);
            }
            var result = await _proformaServices.GetProformas(true);
            return Ok(result);
        }
        [HttpGet]
        [Route("get/clients/{userId}")]
        public async Task<IActionResult> GetClientProformas(int userId, string? search)
        {
            var exist = await _userServices.UserExist(userId);
            if (!exist) return NotFound("User not found.");
            if (search != null)
            {
                var sResult = await _proformaServices.GetProformas(true, userId, search);
                return Ok(sResult);
            }
            var result = await _proformaServices.GetProformas(true, userId);
            return Ok(result);
        }
        [HttpGet]
        [Route("get/clients")]
        public async Task<IActionResult> GetClientProformas(string? search)
        {
            if (search != null)
            {
                var sResult = await _proformaServices.GetProformas(false, search);
                return Ok(sResult);
            }
            var result = await _proformaServices.GetProformas(false);
            return Ok(result);
        }
    }
}
