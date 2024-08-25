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
        public Task<bool> UserExist(int userId);
        public Task<bool> IsOrgUser(string email);
        public Task<bool> IsOrgUser(int userId);
        public Task<SuccesLogin?> VerifyUserPassword(string email, string password, bool isOrg);
        public Task<int> GetCountNotification(int userId);
        public Task<BasicInfo> GetBasicInfo(int userId, bool isOrg);
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

        public async Task<BasicInfo> GetBasicInfo(int userId, bool isOrg)
        {
            List<BasicInfo> result;
            if (isOrg)
            {
                result = await _handlerContext.AppUsers.Include(a => a.OrgUser).ThenInclude(b => b!.Organizations).Where(e => e.IdUser == userId).Select(e => new BasicInfo
                {
                    Username = e.Username,
                    Surname = e.Surname,
                    OrgName = e.OrgUser!.Organizations.OrgName
                }).ToListAsync();

                return result.First();
            }

            result = await _handlerContext.AppUsers.Include(a => a.SoloUser).ThenInclude(b => b!.Organizations).Where(e => e.IdUser == userId).Select(e => new BasicInfo
            {
                Username = e.Username,
                Surname = e.Surname,
                OrgName = e.SoloUser!.Organizations.OrgName
            }).ToListAsync();

            return result.First();
        }

        public async Task<int> GetCountNotification(int userId)
        {
            var result = await _handlerContext.UserNotifications.Where(e => e.UsersId == userId && e.IsRead == false).ToListAsync();
            return result.Count;
        }

        public async Task<int> GetRoleId(string roleName)
        {
            var result = await _handlerContext.UserRoles.Where(e => e.RoleName.Equals(roleName)).Select(e => e.RoleId).ToListAsync();
            return result.First();
        }

        public async Task<bool> IsOrgUser(string email)
        {
            var result = await _handlerContext.AppUsers.Where(e => e.Email == email).Select(e => e.OrgUserId).ToListAsync();
            return result.First() != null;
        }
        public async Task<bool> IsOrgUser(int userId)
        {
            var result = await _handlerContext.AppUsers.Where(e => e.IdUser == userId).Select(e => e.OrgUserId).ToListAsync();
            return result.First() != null;
        }

        public async Task<bool> UserExist(string email)
        {
            return await _handlerContext.AppUsers.Where(e => e.Email == email).AnyAsync();
        }
        public async Task<bool> UserExist(int userId)
        {
            return await _handlerContext.AppUsers.Where(e => e.IdUser == userId).AnyAsync();
        }

        public async Task<SuccesLogin?> VerifyUserPassword(string email, string password, bool isOrg)
        {
            List<UserHash> hashes;

            if (isOrg) {
                hashes = await _handlerContext.AppUsers
                .Include(e => e.OrgUser)
                .ThenInclude(b => b!.Role)
                .Where(e => e.Email == email).Select(e => new UserHash
                {
                    Id = e.IdUser,
                    PassHash = e.PassHash,
                    PassSalt = e.PassSalt,
                    Role = e.OrgUser!.Role.RoleName
                }).ToListAsync();
            } else
            {
                hashes = await _handlerContext.AppUsers
                .Where(e => e.Email == email).Select(e => new UserHash
                {
                    Id = e.IdUser,
                    PassHash = e.PassHash,
                    PassSalt = e.PassSalt,
                    Role = "Solo"
                }).ToListAsync();
            }

            var instance = hashes.First();
            var passHas = instance.PassHash;
            var salt = instance.PassSalt;
            var isVerified = Hasher.VerifyPassword(passHas, salt, password);
            if (isVerified)
            {
                return new SuccesLogin
                {
                    Id = instance.Id,
                    Role = instance.Role
                };
            }

            return null;
        }
    }
}
