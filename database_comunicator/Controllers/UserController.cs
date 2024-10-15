using database_communicator.Models.DTOs;
using database_communicator.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace database_communicator.Controllers
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
            var verify = await _userServices.VerifyUserPassword(loginInfo.Email, loginInfo.Password);
            if (!verify) { return Unauthorized(); }
            int userId = await _userServices.GetUserId(loginInfo.Email);
            if (!isOrg)
            {
                return Ok(new SuccesLogin
                {
                    Id = userId,
                    Role = "Solo"
                });
            }
            string role = await _userServices.GetUserRole(userId);

            return Ok(new SuccesLogin
            {
                Id = userId,
                Role = role
            });
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
        [HttpGet]
        [Route("getEmail/{userId}")]
        public async Task<IActionResult> GetEmail(int userId)
        {
            var isExist = await _userServices.UserExist(userId);
            if (!isExist) return NotFound();
            var result = await _userServices.GetUserEmail(userId);
            return Ok(result);
        }
        [HttpGet]
        [Route("getUsers")]
        public async Task<IActionResult> GetUsers()
        {
            var result = await _userServices.GetUsers();
            return Ok(result);
        }
        [HttpGet]
        [Route("get/organization/{userId}")]
        public async Task<IActionResult> GetUserOrganization(int userId)
        {
            var isExist = await _userServices.UserExist(userId);
            if (!isExist) return NotFound();
            var result = await _userServices.GetUserOrg(userId);
            return Ok(result);
        }
    }
}
