using database_communicator.Data;
using database_communicator.Models;
using database_communicator.Models.DTOs;
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
    public class RolesServices : IRolesServices
    {
        private readonly HandlerContext _handlerContext;
        public RolesServices(HandlerContext handlerContext)
        {
            _handlerContext = handlerContext;
        }
        public async Task<int> GetRoleId(string roleName)
        {
            var result = await _handlerContext.UserRoles.Where(e => e.RoleName.Equals(roleName)).Select(e => e.RoleId).ToListAsync();
            return result[0];
        }

        public async Task<IEnumerable<string>> GetRoleNames()
        {
            return await _handlerContext.UserRoles.Select(e => e.RoleName).ToListAsync();
        }
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
                direction = sort.StartsWith("D");
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
                direction = sort.StartsWith("D");
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
