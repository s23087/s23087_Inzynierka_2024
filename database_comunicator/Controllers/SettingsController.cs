using database_communicator.Models.DTOs.Create;
using database_communicator.Models.DTOs.Modify;
using database_communicator.Services;
using Microsoft.AspNetCore.Mvc;

namespace database_communicator.Controllers
{
    /// <summary>
    /// This controller allow to control user data, organization data and logs. Use db_name parameter to pass the name of database that you want ot connect.
    /// </summary>
    [Route("{db_name}/[controller]")]
    [ApiController]
    public class SettingsController(ILogServices logServices, IOrganizationServices organizationServices, IUserServices userServices, IRolesServices rolesServices) : ControllerBase
    {
        private readonly IUserServices _userServices = userServices;
        private readonly IOrganizationServices _organizationServices = organizationServices;
        private readonly ILogServices _logServices = logServices;
        private readonly IRolesServices _rolesServices = rolesServices;

        /// <summary>
        /// Change user password.
        /// </summary>
        /// <param name="data">Object of <see cref="Models.DTOs.Modify.ChangePassword"/></param>
        /// <returns>200 code when success, 500 when failure, 404 when user not found or 401 when old password does not match.</returns>
        [HttpPost]
        [Route("changePassword")]
        public async Task<IActionResult> ChangePassword(ChangePassword data)
        {
            var exist = await _userServices.UserExist(data.UserId);
            if (!exist) return NotFound();
            var verify = await _userServices.VerifyUserPassword(data.UserId, data.OldPassword);
            if (!verify) return Unauthorized();
            var result = await _userServices.ModifyPassword(data.UserId, data.NewPassword);
            return result ? Ok() : new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
        /// <summary>
        /// Overwrite user old data with new one.
        /// </summary>
        /// <param name="data">New user data wrapped in <see cref="ChangeUserData"/> object.</param>
        /// <returns>200 code when success, 404 when user not found or 400 when new email already exist.</returns>
        [HttpPost]
        [Route("modify/user")]
        public async Task<IActionResult> ModifyUser(ChangeUserData data)
        {
            var exist = await _userServices.UserExist(data.UserId);
            if (!exist) return NotFound();
            if (data.Email != null)
            {
                var emailExist = await _userServices.EmailExist(data.Email);
                if (emailExist) return BadRequest();
            }
            var result = await _userServices.ModifyUserData(data.UserId, data.Email, data.Username, data.Surname);
            if (!result) return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            var logTypeId = await _logServices.getLogTypeId("Modify");
            await _logServices.CreateActionLog($"User with id {data.UserId} has changed their info.", data.UserId, logTypeId);
            return Ok();
        }
        /// <summary>
        /// Add new employee user to database. This action will also create new log entry.
        /// </summary>
        /// <param name="newUser">New user data.</param>
        /// <param name="userId">Id of user that activates action.</param>
        /// <returns>200 when success, 400 when failure or email already exist or 404 when user is not found.</returns>
        [HttpPost]
        [Route("add/user/{userId}")]
        public async Task<IActionResult> AddNewUser(AddUser newUser, int userId)
        {
            var exist = await _userServices.UserExist(userId);
            if (!exist) return NotFound();
            var emailExist = await _userServices.EmailExist(newUser.Email);
            if (emailExist || newUser.RoleName == null) return BadRequest("Email exist or role is null.");
            var orgId = await _userServices.GetOrgId(userId, true);
            var roleId = await _rolesServices.GetRoleId(newUser.RoleName);
            var isCreated = await _userServices.AddUser(newUser, orgId, roleId, true);
            var logTypeId = await _logServices.getLogTypeId("Create");
            await _logServices.CreateActionLog($"New user {newUser.Username} {newUser.Surname} has been added by user with id {userId}.", userId, logTypeId);
            return isCreated ? Ok() : BadRequest();
        }
        /// <summary>
        /// Switch user from solo user to org user.
        /// </summary>
        /// <returns>200 code when success or 400 when failure.</returns>
        [HttpPost]
        [Route("switch")]
        public async Task<IActionResult> SwitchToOrg()
        {
            var orgId = await _userServices.GetOrgId(1, false);
            var roleId = await _rolesServices.GetRoleId("Admin");
            var result = await _userServices.SwitchToOrg(1, roleId, orgId);
            return result ? Ok() : BadRequest();
        }
        /// <summary>
        /// Tries to receive log data from the database.
        /// </summary>
        /// <param name="userId">Id of user that activate this action.</param>
        /// <returns>200 code with list of <see cref="Models.DTOs.Get.GetLogs"/>, 404 when user not found or 401 when user is not an admin or solo role.</returns>
        [HttpGet]
        [Route("get/logs/{userId}")]
        public async Task<IActionResult> GetLogs(int userId)
        {
            var exist = await _userServices.UserExist(userId);
            if (!exist) return NotFound();
            var role = await _userServices.GetUserRole(userId);
            role ??= "Solo";
            if (role != "Admin" && role != "Solo") return Unauthorized();
            var result = await _logServices.GetLogs();
            return Ok(result);
        }
        /// <summary>
        /// Tries to receive user organization information.
        /// </summary>
        /// <param name="userId">Id of user activating this action.</param>
        /// <returns>200 code with object of <see cref="Models.DTOs.Get.GetOrg"/> or 404 when user is not found.</returns>
        [HttpGet]
        [Route("get/modify/rest/{userId}")]
        public async Task<IActionResult> GetUserOrg(int userId)
        {
            var exist = await _userServices.UserExist(userId);
            if (!exist) return NotFound();
            var isOrg = await _userServices.IsOrgUser(userId);
            var orgId = await _userServices.GetOrgId(userId, isOrg);
            var result = await _organizationServices.GetOrg(orgId);
            return Ok(result);
        }
    }
}
