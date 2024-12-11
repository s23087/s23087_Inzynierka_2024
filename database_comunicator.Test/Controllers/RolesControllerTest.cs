using database_communicator.Controllers;
using database_communicator.Models.DTOs.Get;
using database_communicator.Models.DTOs.Modify;
using database_communicator.Services;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;

namespace database_communicator_Test.Controllers
{
    public class RolesControllerTest
    {
        private readonly IRolesServices _rolesServices;
        private readonly IUserServices _userServices;
        public RolesControllerTest()
        {
            _rolesServices = A.Fake<IRolesServices>();
            _userServices = A.Fake<IUserServices>();
        }
        [Fact]
        public async Task RolesController_GetUserWithRoles_ReturnOk()
        {
            //Arrange
            var data = A.Fake<IEnumerable<GetOrgUsersWithRoles>>();
            int userId = 1;
            A.CallTo(() => _userServices.UserExist(userId)).Returns(true);
            A.CallTo(() => _userServices.IsOrgUser(userId)).Returns(true);
            A.CallTo(() => _rolesServices.GetOrgUsersWithRoles(userId, null, null)).Returns(data);
            var controller = new RolesController(_rolesServices, _userServices);

            //Act
            var result = await controller.GetUserWithRoles(userId, null, null, null);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();
        }
        [Fact]
        public async Task RolesController_GetUserWithRoles_UserNotFound_Return404()
        {
            //Arrange
            var data = A.Fake<IEnumerable<GetOrgUsersWithRoles>>();
            int userId = 1;
            A.CallTo(() => _userServices.UserExist(userId)).Returns(false);
            A.CallTo(() => _userServices.IsOrgUser(userId)).Returns(true);
            A.CallTo(() => _rolesServices.GetOrgUsersWithRoles(userId, null, null)).Returns(data);
            var controller = new RolesController(_rolesServices, _userServices);

            //Act
            var result = await controller.GetUserWithRoles(userId, null, null, null);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundResult>();
        }
        [Fact]
        public async Task RolesController_GetUserWithRoles_IsNotOrgUser_Return400()
        {
            //Arrange
            var data = A.Fake<IEnumerable<GetOrgUsersWithRoles>>();
            int userId = 1;
            A.CallTo(() => _userServices.UserExist(userId)).Returns(true);
            A.CallTo(() => _userServices.IsOrgUser(userId)).Returns(false);
            A.CallTo(() => _rolesServices.GetOrgUsersWithRoles(userId, null, null)).Returns(data);
            var controller = new RolesController(_rolesServices, _userServices);

            //Act
            var result = await controller.GetUserWithRoles(userId, null, null, null);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<BadRequestResult>();
        }
        [Fact]
        public async Task RolesController_GetUserWithRoles_SortIsIncorrect_Return400()
        {
            //Arrange
            var data = A.Fake<IEnumerable<GetOrgUsersWithRoles>>();
            int userId = 1;
            string sort = "";
            A.CallTo(() => _userServices.UserExist(userId)).Returns(true);
            A.CallTo(() => _userServices.IsOrgUser(userId)).Returns(true);
            A.CallTo(() => _rolesServices.GetOrgUsersWithRoles(userId, null, null)).Returns(data);
            var controller = new RolesController(_rolesServices, _userServices);

            //Act
            var result = await controller.GetUserWithRoles(userId, null, sort, null);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<BadRequestObjectResult>();
        }
        [Fact]
        public async Task RolesController_GetRoles_ReturnOk()
        {
            //Arrange
            var data = A.Fake<IEnumerable<string>>();
            A.CallTo(() => _rolesServices.GetRoleNames()).Returns(data);
            var controller = new RolesController(_rolesServices, _userServices);

            //Act
            var result = await controller.GetRoles();

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();
        }
        [Fact]
        public async Task RolesController_ModifyUserRole_ReturnOk()
        {
            //Arrange
            var data = A.Fake<ModifyUserRole>();
            int roleId = 1;
            int? orgUserId = 1;
            A.CallTo(() => _userServices.UserExist(data.UserId)).Returns(true);
            A.CallTo(() => _rolesServices.GetRoleId(data.RoleName)).Returns(roleId);
            A.CallTo(() => _userServices.GetOrgUserId(data.ChosenUserId)).Returns(orgUserId);
            A.CallTo(() => _userServices.ModifyUserRole((int)orgUserId, roleId)).Returns(true);
            var controller = new RolesController(_rolesServices, _userServices);

            //Act
            var result = await controller.ModifyUserRole(data);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkResult>();
        }
        [Fact]
        public async Task RolesController_ModifyUserRole_ModifyFails_Return500()
        {
            //Arrange
            var data = A.Fake<ModifyUserRole>();
            int roleId = 1;
            int? orgUserId = 1;
            A.CallTo(() => _userServices.UserExist(data.UserId)).Returns(true);
            A.CallTo(() => _rolesServices.GetRoleId(data.RoleName)).Returns(roleId);
            A.CallTo(() => _userServices.GetOrgUserId(data.ChosenUserId)).Returns(orgUserId);
            A.CallTo(() => _userServices.ModifyUserRole((int)orgUserId, roleId)).Returns(false);
            var controller = new RolesController(_rolesServices, _userServices);

            //Act
            var result = await controller.ModifyUserRole(data);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<StatusCodeResult>();
        }
        [Fact]
        public async Task RolesController_ModifyUserRole_UserNotFound_Return404()
        {
            //Arrange
            var data = A.Fake<ModifyUserRole>();
            int roleId = 1;
            int? orgUserId = 1;
            A.CallTo(() => _userServices.UserExist(data.UserId)).Returns(false);
            A.CallTo(() => _rolesServices.GetRoleId(data.RoleName)).Returns(roleId);
            A.CallTo(() => _userServices.GetOrgUserId(data.ChosenUserId)).Returns(orgUserId);
            A.CallTo(() => _userServices.ModifyUserRole((int)orgUserId, roleId)).Returns(true);
            var controller = new RolesController(_rolesServices, _userServices);

            //Act
            var result = await controller.ModifyUserRole(data);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundObjectResult>();
        }
        [Fact]
        public async Task RolesController_ModifyUserRole_OrgUserNotFound_Return404()
        {
            //Arrange
            var data = A.Fake<ModifyUserRole>();
            int roleId = 1;
            int? orgUserId = null;
            A.CallTo(() => _userServices.UserExist(data.UserId)).Returns(true);
            A.CallTo(() => _rolesServices.GetRoleId(data.RoleName)).Returns(roleId);
            A.CallTo(() => _userServices.GetOrgUserId(data.ChosenUserId)).Returns(orgUserId);
            var controller = new RolesController(_rolesServices, _userServices);

            //Act
            var result = await controller.ModifyUserRole(data);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundObjectResult>();
        }
    }
}
