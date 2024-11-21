using database_communicator.Controllers;
using database_communicator.FilterClass;
using database_communicator.Models.DTOs.Create;
using database_communicator.Models.DTOs.Get;
using database_communicator.Services;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;

namespace database_communicator_Test.Controllers
{
    public class OutsideItemControllerTest
    {
        private readonly IOutsideItemServices _outsideItemServices;
        private readonly IUserServices _userServices;
        private readonly ILogServices _logServices;
        private readonly INotificationServices _notificationServices;
        public OutsideItemControllerTest()
        {
            _outsideItemServices = A.Fake<IOutsideItemServices>();
            _userServices = A.Fake<IUserServices>();
            _logServices = A.Fake<ILogServices>();
            _notificationServices = A.Fake<INotificationServices>();
        }
        [Fact]
        public async Task OutsideItemController_GetOutsideItems_ReturnOk()
        {
            //Arrange
            var data = A.Fake<IEnumerable<GetOutsideItem>>();
            var filters = A.Fake<OutsideItemFiltersTemplate>();
            A.CallTo(() => _outsideItemServices.GetItems(null, filters)).Returns(data);
            var controller = new OutsideItemController(_outsideItemServices, _userServices, _logServices, _notificationServices);

            //Act
            var result = await controller.GetOutsideItems(null, null, null, null, null, null, null, null);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();
        }
        [Fact]
        public async Task OutsideItemController_GetOutsideItems_SortIncorrect_Return400()
        {
            //Arrange
            var data = A.Fake<IEnumerable<GetOutsideItem>>();
            var filters = A.Fake<OutsideItemFiltersTemplate>();
            string sort = "";
            A.CallTo(() => _outsideItemServices.GetItems(sort, filters)).Returns(data);
            var controller = new OutsideItemController(_outsideItemServices, _userServices, _logServices, _notificationServices);

            //Act
            var result = await controller.GetOutsideItems(null, sort, null, null, null, null, null, null);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<BadRequestObjectResult>();
        }
        [Fact]
        public async Task OutsideItemController_DeleteOutsideItem_ReturnOk()
        {
            //Arrange
            int itemId = 1;
            int orgId = 1;
            int userId = 1;
            var owners = A.CollectionOfDummy<int>(2);
            int logTypeId = 1;
            string userFull = "";
            var newNotification = A.Fake<CreateNotification>();
            A.CallTo(() => _userServices.UserExist(userId)).Returns(true);
            A.CallTo(() => _outsideItemServices.ItemExist(itemId, orgId)).Returns(true);
            A.CallTo(() => _outsideItemServices.GetItemOwners(itemId, orgId)).Returns(owners);
            A.CallTo(() => _outsideItemServices.DeleteItem(itemId, orgId)).Returns(true);
            A.CallTo(() => _logServices.getLogTypeId("Delete")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", userId, logTypeId));
            A.CallTo(() => _userServices.GetUserFullName(userId)).Returns(userFull);
            A.CallTo(() => _notificationServices.CreateNotification(newNotification));
            var controller = new OutsideItemController(_outsideItemServices, _userServices, _logServices, _notificationServices);

            //Act
            var result = await controller.DeleteOutsideItem(itemId, orgId, userId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkResult>();
        }
        [Fact]
        public async Task OutsideItemController_DeleteOutsideItem_DeleteFails_Return500()
        {
            //Arrange
            int itemId = 1;
            int orgId = 1;
            int userId = 1;
            var owners = A.CollectionOfDummy<int>(2);
            int logTypeId = 1;
            string userFull = "";
            var newNotification = A.Fake<CreateNotification>();
            A.CallTo(() => _userServices.UserExist(userId)).Returns(true);
            A.CallTo(() => _outsideItemServices.ItemExist(itemId, orgId)).Returns(true);
            A.CallTo(() => _outsideItemServices.GetItemOwners(itemId, orgId)).Returns(owners);
            A.CallTo(() => _outsideItemServices.DeleteItem(itemId, orgId)).Returns(false);
            A.CallTo(() => _logServices.getLogTypeId("Delete")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", userId, logTypeId));
            A.CallTo(() => _userServices.GetUserFullName(userId)).Returns(userFull);
            A.CallTo(() => _notificationServices.CreateNotification(newNotification));
            var controller = new OutsideItemController(_outsideItemServices, _userServices, _logServices, _notificationServices);

            //Act
            var result = await controller.DeleteOutsideItem(itemId, orgId, userId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<StatusCodeResult>();
        }
        [Fact]
        public async Task OutsideItemController_DeleteOutsideItem_UserNotFound_Return404()
        {
            //Arrange
            int itemId = 1;
            int orgId = 1;
            int userId = 1;
            var owners = A.CollectionOfDummy<int>(2);
            int logTypeId = 1;
            string userFull = "";
            var newNotification = A.Fake<CreateNotification>();
            A.CallTo(() => _userServices.UserExist(userId)).Returns(false);
            A.CallTo(() => _outsideItemServices.ItemExist(itemId, orgId)).Returns(true);
            A.CallTo(() => _outsideItemServices.GetItemOwners(itemId, orgId)).Returns(owners);
            A.CallTo(() => _outsideItemServices.DeleteItem(itemId, orgId)).Returns(true);
            A.CallTo(() => _logServices.getLogTypeId("Delete")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", userId, logTypeId));
            A.CallTo(() => _userServices.GetUserFullName(userId)).Returns(userFull);
            A.CallTo(() => _notificationServices.CreateNotification(newNotification));
            var controller = new OutsideItemController(_outsideItemServices, _userServices, _logServices, _notificationServices);

            //Act
            var result = await controller.DeleteOutsideItem(itemId, orgId, userId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundObjectResult>();
        }
        [Fact]
        public async Task OutsideItemController_DeleteOutsideItem_ItemNotFound_Return404()
        {
            //Arrange
            int itemId = 1;
            int orgId = 1;
            int userId = 1;
            var owners = A.CollectionOfDummy<int>(2);
            int logTypeId = 1;
            string userFull = "";
            var newNotification = A.Fake<CreateNotification>();
            A.CallTo(() => _userServices.UserExist(userId)).Returns(true);
            A.CallTo(() => _outsideItemServices.ItemExist(itemId, orgId)).Returns(false);
            A.CallTo(() => _outsideItemServices.GetItemOwners(itemId, orgId)).Returns(owners);
            A.CallTo(() => _outsideItemServices.DeleteItem(itemId, orgId)).Returns(true);
            A.CallTo(() => _logServices.getLogTypeId("Delete")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", userId, logTypeId));
            A.CallTo(() => _userServices.GetUserFullName(userId)).Returns(userFull);
            A.CallTo(() => _notificationServices.CreateNotification(newNotification));
            var controller = new OutsideItemController(_outsideItemServices, _userServices, _logServices, _notificationServices);

            //Act
            var result = await controller.DeleteOutsideItem(itemId, orgId, userId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundObjectResult>();
        }
        [Fact]
        public async Task OutsideItemController_AddOutsideItems_ReturnOk()
        {
            //Arrange
            var data = A.Fake<CreateOutsideItems>();
            data.Items = A.CollectionOfDummy<AddOutsideItem>(2);
            int userId = 1;
            var notification = A.Fake<CreateNotification>();
            var errorItems = A.CollectionOfDummy<string>(0);
            int logTypeId = 1;
            A.CallTo(() => _notificationServices.CreateNotification(notification));
            A.CallTo(() => _userServices.UserExist(userId)).Returns(true);
            A.CallTo(() => _logServices.getLogTypeId("Create")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", userId, logTypeId));
            A.CallTo(() => _notificationServices.CreateNotification(notification));
            var controller = new OutsideItemController(_outsideItemServices, _userServices, _logServices, _notificationServices);

            //Act
            var result = await controller.AddOutsideItems(data, userId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkResult>();
        }
        [Fact]
        public async Task OutsideItemController_AddOutsideItems_UserNotFound_Return404()
        {
            //Arrange
            var data = A.Fake<CreateOutsideItems>();
            data.Items = A.CollectionOfDummy<AddOutsideItem>(2);
            int userId = 1;
            var notification = A.Fake<CreateNotification>();
            var errorItems = A.CollectionOfDummy<string>(0);
            int logTypeId = 1;
            A.CallTo(() => _notificationServices.CreateNotification(notification));
            A.CallTo(() => _userServices.UserExist(userId)).Returns(false);
            A.CallTo(() => _logServices.getLogTypeId("Create")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", userId, logTypeId));
            A.CallTo(() => _notificationServices.CreateNotification(notification));
            var controller = new OutsideItemController(_outsideItemServices, _userServices, _logServices, _notificationServices);

            //Act
            var result = await controller.AddOutsideItems(data, userId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundObjectResult>();
        }
        [Fact]
        public async Task OutsideItemController_AddOutsideItems_AddedWithErrors_ReturnOk()
        {
            //Arrange
            var data = A.Fake<CreateOutsideItems>();
            data.Items = A.CollectionOfDummy<AddOutsideItem>(2);
            int userId = 1;
            var notification = A.Fake<CreateNotification>();
            var errorItems = A.CollectionOfDummy<string>(1);
            int logTypeId = 1;
            A.CallTo(() => _notificationServices.CreateNotification(notification));
            A.CallTo(() => _userServices.UserExist(userId)).Returns(true);
            A.CallTo(() => _logServices.getLogTypeId("Create")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", userId, logTypeId));
            A.CallTo(() => _notificationServices.CreateNotification(notification));
            var controller = new OutsideItemController(_outsideItemServices, _userServices, _logServices, _notificationServices);

            //Act
            var result = await controller.AddOutsideItems(data, userId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkResult>();
        }
        [Fact]
        public async Task OutsideItemController_AddOutsideItems_ImportFailed_ReturnOk()
        {
            //Arrange
            var data = A.Fake<CreateOutsideItems>();
            data.Items = A.CollectionOfDummy<AddOutsideItem>(2);
            int userId = 1;
            var notification = A.Fake<CreateNotification>();
            var errorItems = A.CollectionOfDummy<string>(2);
            A.CallTo(() => _notificationServices.CreateNotification(notification));
            A.CallTo(() => _userServices.UserExist(userId)).Returns(true);
            A.CallTo(() => _notificationServices.CreateNotification(notification));
            var controller = new OutsideItemController(_outsideItemServices, _userServices, _logServices, _notificationServices);

            //Act
            var result = await controller.AddOutsideItems(data, userId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkResult>();
        }
        [Fact]
        public async Task OutsideItemController_AddNotification_ReturnOk()
        {
            //Arrange
            var data = A.Fake<AddImportErrorNotification>();
            int userId = 1;
            var notification = A.Fake<CreateNotification>();
            A.CallTo(() => _userServices.UserExist(userId)).Returns(true);
            A.CallTo(() => _notificationServices.CreateNotification(notification));
            var controller = new OutsideItemController(_outsideItemServices, _userServices, _logServices, _notificationServices);

            //Act
            var result = await controller.AddNotification(data, userId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkResult>();
        }
        [Fact]
        public async Task OutsideItemController_AddNotification_UserNotFound_Return404()
        {
            //Arrange
            var data = A.Fake<AddImportErrorNotification>();
            int userId = 1;
            var notification = A.Fake<CreateNotification>();
            A.CallTo(() => _userServices.UserExist(userId)).Returns(false);
            A.CallTo(() => _notificationServices.CreateNotification(notification));
            var controller = new OutsideItemController(_outsideItemServices, _userServices, _logServices, _notificationServices);

            //Act
            var result = await controller.AddNotification(data, userId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundObjectResult>();
        }
    }
}
