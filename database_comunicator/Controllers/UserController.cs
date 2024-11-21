using database_communicator.Models.DTOs;
using database_communicator.Services;
using Microsoft.AspNetCore.Mvc;

namespace database_communicator.Controllers
{
    /// <summary>
    /// This controller works on User table and few other related tables. Used for receiving, modifying and creating data 
    /// that holds users information. Use db_name parameter to pass the name of database that you want ot connect.
    /// </summary>
    [Route("{db_name}/[controller]")]
    [ApiController]
    public class UserController(IUserServices userServices) : ControllerBase
    {
        private readonly IUserServices _userServices = userServices;

        /// <summary>
        /// Verify user login information.
        /// </summary>
        /// <param name="loginInfo">User login information</param>
        /// <returns>200 code with object of <see cref="SuccessLogin"/> or 401 when email do not exist or user password did not match.</returns>
        [HttpPost]
        [Route("sign_in")]
        public async Task<IActionResult> SignIn(UserLogin loginInfo)
        {
            var doExist = await _userServices.UserExist(loginInfo.Email);
            if (!doExist) return Unauthorized();
            var isOrg = await _userServices.IsOrgUser(loginInfo.Email);
            var verify = await _userServices.VerifyUserPassword(loginInfo.Email, loginInfo.Password);
            if (!verify) return Unauthorized();
            int userId = await _userServices.GetUserId(loginInfo.Email);
            if (!isOrg)
            {
                return Ok(new SuccessLogin
                {
                    Id = userId,
                    Role = "Solo"
                });
            }
            string? role = await _userServices.GetUserRole(userId);

            return Ok(new SuccessLogin
            {
                Id = userId,
                Role = role
            });
        }
        /// <summary>
        /// Tries to receive basic user information from database.
        /// </summary>
        /// <param name="userId">Id of user that information will be received</param>
        /// <returns>200 code with object of <see cref="Models.DTOs.Get.GetUserBasicInfo"/> or 404 when user do not exist.</returns>
        [HttpGet]
        [Route("get/info/{userId}")]
        public async Task<IActionResult> GetBasicInfo(int userId)
        {
            var isExist = await _userServices.UserExist(userId);
            if (!isExist) return NotFound();
            var isOrg = await _userServices.IsOrgUser(userId);
            var result = await _userServices.GetBasicInfo(userId, isOrg);
            return Ok(result);
        }
        /// <summary>
        /// Tries to receive number of unread notification.
        /// </summary>
        /// <param name="userId">Id of user activating this action.</param>
        /// <returns>200 code with number of unread notification or 404 when user is not found.</returns>
        [HttpGet]
        [Route("get/notification_count/{userId}")]
        public async Task<IActionResult> GetNotificationCount(int userId)
        {
            var isExist = await _userServices.UserExist(userId);
            if (!isExist) return NotFound();
            var result = await _userServices.GetCountNotification(userId);
            return Ok(result);
        }
        /// <summary>
        /// Tries to receive user email from database.
        /// </summary>
        /// <param name="userId">Id of user that email will be received</param>
        /// <returns>200 code with string containing email or 404 when user is not found.</returns>
        [HttpGet]
        [Route("get/email/{userId}")]
        public async Task<IActionResult> GetEmail(int userId)
        {
            var isExist = await _userServices.UserExist(userId);
            if (!isExist) return NotFound();
            var result = await _userServices.GetUserEmail(userId);
            return Ok(result);
        }
        /// <summary>
        /// Tries to receive list of user from database.
        /// </summary>
        /// <returns>200 code with list of <see cref="Models.DTOs.Get.GetUsers"/>.</returns>
        [HttpGet]
        [Route("get/users")]
        public async Task<IActionResult> GetUsers()
        {
            var result = await _userServices.GetUsers();
            return Ok(result);
        }
        /// <summary>
        /// Tries to receive user organization name.
        /// </summary>
        /// <param name="userId">Id of user activating this action.</param>
        /// <returns>200 code with name of organization or 404 when user is not found.</returns>
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
