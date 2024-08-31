using database_comunicator.Models.DTOs;
using database_comunicator.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace database_comunicator.Controllers
{
    [Route("{db_name}/[controller]")]
    [ApiController]
    public class SettingsController : ControllerBase
    {
        private readonly IUserServices _userServices;
        private readonly IOrganizationServices _organizationServices;
        private readonly ILogServices _logServices;
        private readonly IRolesServices _rolesServices;

        public SettingsController(ILogServices logServices, IOrganizationServices organizationServices, IUserServices userServices, IRolesServices rolesServices)
        {
            _logServices = logServices;
            _organizationServices = organizationServices;
            _userServices = userServices;
            _rolesServices = rolesServices;
        }

        [HttpPost]
        [Route("changePassword")]
        public async Task<IActionResult> ChangePassword(ChangePassword data)
        {
            var exist = await _userServices.UserExist(data.UserId);
            if (!exist) return NotFound();
            var verify = await _userServices.VerifyUserPassword(data.UserId, data.OldPassword);
            if (!verify) return Unauthorized();
            var checkNewPass = await _userServices.VerifyUserPassword(data.UserId, data.NewPassword);
            if (checkNewPass) return BadRequest();
            await _userServices.ModifyPassword(data.UserId, data.NewPassword);
            return Ok();
        }
        [HttpPost]
        [Route("modifyUser")]
        public async Task<IActionResult> ModifyUser(ChangeUserData data)
        {
            var exist = await _userServices.UserExist(data.UserId);
            if (!exist) return NotFound();
            if (data.Email != null)
            {
                var emailExist = await _userServices.EmailExist(data.Email);
                if (emailExist) return BadRequest();
            }
            await _userServices.ModifyUserData(data.UserId, data.Email, data.Username, data.Surname);
            var logTypeId = await _logServices.getLogTypeId("Modify");
            await _logServices.CreateActionLog($"User with id {data.UserId} has changed their info.", data.UserId, logTypeId);
            return Ok();
        }
        [HttpPost]
        [Route("addNewUser/{userId}")]
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
        [HttpPost]
        [Route("switch")]
        public async Task<IActionResult> SwitchToOrg(int userId)
        {
            var exist = await _userServices.UserExist(userId);
            if (!exist) return NotFound();
            var orgId = await _userServices.GetOrgId(userId, true);
            var roleId = await _rolesServices.GetRoleId("Admin");
            var result = await _userServices.SwitchToOrg(userId, roleId, orgId);
            return result ? Ok() : BadRequest();
        }
        [HttpGet]
        [Route("logs")]
        public async Task<IActionResult> GetLogs(int userId)
        {
            var exist = await _userServices.UserExist(userId);
            if (!exist) return NotFound();
            var role = await _userServices.GetUserRole(userId);
            role ??= "Solo";
            Console.WriteLine(role);
            if (role != "Admin" && role != "Solo") return Unauthorized();
            var result = await _logServices.GetLogs();
            return Ok(result);
        }

        [HttpGet]
        [Route("userOrg")]
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
