﻿using database_communicator.Models.DTOs;
using database_communicator.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace database_communicator.Controllers
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
        [Route("get/{userId}")]
        public async Task<IActionResult> GetUserWithRoles(int userId, string? search, string? sort, string? role)
        {
            var exist = await _userServices.UserExist(userId);
            if (!exist) return NotFound();
            var isOrgUser = await _userServices.IsOrgUser(userId);
            if (!isOrgUser) return BadRequest();
            IEnumerable<GetOrgUsersWithRoles> result;
            if (search == null)
            {
                result = await _rolesServices.GetOrgUsersWithRoles(userId, sort: sort, role);
            } else
            {
                result = await _rolesServices.GetOrgUsersWithRoles(userId, search, sort: sort, role);
            }
            return Ok(result);
        }
        [HttpGet]
        [Route("get")]
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
                return NotFound("Chosen user not found.");
            }
            await _userServices.ModifyUserRole((int)orgUserId, roleId);
            return Ok();
        }

    }
}
