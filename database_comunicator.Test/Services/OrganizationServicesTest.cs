using database_communicator.Data;
using database_communicator.Models.DTOs.Create;
using database_communicator.Models.DTOs.Modify;
using database_communicator.Services;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace database_communicator_Test.Services
{
    public class OrganizationServicesTest
    {
        private readonly OrganizationServices _organizationServices;
        private readonly IConfiguration _configuration;
        public OrganizationServicesTest()
        {
            _configuration = new ConfigurationBuilder().AddUserSecrets("955b92d9-13d8-44b0-b618-19483bfe8b68").Build();
            var testDbConnectionString = _configuration["testDb"];
            var contextOptions = new DbContextOptionsBuilder<HandlerContext>()
                .UseSqlServer(testDbConnectionString);
            var context = new HandlerContext(contextOptions.Options, _configuration, A.Fake<IHttpContextAccessor>());
            _organizationServices = new OrganizationServices(context, A.Fake<ILogger<OrganizationServices>>());
        }
        [Fact]
        public async Task OrganizationServices_AddOrganization_ReturnNewItemId()
        {
            //Arrange
            var data = new AddOrganization
            {
                OrgName = "Test",
                Nip = null,
                Street = "test",
                City = "Test",
                PostalCode = "Test",
                CreditLimit = 10000,
                CountryId = 191
            };
            int userId = 1;

            //Act
            var result = await _organizationServices.AddOrganization(data, userId);

            //Assert
            result.Should().BeGreaterThan(0);
            result.Should().BePositive();
        }
        [Fact]
        public async Task OrganizationServices_CountryExist_ReturnTrue()
        {
            //Arrange
            string countryName = "Poland";

            //Act
            var result = await _organizationServices.CountryExist(countryName);

            //Assert
            result.Should().BeTrue();
        }
        [Fact]
        public async Task OrganizationServices_CountryExist_ReturnFalse()
        {
            //Arrange
            string countryName = "";

            //Act
            var result = await _organizationServices.CountryExist(countryName);

            //Assert
            result.Should().BeFalse();
        }
        [Fact]
        public async Task OrganizationServices_ModifyOrg_ReturnTrue()
        {
            //Arrange
            var data = new ModifyOrg { 
                OrgId = 2,
                CountryId = 44,
                Nip = 56453
            };

            //Act
            var result = await _organizationServices.ModifyOrg(data);

            //Assert
            result.Should().BeTrue();
        }
        [Fact]
        public async Task OrganizationServices_SetClientAvailabilityStatus_ReturnTrue()
        {
            //Arrange
            int orgId = 3;
            int statusId = 4;

            //Act
            var result = await _organizationServices.SetClientAvailabilityStatus(orgId, statusId);

            //Assert
            result.Should().BeTrue();
        }
        [Fact]
        public async Task OrganizationServices_SetClientUserBindings_ReturnTrue()
        {
            //Arrange
            var data = new SetUserClientBindings
            {
                OrgId = 2,
                UsersId = [1,2,3]
            };

            //Act
            var result = await _organizationServices.SetClientUserBindings(data);

            //Assert
            result.Should().BeTrue();
        }
        [Fact]
        public async Task OrganizationServices_AddAvailabilityStatus_ReturnTrue()
        {
            //Arrange
            var data = new AddAvailabilityStatus
            {
                StatusName = "5 days",
                DaysForRealization = 5
            };

            //Act
            var result = await _organizationServices.AddAvailabilityStatus(data);

            //Assert
            result.Should().BeTrue();
        }
        [Fact]
        public async Task OrganizationServices_OrgExist_ReturnTrue()
        {
            //Arrange
            int orgId = 1;

            //Act
            var result = await _organizationServices.OrgExist(orgId);

            //Assert
            result.Should().BeTrue();
        }
        [Fact]
        public async Task OrganizationServices_ManyUserExist_ReturnTrue()
        {
            //Arrange
            List<int> users = [1,2,3];

            //Act
            var result = await _organizationServices.ManyUserExist(users);

            //Assert
            result.Should().BeTrue();
        }
        [Fact]
        public async Task OrganizationServices_StatusExist_ReturnTrue()
        {
            //Arrange
            int statusId = 1;

            //Act
            var result = await _organizationServices.StatusExist(statusId);

            //Assert
            result.Should().BeTrue();
        }
        [Fact]
        public async Task OrganizationServices_OrgHaveRelations_ReturnTrue()
        {
            //Arrange
            int orgId = 1;
            int userId = 1;

            //Act
            var result = await _organizationServices.OrgHaveRelations(orgId, userId);

            //Assert
            result.Should().BeTrue();
        }
        [Fact]
        public async Task OrganizationServices_OrgHaveRelations_ReturnFalse()
        {
            //Arrange
            int orgId = 5;
            int userId = 3;

            //Act
            var result = await _organizationServices.OrgHaveRelations(orgId, userId);

            //Assert
            result.Should().BeFalse();
        }
        [Fact]
        public async Task OrganizationServices_DeleteOrg_ReturnTrue()
        {
            //Arrange
            int orgId = 6;

            //Act
            var result = await _organizationServices.DeleteOrg(orgId);

            //Assert
            result.Should().BeTrue();
        }
    }
}
