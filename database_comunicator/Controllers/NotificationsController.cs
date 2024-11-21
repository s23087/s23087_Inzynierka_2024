using database_communicator.Services;
using Microsoft.AspNetCore.Mvc;

namespace database_communicator.Controllers
{
    /// <summary>
    /// This controller works on Notification table and few other related tables. Used for receiving, modifying and creating data 
    /// that holds users notification information. Use db_name parameter to pass the name of database that you want ot connect.
    /// </summary>
    [Route("{db_name}/[controller]")]
    [ApiController]
    public class NotificationsController : ControllerBase
    {
        private readonly IUserServices _userServices;
        private readonly INotificationServices _notificationServices;
        public NotificationsController(IUserServices userServices, INotificationServices notificationServices)
        {
            _notificationServices = notificationServices;
            _userServices = userServices;
        }
        /// <summary>
        /// Tries to receive notification data from database for chosen user.
        /// </summary>
        /// <param name="userId">Id of user that notification will be received.</param>
        /// <returns>200 code with list of <see cref="Models.DTOs.Get.GetNotifications"/></returns>
        [HttpGet]
        [Route("get/{userId}")]
        public async Task<IActionResult> GetNotifications(int userId)
        {
            var exist = await _userServices.UserExist(userId);
            if (!exist) return NotFound();
            var result = await _notificationServices.GetNotifications(userId);
            return Ok(result);
        }
        /// <summary>
        /// Change notification isRead property to given one.
        /// </summary>
        /// <param name="notifId">Id of notification that property will be changed.</param>
        /// <param name="isRead">New isRead value</param>
        /// <returns>200 code when success, 500 when failure or 404 when notification is not found.</returns>
        [HttpPost]
        [Route("modify/{notifId}/is_read/{isRead}")]
        public async Task<IActionResult> SetNotification(int notifId, bool isRead)
        {
            var exist = await _notificationServices.NotificationExists(notifId);
            if (!exist) return NotFound();
            var result = await _notificationServices.SetIsRead(notifId, isRead);
            if (!result) return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            return Ok();
        }
    }
}
