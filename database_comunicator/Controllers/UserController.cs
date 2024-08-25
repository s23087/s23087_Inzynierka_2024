using database_comunicator.Models.DTOs;
using database_comunicator.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace database_comunicator.Controllers
{
    [Route("{db_name}/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserServices _userServices;
        public UserController(IUserServices userServices)
        {
            _userServices = userServices;
        }
        [HttpPost]
        [Route("sign_in")]
        public async Task<IActionResult> SignIn(UserLogin loginInfo)
        {
            var doExist = await _userServices.UserExist(loginInfo.Email);
            if (!doExist) return Unauthorized();
            var isOrg = await _userServices.IsOrgUser(loginInfo.Email);
            var userId = await _userServices.VerifyUserPassword(loginInfo.Email, loginInfo.Password, isOrg);
            return userId == null ? Unauthorized() : Ok(userId);
        }
        [HttpGet]
        [Route("basicInfo/{userId}")]
        public async Task<IActionResult> getBasicInfo(int userId)
        {
            var isExist = await _userServices.UserExist(userId);
            if (!isExist) return NotFound();
            var isOrg = await _userServices.IsOrgUser(userId);
            var result = await _userServices.GetBasicInfo(userId, isOrg);
            return Ok(result);
        }
        [HttpGet]
        [Route("notificationCount/{userId}")]
        public async Task<IActionResult> getNotificationCount(int userId)
        {
            var isExist = await _userServices.UserExist(userId);
            if (!isExist) return NotFound();
            var result = await _userServices.GetCountNotification(userId);
            return Ok(result);
        }
    }
}
