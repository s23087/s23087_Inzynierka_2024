using database_communicator.Controllers;
using database_communicator.FilterClass;
using database_communicator.Models;
using database_communicator.Models.DTOs.Create;
using database_communicator.Models.DTOs.Get;
using database_communicator.Models.DTOs.Modify;
using database_communicator.Services;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;

namespace database_communicator_Test.Controllers
{
    public class WarehouseControllerTest
    {
        private readonly IUserServices _userServices;
        private readonly IItemServices _itemServices;
        private readonly ILogServices _logServices;
        private readonly INotificationServices _notificationServices;
        public WarehouseControllerTest()
        {
            _userServices = A.Fake<IUserServices>();
            _itemServices = A.Fake<IItemServices>();
            _logServices = A.Fake<ILogServices>();
            _notificationServices = A.Fake<INotificationServices>();
        }
        [Fact]
        public async Task WarehouseController_AddItem_ReturnOk()
        {
            //Arrange
            var newItem = A.Fake<AddItem>();
            int logTypeId = 1;
            int? newItemId = 1;
            A.CallTo(() => _userServices.UserExist(newItem.UserId)).Returns(true);
            A.CallTo(() => _itemServices.EanExist(newItem.Eans)).Returns(false);
            A.CallTo(() => _itemServices.ItemExist(newItem.PartNumber)).Returns(false);
            A.CallTo(() => _itemServices.AddItem(newItem)).Returns(newItemId);
            A.CallTo(() => _logServices.getLogTypeId("Create")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", newItem.UserId, logTypeId));
            var controller = new WarehouseController(_userServices, _itemServices, _logServices, _notificationServices);

            //Act
            var result = await controller.AddItem(newItem);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkResult>();
        }
        [Fact]
        public async Task WarehouseController_AddItem_AddFails_Return500()
        {
            //Arrange
            var newItem = A.Fake<AddItem>();
            int logTypeId = 1;
            int? newItemId = null;
            A.CallTo(() => _userServices.UserExist(newItem.UserId)).Returns(true);
            A.CallTo(() => _itemServices.EanExist(newItem.Eans)).Returns(false);
            A.CallTo(() => _itemServices.ItemExist(newItem.PartNumber)).Returns(false);
            A.CallTo(() => _itemServices.AddItem(newItem)).Returns(newItemId);
            A.CallTo(() => _logServices.getLogTypeId("Create")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", newItem.UserId, logTypeId));
            var controller = new WarehouseController(_userServices, _itemServices, _logServices, _notificationServices);

            //Act
            var result = await controller.AddItem(newItem);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<StatusCodeResult>();
        }
        [Fact]
        public async Task WarehouseController_AddItem_UserNotFound_Return404()
        {
            //Arrange
            var newItem = A.Fake<AddItem>();
            int logTypeId = 1;
            int? newItemId = 1;
            A.CallTo(() => _userServices.UserExist(newItem.UserId)).Returns(false);
            A.CallTo(() => _itemServices.EanExist(newItem.Eans)).Returns(false);
            A.CallTo(() => _itemServices.ItemExist(newItem.PartNumber)).Returns(false);
            A.CallTo(() => _itemServices.AddItem(newItem)).Returns(newItemId);
            A.CallTo(() => _logServices.getLogTypeId("Create")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", newItem.UserId, logTypeId));
            var controller = new WarehouseController(_userServices, _itemServices, _logServices, _notificationServices);

            //Act
            var result = await controller.AddItem(newItem);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundObjectResult>();
        }
        [Fact]
        public async Task WarehouseController_AddItem_EanExist_Return400()
        {
            //Arrange
            var newItem = A.Fake<AddItem>();
            int logTypeId = 1;
            int? newItemId = 1;
            A.CallTo(() => _userServices.UserExist(newItem.UserId)).Returns(true);
            A.CallTo(() => _itemServices.EanExist(newItem.Eans)).Returns(true);
            A.CallTo(() => _itemServices.ItemExist(newItem.PartNumber)).Returns(false);
            A.CallTo(() => _itemServices.AddItem(newItem)).Returns(newItemId);
            A.CallTo(() => _logServices.getLogTypeId("Create")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", newItem.UserId, logTypeId));
            var controller = new WarehouseController(_userServices, _itemServices, _logServices, _notificationServices);

            //Act
            var result = await controller.AddItem(newItem);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<BadRequestObjectResult>();
        }
        [Fact]
        public async Task WarehouseController_AddItem_PartnumberExist_Return400()
        {
            //Arrange
            var newItem = A.Fake<AddItem>();
            int logTypeId = 1;
            int? newItemId = 1;
            A.CallTo(() => _userServices.UserExist(newItem.UserId)).Returns(true);
            A.CallTo(() => _itemServices.EanExist(newItem.Eans)).Returns(false);
            A.CallTo(() => _itemServices.ItemExist(newItem.PartNumber)).Returns(true);
            A.CallTo(() => _itemServices.AddItem(newItem)).Returns(newItemId);
            A.CallTo(() => _logServices.getLogTypeId("Create")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", newItem.UserId, logTypeId));
            var controller = new WarehouseController(_userServices, _itemServices, _logServices, _notificationServices);

            //Act
            var result = await controller.AddItem(newItem);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<BadRequestObjectResult>();
        }
        [Fact]
        public async Task WarehouseController_DeleteItem_ReturnOk()
        {
            //Arrange
            int userId = 1;
            int itemId = 1;
            int logTypeId = 1;
            A.CallTo(() => _userServices.UserExist(userId)).Returns(true);
            A.CallTo(() => _itemServices.ItemExist(itemId)).Returns(true);
            A.CallTo(() => _itemServices.ItemHaveRelations(itemId)).Returns(false);
            A.CallTo(() => _itemServices.RemoveItem(itemId)).Returns(true);
            A.CallTo(() => _logServices.getLogTypeId("Create")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", userId, logTypeId));
            var controller = new WarehouseController(_userServices, _itemServices, _logServices, _notificationServices);

            //Act
            var result = await controller.DeleteItem(itemId, userId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkResult>();
        }
        [Fact]
        public async Task WarehouseController_DeleteItem_DeleteFails_Return500()
        {
            //Arrange
            int userId = 1;
            int itemId = 1;
            int logTypeId = 1;
            A.CallTo(() => _userServices.UserExist(userId)).Returns(true);
            A.CallTo(() => _itemServices.ItemExist(itemId)).Returns(true);
            A.CallTo(() => _itemServices.ItemHaveRelations(itemId)).Returns(false);
            A.CallTo(() => _itemServices.RemoveItem(itemId)).Returns(false);
            A.CallTo(() => _logServices.getLogTypeId("Create")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", userId, logTypeId));
            var controller = new WarehouseController(_userServices, _itemServices, _logServices, _notificationServices);

            //Act
            var result = await controller.DeleteItem(itemId, userId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<StatusCodeResult>();
        }
        [Fact]
        public async Task WarehouseController_DeleteItem_UserNotFound_Return404()
        {
            //Arrange
            int userId = 1;
            int itemId = 1;
            int logTypeId = 1;
            A.CallTo(() => _userServices.UserExist(userId)).Returns(false);
            A.CallTo(() => _itemServices.ItemExist(itemId)).Returns(true);
            A.CallTo(() => _itemServices.ItemHaveRelations(itemId)).Returns(false);
            A.CallTo(() => _itemServices.RemoveItem(itemId)).Returns(true);
            A.CallTo(() => _logServices.getLogTypeId("Create")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", userId, logTypeId));
            var controller = new WarehouseController(_userServices, _itemServices, _logServices, _notificationServices);

            //Act
            var result = await controller.DeleteItem(itemId, userId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundObjectResult>();
        }
        [Fact]
        public async Task WarehouseController_DeleteItem_ItemNotFound_Return404()
        {
            //Arrange
            int userId = 1;
            int itemId = 1;
            int logTypeId = 1;
            A.CallTo(() => _userServices.UserExist(userId)).Returns(true);
            A.CallTo(() => _itemServices.ItemExist(itemId)).Returns(false);
            A.CallTo(() => _itemServices.ItemHaveRelations(itemId)).Returns(false);
            A.CallTo(() => _itemServices.RemoveItem(itemId)).Returns(true);
            A.CallTo(() => _logServices.getLogTypeId("Create")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", userId, logTypeId));
            var controller = new WarehouseController(_userServices, _itemServices, _logServices, _notificationServices);

            //Act
            var result = await controller.DeleteItem(itemId, userId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundObjectResult>();
        }
        [Fact]
        public async Task WarehouseController_DeleteItem_ItemHaveRelations_Return400()
        {
            //Arrange
            int userId = 1;
            int itemId = 1;
            int logTypeId = 1;
            A.CallTo(() => _userServices.UserExist(userId)).Returns(true);
            A.CallTo(() => _itemServices.ItemExist(itemId)).Returns(true);
            A.CallTo(() => _itemServices.ItemHaveRelations(itemId)).Returns(true);
            A.CallTo(() => _itemServices.RemoveItem(itemId)).Returns(true);
            A.CallTo(() => _logServices.getLogTypeId("Create")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", userId, logTypeId));
            var controller = new WarehouseController(_userServices, _itemServices, _logServices, _notificationServices);

            //Act
            var result = await controller.DeleteItem(itemId, userId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<BadRequestObjectResult>();
        }
        [Fact]
        public async Task WarehouseController_ModifyItem_ReturnOk()
        {
            //Arrange
            var newItem = A.Fake<UpdateItem>();
            int logTypeId = 1;
            A.CallTo(() => _userServices.UserExist(newItem.UserId)).Returns(true);
            A.CallTo(() => _itemServices.ItemExist(newItem.Id)).Returns(true);
            A.CallTo(() => _itemServices.ItemExist(newItem.PartNumber ?? "")).Returns(false);
            A.CallTo(() => _itemServices.UpdateItem(newItem)).Returns(true);
            A.CallTo(() => _logServices.getLogTypeId("Modify")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", newItem.UserId, logTypeId));
            var controller = new WarehouseController(_userServices, _itemServices, _logServices, _notificationServices);

            //Act
            var result = await controller.ModifyItem(newItem);

            //Assert
            A.CallTo(() => _itemServices.UpdateItem(newItem)).MustHaveHappened();
            result.Should().NotBeNull();
            result.Should().BeOfType<OkResult>();
        }
        [Fact]
        public async Task WarehouseController_ModifyItem_ModifyFails_Return500()
        {
            //Arrange
            var newItem = A.Fake<UpdateItem>();
            int logTypeId = 1;
            A.CallTo(() => _userServices.UserExist(newItem.UserId)).Returns(true);
            A.CallTo(() => _itemServices.ItemExist(newItem.Id)).Returns(true);
            A.CallTo(() => _itemServices.ItemExist(newItem.PartNumber ?? "")).Returns(false);
            A.CallTo(() => _itemServices.UpdateItem(newItem)).Returns(false);
            A.CallTo(() => _logServices.getLogTypeId("Modify")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", newItem.UserId, logTypeId));
            var controller = new WarehouseController(_userServices, _itemServices, _logServices, _notificationServices);

            //Act
            var result = await controller.ModifyItem(newItem);

            //Assert
            A.CallTo(() => _itemServices.UpdateItem(newItem)).MustHaveHappened();
            result.Should().NotBeNull();
            result.Should().BeOfType<StatusCodeResult>();
        }
        [Fact]
        public async Task WarehouseController_ModifyItem_UserNotFound_Return404()
        {
            //Arrange
            var newItem = A.Fake<UpdateItem>();
            int logTypeId = 1;
            A.CallTo(() => _userServices.UserExist(newItem.UserId)).Returns(false);
            A.CallTo(() => _itemServices.ItemExist(newItem.Id)).Returns(true);
            A.CallTo(() => _itemServices.ItemExist(newItem.PartNumber ?? "")).Returns(false);
            A.CallTo(() => _itemServices.UpdateItem(newItem)).Returns(true);
            A.CallTo(() => _logServices.getLogTypeId("Modify")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", newItem.UserId, logTypeId));
            var controller = new WarehouseController(_userServices, _itemServices, _logServices, _notificationServices);

            //Act
            var result = await controller.ModifyItem(newItem);

            //Assert
            A.CallTo(() => _itemServices.UpdateItem(newItem)).MustNotHaveHappened();
            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundResult>();
        }
        [Fact]
        public async Task WarehouseController_ModifyItem_ItemNotFound_Return404()
        {
            //Arrange
            var newItem = A.Fake<UpdateItem>();
            int logTypeId = 1;
            A.CallTo(() => _userServices.UserExist(newItem.UserId)).Returns(true);
            A.CallTo(() => _itemServices.ItemExist(newItem.Id)).Returns(false);
            A.CallTo(() => _itemServices.ItemExist(newItem.PartNumber ?? "")).Returns(false);
            A.CallTo(() => _itemServices.UpdateItem(newItem)).Returns(true);
            A.CallTo(() => _logServices.getLogTypeId("Modify")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", newItem.UserId, logTypeId));
            var controller = new WarehouseController(_userServices, _itemServices, _logServices, _notificationServices);

            //Act
            var result = await controller.ModifyItem(newItem);

            //Assert
            A.CallTo(() => _itemServices.UpdateItem(newItem)).MustNotHaveHappened();
            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundResult>();
        }
        [Fact]
        public async Task WarehouseController_ModifyItem_NewPartNumberAlreadyExist_Return400()
        {
            //Arrange
            var newItem = A.Fake<UpdateItem>();
            newItem.PartNumber = "";
            int logTypeId = 1;
            A.CallTo(() => _userServices.UserExist(newItem.UserId)).Returns(true);
            A.CallTo(() => _itemServices.ItemExist(newItem.Id)).Returns(true);
            A.CallTo(() => _itemServices.ItemExist(newItem.PartNumber)).Returns(true);
            A.CallTo(() => _itemServices.UpdateItem(newItem)).Returns(true);
            A.CallTo(() => _logServices.getLogTypeId("Modify")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", newItem.UserId, logTypeId));
            var controller = new WarehouseController(_userServices, _itemServices, _logServices, _notificationServices);

            //Act
            var result = await controller.ModifyItem(newItem);

            //Assert
            A.CallTo(() => _itemServices.UpdateItem(newItem)).MustNotHaveHappened();
            result.Should().NotBeNull();
            result.Should().BeOfType<BadRequestResult>();
        }
        [Fact]
        public async Task WarehouseController_GetRestInfo_ReturnOk()
        {
            //Arrange
            var data = A.Fake<GetRestInfo>();
            int itemId = 1;
            int userId = 1;
            string currency = "PLN";
            int logTypeId = 1;
            A.CallTo(() => _userServices.UserExist(userId)).Returns(true);
            A.CallTo(() => _itemServices.ItemExist(itemId)).Returns(true);
            A.CallTo(() => _itemServices.GetRestOfItem(itemId, userId, currency)).Returns(data);
            var controller = new WarehouseController(_userServices, _itemServices, _logServices, _notificationServices);

            //Act
            var result = await controller.GetRestInfo(itemId, userId, currency);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();
        }
        [Fact]
        public async Task WarehouseController_GetRestInfo_UserNotFound_Return404()
        {
            //Arrange
            var data = A.Fake<GetRestInfo>();
            int itemId = 1;
            int userId = 1;
            string currency = "PLN";
            int logTypeId = 1;
            A.CallTo(() => _userServices.UserExist(userId)).Returns(false);
            A.CallTo(() => _itemServices.ItemExist(itemId)).Returns(true);
            A.CallTo(() => _itemServices.GetRestOfItem(itemId, userId, currency)).Returns(data);
            var controller = new WarehouseController(_userServices, _itemServices, _logServices, _notificationServices);

            //Act
            var result = await controller.GetRestInfo(itemId, userId, currency);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundResult>();
        }
        [Fact]
        public async Task WarehouseController_GetRestInfo_ItemNotFound_Return404()
        {
            //Arrange
            var data = A.Fake<GetRestInfo>();
            int itemId = 1;
            int userId = 1;
            string currency = "PLN";
            int logTypeId = 1;
            A.CallTo(() => _userServices.UserExist(userId)).Returns(true);
            A.CallTo(() => _itemServices.ItemExist(itemId)).Returns(false);
            A.CallTo(() => _itemServices.GetRestOfItem(itemId, userId, currency)).Returns(data);
            var controller = new WarehouseController(_userServices, _itemServices, _logServices, _notificationServices);

            //Act
            var result = await controller.GetRestInfo(itemId, userId, currency);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundResult>();
        }
        [Fact]
        public async Task WarehouseController_GetRestOrgInfo_ReturnOk()
        {
            //Arrange
            var data = A.Fake<GetRestInfo>();
            int itemId = 1;
            string currency = "PLN";
            A.CallTo(() => _itemServices.ItemExist(itemId)).Returns(true);
            A.CallTo(() => _itemServices.GetRestOfItemOrg(itemId, currency)).Returns(data);
            var controller = new WarehouseController(_userServices, _itemServices, _logServices, _notificationServices);

            //Act
            var result = await controller.GetRestOrgInfo(itemId, currency);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();
        }
        [Fact]
        public async Task WarehouseController_GetRestOrgInfo_ItemNotFound_Return404()
        {
            //Arrange
            var data = A.Fake<GetRestInfo>();
            int itemId = 1;
            string currency = "PLN";
            A.CallTo(() => _itemServices.ItemExist(itemId)).Returns(false);
            A.CallTo(() => _itemServices.GetRestOfItemOrg(itemId, currency)).Returns(data);
            var controller = new WarehouseController(_userServices, _itemServices, _logServices, _notificationServices);

            //Act
            var result = await controller.GetRestOrgInfo(itemId, currency);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundResult>();
        }
        [Fact]
        public async Task WarehouseController_GetItems_ReturnOk()
        {
            //Arrange
            var data = A.Fake<IEnumerable<GetManyItems>>();
            var filters = A.Fake<ItemFiltersTemplate>();
            int userId = 1;
            string currency = "PLN";
            A.CallTo(() => _userServices.UserExist(userId)).Returns(true);
            A.CallTo(() => _itemServices.GetItems(currency, userId, null, filters)).Returns(data);
            var controller = new WarehouseController(_userServices, _itemServices, _logServices, _notificationServices);

            //Act
            var result = await controller.GetItems(currency, userId, null, null, null, null, null, null, null, null);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();
        }
        [Fact]
        public async Task WarehouseController_GetItems_SortIsIncorrect_Return400()
        {
            //Arrange
            var data = A.Fake<IEnumerable<GetManyItems>>();
            var filters = A.Fake<ItemFiltersTemplate>();
            int userId = 1;
            string sort = "";
            string currency = "PLN";
            A.CallTo(() => _userServices.UserExist(userId)).Returns(true);
            A.CallTo(() => _itemServices.GetItems(currency, userId, null, filters)).Returns(data);
            var controller = new WarehouseController(_userServices, _itemServices, _logServices, _notificationServices);

            //Act
            var result = await controller.GetItems(currency, userId, null, sort, null, null, null, null, null, null);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<BadRequestObjectResult>();
        }
        [Fact]
        public async Task WarehouseController_GetItems_UserNotFound_Return404()
        {
            //Arrange
            var data = A.Fake<IEnumerable<GetManyItems>>();
            var filters = A.Fake<ItemFiltersTemplate>();
            int userId = 1;
            string currency = "PLN";
            A.CallTo(() => _userServices.UserExist(userId)).Returns(false);
            A.CallTo(() => _itemServices.GetItems(currency, userId, null, filters)).Returns(data);
            var controller = new WarehouseController(_userServices, _itemServices, _logServices, _notificationServices);

            //Act
            var result = await controller.GetItems(currency, userId, null, null, null, null, null, null, null, null);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundResult>();
        }
        [Fact]
        public async Task WarehouseController_GetDescription_ReturnOk()
        {
            //Arrange
            int itemId = 1;
            A.CallTo(() => _itemServices.ItemExist(itemId)).Returns(true);
            A.CallTo(() => _itemServices.GetDescription(itemId)).Returns("");
            var controller = new WarehouseController(_userServices, _itemServices, _logServices, _notificationServices);

            //Act
            var result = await controller.GetDescription(itemId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();
        }
        [Fact]
        public async Task WarehouseController_GetDescription_ItemNotFound_Return404()
        {
            //Arrange
            int itemId = 1;
            A.CallTo(() => _itemServices.ItemExist(itemId)).Returns(false);
            A.CallTo(() => _itemServices.GetDescription(itemId)).Returns("");
            var controller = new WarehouseController(_userServices, _itemServices, _logServices, _notificationServices);

            //Act
            var result = await controller.GetDescription(itemId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundResult>();
        }
        [Fact]
        public async Task WarehouseController_GetBindings_ReturnOk()
        {
            //Arrange
            var data = A.Fake<IEnumerable<GetBinding>>();
            int itemId = 1;
            string currency = "PLN";
            A.CallTo(() => _itemServices.ItemExist(itemId)).Returns(true);
            A.CallTo(() => _itemServices.GetModifyRestOfItem(itemId, currency)).Returns(data);
            var controller = new WarehouseController(_userServices, _itemServices, _logServices, _notificationServices);

            //Act
            var result = await controller.GetBindings(itemId, currency);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();
        }
        [Fact]
        public async Task WarehouseController_GetBindings_ItemNotFound_Return404()
        {
            //Arrange
            var data = A.Fake<IEnumerable<GetBinding>>();
            int itemId = 1;
            string currency = "PLN";
            A.CallTo(() => _itemServices.ItemExist(itemId)).Returns(false);
            A.CallTo(() => _itemServices.GetModifyRestOfItem(itemId, currency)).Returns(data);
            var controller = new WarehouseController(_userServices, _itemServices, _logServices, _notificationServices);

            //Act
            var result = await controller.GetBindings(itemId, currency);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundResult>();
        }
        [Fact]
        public async Task WarehouseController_GetItemOwners_ReturnOk()
        {
            //Arrange
            var data = A.Fake<IEnumerable<GetUsers>>();
            int itemId = 1;
            A.CallTo(() => _itemServices.ItemExist(itemId)).Returns(true);
            A.CallTo(() => _itemServices.GetItemOwners(itemId)).Returns(data);
            var controller = new WarehouseController(_userServices, _itemServices, _logServices, _notificationServices);

            //Act
            var result = await controller.GetItemOwners(itemId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();
        }
        [Fact]
        public async Task WarehouseController_GetItemOwners_ItemNotFound_Return404()
        {
            //Arrange
            var data = A.Fake<IEnumerable<GetUsers>>();
            int itemId = 1;
            A.CallTo(() => _itemServices.ItemExist(itemId)).Returns(false);
            A.CallTo(() => _itemServices.GetItemOwners(itemId)).Returns(data);
            var controller = new WarehouseController(_userServices, _itemServices, _logServices, _notificationServices);

            //Act
            var result = await controller.GetItemOwners(itemId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundResult>();
        }
        [Fact]
        public async Task WarehouseController_ChangeBindings_ReturnOk()
        {
            //Arrange
            var data = A.Fake<ChangeBindings>();
            data.Bindings = A.CollectionOfDummy<ModifyBinding>(2);
            int logTypeId = 1;
            string userFull = "";
            var users = data.Bindings.GroupBy(e => e.UserId).Select(e => e.Key);
            A.CallTo(() => _userServices.UserExist(data.UserId)).Returns(true);
            A.CallTo(() => _itemServices.ChangeBindings(data.Bindings)).Returns(true);
            A.CallTo(() => _logServices.getLogTypeId("Modify")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", data.UserId, logTypeId));
            A.CallTo(() => _userServices.GetUserFullName(data.UserId)).Returns(userFull);
            A.CallTo(() => _notificationServices.CreateNotification(A<CreateNotification>.Ignored));
            var controller = new WarehouseController(_userServices, _itemServices, _logServices, _notificationServices);

            //Act
            var result = await controller.ChangeBindings(data);

            //Assert
            A.CallTo(() => _notificationServices.CreateNotification(A<CreateNotification>.Ignored))
                .MustHaveHappenedANumberOfTimesMatching(callNumber => callNumber <= users.Count() && callNumber >= (users.Count() - 1));
            result.Should().NotBeNull();
            result.Should().BeOfType<OkResult>();
        }
        [Fact]
        public async Task WarehouseController_ChangeBindings_ModifyFails_Return500()
        {
            //Arrange
            var data = A.Fake<ChangeBindings>();
            data.Bindings = A.CollectionOfDummy<ModifyBinding>(2);
            int logTypeId = 1;
            string userFull = "";
            var users = data.Bindings.GroupBy(e => e.UserId).Select(e => e.Key);
            A.CallTo(() => _userServices.UserExist(data.UserId)).Returns(true);
            A.CallTo(() => _itemServices.ChangeBindings(data.Bindings)).Returns(false);
            A.CallTo(() => _logServices.getLogTypeId("Modify")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", data.UserId, logTypeId));
            A.CallTo(() => _userServices.GetUserFullName(data.UserId)).Returns(userFull);
            A.CallTo(() => _notificationServices.CreateNotification(A<CreateNotification>.Ignored));
            var controller = new WarehouseController(_userServices, _itemServices, _logServices, _notificationServices);

            //Act
            var result = await controller.ChangeBindings(data);

            //Assert
            A.CallTo(() => _notificationServices.CreateNotification(A<CreateNotification>.Ignored)).MustNotHaveHappened();
            result.Should().NotBeNull();
            result.Should().BeOfType<StatusCodeResult>();
        }
        [Fact]
        public async Task WarehouseController_ChangeBindings_User_NotFound_Return404()
        {
            //Arrange
            var data = A.Fake<ChangeBindings>();
            data.Bindings = A.CollectionOfDummy<ModifyBinding>(2);
            int logTypeId = 1;
            string userFull = "";
            var users = data.Bindings.GroupBy(e => e.UserId).Select(e => e.Key);
            A.CallTo(() => _userServices.UserExist(data.UserId)).Returns(false);
            A.CallTo(() => _itemServices.ChangeBindings(data.Bindings)).Returns(true);
            A.CallTo(() => _logServices.getLogTypeId("Modify")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", data.UserId, logTypeId));
            A.CallTo(() => _userServices.GetUserFullName(data.UserId)).Returns(userFull);
            A.CallTo(() => _notificationServices.CreateNotification(A<CreateNotification>.Ignored));
            var controller = new WarehouseController(_userServices, _itemServices, _logServices, _notificationServices);

            //Act
            var result = await controller.ChangeBindings(data);

            //Assert
            A.CallTo(() => _notificationServices.CreateNotification(A<CreateNotification>.Ignored)).MustNotHaveHappened();
            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundResult>();
        }
    }
}
