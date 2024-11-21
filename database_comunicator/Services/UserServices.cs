using database_communicator.Data;
using database_communicator.Models;
using database_communicator.Models.DTOs;
using database_communicator.Models.DTOs.Create;
using database_communicator.Models.DTOs.Get;
using database_communicator.Utils;
using Microsoft.EntityFrameworkCore;

namespace database_communicator.Services
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
        public Task<GetUserBasicInfo> GetBasicInfo(int userId, bool isOrg);
        public Task<bool> ModifyPassword(int userId, string password);
        public Task<bool> ModifyUserData(int userId, string? email, string? username, string? surname);
        public Task<bool> SwitchToOrg(int userId, int roleId, int orgId);
        public Task<int> GetUserId(string email);
        public Task<string?> GetUserRole(int userId);
        public Task<bool> EmailExist(string email);
        public Task<int> GetOrgId(int userId, bool isOrg);
        public Task<bool> ModifyUserRole(int orgUserId, int roleId);
        public Task<int?> GetOrgUserId(int userId);
        public Task<string?> GetUserEmail(int userId);
        public Task<IEnumerable<GetUsers>> GetUsers();
        public Task<IEnumerable<GetUsers>> GetAccountantUser();
        public Task<string?> GetUserFullName(int userId);
        public Task<string?> GetUserOrg(int userId);
    }
    /// <summary>
    /// Class that interact with database and contains functions allowing to work on users.
    /// </summary>
    public class UserServices : IUserServices
    {
        private readonly HandlerContext _handlerContext;
        private readonly ILogger<UserServices> _logger;
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="handlerContext">Database context</param>
        /// <param name="logger">Log interface</param>
        public UserServices(HandlerContext handlerContext, ILogger<UserServices> logger)
        {
            _handlerContext = handlerContext;
            _logger = logger;
        }
        /// <summary>
        /// Using transactions add new user to database.
        /// </summary>
        /// <param name="user">New user data.</param>
        /// <param name="orgId">User organization id.</param>
        /// <param name="roleId">New user role id.</param>
        /// <param name="isOrg">True if you want create organization type user, false if solo type.</param>
        /// <returns>True if success or false if not.</returns>
        public async Task<bool> AddUser(AddUser user, int orgId, int roleId, bool isOrg)
        {
            using var trans = await _handlerContext.Database.BeginTransactionAsync();
            try
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

                await _handlerContext.AppUsers.AddAsync(newUser);
                await _handlerContext.SaveChangesAsync();
                await trans.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Create user error.");
                await trans.RollbackAsync();
                return false;
            }

        }
        /// <summary>
        /// Do select query to receive basic information about user.
        /// </summary>
        /// <param name="userId">User id.</param>
        /// <param name="isOrg">True if user is organization type, false if solo.</param>
        /// <returns>Object that contains user name, surname and his/her/theirs organization name.</returns>
        public async Task<GetUserBasicInfo> GetBasicInfo(int userId, bool isOrg)
        {

            if (isOrg)
            {
                return await _handlerContext.AppUsers.Include(a => a.OrgUser).ThenInclude(b => b!.Organizations).Where(e => e.IdUser == userId).Select(e => new GetUserBasicInfo
                {
                    Username = e.Username,
                    Surname = e.Surname,
                    OrgName = e.OrgUser!.Organizations.OrgName
                }).FirstAsync();
            }

            return await _handlerContext.AppUsers.Include(a => a.SoloUser).ThenInclude(b => b!.Organizations).Where(e => e.IdUser == userId).Select(e => new GetUserBasicInfo
            {
                Surname = e.Surname,
                OrgName = e.SoloUser!.Organizations.OrgName
            }).FirstAsync();
        }
        /// <summary>
        /// Do select query to receive user unread notification count.
        /// </summary>
        /// <param name="userId">User id.</param>
        /// <returns>Number of unread notifications</returns>
        public async Task<int> GetCountNotification(int userId)
        {
            var result = await _handlerContext.UserNotifications.Where(e => e.UsersId == userId && !e.IsRead).CountAsync();
            return result;
        }
        /// <summary>
        /// Checks if user with given email is organization type user.
        /// </summary>
        /// <param name="email">User email.</param>
        /// <returns>True if is organization type, otherwise false.</returns>
        public async Task<bool> IsOrgUser(string email)
        {
            var result = await _handlerContext.AppUsers.Where(e => e.Email == email).Select(e => e.OrgUserId).FirstOrDefaultAsync();
            return result != 0;
        }
        /// <summary>
        /// Checks if user with given id is organization type user.
        /// </summary>
        /// <param name="userId">User id.</param>
        /// <returns>True if is organization type, otherwise false.</returns>
        public async Task<bool> IsOrgUser(int userId)
        {
            var result = await _handlerContext.AppUsers.Where(e => e.IdUser == userId).Select(e => e.OrgUserId).FirstOrDefaultAsync();
            return result != 0;
        }
        /// <summary>
        /// Checks if user with given email exist.
        /// </summary>
        /// <param name="email">User email.</param>
        /// <returns>True if exist or false if not.</returns>
        public async Task<bool> UserExist(string email)
        {
            return await _handlerContext.AppUsers.Where(e => e.Email == email).AnyAsync();
        }
        /// <summary>
        /// Checks if user with given id exist.
        /// </summary>
        /// <param name="userId">User id.</param>
        /// <returns>True if exist or false if not.</returns>
        public async Task<bool> UserExist(int userId)
        {
            return await _handlerContext.AppUsers.Where(e => e.IdUser == userId).AnyAsync();
        }
        /// <summary>
        /// Verify if user password is correct.
        /// </summary>
        /// <param name="email">User email.</param>
        /// <param name="password">User password.</param>
        /// <returns>True if correct or false if not.</returns>
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
        /// <summary>
        /// Verify if user password is correct.
        /// </summary>
        /// <param name="userId">User id.</param>
        /// <param name="password">User password.</param>
        /// <returns>True if correct or false if not.</returns>
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
        /// <summary>
        /// Using transactions overwrites old user password with new one.
        /// </summary>
        /// <param name="userId">User id.</param>
        /// <param name="password">New password.</param>
        /// <returns>True if success or false if not.</returns>
        public async Task<bool> ModifyPassword(int userId, string password)
        {
            using var trans = await _handlerContext.Database.BeginTransactionAsync();
            try
            {
                var salt = Hasher.GenerateSalt();
                var passHash = Hasher.CreateHashPassword(password, salt);

                await _handlerContext.AppUsers.Where(e => e.IdUser == userId).ExecuteUpdateAsync(e =>
                e.SetProperty(s => s.PassHash, passHash)
                .SetProperty(s => s.PassSalt, salt)
                );
                await _handlerContext.SaveChangesAsync();
                await trans.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Modify user password error.");
                await trans.RollbackAsync();
                return false;
            }
        }
        /// <summary>
        /// Using transactions overwrites old user data with new one.
        /// </summary>
        /// <param name="userId">Id of user to overwrite.</param>
        /// <param name="email">User new email.</param>
        /// <param name="username">User new username.</param>
        /// <param name="surname">User new surname.</param>
        /// <returns>True if success or false if not.</returns>
        public async Task<bool> ModifyUserData(int userId, string? email, string? username, string? surname)
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
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Modify user data error.");
                await trans.RollbackAsync();
                return false;
            }
        }
        /// <summary>
        /// Change solo user to org user with admin role which switch the type of web application to organization.
        /// </summary>
        /// <param name="userId">Id of solo type user.</param>
        /// <param name="roleId">Id of admin role.</param>
        /// <param name="orgId">Given user organization id.</param>
        /// <returns>True if success or false if not.</returns>
        public async Task<bool> SwitchToOrg(int userId, int roleId, int orgId)
        {
            using var trans = await _handlerContext.Database.BeginTransactionAsync();
            try
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
                await trans.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Switch application type error.");
                await trans.RollbackAsync();
                return false;
            }
        }
        /// <summary>
        /// Do select query to receive user id from database.
        /// </summary>
        /// <param name="email">User email.</param>
        /// <returns>Id of user or 0 if user not found.</returns>
        public async Task<int> GetUserId(string email)
        {
            return await _handlerContext.AppUsers.Where(e => e.Email.Equals(email)).Select(e => e.IdUser).FirstOrDefaultAsync();
        }
        /// <summary>
        /// Do select query to receive user role name from database.
        /// </summary>
        /// <param name="userId">User id.</param>
        /// <returns>Role name or null if user do note exist or is solo type.</returns>
        public async Task<string?> GetUserRole(int userId)
        {
            return await _handlerContext.AppUsers
                .Where(e => e.IdUser == userId)
                .Include(e => e.OrgUser)
                .ThenInclude(e => e!.Role)
                .Select(e => e.OrgUser!.Role.RoleName)
                .FirstOrDefaultAsync();
        }
        /// <summary>
        /// Checks if email already exists in database.
        /// </summary>
        /// <param name="email">Email to check.</param>
        /// <returns>True if exist or false if not</returns>
        public async Task<bool> EmailExist(string email)
        {
            return await _handlerContext.AppUsers.AnyAsync(e => e.Email.Equals(email));
        }
        /// <summary>
        /// Do select query to receive given user organization id from database.
        /// </summary>
        /// <param name="userId">User id.</param>
        /// <param name="isOrg">True if user is organization type, false if solo.</param>
        /// <returns>Id of organization or 0 when user do not exist.</returns>
        public async Task<int> GetOrgId(int userId, bool isOrg)
        {
            if (isOrg)
            {
                return await _handlerContext.AppUsers.Where(e => e.IdUser == userId).Include(e => e.OrgUser).Select(e => e.OrgUser!.OrganizationsId).FirstOrDefaultAsync();
            }
            return await _handlerContext.AppUsers.Where(e => e.IdUser == userId).Include(e => e.SoloUser).Select(e => e.SoloUser!.OrganizationsId).FirstOrDefaultAsync();
        }
        /// <summary>
        /// Using transactions change user role to given value.
        /// </summary>
        /// <param name="orgUserId">Id of organization type user.</param>
        /// <param name="roleId">Id of new role.</param>
        /// <returns>True if success or false if failure.</returns>
        public async Task<bool> ModifyUserRole(int orgUserId, int roleId)
        {
            using var trans = await _handlerContext.Database.BeginTransactionAsync();
            try
            {
                await _handlerContext.OrgUsers.Where(e => e.OrgUserId == orgUserId)
                .ExecuteUpdateAsync(setters => setters.SetProperty(s => s.RoleId, roleId));
                await _handlerContext.SaveChangesAsync();
                await trans.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Modify user role error.");
                await trans.RollbackAsync();
                return false;
            }
        }
        /// <summary>
        /// Do select query to get organization user id (not normal user id).
        /// </summary>
        /// <param name="userId">Id of user (main one).</param>
        /// <returns>Return organization id of user.</returns>
        public async Task<int?> GetOrgUserId(int userId)
        {
            return await _handlerContext.AppUsers.Where(e => e.IdUser == userId).Select(e => e.OrgUserId).FirstOrDefaultAsync();
        }
        /// <summary>
        /// Do select query to receive user email from database.
        /// </summary>
        /// <param name="userId">User id.</param>
        /// <returns>User email or null if user not found.</returns>
        public async Task<string?> GetUserEmail(int userId)
        {
            return await _handlerContext.AppUsers.Where(e => e.IdUser == userId).Select(e => e.Email).FirstOrDefaultAsync();
        }
        /// <summary>
        /// Do select query to receive list of existing users from database.
        /// </summary>
        /// <returns>List of users with theirs ids, name and surname.</returns>
        public async Task<IEnumerable<GetUsers>> GetUsers()
        {
            return await _handlerContext.AppUsers.Select(e => new GetUsers
            {
                IdUser = e.IdUser,
                Username = e.Username,
                Surname = e.Surname
            }).ToListAsync();
        }
        /// <summary>
        /// Do select query to receive list of existing users with accountant and admin roles from database.
        /// </summary>
        /// <returns>List of users with theirs ids, name and surname.</returns>
        public async Task<IEnumerable<GetUsers>> GetAccountantUser()
        {
            return await _handlerContext.AppUsers
                .Where(e => e.OrgUser!.Role.RoleName == "Accountant" || e.OrgUser!.Role.RoleName == "Admin")
                .Select(e => new GetUsers
                {
                    IdUser = e.IdUser,
                    Username = e.Username,
                    Surname = e.Surname
                }).ToListAsync();
        }
        /// <summary>
        /// Do select query to receive user name and surname as one string from database.
        /// </summary>
        /// <param name="userId">User id.</param>
        /// <returns>String containing name and surname of give user or null if user not found.</returns>
        public async Task<string?> GetUserFullName(int userId)
        {
            return await _handlerContext.AppUsers
                .Where(e => e.IdUser == userId)
                .Select(e => e.Username + " " + e.Surname).FirstOrDefaultAsync();
        }
        /// <summary>
        ///  Do select query to receive user organization name from database.
        /// </summary>
        /// <param name="userId">User id.</param>
        /// <returns>String containing user organization name or null if user not found.</returns>
        public async Task<string?> GetUserOrg(int userId)
        {
            var isSolo = await _handlerContext.AppUsers.AnyAsync(x => x.IdUser == userId && x.OrgUserId == null);
            if (isSolo)
            {
                return await _handlerContext.AppUsers.Where(e => e.IdUser == userId).Select(e => e.SoloUser!.Organizations.OrgName).FirstOrDefaultAsync();
            }
            else
            {
                return await _handlerContext.AppUsers.Where(e => e.IdUser == userId).Select(e => e.OrgUser!.Organizations.OrgName).FirstOrDefaultAsync();
            }
        }
    }
}
