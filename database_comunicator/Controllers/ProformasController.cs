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
            if (proExist) return BadRequest("This proforma already exist.");
            var proformaId = await _proformaServices.AddProforma(data);
            if (proformaId == 0) return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            var logId = await _logServices.getLogTypeId("Create");
            var desc = $"User with id {data.UserId} has created the proforma with number {data.ProformaNumber} and id {proformaId}.";
            await _logServices.CreateActionLog(desc, userId, logId);
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
                var sResult = await _proformaServices.GetProformas(false, userId, search);
                return Ok(sResult);
            }
            var result = await _proformaServices.GetProformas(false, userId);
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
        [HttpGet]
        [Route("get/clients/rest/{proformaId}")]
        public async Task<IActionResult> GetRestClientProforma(int proformaId)
        {
            var exist = await _proformaServices.ProformaExist(proformaId);
            if (!exist) return NotFound("Proforma not found.");
            var result = await _proformaServices.GetRestProforma(false, proformaId);
            return Ok(result);
        }
        [HttpGet]
        [Route("get/yours/rest/{proformaId}")]
        public async Task<IActionResult> GetRestYourtProforma(int proformaId)
        {
            var exist = await _proformaServices.ProformaExist(proformaId);
            if (!exist) return NotFound("Proforma not found.");
            var result = await _proformaServices.GetRestProforma(true, proformaId);
            return Ok(result);
        }
        [HttpDelete]
        [Route("delete/yours/{proformaId}")]
        public async Task<IActionResult> DeleteYourProforma(int proformaId, int userId)
        {
            var userExist = await _userServices.UserExist(userId);
            if (!userExist) return NotFound("User not found.");
            var exist = await _proformaServices.ProformaExist(proformaId);
            if (!exist) return NotFound("Proforma not found.");
            var deliveryExist = await _proformaServices.DoesDeliveryExist(proformaId);
            if (deliveryExist) return BadRequest("There is existing delivery for this proforma.");
            var owner = await _proformaServices.GetProformaUserId(proformaId);
            var proformaNumber = await _proformaServices.GetProformaNumber(proformaId);
            var result = await _proformaServices.DeleteProforma(true, proformaId);
            if (!result) return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            var logId = await _logServices.getLogTypeId("Delete");
            var desc = $"User with id {userId} has deleted the proforma with number {proformaNumber}.";
            await _logServices.CreateActionLog(desc, userId, logId);
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
        [HttpDelete]
        [Route("delete/clients/{proformaId}")]
        public async Task<IActionResult> DeleteClientProforma(int proformaId, int userId)
        {
            var userExist = await _userServices.UserExist(userId);
            if (!userExist) return NotFound("User not found.");
            var exist = await _proformaServices.ProformaExist(proformaId);
            if (!exist) return NotFound("Proforma not found.");
            var deliveryExist = await _proformaServices.DoesDeliveryExist(proformaId);
            if (deliveryExist) return BadRequest("There is existing delivery for this proforma.");
            var owner = await _proformaServices.GetProformaUserId(proformaId);
            var proformaNumber = await _proformaServices.GetProformaNumber(proformaId);
            var result = await _proformaServices.DeleteProforma(false, proformaId);
            if (!result) return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            var logId = await _logServices.getLogTypeId("Delete");
            var desc = $"User with id {userId} has deleted the proforma with number {proformaNumber}.";
            await _logServices.CreateActionLog(desc, userId, logId);
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
        [HttpGet]
        [Route("get/path/{proformaId}")]
        public async Task<IActionResult> GetProformaPath(int proformaId)
        {
            var exist = await _proformaServices.ProformaExist(proformaId);
            if (!exist) return NotFound("Proforma not found.");
            var result = await _proformaServices.GetProformaPath(proformaId) ?? "";
            return Ok(result);
        }
        [HttpGet]
        [Route("get/rest/modify/{proformaId}")]
        public async Task<IActionResult> GetRestModifyProforma(int proformaId)
        {
            var exist = await _proformaServices.ProformaExist(proformaId);
            if (!exist) return NotFound("Proforma not found.");
            var result = await _proformaServices.GetRestModifyProforma(proformaId);
            return Ok(result);
        }
        [HttpPost]
        [Route("{userId}/modify")]
        public async Task<IActionResult> ModifyRequest(ModifyProforma data, int userId)
        {
            var exist = await _userServices.UserExist(userId);
            if (!exist) return NotFound("User not found.");
            if (data.UserId != null)
            {
                var recExist = await _userServices.UserExist((int)data.UserId);
                if (!recExist) return NotFound("Chosen user not found.");
            }
            var proformaOldNumber = await _proformaServices.GetProformaNumber(data.ProformaId);
            var result = await _proformaServices.ModifyProforma(data);
            if (!result) return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            var logId = await _logServices.getLogTypeId("Modify");
            var desc = $"User with id {userId} has modified the proforma with id {data.ProformaId}.";
            await _logServices.CreateActionLog(desc, userId, logId);
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
