using database_comunicator.Data;
using database_comunicator.Models;
using database_comunicator.Models.DTOs;
using database_comunicator.Utils;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace database_comunicator.Services
{
    public interface IUserServices
    {
        public Task<bool> AddUser(AddUser user, int roleId, bool isOrg);
        public Task<int> GetRoleId(string roleName);
        public Task<bool> UserExist(string email);
        public Task<bool> VerifyUserPassword(string email, string password);
    }
    public class UserServices : IUserServices
    {
        private readonly HandlerContext _handlerContext;
        public UserServices(HandlerContext handlerContext)
        {
            _handlerContext = handlerContext;
        }
        public async Task<bool> AddUser(AddUser user, int roleId, bool isOrg)
        {
            if ((roleId == -1) && (isOrg == true))
            {
                return false;
            }
            var userSalt = Hasher.GenerateSalt();
            var userHashedPassword = Hasher.CreateHashPassword(user.Password, userSalt);
            Console.WriteLine(user.OrganizationsId);

            var newUser = new AppUser
            {
                Email = user.Email,
                Username = user.Username,
                Surname = user.Surname,
                PassHash = userHashedPassword,
                PassSalt = userSalt,
                SoloUser = isOrg ? null : new SoloUser
                {
                    OrganizationsId = user.OrganizationsId,
                },
                OrgUser = isOrg ? new OrgUser
                {
                    OrganizationsId = user.OrganizationsId,
                    RoleId = roleId,
                } : null,
            };

            await _handlerContext.AppUsers.AddAsync(newUser);
            await _handlerContext.SaveChangesAsync();
            return true;
        }

        public async Task<int> GetRoleId(string roleName)
        {
            var result = await _handlerContext.UserRoles.Where(e => e.RoleName.Equals(roleName)).Select(e => e.RoleId).ToListAsync();
            return result.First();
        }

        public async Task<bool> UserExist(string email)
        {
            return await _handlerContext.AppUsers.Where(e => e.Email == email).AnyAsync();
        }

        public async Task<bool> VerifyUserPassword(string email, string password)
        {
            var hashes = await _handlerContext.AppUsers.Where(e => e.Email == email).Select(e => new UserHash
            {
                PassHash = e.PassHash,
                PassSalt = e.PassSalt
            }).ToListAsync();

            var passHas = hashes.First().PassHash;
            var salt = hashes.First().PassSalt;

            return Hasher.VerifyPassword(passHas, salt, password);
        }
    }
}
