using database_comunicator.Models.DTOs;
using database_comunicator.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace database_comunicator.Controllers
{
    [Route("{db_name}/[controller]")]
    [ApiController]
    public class RequestsController : ControllerBase
    {
        private readonly IRequestServices _requestServices;
        private readonly IUserServices _userServices;
        private readonly ILogServices _logServices;
        private readonly INotificationServices _notificationServices;
        public RequestsController(IRequestServices requestServices, IUserServices userServices, ILogServices logServices, INotificationServices notificationServices)
        {
            _requestServices = requestServices;
            _logServices = logServices;
            _userServices = userServices;
            _notificationServices = notificationServices;
        }
        [HttpGet]
        [Route("users")]
        public async Task<IActionResult> GetRequestUsers()
        {
            var result = await _userServices.GetAccountantUser();
            return Ok(result);
        }
        [HttpPost]
        [Route("add")]
        public async Task<IActionResult> AddRequest(AddRequest data)
        {
            var exist = await _userServices.UserExist(data.CreatorId);
            if (!exist) return NotFound("Creator not found.");
            var recExist = await _userServices.UserExist(data.ReceiverId);
            if (!recExist) return NotFound("Reciver not found.");
            var requestId = await _requestServices.AddRequest(data);
            if (requestId == 0) return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            var logId = await _logServices.getLogTypeId("Create");
            var desc = $"User with id {data.CreatorId} has created the request.";
            await _logServices.CreateActionLog(desc, data.CreatorId, logId);
            if (data.CreatorId != data.ReceiverId)
            {
                await _notificationServices.CreateNotification(new CreateNotification
                {
                    UserId = data.ReceiverId,
                    Info = $"User has created a new {data.ObjectType} request for you.",
                    ObjectType = "Requests",
                    Referance = $"{requestId}"
                });
            }
            return Ok();
        }
        [HttpGet]
        [Route("get/created/{userId}")]
        public async Task<IActionResult> GetMyRequests(int userId, string? search)
        {
            var exist = await _userServices.UserExist(userId);
            if (!exist) return NotFound("User not found.");
            if (search != null)
            {
                var sResult = await _requestServices.GetMyRequests(userId,search);
                return Ok(sResult);
            }
            var result = await _requestServices.GetMyRequests(userId);
            return Ok(result);
        }
        [HttpGet]
        [Route("get/recived/{userId}")]
        public async Task<IActionResult> GetRecivedRequests(int userId, string? search)
        {
            var exist = await _userServices.UserExist(userId);
            if (!exist) return NotFound("User not found.");
            if (search != null)
            {
                var sResult = await _requestServices.GetRecivedRequests(userId, search);
                return Ok(sResult);
            }
            var result = await _requestServices.GetRecivedRequests(userId);
            return Ok(result);
        }
        [HttpGet]
        [Route("get/rest/{requestId}")]
        public async Task<IActionResult> GetRestRequest(int requestId)
        {
            var exist = await _requestServices.RequestExist(requestId);
            if (!exist) return NotFound("Requests not found.");
            var result = await _requestServices.GetRestRequest(requestId);
            return Ok(result);
        }
        [HttpDelete]
        [Route("delete/{requestId}")]
        public async Task<IActionResult> DeleteRequest(int requestId, int userId)
        {
            var userExist = await _userServices.UserExist(userId);
            if (!userExist) return NotFound("User not found.");
            var exist = await _requestServices.RequestExist(requestId);
            if (!exist) return NotFound("Requests not found.");
            var recevier = await _requestServices.GetRecevierId(requestId);
            await _requestServices.DeleteRequest(requestId);
            var logId = await _logServices.getLogTypeId("Delete");
            var desc = $"User with id {userId} has deleted the request.";
            await _logServices.CreateActionLog(desc, userId, logId);
            if (userId != recevier)
            {
                await _notificationServices.CreateNotification(new CreateNotification
                {
                    UserId = recevier,
                    Info = $"User has modified a request wit id {requestId}.",
                    ObjectType = "Requests",
                    Referance = $"{requestId}"
                });
            }
            return Ok();
        }
        [HttpPost]
        [Route("{userId}/modify")]
        public async Task<IActionResult> ModifyRequest(ModifyRequest data, int userId)
        {
            var exist = await _userServices.UserExist(userId);
            if (!exist) return NotFound("User not found.");
            if (data.RecevierId != null)
            {
                var recExist = await _userServices.UserExist((int)data.RecevierId);
                if (!recExist) return NotFound("Reciver not found.");
            }
            var result = await _requestServices.ModifyRequest(data);
            if (!result) return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            var logId = await _logServices.getLogTypeId("Modify");
            var desc = $"User with id {userId} has modified the request.";
            await _logServices.CreateActionLog(desc, userId, logId);
            if (userId != data.RecevierId && data.RecevierId != null)
            {
                await _notificationServices.CreateNotification(new CreateNotification
                {
                    UserId = (int)data.RecevierId,
                    Info = $"User has modified a request wit id {data.RequestId}.",
                    ObjectType = "Requests",
                    Referance = $"{data.RequestId}"
                });
            }
            return Ok();
        }
        [HttpPost]
        [Route("modify/{requestId}/status")]
        public async Task<IActionResult> SetStatus(int requestId, SetRequestStatus data)
        {
            var exist = await _requestServices.RequestExist(requestId);
            if (!exist) return NotFound("Requests not found.");
            var statusId = await _requestServices.GetStatusId(data.StatusName);
            var result = await _requestServices.SetRequestStatus(requestId, statusId, data);
            return result ? Ok() : BadRequest();
        }
        [HttpGet]
        [Route("get/path/{requestId}")]
        public async Task<IActionResult> GetRequestPath(int requestId)
        {
            var exist = await _requestServices.RequestExist(requestId);
            if (!exist) return NotFound("Requests not found.");
            var result = await _requestServices.GetRequestPath(requestId);
            return Ok(result);
        }
        [HttpGet]
        [Route("get/modify/rest/{requestId}")]
        public async Task<IActionResult> GetRestModifyRequest(int requestId)
        {
            var exist = await _requestServices.RequestExist(requestId);
            if (!exist) return NotFound("Requests not found.");
            var result = await _requestServices.GetRestModifyRequest(requestId);
            return Ok(result);
        }
    }
}
