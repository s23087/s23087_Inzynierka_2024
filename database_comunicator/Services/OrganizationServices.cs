using database_comunicator.Data;
using database_comunicator.Models;
using database_comunicator.Models.DTOs;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Cryptography;

namespace database_comunicator.Services
{
    public interface IOrganizationServices
    {
        public Task<int> AddOrganization(AddOrganization org);
        public Task<int> GetCountryId(string countryName);
        public Task<bool> CountryExist(string countryName);
        public Task<GetOrg> GetOrg(int orgId);
        public Task ModifyOrg(ModifyOrg data);
        public Task<IEnumerable<GetClient>> GetClients(int userOrgId);
        public Task<IEnumerable<GetClient>> GetClients(int userOrgId, string search);
        public Task<IEnumerable<GetOrgClient>> GetOrgClients(int userOrgId);
        public Task<IEnumerable<GetOrgClient>> GetOrgClients(int userOrgId, string search);
        public Task<GetClientRestInfo> GetClientsRestInfo(int orgId);
        public Task SetClientAvailabilityStatus(int orgId, int statusId);
        public Task<bool> SetClientUserBindings(SetUserClientBindings data);
        public Task AddAvailabilityStatus(AddAvailabilityStatus data);
        public Task<IEnumerable<GetAvailabilityStatuses>> GetAvailabilityStatuses();
        public Task<bool> OrgExist(int orgId);
        public Task<bool> ManyUserExist(IEnumerable<int> users);
        public Task<bool> StatusExist(int statusId);
        public Task<IEnumerable<GetClientBindings>> GetClientBindings(int orgId);
        public Task<bool> OrgHaveRelations(int orgId);
        public Task DeleteOrg(int orgId);
    }
    public class OrganizationServices : IOrganizationServices
    {
        private readonly HandlerContext _handlerContext;
        public OrganizationServices(HandlerContext handlerContext)
        {
            _handlerContext = handlerContext;
        }

        public async Task<int> AddOrganization(AddOrganization org)
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
            _handlerContext.Add<Organization>(newOrg);

            await _handlerContext.SaveChangesAsync();

            return newOrg.OrganizationId;
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
        public async Task ModifyOrg(ModifyOrg data)
        {
            var changedOrg = new Organization
            {
                OrganizationId = data.OrgId,
                OrgName = data.OrgName,
                Nip = data.Nip,
                Street = data.Street,
                City = data.City,
                PostalCode = data.PostalCode,
                CreditLimit = data.CreditLimit,
                CountryId = data.CountryId,
                AvailabilityStatus = null
            };

            _handlerContext.Update<Organization>(changedOrg);
            _handlerContext.Entry(changedOrg).Property("AvailabilityStatusId").IsModified = false;

            if (changedOrg.OrgName == null)
            {
                _handlerContext.Entry(changedOrg).Property("OrgName").IsModified = false;
            }

            if (changedOrg.Street == null)
            {
                _handlerContext.Entry(changedOrg).Property("Street").IsModified = false;
            }
            if (changedOrg.City == null)
            {
                _handlerContext.Entry(changedOrg).Property("City").IsModified = false;
            }
            if (changedOrg.PostalCode == null)
            {
                _handlerContext.Entry(changedOrg).Property("PostalCode").IsModified = false;
            }
            if (changedOrg.CreditLimit == null)
            {
                _handlerContext.Entry(changedOrg).Property("CreditLimit").IsModified = false;
            }

            await _handlerContext.SaveChangesAsync();
        }
        public async Task<IEnumerable<GetClient>> GetClients(int userOrgId)
        {
            return await _handlerContext.Organizations.Where(d => d.OrganizationId != userOrgId)
                .Include(d => d.Country)
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
        public async Task<IEnumerable<GetClient>> GetClients(int userOrgId, string search)
        {
            return await _handlerContext.Organizations.Where(e => e.OrganizationId != userOrgId)
                .Where(e => EF.Functions.FreeText(e.OrgName, search) || EF.Functions.FreeText(e.Street, search) || EF.Functions.FreeText(e.City, search))
                .Include(e => e.Country)
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
        public async Task<IEnumerable<GetOrgClient>> GetOrgClients(int userOrgId)
        {
            return await _handlerContext.Organizations
                .Where(e => e.OrganizationId != userOrgId)
                .Include(e => e.AppUsers)
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
        public async Task<IEnumerable<GetOrgClient>> GetOrgClients(int userOrgId, string search)
        {
            return await _handlerContext.Organizations
                .Where(d => d.OrganizationId != userOrgId)
                .Where(d => EF.Functions.FreeText(d.OrgName, search) || EF.Functions.FreeText(d.Street, search) || EF.Functions.FreeText(d.City, search))
                .Include(d => d.AppUsers)
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

        public async Task SetClientAvailabilityStatus(int orgId, int statusId)
        {
            await _handlerContext.Organizations.ExecuteUpdateAsync(setters => 
            setters.SetProperty(s => s.AvailabilityStatusId, statusId));
            await _handlerContext.SaveChangesAsync();
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
                    _handlerContext.Database.ExecuteSql($"Delete from User_client where organization_id = {data.OrgId} and users_id={user}");
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
                    _handlerContext.Database.ExecuteSql($"insert into User_client (users_id, organization_id) Values ({relation.IdUser}, {relation.OrganizationId})");
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
        public async Task<bool> OrgHaveRelations(int orgId)
        {
            var invoicesCheck = await _handlerContext.Invoices.Where(e => e.Buyer == orgId || e.Seller == orgId).AnyAsync();
            var proformaCheck = await _handlerContext.Proformas.Where(e => e.Buyer == orgId || e.Seller == orgId).AnyAsync();
            var soloUserCheck = await _handlerContext.SoloUsers.Where(e => e.OrganizationsId == orgId).AnyAsync();
            var orgUserCheck = await _handlerContext.OrgUsers.Where(e => e.OrganizationsId == orgId).AnyAsync();
            var offerCheck = await _handlerContext.Offers.Where(e => e.OrganizationsId == orgId).AnyAsync();
            var outsideItemsCheck = await _handlerContext.OutsideItems.Where(e => e.OrganizationId == orgId).AnyAsync();
            var userClientCheck = await _handlerContext.Database.ExecuteSqlAsync($"Select 1 from User_Client where organization_id = {orgId}");

            return invoicesCheck || proformaCheck || soloUserCheck || orgUserCheck || offerCheck || outsideItemsCheck || userClientCheck.ToString().Contains('1'); ;
        }
        public async Task DeleteOrg(int orgId)
        {
            await _handlerContext.Organizations.Where(e => e.OrganizationId == orgId).ExecuteDeleteAsync();
        }
    }
}
