using database_communicator.Models.DTOs.Create;
using database_communicator.Models.DTOs.Modify;
using database_communicator.Services;
using Microsoft.AspNetCore.Mvc;

namespace database_communicator.Controllers
{
    /// <summary>
    /// This controller works on Request table and few other related tables. Used for receiving, modifying and creating data 
    /// that holds users requests information. Use db_name parameter to pass the name of database that you want ot connect.
    /// </summary>
    [Route("{db_name}/[controller]")]
    [ApiController]
    public class RequestsController(IRequestServices requestServices, IUserServices userServices, ILogServices logServices, INotificationServices notificationServices) : ControllerBase
    {
        private const string userNotFoundMessage = "User not found.";
        private const string requestNotFoundMessage = "Requests not found.";
        private readonly IRequestServices _requestServices = requestServices;
        private readonly IUserServices _userServices = userServices;
        private readonly ILogServices _logServices = logServices;
        private readonly INotificationServices _notificationServices = notificationServices;

        /// <summary>
        /// Tries to receive list of users eligible to complete request from database.
        /// </summary>
        /// <returns>200 code with list of <see cref="Models.DTOs.Get.GetUsers"/>.</returns>
        [HttpGet]
        [Route("get/receivers")]
        public async Task<IActionResult> GetReceivers()
        {
            var result = await _userServices.GetAccountantUser();
            return Ok(result);
        }
        /// <summary>
        /// Add new request to database with given data. This action will also create new log entry. Notification also will be created if creator of request is different then receiver.
        /// </summary>
        /// <param name="data">New request data wrapped in <see cref="Models.DTOs.Create.AddRequest"/> object.</param>
        /// <returns>200 code when success, 500 when failure or 404 when users are not found.</returns>
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
            var logDescription = $"User with id {data.CreatorId} has created the request.";
            await _logServices.CreateActionLog(logDescription, data.CreatorId, logId);
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
        /// <summary>
        /// Tries to receive basic information about given user requests.
        /// </summary>
        /// <param name="userId">User id that data is specially filtered on.</param>
        /// <param name="search">The phrase searched in request information. It will check if phrase exist in title.</param>
        /// <param name="sort">Contains parameter that object will be sorted by. Must start with D or A to determine ascending order. Then is follow by name of property.</param>
        /// <param name="dateL">Filter that search for date that is lower then given value.</param>
        /// <param name="dateG">Filter that search for date that is greater then given value.</param>
        /// <param name="type">Filter that search for type with given value</param>
        /// <param name="status">Filter that search for status with given value</param>
        /// <returns>200 code with list of <see cref="Models.DTOs.Get.GetRequest"/>, 400 when sort if given sort is incorrect or 404 when user do not exist.</returns>
        [HttpGet]
        [Route("get/created/{userId}")]
        public async Task<IActionResult> GetMyRequests(int userId, string? search, string? sort, string? dateL, string? dateG,
            string? type, int? status)
        {
            if (sort != null)
            {
                bool isSortOk = sort.StartsWith('A') || sort.StartsWith('D');
                if (!isSortOk) return BadRequest("Sort value is incorrect.");
            }
            var exist = await _userServices.UserExist(userId);
            if (!exist) return NotFound(userNotFoundMessage);
            if (search != null)
            {
                var sResult = await _requestServices.GetMyRequests(userId, search, sort: sort, dateL, dateG, type, status);
                return Ok(sResult);
            }
            var result = await _requestServices.GetMyRequests(userId, sort: sort, dateL, dateG, type, status);
            return Ok(result);
        }
        /// <summary>
        /// Tries to receive basic information about requests that user received.
        /// </summary>
        /// <param name="userId">User id that data is specially filtered on.</param>
        /// <param name="search">The phrase searched in request information. It will check if phrase exist in title.</param>
        /// <param name="sort">Contains parameter that object will be sorted by. Must start with D or A to determine ascending order. Then is follow by name of property.</param>
        /// <param name="dateL">Filter that search for date that is lower then given value.</param>
        /// <param name="dateG">Filter that search for date that is greater then given value.</param>
        /// <param name="type">Filter that search for type with given value</param>
        /// <param name="status">Filter that search for status with given value</param>
        /// <returns>200 code with list of <see cref="Models.DTOs.Get.GetRequest"/>, 400 when sort if given sort is incorrect or 404 when user do not exist.</returns>
        [HttpGet]
        [Route("get/received/{userId}")]
        public async Task<IActionResult> GetReceivedRequests(int userId, string? search, string? sort, string? dateL, string? dateG,
            string? type, int? status)
        {
            if (sort != null)
            {
                bool isSortOk = sort.StartsWith('A') || sort.StartsWith('D');
                if (!isSortOk) return BadRequest("Sort value is incorrect.");
            }
            var exist = await _userServices.UserExist(userId);
            if (!exist) return NotFound(userNotFoundMessage);
            if (search != null)
            {
                var sResult = await _requestServices.GetReceivedRequests(userId, search, sort: sort, dateL, dateG, type, status);
                return Ok(sResult);
            }
            var result = await _requestServices.GetReceivedRequests(userId, sort: sort, dateL, dateG, type, status);
            return Ok(result);
        }
        /// <summary>
        /// Tries to receive information that was not passed as basic information in <see cref="GetMyRequests"/> or <see cref="GetReceivedRequests"/> 
        /// function and are needed to showcase object for user.
        /// </summary>
        /// <param name="requestId">Id of showcased request.</param>
        /// <returns>200 code with object of <see cref="Models.DTOs.Get.GetRestRequest"/> or 404 when request not found.</returns>
        [HttpGet]
        [Route("get/rest/{requestId}")]
        public async Task<IActionResult> GetRestRequest(int requestId)
        {
            var exist = await _requestServices.RequestExist(requestId);
            if (!exist) return NotFound(requestNotFoundMessage);
            var result = await _requestServices.GetRestRequest(requestId);
            return Ok(result);
        }
        /// <summary>
        /// Delete chosen request from database. This action will also create new log entry. Notification also will be created when user activating it is not receiver.
        /// </summary>
        /// <param name="requestId">Id of request to delete</param>
        /// <param name="userId">Id of user that's activating this action.</param>
        /// <returns>200 code when success or 404 when user or request is not found.</returns>
        [HttpDelete]
        [Route("delete/{requestId}/user/{userId}")]
        public async Task<IActionResult> DeleteRequest(int requestId, int userId)
        {
            var userExist = await _userServices.UserExist(userId);
            if (!userExist) return NotFound(userNotFoundMessage);
            var exist = await _requestServices.RequestExist(requestId);
            if (!exist) return NotFound(requestNotFoundMessage);
            var receiver = await _requestServices.GetReceiverId(requestId);
            var result = await _requestServices.DeleteRequest(requestId);
            if (!result) return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            var logId = await _logServices.getLogTypeId("Delete");
            var logDescription = $"User with id {userId} has deleted the request.";
            await _logServices.CreateActionLog(logDescription, userId, logId);
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
        /// <summary>
        /// Overwrite chosen request data with given new one. This action will also create new log entry. Notification will be created if activating this user is not the receiver.
        /// </summary>
        /// <param name="data">New request data wrapped in <see cref="Models.DTOs.Modify.ModifyRequest"/> object.</param>
        /// <param name="userId">Id of user that's activating this action.</param>
        /// <returns>200 code when success, 500 when failure or 404 when users not found.</returns>
        [HttpPost]
        [Route("modify/{userId}")]
        public async Task<IActionResult> ModifyRequest(ModifyRequest data, int userId)
        {
            var exist = await _userServices.UserExist(userId);
            if (!exist) return NotFound(userNotFoundMessage);
            var reqExist = await _requestServices.RequestExist(data.RequestId);
            if (!reqExist) return NotFound(requestNotFoundMessage);
            if (data.ReceiverId != null)
            {
                var recExist = await _userServices.UserExist((int)data.ReceiverId);
                if (!recExist) return NotFound("Receiver not found.");
            }
            var result = await _requestServices.ModifyRequest(data);
            if (!result) return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            var logId = await _logServices.getLogTypeId("Modify");
            var logDescription = $"User with id {userId} has modified the request.";
            await _logServices.CreateActionLog(logDescription, userId, logId);
            if (userId != data.ReceiverId && data.ReceiverId != null)
            {
                var userFull = await _userServices.GetUserFullName(userId);
                await _notificationServices.CreateNotification(new CreateNotification
                {
                    UserId = (int)data.ReceiverId,
                    Info = $"{userFull} has modified a request with id {data.RequestId}.",
                    ObjectType = "Requests",
                    Referance = $"{data.RequestId}"
                });
            }
            return Ok();
        }
        /// <summary>
        /// Set chosen request status to new one.
        /// </summary>
        /// <param name="requestId">Id of request that status will be changed.</param>
        /// <param name="data">New status data.</param>
        /// <returns>200 code when success, 400 when failure or 404 when request not found.</returns>
        [HttpPost]
        [Route("modify/{requestId}/status")]
        public async Task<IActionResult> SetStatus(int requestId, SetRequestStatus data)
        {
            var exist = await _requestServices.RequestExist(requestId);
            if (!exist) return NotFound(requestNotFoundMessage);
            var statusId = await _requestServices.GetStatusId(data.StatusName);
            var result = await _requestServices.SetRequestStatus(requestId, statusId, data);
            return result ? Ok() : BadRequest();
        }
        /// <summary>
        /// Tries to receive request file path from the database.
        /// </summary>
        /// <param name="requestId">Id of request that path will be received</param>
        /// <returns>200 code with string containing file path or 404 when request is not found.</returns>
        [HttpGet]
        [Route("get/path/{requestId}")]
        public async Task<IActionResult> GetRequestPath(int requestId)
        {
            var exist = await _requestServices.RequestExist(requestId);
            if (!exist) return NotFound(requestNotFoundMessage);
            var result = await _requestServices.GetRequestPath(requestId);
            return Ok(result);
        }
        /// <summary>
        /// Tries to receive information that was not passed as basic information in <see cref="GetMyRequests"/> or <see cref="GetReceivedRequests"/> 
        /// function and are needed to modify object.
        /// </summary>
        /// <param name="requestId">Request id.</param>
        /// <returns>200 code with object of <see cref="Models.DTOs.Get.GetRestModifyRequest"/> or 404 when request not found.</returns>
        [HttpGet]
        [Route("get/modify/rest/{requestId}")]
        public async Task<IActionResult> GetRestModifyRequest(int requestId)
        {
            var exist = await _requestServices.RequestExist(requestId);
            if (!exist) return NotFound(requestNotFoundMessage);
            var result = await _requestServices.GetRestModifyRequest(requestId);
            return Ok(result);
        }
        /// <summary>
        /// Tries to receive list of request statuses from database.
        /// </summary>
        /// <returns>200 code with list of <see cref="Models.DTOs.Get.GetRequestStatus"/></returns>
        [HttpGet]
        [Route("get/statuses")]
        public async Task<IActionResult> GetStatuses()
        {
            var result = await _requestServices.GetRequestStatuses();
            return Ok(result);
        }
    }
}
