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
        public Task<bool> AddUser(AddUser user, int orgId, int roleId, bool isOrg);
        public Task<bool> UserExist(string email);
        public Task<bool> UserExist(int userId);
        public Task<bool> IsOrgUser(string email);
        public Task<bool> IsOrgUser(int userId);
        public Task<bool> VerifyUserPassword(string email, string password);
        public Task<bool> VerifyUserPassword(int userId, string password);
        public Task<int> GetCountNotification(int userId);
        public Task<BasicInfo> GetBasicInfo(int userId, bool isOrg);
        public Task SetOrgUserRole(int orgUserId, int roleId);
        public Task<IEnumerable<GetOrgUsersWithRoles>> GetOrgUsersWithRoles(int userId);
        public Task<IEnumerable<GetOrgUsersWithRoles>> GetOrgUsersWithRoles(int userId, string search);
        public Task ModifyPassword(int userId, string password);
        public Task ModifyUserData(int userId, string? email, string? username, string? surname);
        public Task<bool> SwitchToOrg(int userId, int roleId, int orgId);
        public Task<int> GetUserId(string email);
        public Task<string> GetUserRole(int userId);
        public Task<bool> EmailExist(string email);
        public Task<int> GetOrgId(int userId, bool isOrg);
        public Task ModifyUserRole(int orgUserId, int roleId);
        public Task<int?> GetOrgUserId(int userId);
        public Task<string> GetUserEmail(int userId);
        public Task<IEnumerable<GetUsers>> GetUsers();
        public Task<IEnumerable<GetUsers>> GetAccountantUser();
        public Task<string> GetUserFullName(int userId);
        public Task<string> GetUserOrg(int userId);
    }
    public class UserServices : IUserServices
    {
        private readonly HandlerContext _handlerContext;
        public UserServices(HandlerContext handlerContext)
        {
            _handlerContext = handlerContext;
        }
        public async Task<bool> AddUser(AddUser user, int orgId, int roleId, bool isOrg)
        {
            if ((roleId == -1) && (isOrg))
            {
                return false;
            }
            var userSalt = Hasher.GenerateSalt();
            var userHashedPassword = Hasher.CreateHashPassword(user.Password, userSalt);

            var newUser = new AppUser
            {
                Email = user.Email,
                Username = user.Username,
                Surname = user.Surname,
                PassHash = userHashedPassword,
                PassSalt = userSalt,
                SoloUser = isOrg ? null : new SoloUser
                {
                    OrganizationsId = orgId,
                },
                OrgUser = isOrg ? new OrgUser
                {
                    OrganizationsId = orgId,
                    RoleId = roleId,
                } : null,
            };

            _handlerContext.AppUsers.Add(newUser);
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

                return result[0];
            }

            result = await _handlerContext.AppUsers.Include(a => a.SoloUser).ThenInclude(b => b!.Organizations).Where(e => e.IdUser == userId).Select(e => new BasicInfo
            {
                Username = e.Username,
                Surname = e.Surname,
                OrgName = e.SoloUser!.Organizations.OrgName
            }).ToListAsync();

            return result[0];
        }

        public async Task<int> GetCountNotification(int userId)
        {
            var result = await _handlerContext.UserNotifications.Where(e => e.UsersId == userId && !e.IsRead).CountAsync();
            return result;
        }

        public async Task<IEnumerable<GetOrgUsersWithRoles>> GetOrgUsersWithRoles(int userId)
        {
            return await _handlerContext.AppUsers
                .Where(e => e.IdUser != userId && e.OrgUser != null)
                .Include(e => e.OrgUser)
                    .ThenInclude(e => e!.Role)
                .Select(e => new GetOrgUsersWithRoles
                {
                    UserId = e.IdUser,
                    Username = e.Username,
                    Surname = e.Surname,
                    RoleName = e.OrgUser!.Role.RoleName
                }).ToListAsync();
        }

        public async Task<IEnumerable<GetOrgUsersWithRoles>> GetOrgUsersWithRoles(int userId, string search)
        {
            return await _handlerContext.AppUsers
                .Where(ent => ent.IdUser != userId 
                && (EF.Functions.FreeText(ent.Surname, search) || EF.Functions.FreeText(ent.Username, search))
                && ent.OrgUser != null
                )
                .Include(ent => ent.OrgUser)
                    .ThenInclude(ad => ad!.Role)
                .Select(ent => new GetOrgUsersWithRoles
                {
                    UserId = ent.IdUser,
                    Username = ent.Username,
                    Surname = ent.Surname,
                    RoleName = ent.OrgUser!.Role.RoleName
                }).ToListAsync();
        }

        public async Task<bool> IsOrgUser(string email)
        {
            var result = await _handlerContext.AppUsers.Where(e => e.Email == email).Select(e => e.OrgUserId).ToListAsync();
            return result[0] != null;
        }
        public async Task<bool> IsOrgUser(int userId)
        {
            var result = await _handlerContext.AppUsers.Where(e => e.IdUser == userId).Select(e => e.OrgUserId).ToListAsync();
            return result[0] != null;
        }

        public async Task SetOrgUserRole(int orgUserId, int roleId)
        {
            await _handlerContext.OrgUsers.Where(e => e.OrgUserId == orgUserId).ExecuteUpdateAsync(e => e.SetProperty(s => s.RoleId, roleId));
            await _handlerContext.SaveChangesAsync();
        }

        public async Task<bool> UserExist(string email)
        {
            return await _handlerContext.AppUsers.Where(e => e.Email == email).AnyAsync();
        }
        public async Task<bool> UserExist(int userId)
        {
            return await _handlerContext.AppUsers.Where(e => e.IdUser == userId).AnyAsync();
        }

        public async Task<bool> VerifyUserPassword(string email, string password)
        {

            var instance = await _handlerContext.AppUsers.Where(e => e.Email.Equals(email)).Select(e => new UserHash
            {
                PassHash = e.PassHash,
                PassSalt = e.PassSalt
            }).ToListAsync();
            var passHas = instance[0].PassHash;
            var salt = instance[0].PassSalt;
            var isVerified = Hasher.VerifyPassword(passHas, salt, password);

            return isVerified;
        }
        public async Task<bool> VerifyUserPassword(int userId, string password)
        {
            var instance = await _handlerContext.AppUsers.Where(e => e.IdUser == userId).Select(e => new UserHash
            {
                PassHash = e.PassHash,
                PassSalt = e.PassSalt
            }).ToListAsync();
            var passHas = instance[0].PassHash;
            var salt = instance[0].PassSalt;
            var isVerified = Hasher.VerifyPassword(passHas, salt, password);

            return isVerified;
        }
        public async Task ModifyPassword(int userId, string password)
        {
            var salt = Hasher.GenerateSalt();
            var passHash = Hasher.CreateHashPassword(password, salt);

            await _handlerContext.AppUsers.Where(e => e.IdUser == userId).ExecuteUpdateAsync(e => 
            e.SetProperty(s => s.PassHash, passHash)
            .SetProperty(s => s.PassSalt, salt)
            );
            await _handlerContext.SaveChangesAsync();
        }
        public async Task ModifyUserData(int userId, string? email, string? username, string? surname)
        {
            using var trans = await _handlerContext.Database.BeginTransactionAsync();

            try
            {
                if (email is not null)
                {
                    await _handlerContext.AppUsers
                    .Where(e => e.IdUser == userId)
                    .ExecuteUpdateAsync(setters => setters.SetProperty(s => s.Email, email));
                }

                if (username is not null)
                {
                    await _handlerContext.AppUsers
                    .Where(e => e.IdUser == userId)
                    .ExecuteUpdateAsync(setters => setters.SetProperty(s => s.Username, username));
                }

                if (surname is not null)
                {
                    await _handlerContext.AppUsers
                    .Where(e => e.IdUser == userId)
                    .ExecuteUpdateAsync(setters => setters.SetProperty(s => s.Surname, surname));
                }

                await _handlerContext.SaveChangesAsync();
                await trans.CommitAsync();
            } catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                await trans.RollbackAsync();
            }
        }
        public async Task<bool> SwitchToOrg(int userId, int roleId, int orgId)
        {
            var soloUserId = await _handlerContext.AppUsers.Where(e => e.IdUser == userId).Select(e => e.SoloUserId).FirstOrDefaultAsync();
            if (soloUserId == null) return false;
            var changedUser = new AppUser
            {
                IdUser = userId,
                SoloUserId = null,
                OrgUser = new OrgUser
                {
                    RoleId = roleId,
                    OrganizationsId = orgId
                }
            };

            _handlerContext.Update<AppUser>(changedUser);
            _handlerContext.SoloUsers.Remove(new SoloUser { SoloUserId = (int)soloUserId, OrganizationsId = orgId });
            await _handlerContext.SaveChangesAsync();
            return true;
        }
        public async Task<int> GetUserId(string email)
        {
            return await _handlerContext.AppUsers.Where(e => e.Email.Equals(email)).Select(e => e.IdUser).FirstAsync();
        }
        public async Task<string> GetUserRole(int userId)
        {
            return await _handlerContext.AppUsers
                .Where(e => e.IdUser == userId)
                .Include(e => e.OrgUser)
                .ThenInclude(e => e!.Role)
                .Select(e => e.OrgUser!.Role.RoleName)
                .FirstAsync();
        }
        public async Task<bool> EmailExist(string email)
        {
            return await _handlerContext.AppUsers.Where(e => e.Email.Equals(email)).AnyAsync();
        }
        public async Task<int> GetOrgId(int userId, bool isOrg)
        {
            if (isOrg)
            {
                return await _handlerContext.AppUsers.Where(e => e.IdUser == userId).Include(e => e.OrgUser).Select(e => e.OrgUser!.OrganizationsId).FirstAsync();
            }
            return await _handlerContext.AppUsers.Where(e => e.IdUser == userId).Include(e => e.SoloUser).Select(e => e.SoloUser!.OrganizationsId).FirstAsync();
        }
        public async Task ModifyUserRole(int orgUserId, int roleId)
        {
            await _handlerContext.OrgUsers.Where(e => e.OrgUserId == orgUserId)
                .ExecuteUpdateAsync(setters => setters.SetProperty(s => s.RoleId, roleId));
            await _handlerContext.SaveChangesAsync();
        }
        public async Task<int?> GetOrgUserId(int userId)
        {
            return await _handlerContext.AppUsers.Where(e => e.IdUser == userId).Select(e => e.OrgUserId).FirstAsync();
        }
        public async Task<string> GetUserEmail(int userId)
        {
            return await _handlerContext.AppUsers.Where(e => e.IdUser == userId).Select(e => e.Email).FirstAsync();
        }
        public async Task<IEnumerable<GetUsers>> GetUsers()
        {
            return await _handlerContext.AppUsers.Select(e => new GetUsers
            {
                IdUser = e.IdUser,
                Username = e.Username,
                Surname = e.Surname
            }).ToListAsync();
        }
        public async Task<IEnumerable<GetUsers>> GetAccountantUser()
        {
            return await _handlerContext.AppUsers
                .Where(e => e.OrgUser!.Role.RoleName == "Merchant" || e.OrgUser!.Role.RoleName == "Admin")
                .Select(e => new GetUsers
            {
                IdUser = e.IdUser,
                Username = e.Username,
                Surname = e.Surname
            }).ToListAsync();
        }
        public async Task<string> GetUserFullName(int userId)
        {
            return await _handlerContext.AppUsers
                .Where(e => e.IdUser == userId)
                .Select(e => e.Username + " " + e.Surname).FirstAsync();
        }
        public async Task<string> GetUserOrg(int userId)
        {
            var isSolo = await _handlerContext.AppUsers.AnyAsync(x => x.IdUser == userId && x.OrgUserId == null);
            string result;
            if (isSolo)
            {
                result = await _handlerContext.AppUsers.Where(e => e.IdUser == userId).Select(e => e.SoloUser!.Organizations.OrgName).FirstAsync();
            } else
            {
                result = await _handlerContext.AppUsers.Where(e => e.IdUser == userId).Select(e => e.OrgUser!.Organizations.OrgName).FirstAsync();
            }
            return result;
        }
    }
}
