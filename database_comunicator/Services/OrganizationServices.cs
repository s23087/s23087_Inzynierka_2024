using database_communicator.Data;
using database_communicator.FilterClass;
using database_communicator.Models;
using database_communicator.Models.DTOs.Create;
using database_communicator.Models.DTOs.Get;
using database_communicator.Models.DTOs.Modify;
using database_communicator.Utils;
using Microsoft.EntityFrameworkCore;

namespace database_communicator.Services
{
    public interface IOrganizationServices
    {
        /// <summary>
        /// Using transactions add organization to database. If user id is passed will make this new organization as client to given user.
        /// </summary>
        /// <param name="org">New organization data</param>
        /// <param name="userId">USer id.</param>
        /// <returns>Id of new organization or 0 if failed</returns>
        public Task<int> AddOrganization(AddOrganization org, int? userId);
        public Task<int> GetCountryId(string countryName);
        public Task<bool> CountryExist(string countryName);
        public Task<GetOrg> GetOrg(int orgId);
        public Task<bool> ModifyOrg(ModifyOrg data);
        public Task<IEnumerable<GetClient>> GetClients(int userId, int userOrgId, string? sort, int? country);
        public Task<IEnumerable<GetClient>> GetClients(int userId, int userOrgId, string search, string? sort, int? country);
        public Task<IEnumerable<GetOrgClient>> GetOrgClients(int userOrgId, string? sort, int? country);
        public Task<IEnumerable<GetOrgClient>> GetOrgClients(int userOrgId, string search, string? sort, int? country);
        public Task<GetClientRestInfo> GetClientsRestInfo(int orgId);
        public Task<bool> SetClientAvailabilityStatus(int orgId, int statusId);
        public Task<bool> SetClientUserBindings(SetUserClientBindings data);
        public Task<bool> AddAvailabilityStatus(AddAvailabilityStatus data);
        public Task<IEnumerable<GetAvailabilityStatuses>> GetAvailabilityStatuses();
        public Task<bool> OrgExist(int orgId);
        public Task<bool> ManyUserExist(IEnumerable<int> users);
        public Task<bool> StatusExist(int statusId);
        public Task<IEnumerable<GetClientBindings>> GetClientBindings(int orgId);
        public Task<bool> OrgHaveRelations(int orgId, int userId);
        public Task<bool> DeleteOrg(int orgId);
    }
    /// <summary>
    /// Class that interact with database and contains functions allowing to work on organizations.
    /// </summary>
    public class OrganizationServices : IOrganizationServices
    {
        private readonly HandlerContext _handlerContext;
        private readonly ILogger<OrganizationServices> _logger;
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="handlerContext">Database context</param>
        /// <param name="logger">Log interface</param>
        public OrganizationServices(HandlerContext handlerContext, ILogger<OrganizationServices> logger)
        {
            _handlerContext = handlerContext;
            _logger = logger;
        }
        public async Task<int> AddOrganization(AddOrganization org, int? userId)
        {
            using var trans = await _handlerContext.Database.BeginTransactionAsync();
            try
            {
                var newOrg = new Organization
                {
                    OrgName = org.OrgName,
                    Nip = org.Nip,
                    Street = org.Street,
                    City = org.City,
                    PostalCode = org.PostalCode,
                    CreditLimit = org.CreditLimit,
                    CountryId = org.CountryId,
                    AvailabilityStatus = null
                };
                await _handlerContext.AddAsync<Organization>(newOrg);
                await _handlerContext.SaveChangesAsync();
                if (userId != null)
                {
                    await _handlerContext.Database.ExecuteSqlAsync($"insert into User_client (users_id, organization_id) Values ({userId}, {newOrg.OrganizationId})");
                    await _handlerContext.SaveChangesAsync();
                }
                await trans.CommitAsync();
                return newOrg.OrganizationId;
            }
            catch (Exception error)
            {
                _logger.LogError(error, "Create organization error.");
                await trans.RollbackAsync();
                return 0;
            }
        }
        /// <summary>
        /// Checks if country with given name exists in database.
        /// </summary>
        /// <param name="countryName">Country name</param>
        /// <returns>True if exist, false if not.</returns>
        public async Task<bool> CountryExist(string countryName)
        {
            return await _handlerContext.Countries.Where(e => e.CountryName == countryName).AnyAsync();
        }
        /// <summary>
        /// Do select query to receive id of country with given name.
        /// </summary>
        /// <param name="countryName">Country name</param>
        /// <returns>Id of country or 0 if not found.</returns>
        public async Task<int> GetCountryId(string countryName)
        {
            return await _handlerContext.Countries.Where(e => e.CountryName == countryName).Select(e => e.CountryId).FirstOrDefaultAsync();
        }
        /// <summary>
        /// Do select query to receive organization information with given id.
        /// </summary>
        /// <param name="orgId">Organization id</param>
        /// <returns>Object that contains chosen organization information.</returns>
        public async Task<GetOrg> GetOrg(int orgId)
        {
            return await _handlerContext.Organizations.Where(e => e.OrganizationId == orgId).Include(e => e.Country).Select(e => new GetOrg
            {
                Id = e.OrganizationId,
                OrgName = e.OrgName,
                Nip = e.Nip,
                Street = e.Street,
                City = e.City,
                PostalCode = e.PostalCode,
                CountryId = e.CountryId,
                Country = e.Country.CountryName
            }).FirstAsync();
        }
        /// <summary>
        /// Using transactions overwrites organizations properties with given new ones.
        /// </summary>
        /// <param name="data">New organization property values</param>
        /// <returns>True if success or false if failure.</returns>
        public async Task<bool> ModifyOrg(ModifyOrg data)
        {
            using var trans = await _handlerContext.Database.BeginTransactionAsync();
            try
            {
                await _handlerContext.Organizations.Where(e => e.OrganizationId == data.OrgId)
                    .ExecuteUpdateAsync(setter =>
                        setter.SetProperty(s => s.Nip, data.Nip)
                        .SetProperty(s => s.CreditLimit, data.CreditLimit)
                        .SetProperty(s => s.CountryId, data.CountryId)
                    );
                if (data.OrgName != null)
                {
                    await _handlerContext.Organizations.Where(e => e.OrganizationId == data.OrgId)
                        .ExecuteUpdateAsync(setter =>
                            setter.SetProperty(s => s.OrgName, data.OrgName)
                        );
                }
                if (data.Street != null)
                {
                    await _handlerContext.Organizations.Where(e => e.OrganizationId == data.OrgId)
                        .ExecuteUpdateAsync(setter =>
                            setter.SetProperty(s => s.Street, data.Street)
                        );
                }
                if (data.City != null)
                {
                    await _handlerContext.Organizations.Where(e => e.OrganizationId == data.OrgId)
                        .ExecuteUpdateAsync(setter =>
                            setter.SetProperty(s => s.City, data.City)
                        );
                }
                if (data.PostalCode != null)
                {
                    await _handlerContext.Organizations.Where(e => e.OrganizationId == data.OrgId)
                        .ExecuteUpdateAsync(setter =>
                            setter.SetProperty(s => s.PostalCode, data.PostalCode)
                        );
                }
                await _handlerContext.SaveChangesAsync();
                await trans.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Modify organization error.");
                await trans.RollbackAsync();
                return false;
            }
        }
        /// <summary>
        /// Do select query with given sort and filter to receive user clients information from database.
        /// </summary>
        /// <param name="userId">User id</param>
        /// <param name="userOrgId">User organization id</param>
        /// <param name="sort">Contains parameter that object will be sorted by. Must start with D or A to determine ascending order. Then is follow by name of property.</param>
        /// <param name="country">Filter value. If passed return client that country is equal to given value.</param>
        /// <returns>List of user clients.</returns>
        public async Task<IEnumerable<GetClient>> GetClients(int userId, int userOrgId, string? sort, int? country)
        {
            var sortFunc = SortFilterUtils.GetClientSort(sort);
            bool direction;
            if (sort == null)
            {
                direction = true;
            }
            else
            {
                direction = sort.StartsWith('D');
            }
            var countryCond = ClientFilters.GetCountryFilter(country);

            return await _handlerContext.Organizations
                .Where(d => d.OrganizationId != userOrgId && d.AppUsers.Any(x => x.IdUser == userId))
                .Where(countryCond)
                .OrderByWithDirection(sortFunc, direction)
                .Select(d => new GetClient
                {
                    ClientId = d.OrganizationId,
                    ClientName = d.OrgName,
                    Street = d.Street,
                    City = d.City,
                    Postal = d.PostalCode,
                    Nip = d.Nip,
                    Country = d.Country.CountryName

                }).ToListAsync();
        }
        /// <summary>
        /// Do select query with given sort and filter to receive user clients information from database.
        /// </summary>
        /// <param name="userId">User id</param>
        /// <param name="userOrgId">User organization id</param>
        /// <param name="search">The phrase searched in clients information. It will check if phrase exist in organization name, city or street.</param>
        /// <param name="sort">Contains parameter that object will be sorted by. Must start with D or A to determine ascending order. Then is follow by name of property.</param>
        /// <param name="country">Filter value. If passed return client that country is equal to given value.</param>
        /// <returns>List of user clients.</returns>
        public async Task<IEnumerable<GetClient>> GetClients(int userId, int userOrgId, string search, string? sort, int? country)
        {
            var sortFunc = SortFilterUtils.GetClientSort(sort);
            bool direction;
            if (sort == null)
            {
                direction = true;
            }
            else
            {
                direction = sort.StartsWith('D');
            }
            var countryCond = ClientFilters.GetCountryFilter(country);

            return await _handlerContext.Organizations
                .Where(e => e.OrganizationId != userOrgId && e.AppUsers.Any(x => x.IdUser == userId))
                .Where(e => EF.Functions.FreeText(e.OrgName, search) || EF.Functions.FreeText(e.Street, search) || EF.Functions.FreeText(e.City, search))
                .Where(countryCond)
                .OrderByWithDirection(sortFunc, direction)
                .Select(e => new GetClient
                {
                    ClientId = e.OrganizationId,
                    ClientName = e.OrgName,
                    Street = e.Street,
                    City = e.City,
                    Postal = e.PostalCode,
                    Nip = e.Nip,
                    Country = e.Country.CountryName

                }).ToListAsync();
        }
        /// <summary>
        /// Do select query with given sort and filter to receive organization clients information from database.
        /// </summary>
        /// <param name="userOrgId">User organization id</param>
        /// <param name="sort">Contains parameter that object will be sorted by. Must start with D or A to determine ascending order. Then is follow by name of property.</param>
        /// <param name="country">Filter value. If passed return client that country is equal to given value.</param>
        /// <returns>List of user clients.</returns>
        public async Task<IEnumerable<GetOrgClient>> GetOrgClients(int userOrgId, string? sort, int? country)
        {
            var sortFunc = SortFilterUtils.GetClientSort(sort);
            bool direction;
            if (sort == null)
            {
                direction = true;
            }
            else
            {
                direction = sort.StartsWith('D');
            }
            var countryCond = ClientFilters.GetCountryFilter(country);

            return await _handlerContext.Organizations
                .Where(e => e.OrganizationId != userOrgId)
                .Where(countryCond)
                .OrderByWithDirection(sortFunc, direction)
                .Select(e => new GetOrgClient
                {
                    ClientId = e.OrganizationId,
                    Users = e.AppUsers.Select(e => e.Username + " " + e.Surname).ToList(),
                    ClientName = e.OrgName,
                    Street = e.Street,
                    City = e.City,
                    Postal = e.PostalCode,
                    Nip = e.Nip,
                    Country = e.Country.CountryName
                }).ToListAsync();
        }
        /// <summary>
        /// Do select query with given search, sort and filter to receive organization clients information from database.
        /// </summary>
        /// <param name="userOrgId">User organization id</param>
        /// <param name="search">The phrase searched in clients information. It will check if phrase exist in organization name, city or street.</param>
        /// <param name="sort">Contains parameter that object will be sorted by. Must start with D or A to determine ascending order. Then is follow by name of property.</param>
        /// <param name="country">Filter value. If passed return client that country is equal to given value.</param>
        /// <returns>List of user clients.</returns>
        public async Task<IEnumerable<GetOrgClient>> GetOrgClients(int userOrgId, string search, string? sort, int? country)
        {
            var sortFunc = SortFilterUtils.GetClientSort(sort);
            bool direction;
            if (sort == null)
            {
                direction = true;
            }
            else
            {
                direction = sort.StartsWith('D');
            }
            var countryCond = ClientFilters.GetCountryFilter(country);

            return await _handlerContext.Organizations
                .Where(d => d.OrganizationId != userOrgId)
                .Where(d => EF.Functions.FreeText(d.OrgName, search) || EF.Functions.FreeText(d.Street, search) || EF.Functions.FreeText(d.City, search))
                .Where(countryCond)
                .OrderByWithDirection(sortFunc, direction)
                .Select(d => new GetOrgClient
                {
                    ClientId = d.OrganizationId,
                    Users = d.AppUsers.Select(e => e.Username + " " + e.Surname).ToList(),
                    ClientName = d.OrgName,
                    Street = d.Street,
                    City = d.City,
                    Postal = d.PostalCode,
                    Nip = d.Nip,
                    Country = d.Country.CountryName
                }).ToListAsync();
        }
        /// <summary>
        /// Do select query using given id to receive client information that was not given in bulk query.
        /// </summary>
        /// <param name="orgId">Client id</param>
        /// <returns>Object containing client information.</returns>
        public async Task<GetClientRestInfo> GetClientsRestInfo(int orgId)
        {
            return await _handlerContext.Organizations.Where(e => e.OrganizationId == orgId)
                .Include(e => e.AvailabilityStatus)
                .Select(e => new GetClientRestInfo
                {
                    CreditLimit = e.CreditLimit,
                    Availability = e.AvailabilityStatus == null ? "" : e.AvailabilityStatus.StatusName,
                    DaysForRealization = e.AvailabilityStatus == null ? 0 : e.AvailabilityStatus.DaysForRealization
                }).FirstAsync();
        }
        /// <summary>
        /// Using transactions set client availability status to the give one.
        /// </summary>
        /// <param name="orgId">Client id</param>
        /// <param name="statusId">New status id</param>
        /// <returns>True if success or false if failure.</returns>
        public async Task<bool> SetClientAvailabilityStatus(int orgId, int statusId)
        {
            using var trans = await _handlerContext.Database.BeginTransactionAsync();
            try
            {
                await _handlerContext.Organizations.Where(e => e.OrganizationId == orgId).ExecuteUpdateAsync(setters =>
                    setters.SetProperty(s => s.AvailabilityStatusId, statusId));
                await _handlerContext.SaveChangesAsync();
                await trans.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Change client availability status error.");
                await trans.RollbackAsync();
                return false;
            }
        }
        /// <summary>
        /// Using transactions add new client availability status.
        /// </summary>
        /// <param name="data">New status data</param>
        /// <returns>True if success or false if failure.</returns>
        public async Task<bool> AddAvailabilityStatus(AddAvailabilityStatus data)
        {
            using var trans = await _handlerContext.Database.BeginTransactionAsync();
            try
            {
                var newStatus = new AvailabilityStatus
                {
                    StatusName = data.StatusName,
                    DaysForRealization = data.DaysForRealization
                };

                _handlerContext.Add<AvailabilityStatus>(newStatus);
                await _handlerContext.SaveChangesAsync();
                await trans.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Add client availability status error.");
                await trans.RollbackAsync();
                return false;
            }
        }
        /// <summary>
        /// Do select query to receive list of client availability statues from database.
        /// </summary>
        /// <returns>List of availability statuses.</returns>
        public async Task<IEnumerable<GetAvailabilityStatuses>> GetAvailabilityStatuses()
        {
            return await _handlerContext.AvailabilityStatuses.Select(e => new GetAvailabilityStatuses
            {
                Id = e.AvailabilityStatusId,
                Name = e.StatusName,
                Days = e.DaysForRealization
            }).ToListAsync();
        }
        /// <summary>
        /// Using transactions overwrites client-user bindings to given new ones.
        /// </summary>
        /// <param name="data">New client-user binding data.</param>
        /// <returns>True if success or false if failure.</returns>
        public async Task<bool> SetClientUserBindings(SetUserClientBindings data)
        {
            using var trans = await _handlerContext.Database.BeginTransactionAsync();
            try
            {
                var current = await _handlerContext.Organizations
                    .Where(e => e.OrganizationId == data.OrgId)
                    .SelectMany(e => e.AppUsers)
                    .Select(e => e.IdUser).ToListAsync();
                var deletedUsers = current.Where(e => !data.UsersId.Contains(e)).ToList();
                foreach (var user in deletedUsers)
                {
                    await _handlerContext.Database.ExecuteSqlAsync($"Delete from User_client where organization_id = {data.OrgId} and users_id={user}");
                }
                await _handlerContext.SaveChangesAsync();
                var withoutExisting = data.UsersId.Where(e => !current.Contains(e)).ToList();
                var toAdd = withoutExisting.Select(e => new
                {
                    OrganizationId = data.OrgId,
                    IdUser = e,
                }).ToList();

                foreach (var relation in toAdd)
                {
                    await _handlerContext.Database.ExecuteSqlAsync($"insert into User_client (users_id, organization_id) Values ({relation.IdUser}, {relation.OrganizationId})");
                }

                await _handlerContext.SaveChangesAsync();
                await trans.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Set client-user bindings error.");
                await trans.RollbackAsync();
                return false;
            }

        }
        /// <summary>
        /// Checks if organizations exists in database.
        /// </summary>
        /// <param name="orgId">Organization id.</param>
        /// <returns>True if exist or false if not.</returns>
        public async Task<bool> OrgExist(int orgId)
        {
            return await _handlerContext.Organizations.Where(e => e.OrganizationId == orgId).AnyAsync();
        }
        /// <summary>
        /// Checks if All given user exist in database.
        /// </summary>
        /// <param name="users">List of users ids.</param>
        /// <returns>True if all users exist, false otherwise.</returns>
        public async Task<bool> ManyUserExist(IEnumerable<int> users)
        {
            if (!users.Any()) return true;
            foreach (var user in users)
            {
                var exists = await _handlerContext.AppUsers.AnyAsync(e => e.IdUser == user);
                if (!exists) return false;
            }
            return true;
        }
        /// <summary>
        /// Checks if client availability status with given id exist in database.
        /// </summary>
        /// <param name="statusId">Status id</param>
        /// <returns>True if exist, false if not.</returns>
        public async Task<bool> StatusExist(int statusId)
        {
            return await _handlerContext.AvailabilityStatuses.Where(e => e.AvailabilityStatusId == statusId).AnyAsync();
        }
        /// <summary>
        /// Do select query to receive organization client-user bindings information from database
        /// </summary>
        /// <param name="orgId">Organization id</param>
        /// <returns>List containing users information bound to the organization.</returns>
        public async Task<IEnumerable<GetClientBindings>> GetClientBindings(int orgId)
        {
            return await _handlerContext.Organizations.Where(e => e.OrganizationId == orgId).SelectMany(e => e.AppUsers).Select(e => new GetClientBindings
            {
                IdUser = e.IdUser,
                Username = e.Username,
                Surname = e.Surname
            }).ToListAsync();
        }
        /// <summary>
        /// Checks if organization have any relation that might for example prevent from deleting given organization form database.
        /// </summary>
        /// <param name="orgId">Organization id.</param>
        /// <param name="userId">Id of user.</param>
        /// <returns>Return true if only existing relation is that of given user, otherwise false.</returns>
        public async Task<bool> OrgHaveRelations(int orgId, int userId)
        {
            var invoicesCheck = await _handlerContext.Invoices.AnyAsync(e => e.Buyer == orgId || e.Seller == orgId);
            var proformaCheck = await _handlerContext.Proformas.AnyAsync(e => e.Buyer == orgId || e.Seller == orgId);
            var soloUserCheck = await _handlerContext.SoloUsers.AnyAsync(e => e.OrganizationsId == orgId);
            var orgUserCheck = await _handlerContext.OrgUsers.AnyAsync(e => e.OrganizationsId == orgId);
            var outsideItemsCheck = await _handlerContext.OutsideItems.AnyAsync(e => e.OrganizationId == orgId);
            var userClientCheck = await _handlerContext.AppUsers.AnyAsync(e => e.IdUser != userId && e.Clients.Any(x => x.OrganizationId == orgId));

            return invoicesCheck || proformaCheck || soloUserCheck || orgUserCheck || outsideItemsCheck || userClientCheck;
        }
        /// <summary>
        /// Using transactions delete organization from database.
        /// </summary>
        /// <param name="orgId">Id of organization to delete.</param>
        /// <returns>True if success or false if failure.</returns>
        public async Task<bool> DeleteOrg(int orgId)
        {
            using var trans = await _handlerContext.Database.BeginTransactionAsync();
            try
            {
                await _handlerContext.Database.ExecuteSqlAsync($"Delete from User_client where organization_id = {orgId}");
                await _handlerContext.Organizations.Where(e => e.OrganizationId == orgId).ExecuteDeleteAsync();
                await _handlerContext.SaveChangesAsync();
                await trans.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Delete organization error.");
                await trans.RollbackAsync();
                return false;
            }
        }
    }
}
