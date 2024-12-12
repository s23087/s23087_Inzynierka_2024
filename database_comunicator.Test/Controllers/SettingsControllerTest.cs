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
    public class SettingsControllerTest
    {
        private readonly IUserServices _userServices;
        private readonly IOrganizationServices _organizationServices;
        private readonly ILogServices _logServices;
        private readonly IRolesServices _rolesServices;
        public SettingsControllerTest()
        {
            _userServices = A.Fake<IUserServices>();
            _organizationServices = A.Fake<IOrganizationServices>();
            _logServices = A.Fake<ILogServices>();
            _rolesServices = A.Fake<IRolesServices>();
        }
        [Fact]
        public async Task SettingsController_ChangePassword_ReturnOk()
        {
            //Arrange
            var data = A.Fake<ChangePassword>();
            A.CallTo(() => _userServices.UserExist(data.UserId)).Returns(true);
            A.CallTo(() => _userServices.VerifyUserPassword(data.UserId, data.OldPassword)).Returns(true);
            A.CallTo(() => _userServices.ModifyPassword(data.UserId, data.NewPassword)).Returns(true);
            var controller = new SettingsController(_logServices, _organizationServices, _userServices, _rolesServices);

            //Act
            var result = await controller.ChangePassword(data);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkResult>();
        }
        [Fact]
        public async Task SettingsController_ChangePassword_ChangeFails_Return500()
        {
            //Arrange
            var data = A.Fake<ChangePassword>();
            A.CallTo(() => _userServices.UserExist(data.UserId)).Returns(true);
            A.CallTo(() => _userServices.VerifyUserPassword(data.UserId, data.OldPassword)).Returns(true);
            A.CallTo(() => _userServices.ModifyPassword(data.UserId, data.NewPassword)).Returns(false);
            var controller = new SettingsController(_logServices, _organizationServices, _userServices, _rolesServices);

            //Act
            var result = await controller.ChangePassword(data);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<StatusCodeResult>();
        }
        [Fact]
        public async Task SettingsController_ChangePassword_UserNotFound_Return404()
        {
            //Arrange
            var data = A.Fake<ChangePassword>();
            A.CallTo(() => _userServices.UserExist(data.UserId)).Returns(false);
            A.CallTo(() => _userServices.VerifyUserPassword(data.UserId, data.OldPassword)).Returns(true);
            A.CallTo(() => _userServices.ModifyPassword(data.UserId, data.NewPassword)).Returns(true);
            var controller = new SettingsController(_logServices, _organizationServices, _userServices, _rolesServices);

            //Act
            var result = await controller.ChangePassword(data);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundResult>();
        }
        [Fact]
        public async Task SettingsController_ChangePassword_UserPasswordFailsVerification_Return401()
        {
            //Arrange
            var data = A.Fake<ChangePassword>();
            A.CallTo(() => _userServices.UserExist(data.UserId)).Returns(true);
            A.CallTo(() => _userServices.VerifyUserPassword(data.UserId, data.OldPassword)).Returns(false);
            A.CallTo(() => _userServices.ModifyPassword(data.UserId, data.NewPassword)).Returns(true);
            var controller = new SettingsController(_logServices, _organizationServices, _userServices, _rolesServices);

            //Act
            var result = await controller.ChangePassword(data);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<UnauthorizedResult>();
        }
        [Fact]
        public async Task SettingsController_ModifyUser_ReturnOk()
        {
            //Arrange
            var data = A.Fake<ChangeUserData>();
            data.Email = "";
            int logTypeId = 1;
            A.CallTo(() => _userServices.UserExist(data.UserId)).Returns(true);
            A.CallTo(() => _userServices.EmailExist(data.Email)).Returns(false);
            A.CallTo(() => _userServices.ModifyUserData(data.UserId, data.Email, data.Username, data.Surname)).Returns(true);
            A.CallTo(() => _logServices.getLogTypeId("Modify")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", data.UserId, logTypeId));
            var controller = new SettingsController(_logServices, _organizationServices, _userServices, _rolesServices);

            //Act
            var result = await controller.ModifyUser(data);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkResult>();
        }
        [Fact]
        public async Task SettingsController_ModifyUser_ModifyFails_Return500()
        {
            //Arrange
            var data = A.Fake<ChangeUserData>();
            data.Email = "";
            int logTypeId = 1;
            A.CallTo(() => _userServices.UserExist(data.UserId)).Returns(true);
            A.CallTo(() => _userServices.EmailExist(data.Email)).Returns(false);
            A.CallTo(() => _userServices.ModifyUserData(data.UserId, data.Email, data.Username, data.Surname)).Returns(false);
            A.CallTo(() => _logServices.getLogTypeId("Modify")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", data.UserId, logTypeId));
            var controller = new SettingsController(_logServices, _organizationServices, _userServices, _rolesServices);

            //Act
            var result = await controller.ModifyUser(data);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<StatusCodeResult>();
        }
        [Fact]
        public async Task SettingsController_ModifyUser_UserNotFound_Return404()
        {
            //Arrange
            var data = A.Fake<ChangeUserData>();
            data.Email = "";
            int logTypeId = 1;
            A.CallTo(() => _userServices.UserExist(data.UserId)).Returns(false);
            A.CallTo(() => _userServices.EmailExist(data.Email)).Returns(false);
            A.CallTo(() => _userServices.ModifyUserData(data.UserId, data.Email, data.Username, data.Surname)).Returns(true);
            A.CallTo(() => _logServices.getLogTypeId("Modify")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", data.UserId, logTypeId));
            var controller = new SettingsController(_logServices, _organizationServices, _userServices, _rolesServices);

            //Act
            var result = await controller.ModifyUser(data);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundResult>();
        }
        [Fact]
        public async Task SettingsController_ModifyUser_NewEmailAlreadyExist_Return400()
        {
            //Arrange
            var data = A.Fake<ChangeUserData>();
            data.Email = "";
            int logTypeId = 1;
            A.CallTo(() => _userServices.UserExist(data.UserId)).Returns(true);
            A.CallTo(() => _userServices.EmailExist(data.Email)).Returns(true);
            A.CallTo(() => _userServices.ModifyUserData(data.UserId, data.Email, data.Username, data.Surname)).Returns(true);
            A.CallTo(() => _logServices.getLogTypeId("Modify")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", data.UserId, logTypeId));
            var controller = new SettingsController(_logServices, _organizationServices, _userServices, _rolesServices);

            //Act
            var result = await controller.ModifyUser(data);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<BadRequestResult>();
        }
        [Fact]
        public async Task SettingsController_AddNewUser_ReturnOk()
        {
            //Arrange
            var newUser = A.Fake<AddUser>();
            newUser.RoleName = "";
            int userId = 1;
            int orgId = 1;
            int roleId = 1;
            int logTypeId = 1;
            A.CallTo(() => _userServices.UserExist(userId)).Returns(true);
            A.CallTo(() => _userServices.EmailExist(newUser.Email)).Returns(false);
            A.CallTo(() => _userServices.GetOrgId(userId, true)).Returns(orgId);
            A.CallTo(() => _rolesServices.GetRoleId(newUser.RoleName)).Returns(roleId);
            A.CallTo(() => _userServices.AddUser(newUser, orgId, roleId, true)).Returns(true);
            A.CallTo(() => _logServices.getLogTypeId("Create")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", userId, logTypeId));
            var controller = new SettingsController(_logServices, _organizationServices, _userServices, _rolesServices);

            //Act
            var result = await controller.AddNewUser(newUser, userId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkResult>();
        }
        [Fact]
        public async Task SettingsController_AddNewUser_AddFails_Return400()
        {
            //Arrange
            var newUser = A.Fake<AddUser>();
            newUser.RoleName = "";
            int userId = 1;
            int orgId = 1;
            int roleId = 1;
            int logTypeId = 1;
            A.CallTo(() => _userServices.UserExist(userId)).Returns(true);
            A.CallTo(() => _userServices.EmailExist(newUser.Email)).Returns(false);
            A.CallTo(() => _userServices.GetOrgId(userId, true)).Returns(orgId);
            A.CallTo(() => _rolesServices.GetRoleId(newUser.RoleName)).Returns(roleId);
            A.CallTo(() => _userServices.AddUser(newUser, orgId, roleId, true)).Returns(false);
            A.CallTo(() => _logServices.getLogTypeId("Create")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", userId, logTypeId));
            var controller = new SettingsController(_logServices, _organizationServices, _userServices, _rolesServices);

            //Act
            var result = await controller.AddNewUser(newUser, userId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<BadRequestResult>();
        }
        [Fact]
        public async Task SettingsController_AddNewUser_RoleNameNull_Return400()
        {
            //Arrange
            var newUser = A.Fake<AddUser>();
            newUser.RoleName = null;
            int userId = 1;
            int orgId = 1;
            A.CallTo(() => _userServices.UserExist(userId)).Returns(true);
            A.CallTo(() => _userServices.EmailExist(newUser.Email)).Returns(false);
            A.CallTo(() => _userServices.GetOrgId(userId, true)).Returns(orgId);
            var controller = new SettingsController(_logServices, _organizationServices, _userServices, _rolesServices);

            //Act
            var result = await controller.AddNewUser(newUser, userId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<BadRequestObjectResult>();
        }
        [Fact]
        public async Task SettingsController_AddNewUser_EmailAlreadyExist_Return400()
        {
            //Arrange
            var newUser = A.Fake<AddUser>();
            newUser.RoleName = "";
            int userId = 1;
            int orgId = 1;
            int roleId = 1;
            int logTypeId = 1;
            A.CallTo(() => _userServices.UserExist(userId)).Returns(true);
            A.CallTo(() => _userServices.EmailExist(newUser.Email)).Returns(true);
            A.CallTo(() => _userServices.GetOrgId(userId, true)).Returns(orgId);
            A.CallTo(() => _rolesServices.GetRoleId(newUser.RoleName)).Returns(roleId);
            A.CallTo(() => _userServices.AddUser(newUser, orgId, roleId, true)).Returns(true);
            A.CallTo(() => _logServices.getLogTypeId("Create")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", userId, logTypeId));
            var controller = new SettingsController(_logServices, _organizationServices, _userServices, _rolesServices);

            //Act
            var result = await controller.AddNewUser(newUser, userId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<BadRequestObjectResult>();
        }
        [Fact]
        public async Task SettingsController_AddNewUser_UserNotFound_Return404()
        {
            //Arrange
            var newUser = A.Fake<AddUser>();
            newUser.RoleName = "";
            int userId = 1;
            int orgId = 1;
            int roleId = 1;
            int logTypeId = 1;
            A.CallTo(() => _userServices.UserExist(userId)).Returns(false);
            A.CallTo(() => _userServices.EmailExist(newUser.Email)).Returns(false);
            A.CallTo(() => _userServices.GetOrgId(userId, true)).Returns(orgId);
            A.CallTo(() => _rolesServices.GetRoleId(newUser.RoleName)).Returns(roleId);
            A.CallTo(() => _userServices.AddUser(newUser, orgId, roleId, true)).Returns(true);
            A.CallTo(() => _logServices.getLogTypeId("Create")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", userId, logTypeId));
            var controller = new SettingsController(_logServices, _organizationServices, _userServices, _rolesServices);

            //Act
            var result = await controller.AddNewUser(newUser, userId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundResult>();
        }
        [Fact]
        public async Task SettingsController_SwitchToOrg_ReturnOk()
        {
            //Arrange
            int orgId = 1;
            int roleId = 1;
            A.CallTo(() => _userServices.GetOrgId(A<int>.Ignored, false)).Returns(orgId);
            A.CallTo(() => _rolesServices.GetRoleId("Admin")).Returns(roleId);
            A.CallTo(() => _userServices.SwitchToOrg(A<int>.Ignored, roleId, orgId)).Returns(true);
            var controller = new SettingsController(_logServices, _organizationServices, _userServices, _rolesServices);

            //Act
            var result = await controller.SwitchToOrg();

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkResult>();
        }
        [Fact]
        public async Task SettingsController_SwitchToOrg_SwitchFails_Return400()
        {
            //Arrange
            int orgId = 1;
            int roleId = 1;;
            A.CallTo(() => _userServices.GetOrgId(A<int>.Ignored, false)).Returns(orgId);
            A.CallTo(() => _rolesServices.GetRoleId("Admin")).Returns(roleId);
            A.CallTo(() => _userServices.SwitchToOrg(A<int>.Ignored, roleId, orgId)).Returns(false);
            var controller = new SettingsController(_logServices, _organizationServices, _userServices, _rolesServices);

            //Act
            var result = await controller.SwitchToOrg();

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<BadRequestResult>();
        }
        [Fact]
        public async Task SettingsController_GetLogs_ReturnOk()
        {
            //Arrange
            var data = A.Fake<IEnumerable<GetLogs>>();
            int userId = 1;
            string role = "Admin";
            A.CallTo(() => _userServices.UserExist(userId)).Returns(true);
            A.CallTo(() => _userServices.GetUserRole(userId)).Returns(role);
            A.CallTo(() => _logServices.GetLogs()).Returns(data);
            var controller = new SettingsController(_logServices, _organizationServices, _userServices, _rolesServices);

            //Act
            var result = await controller.GetLogs(userId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();
        }
        [Fact]
        public async Task SettingsController_GetLogs_UserNotAdminNotSolo_Return401()
        {
            //Arrange
            var data = A.Fake<IEnumerable<GetLogs>>();
            int userId = 1;
            string role = "";
            A.CallTo(() => _userServices.UserExist(userId)).Returns(true);
            A.CallTo(() => _userServices.GetUserRole(userId)).Returns(role);
            A.CallTo(() => _logServices.GetLogs()).Returns(data);
            var controller = new SettingsController(_logServices, _organizationServices, _userServices, _rolesServices);

            //Act
            var result = await controller.GetLogs(userId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<UnauthorizedResult>();
        }
        [Fact]
        public async Task SettingsController_GetLogs_UserNotFound_Return404()
        {
            //Arrange
            var data = A.Fake<IEnumerable<GetLogs>>();
            int userId = 1;
            string role = "Admin";
            A.CallTo(() => _userServices.UserExist(userId)).Returns(false);
            A.CallTo(() => _userServices.GetUserRole(userId)).Returns(role);
            A.CallTo(() => _logServices.GetLogs()).Returns(data);
            var controller = new SettingsController(_logServices, _organizationServices, _userServices, _rolesServices);

            //Act
            var result = await controller.GetLogs(userId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundResult>();
        }
        [Fact]
        public async Task SettingsController_GetUserOrg_ReturnOk()
        {
            //Arrange
            var data = A.Fake<GetOrg>();
            int userId = 1;
            bool isOrg = true;
            int orgId = 1;
            A.CallTo(() => _userServices.UserExist(userId)).Returns(true);
            A.CallTo(() => _userServices.IsOrgUser(userId)).Returns(isOrg);
            A.CallTo(() => _userServices.GetOrgId(userId, isOrg)).Returns(orgId);
            A.CallTo(() => _organizationServices.GetOrg(orgId)).Returns(data);
            var controller = new SettingsController(_logServices, _organizationServices, _userServices, _rolesServices);

            //Act
            var result = await controller.GetUserOrg(userId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();
        }
        [Fact]
        public async Task SettingsController_GetUserOrg_UserNotFound_Return404()
        {
            //Arrange
            var data = A.Fake<GetOrg>();
            int userId = 1;
            bool isOrg = true;
            int orgId = 1;
            A.CallTo(() => _userServices.UserExist(userId)).Returns(false);
            A.CallTo(() => _userServices.IsOrgUser(userId)).Returns(isOrg);
            A.CallTo(() => _userServices.GetOrgId(userId, isOrg)).Returns(orgId);
            A.CallTo(() => _organizationServices.GetOrg(orgId)).Returns(data);
            var controller = new SettingsController(_logServices, _organizationServices, _userServices, _rolesServices);

            //Act
            var result = await controller.GetUserOrg(userId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundResult>();
        }
    }
}
