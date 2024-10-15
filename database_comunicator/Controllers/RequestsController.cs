using database_communicator.Models.DTOs;
using database_communicator.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace database_communicator.Controllers
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
            if (!recExist) return NotFound("Receiver not found.");
            var requestId = await _requestServices.AddRequest(data);
            if (requestId == 0) return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            var logId = await _logServices.getLogTypeId("Create");
            var desc = $"User with id {data.CreatorId} has created the request.";
            await _logServices.CreateActionLog(desc, data.CreatorId, logId);
            if (data.CreatorId != data.ReceiverId)
            {
                var userFull = await _userServices.GetUserFullName(data.CreatorId);
                await _notificationServices.CreateNotification(new CreateNotification
                {
                    UserId = data.ReceiverId,
                    Info = $"{userFull} has created a new {data.ObjectType} request for you.",
                    ObjectType = "Requests",
                    Referance = $"{requestId}"
                });
            }
            return Ok();
        }
        [HttpGet]
        [Route("get/created/{userId}")]
        public async Task<IActionResult> GetMyRequests(int userId, string? search, string? sort, string? dateL, string? dateG,
            string? type, int? status)
        {
            var exist = await _userServices.UserExist(userId);
            if (!exist) return NotFound("User not found.");
            if (search != null)
            {
                var sResult = await _requestServices.GetMyRequests(userId,search, sort: sort, dateL, dateG, type, status);
                return Ok(sResult);
            }
            var result = await _requestServices.GetMyRequests(userId, sort: sort, dateL, dateG, type, status);
            return Ok(result);
        }
        [HttpGet]
        [Route("get/received/{userId}")]
        public async Task<IActionResult> GetReceivedRequests(int userId, string? search, string? sort, string? dateL, string? dateG,
            string? type, int? status)
        {
            var exist = await _userServices.UserExist(userId);
            if (!exist) return NotFound("User not found.");
            if (search != null)
            {
                var sResult = await _requestServices.GetReceivedRequests(userId, search, sort: sort, dateL, dateG, type, status);
                return Ok(sResult);
            }
            var result = await _requestServices.GetReceivedRequests(userId, sort: sort, dateL, dateG, type, status);
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
            var receiver = await _requestServices.GetReceiverId(requestId);
            await _requestServices.DeleteRequest(requestId);
            var logId = await _logServices.getLogTypeId("Delete");
            var desc = $"User with id {userId} has deleted the request.";
            await _logServices.CreateActionLog(desc, userId, logId);
            if (userId != receiver)
            {
                var userFull = await _userServices.GetUserFullName(userId);
                await _notificationServices.CreateNotification(new CreateNotification
                {
                    UserId = receiver,
                    Info = $"{userFull} has deleted a request with id {requestId}.",
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
                if (!recExist) return NotFound("Receiver not found.");
            }
            var result = await _requestServices.ModifyRequest(data);
            if (!result) return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            var logId = await _logServices.getLogTypeId("Modify");
            var desc = $"User with id {userId} has modified the request.";
            await _logServices.CreateActionLog(desc, userId, logId);
            if (userId != data.RecevierId && data.RecevierId != null)
            {
                var userFull = await _userServices.GetUserFullName(userId);
                await _notificationServices.CreateNotification(new CreateNotification
                {
                    UserId = (int)data.RecevierId,
                    Info = $"{userFull} has modified a request with id {data.RequestId}.",
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
        [HttpGet]
        [Route("get/statuses")]
        public async Task<IActionResult> GetStatuses()
        {
            var result = await _requestServices.GetRequestStatuses();
            return Ok(result);
        }
    }
}
