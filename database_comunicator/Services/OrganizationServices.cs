using database_communicator.Data;
using database_communicator.Models;
using database_communicator.Models.DTOs;
using database_communicator.Utils;
using database_comunicator.FilterClass;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace database_communicator.Services
{
    public interface IOrganizationServices
    {
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
        public Task AddAvailabilityStatus(AddAvailabilityStatus data);
        public Task<IEnumerable<GetAvailabilityStatuses>> GetAvailabilityStatuses();
        public Task<bool> OrgExist(int orgId);
        public Task<bool> ManyUserExist(IEnumerable<int> users);
        public Task<bool> StatusExist(int statusId);
        public Task<IEnumerable<GetClientBindings>> GetClientBindings(int orgId);
        public Task<bool> OrgHaveRelations(int orgId, int userId);
        public Task<bool> DeleteOrg(int orgId);
    }
    public class OrganizationServices : IOrganizationServices
    {
        private readonly HandlerContext _handlerContext;
        public OrganizationServices(HandlerContext handlerContext)
        {
            _handlerContext = handlerContext;
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
            } catch (Exception error)
            {
                Console.WriteLine(error.ToString());
                await trans.RollbackAsync();
                return 0;
            }
        }

        public async Task<bool> CountryExist(string countryName)
        {
            return await _handlerContext.Countries.Where(e => e.CountryName == countryName).AnyAsync();
        }

        public async Task<int> GetCountryId(string countryName)
        {
            var result = await _handlerContext.Countries.Where(e => e.CountryName == countryName).Select(e => e.CountryId).ToListAsync();
            return result[0];
        }
        public async Task<GetOrg> GetOrg(int orgId)
        {
            return await _handlerContext.Organizations.Where(e => e.OrganizationId == orgId).Include(e => e.Country).Select(e => new GetOrg
            {
                Id = e.OrganizationId,
                OrgName=e.OrgName,
                Nip=e.Nip,
                Street=e.Street,
                City=e.City,
                PostalCode=e.PostalCode,
                CountryId=e.CountryId,
                Country=e.Country.CountryName
            }).FirstAsync();
        }
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
            } catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                await trans.RollbackAsync();
                return false;
            }
        }
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
                direction = sort.StartsWith("D");
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
                direction = sort.StartsWith("D");
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
                direction = sort.StartsWith("D");
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
                direction = sort.StartsWith("D");
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

        public async Task<bool> SetClientAvailabilityStatus(int orgId, int statusId)
        {
            try
            {
                await _handlerContext.Organizations.ExecuteUpdateAsync(setters =>
                    setters.SetProperty(s => s.AvailabilityStatusId, statusId));
                await _handlerContext.SaveChangesAsync();
                return true;
            } catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
        }

        public async Task AddAvailabilityStatus(AddAvailabilityStatus data)
        {
            var newStatus = new AvailabilityStatus
            {
                StatusName = data.StatusName,
                DaysForRealization = data.DaysForRealization
            };

            _handlerContext.Add<AvailabilityStatus>(newStatus);
            await _handlerContext.SaveChangesAsync();
        }
        public async Task<IEnumerable<GetAvailabilityStatuses>> GetAvailabilityStatuses()
        {
            return await _handlerContext.AvailabilityStatuses.Select(e => new GetAvailabilityStatuses
            {
                Id = e.AvailabilityStatusId,
                Name = e.StatusName,
                Days = e.DaysForRealization
            }).ToListAsync();
        }
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
            } catch (Exception ex)
            {
                Console.Write(ex.ToString());
                await trans.RollbackAsync();
                return false;
            }

        }
        public async Task<bool> OrgExist(int orgId)
        {
            return await _handlerContext.Organizations.Where(e => e.OrganizationId == orgId).AnyAsync();
        }
        public async Task<bool> ManyUserExist(IEnumerable<int> users)
        {
            if (!users.Any()) return true;
            return await _handlerContext.AppUsers.AnyAsync(e => users.Contains(e.IdUser));
        }
        public async Task<bool> StatusExist(int statusId)
        {
            return await _handlerContext.AvailabilityStatuses.Where(e => e.AvailabilityStatusId == statusId).AnyAsync();
        }
        public async Task<IEnumerable<GetClientBindings>> GetClientBindings(int orgId)
        {
            return await _handlerContext.Organizations.Where(e => e.OrganizationId == orgId).SelectMany(e => e.AppUsers).Select(e => new GetClientBindings
            {
                IdUser = e.IdUser,
                Username = e.Username,
                Surname = e.Surname
            }).ToListAsync();
        }
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
            } catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                await trans.RollbackAsync();
                return false;
            }
        }
    }
}
