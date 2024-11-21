using database_communicator.Data;
using database_communicator.Models;
using database_communicator.Models.DTOs.Get;
using database_communicator.Utils;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace database_communicator.Services
{
    public interface IRolesServices
    {
        public Task<int> GetRoleId(string roleName);
        public Task<IEnumerable<string>> GetRoleNames();
        public Task<IEnumerable<GetOrgUsersWithRoles>> GetOrgUsersWithRoles(int userId, string? sort, string? role);
        public Task<IEnumerable<GetOrgUsersWithRoles>> GetOrgUsersWithRoles(int userId, string search, string? sort, string? role);
    }
    /// <summary>
    /// Class that interact with database and contains functions allowing to work on roles.
    /// </summary>
    public class RolesServices : IRolesServices
    {
        private readonly HandlerContext _handlerContext;
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="handlerContext">Database context</param>
        public RolesServices(HandlerContext handlerContext)
        {
            _handlerContext = handlerContext;
        }
        /// <summary>
        /// Do select query to receive id of role with given name.
        /// </summary>
        /// <param name="roleName">Role name</param>
        /// <returns>Id of role or 0 if do not exist.</returns>
        public async Task<int> GetRoleId(string roleName)
        {
            return await _handlerContext.UserRoles.Where(e => e.RoleName.Equals(roleName)).Select(e => e.RoleId).FirstOrDefaultAsync();
        }
        /// <summary>
        /// Do select query to receive list containing role names.
        /// </summary>
        /// <returns>List of role names.</returns>
        public async Task<IEnumerable<string>> GetRoleNames()
        {
            return await _handlerContext.UserRoles.Select(e => e.RoleName).ToListAsync();
        }
        /// <summary>
        /// Do select query to receive sorted and filtered user list with roles from database.
        /// </summary>
        /// <param name="userId">Id of user activating this action.</param>
        /// <param name="sort">Contains parameter that object will be sorted by. Must start with D or A to determine ascending order. Then is follow by name of property.</param>
        /// <param name="role">Filter value. If given the return value will only contain objects where role is equal to given value.</param>
        /// <returns>List of users with their roles.</returns>
        public async Task<IEnumerable<GetOrgUsersWithRoles>> GetOrgUsersWithRoles(int userId, string? sort, string? role)
        {
            Expression<Func<AppUser, Object>> sortFunc = sort == null ? e => true : e => e.Username;
            Expression<Func<AppUser, bool>> roleCond = role == null ? e => true : e => e.OrgUser!.Role.RoleName == role;
            bool direction;
            if (sort == null)
            {
                direction = true;
            }
            else
            {
                direction = sort.StartsWith('D');
            }
            return await _handlerContext.AppUsers
                .Where(e => e.IdUser != userId && e.OrgUser != null)
                .Where(roleCond)
                .OrderByWithDirection(sortFunc, direction)
                .Select(e => new GetOrgUsersWithRoles
                {
                    UserId = e.IdUser,
                    Username = e.Username,
                    Surname = e.Surname,
                    RoleName = e.OrgUser!.Role.RoleName
                }).ToListAsync();
        }
        /// <summary>
        /// Do select query to receive searched, sorted and filtered user list with roles from database.
        /// </summary>
        /// <param name="userId">Id of user activating this action.</param>
        /// <param name="search">The phrase searched in user information. It will check if phrase exist in username or surname.</param>
        /// <param name="sort">Contains parameter that object will be sorted by. Must start with D or A to determine ascending order. Then is follow by name of property.</param>
        /// <param name="role">Filter value. If given the return value will only contain objects where role is equal to given value.</param>
        /// <returns>List of users with their roles.</returns>
        public async Task<IEnumerable<GetOrgUsersWithRoles>> GetOrgUsersWithRoles(int userId, string search, string? sort, string? role)
        {
            Expression<Func<AppUser, Object>> sortFunc = sort == null ? e => true : e => e.Username;
            Expression<Func<AppUser, bool>> roleCond = role == null ? e => true : e => e.OrgUser!.Role.RoleName == role;
            bool direction;
            if (sort == null)
            {
                direction = true;
            }
            else
            {
                direction = sort.StartsWith('D');
            }
            return await _handlerContext.AppUsers
                .Where(ent => ent.IdUser != userId
                && (EF.Functions.FreeText(ent.Surname, search) || EF.Functions.FreeText(ent.Username, search))
                && ent.OrgUser != null
                )
                .Where(roleCond)
                .OrderByWithDirection(sortFunc, direction)
                .Select(ent => new GetOrgUsersWithRoles
                {
                    UserId = ent.IdUser,
                    Username = ent.Username,
                    Surname = ent.Surname,
                    RoleName = ent.OrgUser!.Role.RoleName
                }).ToListAsync();
        }
    }
}
