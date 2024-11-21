using database_communicator.Controllers;
using database_communicator.Models.DTOs.Create;
using database_communicator.Models.DTOs.Get;
using database_communicator.Models.DTOs.Modify;
using database_communicator.Services;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;

namespace database_communicator_Test.Controllers
{
    public class ClientControllerTest
    {
        private readonly IOrganizationServices _organizationServices;
        private readonly IUserServices _userServices;
        private readonly ILogServices _logServices;
        public ClientControllerTest()
        {
            _organizationServices = A.Fake<IOrganizationServices>();
            _userServices = A.Fake<IUserServices>();
            _logServices = A.Fake<ILogServices>();
        }
        [Fact]
        public async Task ClientController_GetClients_UserExist_ReturnOk()
        {
            //Arrange
            var clients = A.Fake<IEnumerable<GetClient>>();
            int userId = 1;
            bool isOrg = true;
            int orgId = 1;
            A.CallTo(() => _userServices.UserExist(userId)).Returns(true);
            A.CallTo(() => _userServices.IsOrgUser(userId)).Returns(isOrg);
            A.CallTo(() => _userServices.GetOrgId(userId, isOrg)).Returns(orgId);
            A.CallTo(() => _organizationServices.GetClients(userId, orgId, null, null)).Returns(clients);
            var controller = new ClientController(_organizationServices, _userServices, _logServices);

            //Act
            var result = await controller.GetClients(userId, null, null, null);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>("because user exist return true.");
        }
        [Fact]
        public async Task ClientController_GetClients_UserDoNotExist_Return404()
        {
            //Arrange
            var clients = A.Fake<IEnumerable<GetClient>>();
            int userId = 1;
            bool isOrg = true;
            int orgId = 1;
            A.CallTo(() => _userServices.UserExist(userId)).Returns(false);
            A.CallTo(() => _userServices.IsOrgUser(userId)).Returns(isOrg);
            A.CallTo(() => _userServices.GetOrgId(userId, isOrg)).Returns(orgId);
            A.CallTo(() => _organizationServices.GetClients(userId, orgId, null, null)).Returns(clients);
            var controller = new ClientController(_organizationServices, _userServices, _logServices);

            //Act
            var result = await controller.GetClients(userId, null, null, null);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundResult>("because user exist return false.");
        }
        [Fact]
        public async Task ClientController_GetClients_SortIsIncorrect_Return400()
        {
            //Arrange
            var clients = A.Fake<IEnumerable<GetClient>>();
            int userId = 1;
            bool isOrg = true;
            int orgId = 1;
            var sort = "";
            A.CallTo(() => _userServices.UserExist(userId)).Returns(true);
            A.CallTo(() => _userServices.IsOrgUser(userId)).Returns(isOrg);
            A.CallTo(() => _userServices.GetOrgId(userId, isOrg)).Returns(orgId);
            A.CallTo(() => _organizationServices.GetClients(userId, orgId, null, null)).Returns(clients);
            var controller = new ClientController(_organizationServices, _userServices, _logServices);

            //Act
            var result = await controller.GetClients(userId, null, sort, null);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<BadRequestResult>("because sort value is incorrect.");
        }
        [Fact]
        public async Task ClientController_GetOrgClients_UserExist_ReturnOk()
        {
            //Arrange
            var clients = A.Fake<IEnumerable<GetOrgClient>>();
            int userId = 1;
            int orgId = 1;
            A.CallTo(() => _userServices.UserExist(userId)).Returns(true);
            A.CallTo(() => _userServices.GetOrgId(userId, true)).Returns(orgId);
            A.CallTo(() => _organizationServices.GetOrgClients(orgId, null, null)).Returns(clients);
            var controller = new ClientController(_organizationServices, _userServices, _logServices);

            //Act
            var result = await controller.GetOrgClients(userId, null, null, null);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>("because user exist return true.");
        }
        [Fact]
        public async Task ClientController_GetOrgClients_UserDoNotExist_Return404()
        {
            //Arrange
            var clients = A.Fake<IEnumerable<GetOrgClient>>();
            int userId = 1;
            int orgId = 1;
            A.CallTo(() => _userServices.UserExist(userId)).Returns(false);
            A.CallTo(() => _userServices.GetOrgId(userId, true)).Returns(orgId);
            A.CallTo(() => _organizationServices.GetOrgClients(orgId, null, null)).Returns(clients);
            var controller = new ClientController(_organizationServices, _userServices, _logServices);

            //Act
            var result = await controller.GetOrgClients(userId, null, null, null);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundResult>("because user exist return false.");
        }
        [Fact]
        public async Task ClientController_GetOrgClients_SortIsIncorrect_Return400()
        {
            //Arrange
            var clients = A.Fake<IEnumerable<GetOrgClient>>();
            int userId = 1;
            int orgId = 1;
            var sort = "";
            A.CallTo(() => _userServices.UserExist(userId)).Returns(false);
            A.CallTo(() => _userServices.GetOrgId(userId, true)).Returns(orgId);
            A.CallTo(() => _organizationServices.GetOrgClients(orgId, null, null)).Returns(clients);
            var controller = new ClientController(_organizationServices, _userServices, _logServices);

            //Act
            var result = await controller.GetOrgClients(userId, null, sort, null);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<BadRequestResult>("because sort value is incorrect.");
        }
        [Fact]
        public async Task ClientController_GetRestInfoOrg_OrganizationExist_ReturnOk()
        {
            //Arrange
            var restInfo = A.Fake<GetClientRestInfo>();
            int orgId = 1;
            A.CallTo(() => _organizationServices.OrgExist(orgId)).Returns(true);
            A.CallTo(() => _organizationServices.GetClientsRestInfo(orgId)).Returns(restInfo);
            var controller = new ClientController(_organizationServices, _userServices, _logServices);

            //Act
            var result = await controller.GetRestInfoOrg(orgId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>("because organization exist.");
        }
        [Fact]
        public async Task ClientController_GetRestInfoOrg_OrganizationDoNotExist_Return404()
        {
            //Arrange
            var restInfo = A.Fake<GetClientRestInfo>();
            int orgId = 1;
            A.CallTo(() => _organizationServices.OrgExist(orgId)).Returns(false);
            A.CallTo(() => _organizationServices.GetClientsRestInfo(orgId)).Returns(restInfo);
            var controller = new ClientController(_organizationServices, _userServices, _logServices);

            //Act
            var result = await controller.GetRestInfoOrg(orgId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundObjectResult>("because organization exist.");
        }
        [Fact]
        public async Task ClientController_GetAvailabilityStatuses_ReturnOk()
        {
            //Arrange
            var statuses = A.Fake<IEnumerable<GetAvailabilityStatuses>>();
            A.CallTo(() => _organizationServices.GetAvailabilityStatuses()).Returns(statuses);
            var controller = new ClientController(_organizationServices, _userServices, _logServices);

            //Act
            var result = await controller.GetAvailabilityStatuses();

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();
        }
        [Fact]
        public async Task ClientController_AddAvailabilityStatuses_UserExist_ReturnOk()
        {
            //Arrange
            var data = A.Fake<AddAvailabilityStatus>();
            int userId = 1;
            int logTypeId = 1;
            bool AddResult = true;
            A.CallTo(() => _userServices.UserExist(userId)).Returns(true);
            A.CallTo(() => _logServices.getLogTypeId("Create")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", userId, logTypeId));
            A.CallTo(() => _organizationServices.AddAvailabilityStatus(data)).Returns(AddResult);
            var controller = new ClientController(_organizationServices, _userServices, _logServices);

            //Act
            var result = await controller.AddAvailabilityStatuses(data, userId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkResult>();
        }
        [Fact]
        public async Task ClientController_AddAvailabilityStatuses_UserDoNotExist_Return404()
        {
            //Arrange
            var data = A.Fake<AddAvailabilityStatus>();
            int userId = 1;
            int logTypeId = 1;
            bool AddResult = true;
            A.CallTo(() => _userServices.UserExist(userId)).Returns(false);
            A.CallTo(() => _logServices.getLogTypeId("Create")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", userId, logTypeId));
            A.CallTo(() => _organizationServices.AddAvailabilityStatus(data)).Returns(AddResult);
            var controller = new ClientController(_organizationServices, _userServices, _logServices);

            //Act
            var result = await controller.AddAvailabilityStatuses(data, userId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundResult>();
        }
        [Fact]
        public async Task ClientController_AddAvailabilityStatuses_AddFailed_Return500()
        {
            //Arrange
            var data = A.Fake<AddAvailabilityStatus>();
            int userId = 1;
            int logTypeId = 1;
            bool AddResult = false;
            A.CallTo(() => _userServices.UserExist(userId)).Returns(true);
            A.CallTo(() => _logServices.getLogTypeId("Create")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", userId, logTypeId));
            A.CallTo(() => _organizationServices.AddAvailabilityStatus(data)).Returns(AddResult);
            var controller = new ClientController(_organizationServices, _userServices, _logServices);

            //Act
            var result = await controller.AddAvailabilityStatuses(data, userId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<StatusCodeResult>();
        }
        [Fact]
        public async Task ClientController_SetUserClientBindings_ReturnOk()
        {
            //Arrange
            var data = A.Fake<SetUserClientBindings>();
            int userId = 1;
            int logTypeId = 1;
            bool modifyResult = true;
            A.CallTo(() => _userServices.UserExist(userId)).Returns(true);
            A.CallTo(() => _organizationServices.OrgExist(data.OrgId)).Returns(true);
            A.CallTo(() => _organizationServices.ManyUserExist(data.UsersId)).Returns(true);
            A.CallTo(() => _logServices.getLogTypeId("Modify")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", userId, logTypeId));
            A.CallTo(() => _organizationServices.SetClientUserBindings(data)).Returns(modifyResult);
            var controller = new ClientController(_organizationServices, _userServices, _logServices);

            //Act
            var result = await controller.SetUserClientBindings(data, userId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkResult>();
        }
        [Fact]
        public async Task ClientController_SetUserClientBindings_ModifyFailed_Return500()
        {
            //Arrange
            var data = A.Fake<SetUserClientBindings>();
            int userId = 1;
            int logTypeId = 1;
            bool modifyResult = false;
            A.CallTo(() => _userServices.UserExist(userId)).Returns(true);
            A.CallTo(() => _organizationServices.OrgExist(data.OrgId)).Returns(true);
            A.CallTo(() => _organizationServices.ManyUserExist(data.UsersId)).Returns(true);
            A.CallTo(() => _logServices.getLogTypeId("Modify")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", userId, logTypeId));
            A.CallTo(() => _organizationServices.SetClientUserBindings(data)).Returns(modifyResult);
            var controller = new ClientController(_organizationServices, _userServices, _logServices);

            //Act
            var result = await controller.SetUserClientBindings(data, userId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<StatusCodeResult>();
        }
        [Fact]
        public async Task ClientController_SetUserClientBindings_UserNotFound_Return404()
        {
            //Arrange
            var data = A.Fake<SetUserClientBindings>();
            int userId = 1;
            int logTypeId = 1;
            bool modifyResult = true;
            A.CallTo(() => _userServices.UserExist(userId)).Returns(false);
            A.CallTo(() => _organizationServices.OrgExist(data.OrgId)).Returns(true);
            A.CallTo(() => _organizationServices.ManyUserExist(data.UsersId)).Returns(true);
            A.CallTo(() => _logServices.getLogTypeId("Modify")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", userId, logTypeId));
            A.CallTo(() => _organizationServices.SetClientUserBindings(data)).Returns(modifyResult);
            var controller = new ClientController(_organizationServices, _userServices, _logServices);

            //Act
            var result = await controller.SetUserClientBindings(data, userId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundObjectResult>();
        }
        [Fact]
        public async Task ClientController_SetUserClientBindings_OrgNotFound_Return404()
        {
            //Arrange
            var data = A.Fake<SetUserClientBindings>();
            int userId = 1;
            int logTypeId = 1;
            bool modifyResult = true;
            A.CallTo(() => _userServices.UserExist(userId)).Returns(true);
            A.CallTo(() => _organizationServices.OrgExist(data.OrgId)).Returns(false);
            A.CallTo(() => _organizationServices.ManyUserExist(data.UsersId)).Returns(true);
            A.CallTo(() => _logServices.getLogTypeId("Modify")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", userId, logTypeId));
            A.CallTo(() => _organizationServices.SetClientUserBindings(data)).Returns(modifyResult);
            var controller = new ClientController(_organizationServices, _userServices, _logServices);

            //Act
            var result = await controller.SetUserClientBindings(data, userId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundObjectResult>();
        }
        [Fact]
        public async Task ClientController_SetUserClientBindings_OneOfUsersNotFound_Return404()
        {
            //Arrange
            var data = A.Fake<SetUserClientBindings>();
            int userId = 1;
            int logTypeId = 1;
            bool modifyResult = true;
            A.CallTo(() => _userServices.UserExist(userId)).Returns(true);
            A.CallTo(() => _organizationServices.OrgExist(data.OrgId)).Returns(true);
            A.CallTo(() => _organizationServices.ManyUserExist(data.UsersId)).Returns(false);
            A.CallTo(() => _logServices.getLogTypeId("Modify")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", userId, logTypeId));
            A.CallTo(() => _organizationServices.SetClientUserBindings(data)).Returns(modifyResult);
            var controller = new ClientController(_organizationServices, _userServices, _logServices);

            //Act
            var result = await controller.SetUserClientBindings(data, userId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundObjectResult>();
        }
        [Fact]
        public async Task ClientController_SetAvailabilityStatusesToClient_ReturnOk()
        {
            //Arrange
            var data = A.Fake<SetAvailabilityStatusesToClient>();
            int userId = 1;
            int logTypeId = 1;
            bool modifyResult = true;
            A.CallTo(() => _userServices.UserExist(userId)).Returns(true);
            A.CallTo(() => _organizationServices.OrgExist(data.OrgId)).Returns(true);
            A.CallTo(() => _organizationServices.StatusExist(data.StatusId)).Returns(true);
            A.CallTo(() => _logServices.getLogTypeId("Modify")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", userId, logTypeId));
            A.CallTo(() => _organizationServices.SetClientAvailabilityStatus(data.OrgId, data.StatusId)).Returns(modifyResult);
            var controller = new ClientController(_organizationServices, _userServices, _logServices);

            //Act
            var result = await controller.SetAvailabilityStatusesToClient(data, userId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkResult>();
        }
        [Fact]
        public async Task ClientController_SetAvailabilityStatusesToClient_ModifyFailed_Return500()
        {
            //Arrange
            var data = A.Fake<SetAvailabilityStatusesToClient>();
            int userId = 1;
            int logTypeId = 1;
            bool modifyResult = false;
            A.CallTo(() => _userServices.UserExist(userId)).Returns(true);
            A.CallTo(() => _organizationServices.OrgExist(data.OrgId)).Returns(true);
            A.CallTo(() => _organizationServices.StatusExist(data.StatusId)).Returns(true);
            A.CallTo(() => _logServices.getLogTypeId("Modify")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", userId, logTypeId));
            A.CallTo(() => _organizationServices.SetClientAvailabilityStatus(data.OrgId, data.StatusId)).Returns(modifyResult);
            var controller = new ClientController(_organizationServices, _userServices, _logServices);

            //Act
            var result = await controller.SetAvailabilityStatusesToClient(data, userId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<StatusCodeResult>();
        }
        [Fact]
        public async Task ClientController_SetAvailabilityStatusesToClient_UserDoNotExist_Return404()
        {
            //Arrange
            var data = A.Fake<SetAvailabilityStatusesToClient>();
            int userId = 1;
            int logTypeId = 1;
            bool modifyResult = true;
            A.CallTo(() => _userServices.UserExist(userId)).Returns(false);
            A.CallTo(() => _organizationServices.OrgExist(data.OrgId)).Returns(true);
            A.CallTo(() => _organizationServices.StatusExist(data.StatusId)).Returns(true);
            A.CallTo(() => _logServices.getLogTypeId("Modify")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", userId, logTypeId));
            A.CallTo(() => _organizationServices.SetClientAvailabilityStatus(data.OrgId, data.StatusId)).Returns(modifyResult);
            var controller = new ClientController(_organizationServices, _userServices, _logServices);

            //Act
            var result = await controller.SetAvailabilityStatusesToClient(data, userId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundObjectResult>();
        }
        [Fact]
        public async Task ClientController_SetAvailabilityStatusesToClient_OrgDoNotExist_Return404()
        {
            //Arrange
            var data = A.Fake<SetAvailabilityStatusesToClient>();
            int userId = 1;
            int logTypeId = 1;
            bool modifyResult = true;
            A.CallTo(() => _userServices.UserExist(userId)).Returns(true);
            A.CallTo(() => _organizationServices.OrgExist(data.OrgId)).Returns(false);
            A.CallTo(() => _organizationServices.StatusExist(data.StatusId)).Returns(true);
            A.CallTo(() => _logServices.getLogTypeId("Modify")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", userId, logTypeId));
            A.CallTo(() => _organizationServices.SetClientAvailabilityStatus(data.OrgId, data.StatusId)).Returns(modifyResult);
            var controller = new ClientController(_organizationServices, _userServices, _logServices);

            //Act
            var result = await controller.SetAvailabilityStatusesToClient(data, userId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundObjectResult>();
        }
        [Fact]
        public async Task ClientController_SetAvailabilityStatusesToClient_StatusDoNotExist_Return404()
        {
            //Arrange
            var data = A.Fake<SetAvailabilityStatusesToClient>();
            int userId = 1;
            int logTypeId = 1;
            bool modifyResult = true;
            A.CallTo(() => _userServices.UserExist(userId)).Returns(true);
            A.CallTo(() => _organizationServices.OrgExist(data.OrgId)).Returns(true);
            A.CallTo(() => _organizationServices.StatusExist(data.StatusId)).Returns(false);
            A.CallTo(() => _logServices.getLogTypeId("Modify")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", userId, logTypeId));
            A.CallTo(() => _organizationServices.SetClientAvailabilityStatus(data.OrgId, data.StatusId)).Returns(modifyResult);
            var controller = new ClientController(_organizationServices, _userServices, _logServices);

            //Act
            var result = await controller.SetAvailabilityStatusesToClient(data, userId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundObjectResult>();
        }
        [Fact]
        public async Task ClientController_ModifyOrg_ReturnOk()
        {
            //Arrange
            var data = A.Fake<ModifyOrg>();
            int userId = 1;
            int logTypeId = 1;
            bool modifyResult = true;
            A.CallTo(() => _userServices.UserExist(userId)).Returns(true);
            A.CallTo(() => _organizationServices.OrgExist(data.OrgId)).Returns(true);
            A.CallTo(() => _organizationServices.ModifyOrg(data)).Returns(modifyResult);
            A.CallTo(() => _logServices.getLogTypeId("Modify")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", userId, logTypeId));
            var controller = new ClientController(_organizationServices, _userServices, _logServices);

            //Act
            var result = await controller.ModifyOrg(data, userId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkResult>();
        }
        [Fact]
        public async Task ClientController_ModifyOrg_ModifyFailed_Return500()
        {
            //Arrange
            var data = A.Fake<ModifyOrg>();
            int userId = 1;
            int logTypeId = 1;
            bool modifyResult = false;
            A.CallTo(() => _userServices.UserExist(userId)).Returns(true);
            A.CallTo(() => _organizationServices.OrgExist(data.OrgId)).Returns(true);
            A.CallTo(() => _organizationServices.ModifyOrg(data)).Returns(modifyResult);
            A.CallTo(() => _logServices.getLogTypeId("Modify")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", userId, logTypeId));
            var controller = new ClientController(_organizationServices, _userServices, _logServices);

            //Act
            var result = await controller.ModifyOrg(data, userId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<StatusCodeResult>();
        }
        [Fact]
        public async Task ClientController_ModifyOrg_UserNotFound_Return404()
        {
            //Arrange
            var data = A.Fake<ModifyOrg>();
            int userId = 1;
            int logTypeId = 1;
            bool modifyResult = true;
            A.CallTo(() => _userServices.UserExist(userId)).Returns(false);
            A.CallTo(() => _organizationServices.OrgExist(data.OrgId)).Returns(true);
            A.CallTo(() => _organizationServices.ModifyOrg(data)).Returns(modifyResult);
            A.CallTo(() => _logServices.getLogTypeId("Modify")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", userId, logTypeId));
            var controller = new ClientController(_organizationServices, _userServices, _logServices);

            //Act
            var result = await controller.ModifyOrg(data, userId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundObjectResult>();
        }
        [Fact]
        public async Task ClientController_ModifyOrg_OrgNotFound_Return404()
        {
            //Arrange
            var data = A.Fake<ModifyOrg>();
            int userId = 1;
            int logTypeId = 1;
            bool modifyResult = true;
            A.CallTo(() => _userServices.UserExist(userId)).Returns(true);
            A.CallTo(() => _organizationServices.OrgExist(data.OrgId)).Returns(false);
            A.CallTo(() => _organizationServices.ModifyOrg(data)).Returns(modifyResult);
            A.CallTo(() => _logServices.getLogTypeId("Modify")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", userId, logTypeId));
            var controller = new ClientController(_organizationServices, _userServices, _logServices);

            //Act
            var result = await controller.ModifyOrg(data, userId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundObjectResult>();
        }
        [Fact]
        public async Task ClientController_AddOrganization_ReturnOk()
        {
            //Arrange
            var data = A.Fake<AddOrganization>();
            int userId = 1;
            int logTypeId = 1;
            int newOrgId = 1;
            A.CallTo(() => _userServices.UserExist(userId)).Returns(true);
            A.CallTo(() => _organizationServices.AddOrganization(data, userId)).Returns(newOrgId);
            A.CallTo(() => _logServices.getLogTypeId("Create")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", userId, logTypeId));
            var controller = new ClientController(_organizationServices, _userServices, _logServices);

            //Act
            var result = await controller.AddOrganization(data, userId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();
        }
        [Fact]
        public async Task ClientController_AddOrganization_AddFailed_Return500()
        {
            //Arrange
            var data = A.Fake<AddOrganization>();
            int userId = 1;
            int logTypeId = 1;
            int newOrgId = 0;
            A.CallTo(() => _userServices.UserExist(userId)).Returns(true);
            A.CallTo(() => _organizationServices.AddOrganization(data, userId)).Returns(newOrgId);
            A.CallTo(() => _logServices.getLogTypeId("Create")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", userId, logTypeId));
            var controller = new ClientController(_organizationServices, _userServices, _logServices);

            //Act
            var result = await controller.AddOrganization(data, userId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<StatusCodeResult>();
        }
        [Fact]
        public async Task ClientController_AddOrganization_UserNotFound_Return404()
        {
            //Arrange
            var data = A.Fake<AddOrganization>();
            int userId = 1;
            int logTypeId = 1;
            int newOrgId = 1;
            A.CallTo(() => _userServices.UserExist(userId)).Returns(false);
            A.CallTo(() => _organizationServices.AddOrganization(data, userId)).Returns(newOrgId);
            A.CallTo(() => _logServices.getLogTypeId("Create")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", userId, logTypeId));
            var controller = new ClientController(_organizationServices, _userServices, _logServices);

            //Act
            var result = await controller.AddOrganization(data, userId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundResult>();
        }
        [Fact]
        public async Task ClientController_GetUserClientBindings_ReturnOk()
        {
            //Arrange
            var bindings = A.Fake<IEnumerable<GetClientBindings>>();
            int orgId = 1;
            A.CallTo(() => _organizationServices.OrgExist(orgId)).Returns(true);
            A.CallTo(() => _organizationServices.GetClientBindings(orgId)).Returns(bindings);
            var controller = new ClientController(_organizationServices, _userServices, _logServices);

            //Act
            var result = await controller.GetUserClientBindings(orgId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();
        }
        [Fact]
        public async Task ClientController_GetUserClientBindings_OrgNotFound_Return404()
        {
            //Arrange
            var bindings = A.Fake<IEnumerable<GetClientBindings>>();
            int orgId = 1;
            A.CallTo(() => _organizationServices.OrgExist(orgId)).Returns(false);
            A.CallTo(() => _organizationServices.GetClientBindings(orgId)).Returns(bindings);
            var controller = new ClientController(_organizationServices, _userServices, _logServices);

            //Act
            var result = await controller.GetUserClientBindings(orgId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundResult>();
        }
        [Fact]
        public async Task ClientController_DeleteClient_ReturnOk()
        {
            //Arrange
            int userId = 1;
            int orgId = 1;
            int logTypeId = 1;
            bool deleteResult = true;
            A.CallTo(() => _userServices.UserExist(userId)).Returns(true);
            A.CallTo(() => _organizationServices.OrgExist(orgId)).Returns(true);
            A.CallTo(() => _organizationServices.OrgHaveRelations(orgId, userId)).Returns(false);
            A.CallTo(() => _organizationServices.DeleteOrg(orgId)).Returns(deleteResult);
            A.CallTo(() => _logServices.getLogTypeId("Delete")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", userId, logTypeId));
            var controller = new ClientController(_organizationServices, _userServices, _logServices);

            //Act
            var result = await controller.DeleteClient(orgId, userId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkResult>();
        }
        [Fact]
        public async Task ClientController_DeleteClient_DeleteFailed_Return500()
        {
            //Arrange
            int userId = 1;
            int orgId = 1;
            int logTypeId = 1;
            bool deleteResult = false;
            A.CallTo(() => _userServices.UserExist(userId)).Returns(true);
            A.CallTo(() => _organizationServices.OrgExist(orgId)).Returns(true);
            A.CallTo(() => _organizationServices.OrgHaveRelations(orgId, userId)).Returns(false);
            A.CallTo(() => _organizationServices.DeleteOrg(orgId)).Returns(deleteResult);
            A.CallTo(() => _logServices.getLogTypeId("Delete")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", userId, logTypeId));
            var controller = new ClientController(_organizationServices, _userServices, _logServices);

            //Act
            var result = await controller.DeleteClient(orgId, userId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<StatusCodeResult>();
        }
        [Fact]
        public async Task ClientController_DeleteClient_UserNotFound_Return404()
        {
            //Arrange
            int userId = 1;
            int orgId = 1;
            int logTypeId = 1;
            bool deleteResult = true;
            A.CallTo(() => _userServices.UserExist(userId)).Returns(false);
            A.CallTo(() => _organizationServices.OrgExist(orgId)).Returns(true);
            A.CallTo(() => _organizationServices.OrgHaveRelations(orgId, userId)).Returns(false);
            A.CallTo(() => _organizationServices.DeleteOrg(orgId)).Returns(deleteResult);
            A.CallTo(() => _logServices.getLogTypeId("Delete")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", userId, logTypeId));
            var controller = new ClientController(_organizationServices, _userServices, _logServices);

            //Act
            var result = await controller.DeleteClient(orgId, userId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundObjectResult>();
        }
        [Fact]
        public async Task ClientController_DeleteClient_OrgNotFound_Return404()
        {
            //Arrange
            int userId = 1;
            int orgId = 1;
            int logTypeId = 1;
            bool deleteResult = true;
            A.CallTo(() => _userServices.UserExist(userId)).Returns(true);
            A.CallTo(() => _organizationServices.OrgExist(orgId)).Returns(false);
            A.CallTo(() => _organizationServices.OrgHaveRelations(orgId, userId)).Returns(false);
            A.CallTo(() => _organizationServices.DeleteOrg(orgId)).Returns(deleteResult);
            A.CallTo(() => _logServices.getLogTypeId("Delete")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", userId, logTypeId));
            var controller = new ClientController(_organizationServices, _userServices, _logServices);

            //Act
            var result = await controller.DeleteClient(orgId, userId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundObjectResult>();
        }
        [Fact]
        public async Task ClientController_DeleteClient_HaveRelations_Return400()
        {
            //Arrange
            int userId = 1;
            int orgId = 1;
            int logTypeId = 1;
            bool deleteResult = true;
            A.CallTo(() => _userServices.UserExist(userId)).Returns(true);
            A.CallTo(() => _organizationServices.OrgExist(orgId)).Returns(true);
            A.CallTo(() => _organizationServices.OrgHaveRelations(orgId, userId)).Returns(true);
            A.CallTo(() => _organizationServices.DeleteOrg(orgId)).Returns(deleteResult);
            A.CallTo(() => _logServices.getLogTypeId("Delete")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", userId, logTypeId));
            var controller = new ClientController(_organizationServices, _userServices, _logServices);

            //Act
            var result = await controller.DeleteClient(orgId, userId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<BadRequestObjectResult>();
        }
    }
}
