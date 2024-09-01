using database_comunicator.Models.DTOs;
using database_comunicator.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace database_comunicator.Controllers
{
    [Route("{db_name}/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly IRolesServices _rolesServices;
        private readonly IUserServices _userServices;
        public RolesController(IRolesServices rolesServices, IUserServices userServices)
        {
            _rolesServices = rolesServices;
            _userServices = userServices;
        }
        [HttpGet]
        [Route("getUserRoles")]
        public async Task<IActionResult> GetUserWithRoles(int userId, string? search)
        {
            var exist = await _userServices.UserExist(userId);
            if (!exist) return NotFound();
            IEnumerable<GetOrgUsersWithRoles> result;
            if (search == null)
            {
                result = await _userServices.GetOrgUsersWithRoles(userId);
            } else
            {
                result = await _userServices.GetOrgUsersWithRoles(userId, search);
            }
            return Ok(result);
        }
        [HttpGet]
        [Route("roles")]
        public async Task<IActionResult> GetRoles()
        {
            var result = await _rolesServices.GetRoleNames();
            return Ok(result);
        }

        [HttpPost]
        [Route("modify")]
        public async Task<IActionResult> ModifyUserRole(ModifyUserRole data)
        {
            var exist = await _userServices.UserExist(data.UserId);
            if (!exist) return NotFound("User not found.");
            var roleId = await _rolesServices.GetRoleId(data.RoleName);
            var orgUserId = await _userServices.GetOrgUserId(data.ChoosenUserId);
            if (orgUserId == null)
            {
                return NotFound("Choosen user not found.");
            }
            await _userServices.ModifyUserRole((int)orgUserId, roleId);
            return Ok();
        }

    }
}
