using database_communicator.Models.DTOs.Get;
using database_communicator.Models.DTOs.Modify;
using database_communicator.Services;
using Microsoft.AspNetCore.Mvc;

namespace database_communicator.Controllers
{
    /// <summary>
    /// This controller works on Roles table and few other related tables. Used for receiving, modifying and creating data 
    /// that holds users roles information. Use db_name parameter to pass the name of database that you want ot connect.
    /// </summary>
    [Route("{db_name}/[controller]")]
    [ApiController]
    public class RolesController(IRolesServices rolesServices, IUserServices userServices) : ControllerBase
    {
        private readonly IRolesServices _rolesServices = rolesServices;
        private readonly IUserServices _userServices = userServices;

        /// <summary>
        /// Tries to receive list of users with their roles from database.
        /// </summary>
        /// <param name="userId">Id of user that's activating this action.</param>
        /// <param name="search">The phrase searched in user information. It will check if phrase exist in username or surname.</param>
        /// <param name="sort">Contains parameter that object will be sorted by. Must start with D or A to determine ascending order. Then is follow by name of property.</param>
        /// <param name="role">Filter that search for user with role equal to given value</param>
        /// <returns>200 code with list of <see cref="GetOrgUsersWithRoles"/>, 404 when user is not found or 400 when user is not using organization type or when given sort is incorrect.</returns>
        [HttpGet]
        [Route("get/{userId}")]
        public async Task<IActionResult> GetUserWithRoles(int userId, string? search, string? sort, string? role)
        {
            if (sort != null)
            {
                bool isSortOk = sort.StartsWith('A') || sort.StartsWith('D');
                if (!isSortOk) return BadRequest("Sort value is incorrect.");
            }
            var exist = await _userServices.UserExist(userId);
            if (!exist) return NotFound();
            var isOrgUser = await _userServices.IsOrgUser(userId);
            if (!isOrgUser) return BadRequest();
            IEnumerable<GetOrgUsersWithRoles> result;
            if (search == null)
            {
                result = await _rolesServices.GetOrgUsersWithRoles(userId, sort: sort, role);
            }
            else
            {
                result = await _rolesServices.GetOrgUsersWithRoles(userId, search, sort: sort, role);
            }
            return Ok(result);
        }
        /// <summary>
        /// Tries to receive list of roles from database.
        /// </summary>
        /// <returns>200 code wit list of strings including role names.</returns>
        [HttpGet]
        [Route("get")]
        public async Task<IActionResult> GetRoles()
        {
            var result = await _rolesServices.GetRoleNames();
            return Ok(result);
        }
        /// <summary>
        /// Change chosen user role.
        /// </summary>
        /// <param name="data">Id of user and name of new role assigned to.</param>
        /// <returns>200 code when success or 404 when user not found.</returns>
        [HttpPost]
        [Route("modify")]
        public async Task<IActionResult> ModifyUserRole(ModifyUserRole data)
        {
            var exist = await _userServices.UserExist(data.UserId);
            if (!exist) return NotFound("User not found.");
            var roleId = await _rolesServices.GetRoleId(data.RoleName);
            var orgUserId = await _userServices.GetOrgUserId(data.ChosenUserId);
            if (orgUserId == null)
            {
                return NotFound("Chosen user not found.");
            }
            var result = await _userServices.ModifyUserRole((int)orgUserId, roleId);
            return result ? Ok() : new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }

    }
}
