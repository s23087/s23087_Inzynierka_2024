using database_comunicator.Data;
using database_comunicator.Models;
using database_comunicator.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace database_comunicator.Services
{
    public interface IOrganizationServices
    {
        public Task<int> AddOrganization(AddOrganization org);
        public Task<int> GetCountryId(string countryName);
        public Task<bool> CountryExist(string countryName);
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
                CountryId = org.CountryId
            };
            await _handlerContext.AddAsync<Organization>(newOrg);

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
            return result.First();
        }
    }
}
