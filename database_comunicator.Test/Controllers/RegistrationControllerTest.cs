using database_communicator.Controllers;
using database_communicator.Models.DTOs.Create;
using database_communicator.Models.DTOs.Get;
using database_communicator.Services;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;

namespace database_communicator_Test.Controllers
{
    public class RegistrationControllerTest
    {
        private readonly IRegistrationServices _registrationServices;
        private readonly IOrganizationServices _organizationServices;
        private readonly IUserServices _userServices;
        private readonly IRolesServices _rolesServices;
        public RegistrationControllerTest()
        {
            _registrationServices = A.Fake<IRegistrationServices>();
            _organizationServices = A.Fake<IOrganizationServices>();
            _userServices = A.Fake<IUserServices>();
            _rolesServices = A.Fake<IRolesServices>();
        }
        [Fact]
        public async Task RegistrationController_GetCountries_ReturnOk()
        {
            //Arrange
            var data = A.Fake<IEnumerable<GetCountries>>();
            A.CallTo(() => _registrationServices.GetCountries()).Returns(data);
            var controller = new RegistrationController(_registrationServices, _organizationServices, _userServices, _rolesServices);

            //Act
            var result = await controller.GetCountries();

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();
        }
        [Fact]
        public async Task RegistrationController_CreateDb_ReturnOk()
        {
            //Arrange
            string orgName = "test";
            A.CallTo(() => _registrationServices.CreateNewDatabase(orgName)).Returns(true);
            var controller = new RegistrationController(_registrationServices, _organizationServices, _userServices, _rolesServices);

            //Act
            var result = await controller.CreateDb(orgName);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkResult>();
        }
        [Fact]
        public async Task RegistrationController_CreateDb_OrgNameIncorrect_Return404()
        {
            //Arrange
            string orgName = "";
            A.CallTo(() => _registrationServices.CreateNewDatabase(orgName)).Returns(true);
            var controller = new RegistrationController(_registrationServices, _organizationServices, _userServices, _rolesServices);

            //Act
            var result = await controller.CreateDb(orgName);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<BadRequestResult>();
        }
        [Fact]
        public async Task RegistrationController_CreateDb_CreateFails_Return404()
        {
            //Arrange
            string orgName = "test";
            A.CallTo(() => _registrationServices.CreateNewDatabase(orgName)).Returns(false);
            var controller = new RegistrationController(_registrationServices, _organizationServices, _userServices, _rolesServices);

            //Act
            var result = await controller.CreateDb(orgName);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<BadRequestResult>();
        }
        [Fact]
        public async Task RegistrationController_SetupDb_ReturnOk()
        {
            //Arrange
            A.CallTo(() => _registrationServices.SetupDatabase()).Returns(true);
            var controller = new RegistrationController(_registrationServices, _organizationServices, _userServices, _rolesServices);

            //Act
            var result = await controller.SetupDb();

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkResult>();
        }
        [Fact]
        public async Task RegistrationController_SetupDb_ActionFails_Return400()
        {
            //Arrange
            A.CallTo(() => _registrationServices.SetupDatabase()).Returns(false);
            var controller = new RegistrationController(_registrationServices, _organizationServices, _userServices, _rolesServices);

            //Act
            var result = await controller.SetupDb();

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<BadRequestResult>();
        }
        [Fact]
        public async Task RegistrationController_RegisterUser_ReturnOk()
        {
            //Arrange
            var newUser = A.Fake<RegisterUser>();
            newUser.IsOrg = true;
            int countryId = 1;
            int orgId = 1;
            int roleId = 1;
            A.CallTo(() => _organizationServices.CountryExist(newUser.Country)).Returns(true);
            A.CallTo(() => _organizationServices.GetCountryId(newUser.Country)).Returns(countryId);
            A.CallTo(() => _organizationServices.AddOrganization(A<AddOrganization>._, null)).Returns(orgId);
            A.CallTo(() => _rolesServices.GetRoleId("Admin")).Returns(roleId);
            A.CallTo(() => _userServices.AddUser(A<AddUser>._, orgId, roleId, newUser.IsOrg)).Returns(true);
            var controller = new RegistrationController(_registrationServices, _organizationServices, _userServices, _rolesServices);

            //Act
            var result = await controller.RegisterUser(newUser);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkResult>();
        }
        [Fact]
        public async Task RegistrationController_RegisterUser_ActionFails_Return400()
        {
            //Arrange
            var newUser = A.Fake<RegisterUser>();
            newUser.IsOrg = true;
            var org = A.Fake<AddOrganization>();
            int orgId = 1;
            int roleId = 1;
            var addUser = A.Fake<AddUser>();
            A.CallTo(() => _organizationServices.CountryExist(newUser.Country)).Returns(true);
            A.CallTo(() => _organizationServices.GetCountryId(newUser.Country));
            A.CallTo(() => _organizationServices.AddOrganization(org, null)).Returns(orgId);
            A.CallTo(() => _rolesServices.GetRoleId("Admin")).Returns(roleId);
            A.CallTo(() => _userServices.AddUser(addUser, orgId, roleId, newUser.IsOrg)).Returns(false);
            var controller = new RegistrationController(_registrationServices, _organizationServices, _userServices, _rolesServices);

            //Act
            var result = await controller.RegisterUser(newUser);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<BadRequestResult>();
        }
        [Fact]
        public async Task RegistrationController_RegisterUser_OrgCreationFails_Return400()
        {
            //Arrange
            var newUser = A.Fake<RegisterUser>();
            newUser.IsOrg = true;
            var org = A.Fake<AddOrganization>();
            int orgId = 0;
            int roleId = 1;
            var addUser = A.Fake<AddUser>();
            A.CallTo(() => _organizationServices.CountryExist(newUser.Country)).Returns(true);
            A.CallTo(() => _organizationServices.GetCountryId(newUser.Country));
            A.CallTo(() => _organizationServices.AddOrganization(org, null)).Returns(orgId);
            A.CallTo(() => _rolesServices.GetRoleId("Admin")).Returns(roleId);
            A.CallTo(() => _userServices.AddUser(addUser, orgId, roleId, newUser.IsOrg)).Returns(true);
            var controller = new RegistrationController(_registrationServices, _organizationServices, _userServices, _rolesServices);

            //Act
            var result = await controller.RegisterUser(newUser);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<BadRequestResult>();
        }
        [Fact]
        public async Task RegistrationController_RegisterUser_CountryNotFound_Return404()
        {
            //Arrange
            var newUser = A.Fake<RegisterUser>();
            newUser.IsOrg = true;
            var org = A.Fake<AddOrganization>();
            int orgId = 1;
            int roleId = 1;
            var addUser = A.Fake<AddUser>();
            A.CallTo(() => _organizationServices.CountryExist(newUser.Country)).Returns(false);
            A.CallTo(() => _organizationServices.GetCountryId(newUser.Country));
            A.CallTo(() => _organizationServices.AddOrganization(org, null)).Returns(orgId);
            A.CallTo(() => _rolesServices.GetRoleId("Admin")).Returns(roleId);
            A.CallTo(() => _userServices.AddUser(addUser, orgId, roleId, newUser.IsOrg)).Returns(true);
            var controller = new RegistrationController(_registrationServices, _organizationServices, _userServices, _rolesServices);

            //Act
            var result = await controller.RegisterUser(newUser);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundResult>();
        }
    }
}
