using database_communicator.Controllers;
using database_communicator.Models.DTOs;
using database_communicator.Models.DTOs.Get;
using database_communicator.Services;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;

namespace database_communicator_Test.Controllers
{
    public class UserControllerTest
    {
        private readonly IUserServices _userServices;
        public UserControllerTest()
        {
            _userServices = A.Fake<IUserServices>();
        }
        [Fact]
        public async Task UserController_SignIn_ReturnOk()
        {
            //Arrange
            var loginInfo = A.Fake<UserLogin>();
            int userId = 1;
            A.CallTo(() => _userServices.UserExist(loginInfo.Email)).Returns(true);
            A.CallTo(() => _userServices.IsOrgUser(loginInfo.Email));
            A.CallTo(() => _userServices.VerifyUserPassword(loginInfo.Email, loginInfo.Password)).Returns(true);
            A.CallTo(() => _userServices.GetUserId(loginInfo.Email)).Returns(userId);
            A.CallTo(() => _userServices.GetUserRole(userId));
            var controller = new UserController(_userServices);

            //Act
            var result = await controller.SignIn(loginInfo);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();
        }
        [Fact]
        public async Task UserController_SignIn_UserNotFound_Return401()
        {
            //Arrange
            var loginInfo = A.Fake<UserLogin>();
            int userId = 1;
            A.CallTo(() => _userServices.UserExist(loginInfo.Email)).Returns(false);
            A.CallTo(() => _userServices.IsOrgUser(loginInfo.Email));
            A.CallTo(() => _userServices.VerifyUserPassword(loginInfo.Email, loginInfo.Password)).Returns(true);
            A.CallTo(() => _userServices.GetUserId(loginInfo.Email)).Returns(userId);
            A.CallTo(() => _userServices.GetUserRole(userId));
            var controller = new UserController(_userServices);

            //Act
            var result = await controller.SignIn(loginInfo);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<UnauthorizedResult>();
        }
        [Fact]
        public async Task UserController_SignIn_VerifyFails_Return401()
        {
            //Arrange
            var loginInfo = A.Fake<UserLogin>();
            int userId = 1;
            A.CallTo(() => _userServices.UserExist(loginInfo.Email)).Returns(true);
            A.CallTo(() => _userServices.IsOrgUser(loginInfo.Email));
            A.CallTo(() => _userServices.VerifyUserPassword(loginInfo.Email, loginInfo.Password)).Returns(false);
            A.CallTo(() => _userServices.GetUserId(loginInfo.Email)).Returns(userId);
            A.CallTo(() => _userServices.GetUserRole(userId));
            var controller = new UserController(_userServices);

            //Act
            var result = await controller.SignIn(loginInfo);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<UnauthorizedResult>();
        }
        [Fact]
        public async Task UserController_GetBasicInfo_ReturnOk()
        {
            //Arrange
            var data = A.Fake<GetUserBasicInfo>();
            int userId = 1;
            bool isOrg = true;
            A.CallTo(() => _userServices.UserExist(userId)).Returns(true);
            A.CallTo(() => _userServices.IsOrgUser(userId)).Returns(isOrg);
            A.CallTo(() => _userServices.GetBasicInfo(userId, isOrg)).Returns(data);
            var controller = new UserController(_userServices);

            //Act
            var result = await controller.GetBasicInfo(userId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();
        }
        [Fact]
        public async Task UserController_GetBasicInfo_UserNotFound_Return404()
        {
            //Arrange
            var data = A.Fake<GetUserBasicInfo>();
            int userId = 1;
            bool isOrg = true;
            A.CallTo(() => _userServices.UserExist(userId)).Returns(false);
            A.CallTo(() => _userServices.IsOrgUser(userId)).Returns(isOrg);
            A.CallTo(() => _userServices.GetBasicInfo(userId, isOrg)).Returns(data);
            var controller = new UserController(_userServices);

            //Act
            var result = await controller.GetBasicInfo(userId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundResult>();
        }
        [Fact]
        public async Task UserController_GetNotificationCount_ReturnOk()
        {
            //Arrange
            var data = 2;
            int userId = 1;
            A.CallTo(() => _userServices.UserExist(userId)).Returns(true);
            A.CallTo(() => _userServices.GetCountNotification(userId)).Returns(data);
            var controller = new UserController(_userServices);

            //Act
            var result = await controller.GetNotificationCount(userId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();
        }
        [Fact]
        public async Task UserController_GetNotificationCount_UserNotFound_Return404()
        {
            //Arrange
            var data = 2;
            int userId = 1;
            A.CallTo(() => _userServices.UserExist(userId)).Returns(false);
            A.CallTo(() => _userServices.GetCountNotification(userId)).Returns(data);
            var controller = new UserController(_userServices);

            //Act
            var result = await controller.GetNotificationCount(userId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundResult>();
        }
        [Fact]
        public async Task UserController_GetEmail_ReturnOk()
        {
            //Arrange
            var data = "";
            int userId = 1;
            A.CallTo(() => _userServices.UserExist(userId)).Returns(true);
            A.CallTo(() => _userServices.GetUserEmail(userId)).Returns(data);
            var controller = new UserController(_userServices);

            //Act
            var result = await controller.GetEmail(userId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();
        }
        [Fact]
        public async Task UserController_GetEmail_UserNotFound_Return404()
        {
            //Arrange
            var data = "";
            int userId = 1;
            A.CallTo(() => _userServices.UserExist(userId)).Returns(false);
            A.CallTo(() => _userServices.GetUserEmail(userId)).Returns(data);
            var controller = new UserController(_userServices);

            //Act
            var result = await controller.GetEmail(userId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundResult>();
        }
        [Fact]
        public async Task UserController_GetUsers_ReturnOk()
        {
            //Arrange
            var data = A.Fake<IEnumerable<GetUsers>>();
            A.CallTo(() => _userServices.GetUsers()).Returns(data);
            var controller = new UserController(_userServices);

            //Act
            var result = await controller.GetUsers();

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();
        }
        [Fact]
        public async Task UserController_GetUserOrganization_ReturnOk()
        {
            //Arrange
            var data = "";
            int userId = 1;
            A.CallTo(() => _userServices.UserExist(userId)).Returns(true);
            A.CallTo(() => _userServices.GetUserOrg(userId)).Returns(data);
            var controller = new UserController(_userServices);

            //Act
            var result = await controller.GetUserOrganization(userId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();
        }
        [Fact]
        public async Task UserController_GetUserOrganization_UserNotFound_Return404()
        {
            //Arrange
            var data = "";
            int userId = 1;
            A.CallTo(() => _userServices.UserExist(userId)).Returns(false);
            A.CallTo(() => _userServices.GetUserOrg(userId)).Returns(data);
            var controller = new UserController(_userServices);

            //Act
            var result = await controller.GetUserOrganization(userId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundResult>();
        }
    }
}
