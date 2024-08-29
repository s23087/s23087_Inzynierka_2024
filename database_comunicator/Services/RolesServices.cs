using database_comunicator.Data;
using Microsoft.EntityFrameworkCore;

namespace database_comunicator.Services
{
    public interface IRolesServices
    {
        public Task<int> GetRoleId(string roleName);
        public Task<IEnumerable<string>> GetRoleNames();
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
    }
}
