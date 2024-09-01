using database_comunicator.Models.DTOs;
using database_comunicator.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace database_comunicator.Controllers
{
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
        [HttpGet]
        [Route("getNotifications/{userId}")]
        public async Task<IActionResult> GetNotifications(int userId)
        {
            var exist = await _userServices.UserExist(userId);
            if (!exist) return NotFound();
            var result = await _notificationServices.GetNotifications(userId);
            return Ok(result);
        }
        [HttpPost]
        [Route("setNotification/{notifId}")]
        public async Task<IActionResult> SetNotification(int notifId, bool isRead)
        {
            var exist = await _notificationServices.NotifExists(notifId);
            if (!exist) return NotFound();
            await _notificationServices.SetIsRead(notifId, isRead);
            return Ok();
        }
        [HttpPost]
        [Route("createNotifcation")]
        public async Task<IActionResult> CreateNotifcation(CreateNotification data)
        {
            var exist = await _userServices.UserExist(data.UserId);
            if (!exist) return NotFound();
            await _notificationServices.CreateNotification(data);
            return Ok();
        }
    }
}
