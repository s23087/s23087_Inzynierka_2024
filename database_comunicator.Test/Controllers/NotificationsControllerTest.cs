using database_communicator.Controllers;
using database_communicator.Models.DTOs.Get;
using database_communicator.Services;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;

namespace database_communicator_Test.Controllers
{
    public class NotificationsControllerTest
    {
        private readonly IUserServices _userServices;
        private readonly INotificationServices _notificationServices;
        public NotificationsControllerTest()
        {
            _userServices = A.Fake<IUserServices>();
            _notificationServices = A.Fake<INotificationServices>();
        }
        [Fact]
        public async Task NotificationsController_GetNotifications_ReturnOk()
        {
            //Arrange
            var data = A.Fake<IEnumerable<GetNotifications>>();
            int userId = 1;
            A.CallTo(() => _userServices.UserExist(userId)).Returns(true);
            A.CallTo(() => _notificationServices.GetNotifications(userId)).Returns(data);
            var controller = new NotificationsController(_userServices, _notificationServices);

            //Act
            var result = await controller.GetNotifications(userId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();
        }
        [Fact]
        public async Task NotificationsController_GetNotifications_UserNotFound_Return404()
        {
            //Arrange
            var data = A.Fake<IEnumerable<GetNotifications>>();
            int userId = 1;
            A.CallTo(() => _userServices.UserExist(userId)).Returns(false);
            A.CallTo(() => _notificationServices.GetNotifications(userId)).Returns(data);
            var controller = new NotificationsController(_userServices, _notificationServices);

            //Act
            var result = await controller.GetNotifications(userId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundResult>();
        }
        [Fact]
        public async Task NotificationsController_SetNotification_ReturnOk()
        {
            //Arrange
            int notificationId = 1;
            bool isRead = true;
            A.CallTo(() => _notificationServices.NotifExists(notificationId)).Returns(true);
            A.CallTo(() => _notificationServices.SetIsRead(notificationId, isRead)).Returns(true);
            var controller = new NotificationsController(_userServices, _notificationServices);

            //Act
            var result = await controller.SetNotification(notificationId, isRead);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkResult>();
        }
        [Fact]
        public async Task NotificationsController_SetNotification_NotificationNotFound_Return404()
        {
            //Arrange
            int notificationId = 1;
            bool isRead = true;
            A.CallTo(() => _notificationServices.NotifExists(notificationId)).Returns(false);
            A.CallTo(() => _notificationServices.SetIsRead(notificationId, isRead)).Returns(true);
            var controller = new NotificationsController(_userServices, _notificationServices);

            //Act
            var result = await controller.SetNotification(notificationId, isRead);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundResult>();
        }
        [Fact]
        public async Task NotificationsController_SetNotification_ModifyFails_Return500()
        {
            //Arrange
            int notificationId = 1;
            bool isRead = true;
            A.CallTo(() => _notificationServices.NotifExists(notificationId)).Returns(true);
            A.CallTo(() => _notificationServices.SetIsRead(notificationId, isRead)).Returns(false);
            var controller = new NotificationsController(_userServices, _notificationServices);

            //Act
            var result = await controller.SetNotification(notificationId, isRead);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<StatusCodeResult>();
        }
    }
}
