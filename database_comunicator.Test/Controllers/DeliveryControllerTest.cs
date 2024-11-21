using database_communicator.Controllers;
using database_communicator.FilterClass;
using database_communicator.Models.DTOs.Create;
using database_communicator.Models.DTOs.Get;
using database_communicator.Models.DTOs.Modify;
using database_communicator.Services;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;

namespace database_communicator_Test.Controllers
{
    public class DeliveryControllerTest
    {
        private readonly IDeliveryServices _deliveryService;
        private readonly IUserServices _userServices;
        private readonly ILogServices _logServices;
        private readonly INotificationServices _notificationServices;
        public DeliveryControllerTest()
        {
            _deliveryService = A.Fake<IDeliveryServices>();
            _userServices = A.Fake<IUserServices>();
            _logServices = A.Fake<ILogServices>();
            _notificationServices = A.Fake<INotificationServices>();
        }
        [Fact]
        public async Task DeliveryController_GetToUserDeliveries_ReturnOk()
        {
            //Arrange
            var data = A.Fake<IEnumerable<GetDelivery>>();
            int userId = 1;
            var filters = A.Fake<DeliveryFiltersTemplate>();
            A.CallTo(() => _userServices.UserExist(userId)).Returns(true);
            A.CallTo(() => _deliveryService.GetDeliveries(true, userId, null, filters)).Returns(data);
            var controller = new DeliveryController(_deliveryService, _userServices, _logServices, _notificationServices);

            //Act
            var result = await controller.GetToUserDeliveries(userId, null, null, null, null, null, null, null, null, null, null);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();
        }
        [Fact]
        public async Task DeliveryController_GetToUserDeliveries_UserNotFound_Return404()
        {
            //Arrange
            var data = A.Fake<IEnumerable<GetDelivery>>();
            int userId = 1;
            var filters = A.Fake<DeliveryFiltersTemplate>();
            A.CallTo(() => _userServices.UserExist(userId)).Returns(false);
            A.CallTo(() => _deliveryService.GetDeliveries(true, userId, null, filters)).Returns(data);
            var controller = new DeliveryController(_deliveryService, _userServices, _logServices, _notificationServices);

            //Act
            var result = await controller.GetToUserDeliveries(userId, null, null, null, null, null, null, null, null, null, null);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundObjectResult>();
        }
        [Fact]
        public async Task DeliveryController_GetToUserDeliveries_SortIsIncorrect_Return400()
        {
            //Arrange
            var data = A.Fake<IEnumerable<GetDelivery>>();
            int userId = 1;
            var filters = A.Fake<DeliveryFiltersTemplate>();
            string sort = "";
            A.CallTo(() => _userServices.UserExist(userId)).Returns(false);
            A.CallTo(() => _deliveryService.GetDeliveries(true, userId, null, filters)).Returns(data);
            var controller = new DeliveryController(_deliveryService, _userServices, _logServices, _notificationServices);

            //Act
            var result = await controller.GetToUserDeliveries(userId, null, sort, null, null, null, null, null, null, null, null);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<BadRequestObjectResult>();
        }
        [Fact]
        public async Task DeliveryController_GetToUserDeliveries_NoUser_ReturnOk()
        {
            //Arrange
            var data = A.Fake<IEnumerable<GetDelivery>>();
            var filters = A.Fake<DeliveryFiltersTemplate>();
            A.CallTo(() => _deliveryService.GetDeliveries(true, null, filters)).Returns(data);
            var controller = new DeliveryController(_deliveryService, _userServices, _logServices, _notificationServices);

            //Act
            var result = await controller.GetToUserDeliveries(null, null, null, null, null, null, null, null, null, null);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();
        }
        [Fact]
        public async Task DeliveryController_GetToUserDeliveries_NoUser_SortIsIncorrect_Return400()
        {
            //Arrange
            var data = A.Fake<IEnumerable<GetDelivery>>();
            var filters = A.Fake<DeliveryFiltersTemplate>();
            string sort = "";
            A.CallTo(() => _deliveryService.GetDeliveries(true, null, filters)).Returns(data);
            var controller = new DeliveryController(_deliveryService, _userServices, _logServices, _notificationServices);

            //Act
            var result = await controller.GetToUserDeliveries(null, sort, null, null, null, null, null, null, null, null);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<BadRequestObjectResult>();
        }
        [Fact]
        public async Task DeliveryController_GetClientDeliveries_ReturnOk()
        {
            //Arrange
            var data = A.Fake<IEnumerable<GetDelivery>>();
            int userId = 1;
            var filters = A.Fake<DeliveryFiltersTemplate>();
            A.CallTo(() => _userServices.UserExist(userId)).Returns(true);
            A.CallTo(() => _deliveryService.GetDeliveries(false, userId, null, filters)).Returns(data);
            var controller = new DeliveryController(_deliveryService, _userServices, _logServices, _notificationServices);

            //Act
            var result = await controller.GetClientDeliveries(userId, null, null, null, null, null, null, null, null, null, null);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();
        }
        [Fact]
        public async Task DeliveryController_GetClientDeliveries_UserNotFound_Return404()
        {
            //Arrange
            var data = A.Fake<IEnumerable<GetDelivery>>();
            int userId = 1;
            var filters = A.Fake<DeliveryFiltersTemplate>();
            A.CallTo(() => _userServices.UserExist(userId)).Returns(false);
            A.CallTo(() => _deliveryService.GetDeliveries(false, userId, null, filters)).Returns(data);
            var controller = new DeliveryController(_deliveryService, _userServices, _logServices, _notificationServices);

            //Act
            var result = await controller.GetClientDeliveries(userId, null, null, null, null, null, null, null, null, null, null);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundObjectResult>();
        }
        [Fact]
        public async Task DeliveryController_GetClientDeliveries_SortIsIncorrect_Return400()
        {
            //Arrange
            var data = A.Fake<IEnumerable<GetDelivery>>();
            int userId = 1;
            var filters = A.Fake<DeliveryFiltersTemplate>();
            string sort = "";
            A.CallTo(() => _userServices.UserExist(userId)).Returns(false);
            A.CallTo(() => _deliveryService.GetDeliveries(false, userId, null, filters)).Returns(data);
            var controller = new DeliveryController(_deliveryService, _userServices, _logServices, _notificationServices);

            //Act
            var result = await controller.GetClientDeliveries(userId, null, sort, null, null, null, null, null, null, null, null);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<BadRequestObjectResult>();
        }
        [Fact]
        public async Task DeliveryController_GetClientDeliveries_NoUser_ReturnOk()
        {
            //Arrange
            var data = A.Fake<IEnumerable<GetDelivery>>();
            var filters = A.Fake<DeliveryFiltersTemplate>();
            A.CallTo(() => _deliveryService.GetDeliveries(false, null, filters)).Returns(data);
            var controller = new DeliveryController(_deliveryService, _userServices, _logServices, _notificationServices);

            //Act
            var result = await controller.GetClientDeliveries(null, null, null, null, null, null, null, null, null, null);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();
        }
        [Fact]
        public async Task DeliveryController_GetClientDeliveries_NoUser_SortIsIncorrect_Return400()
        {
            //Arrange
            var data = A.Fake<IEnumerable<GetDelivery>>();
            var filters = A.Fake<DeliveryFiltersTemplate>();
            string sort = "";
            A.CallTo(() => _deliveryService.GetDeliveries(false, null, filters)).Returns(data);
            var controller = new DeliveryController(_deliveryService, _userServices, _logServices, _notificationServices);

            //Act
            var result = await controller.GetClientDeliveries(null, sort, null, null, null, null, null, null, null, null);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<BadRequestObjectResult>();
        }
        [Fact]
        public async Task DeliveryController_AddDelivery_ReturnOk()
        {
            //Arrange
            var data = A.Fake<AddDelivery>();
            int newDeliveryId = 1;
            int logTypeId = 1;
            string userFull = "";
            var newNotification = A.Fake<CreateNotification>();
            A.CallTo(() => _userServices.UserExist(data.UserId)).Returns(true);
            A.CallTo(() => _deliveryService.DeliveryProformaExist(data.ProformaId)).Returns(false);
            A.CallTo(() => _deliveryService.AddDelivery(data)).Returns(newDeliveryId);
            A.CallTo(() => _logServices.getLogTypeId("Create")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", data.UserId, logTypeId));
            A.CallTo(() => _deliveryService.GetWarehouseManagerIds());
            A.CallTo(() => _userServices.GetUserFullName(data.UserId)).Returns(userFull);
            A.CallTo(() => _notificationServices.CreateNotification(newNotification));
            var controller = new DeliveryController(_deliveryService, _userServices, _logServices, _notificationServices);

            //Act
            var result = await controller.AddDelivery(data);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkResult>();
        }
        [Fact]
        public async Task DeliveryController_AddDelivery_AddFails_Return500()
        {
            //Arrange
            var data = A.Fake<AddDelivery>();
            int newDeliveryId = 0;
            int logTypeId = 1;
            string userFull = "";
            var newNotification = A.Fake<CreateNotification>();
            A.CallTo(() => _userServices.UserExist(data.UserId)).Returns(true);
            A.CallTo(() => _deliveryService.DeliveryProformaExist(data.ProformaId)).Returns(false);
            A.CallTo(() => _deliveryService.AddDelivery(data)).Returns(newDeliveryId);
            A.CallTo(() => _logServices.getLogTypeId("Create")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", data.UserId, logTypeId));
            A.CallTo(() => _deliveryService.GetWarehouseManagerIds());
            A.CallTo(() => _userServices.GetUserFullName(data.UserId)).Returns(userFull);
            A.CallTo(() => _notificationServices.CreateNotification(newNotification));
            var controller = new DeliveryController(_deliveryService, _userServices, _logServices, _notificationServices);

            //Act
            var result = await controller.AddDelivery(data);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<StatusCodeResult>();
        }
        [Fact]
        public async Task DeliveryController_AddDelivery_UserNotFound_Return404()
        {
            //Arrange
            var data = A.Fake<AddDelivery>();
            int newDeliveryId = 1;
            int logTypeId = 1;
            string userFull = "";
            var newNotification = A.Fake<CreateNotification>();
            A.CallTo(() => _userServices.UserExist(data.UserId)).Returns(false);
            A.CallTo(() => _deliveryService.DeliveryProformaExist(data.ProformaId)).Returns(false);
            A.CallTo(() => _deliveryService.AddDelivery(data)).Returns(newDeliveryId);
            A.CallTo(() => _logServices.getLogTypeId("Create")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", data.UserId, logTypeId));
            A.CallTo(() => _deliveryService.GetWarehouseManagerIds());
            A.CallTo(() => _userServices.GetUserFullName(data.UserId)).Returns(userFull);
            A.CallTo(() => _notificationServices.CreateNotification(newNotification));
            var controller = new DeliveryController(_deliveryService, _userServices, _logServices, _notificationServices);

            //Act
            var result = await controller.AddDelivery(data);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundObjectResult>();
        }
        [Fact]
        public async Task DeliveryController_AddDelivery_DeliveryAlreadyExist_Return400()
        {
            //Arrange
            var data = A.Fake<AddDelivery>();
            int newDeliveryId = 1;
            int logTypeId = 1;
            string userFull = "";
            var newNotification = A.Fake<CreateNotification>();
            A.CallTo(() => _userServices.UserExist(data.UserId)).Returns(true);
            A.CallTo(() => _deliveryService.DeliveryProformaExist(data.ProformaId)).Returns(true);
            A.CallTo(() => _deliveryService.AddDelivery(data)).Returns(newDeliveryId);
            A.CallTo(() => _logServices.getLogTypeId("Create")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", data.UserId, logTypeId));
            A.CallTo(() => _deliveryService.GetWarehouseManagerIds());
            A.CallTo(() => _userServices.GetUserFullName(data.UserId)).Returns(userFull);
            A.CallTo(() => _notificationServices.CreateNotification(newNotification));
            var controller = new DeliveryController(_deliveryService, _userServices, _logServices, _notificationServices);

            //Act
            var result = await controller.AddDelivery(data);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<BadRequestObjectResult>();
        }
        [Fact]
        public async Task DeliveryController_AddDeliveryCompany_ReturnOk()
        {
            //Arrange
            var data = A.Fake<AddDeliveryComapny>();
            int userId = 1;
            bool addResult = true;
            int logTypeId = 1;
            A.CallTo(() => _userServices.UserExist(userId)).Returns(true);
            A.CallTo(() => _deliveryService.DoesDeliveryCompanyExist(data.CompanyName)).Returns(false);
            A.CallTo(() => _deliveryService.AddDeliveryCompany(data.CompanyName)).Returns(addResult);
            A.CallTo(() => _logServices.getLogTypeId("Create")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", userId, logTypeId));
            var controller = new DeliveryController(_deliveryService, _userServices, _logServices, _notificationServices);

            //Act
            var result = await controller.AddDeliveryCompany(data, userId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkResult>();
        }
        [Fact]
        public async Task DeliveryController_AddDeliveryCompany_AddFails_Return500()
        {
            //Arrange
            var data = A.Fake<AddDeliveryComapny>();
            int userId = 1;
            bool addResult = false;
            int logTypeId = 1;
            A.CallTo(() => _userServices.UserExist(userId)).Returns(true);
            A.CallTo(() => _deliveryService.DoesDeliveryCompanyExist(data.CompanyName)).Returns(false);
            A.CallTo(() => _deliveryService.AddDeliveryCompany(data.CompanyName)).Returns(addResult);
            A.CallTo(() => _logServices.getLogTypeId("Create")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", userId, logTypeId));
            var controller = new DeliveryController(_deliveryService, _userServices, _logServices, _notificationServices);

            //Act
            var result = await controller.AddDeliveryCompany(data, userId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<StatusCodeResult>();
        }
        [Fact]
        public async Task DeliveryController_AddDeliveryCompany_UserNotFound_Return404()
        {
            //Arrange
            var data = A.Fake<AddDeliveryComapny>();
            int userId = 1;
            bool addResult = true;
            int logTypeId = 1;
            A.CallTo(() => _userServices.UserExist(userId)).Returns(false);
            A.CallTo(() => _deliveryService.DoesDeliveryCompanyExist(data.CompanyName)).Returns(false);
            A.CallTo(() => _deliveryService.AddDeliveryCompany(data.CompanyName)).Returns(addResult);
            A.CallTo(() => _logServices.getLogTypeId("Create")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", userId, logTypeId));
            var controller = new DeliveryController(_deliveryService, _userServices, _logServices, _notificationServices);

            //Act
            var result = await controller.AddDeliveryCompany(data, userId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundObjectResult>();
        }
        [Fact]
        public async Task DeliveryController_AddDeliveryCompany_CompanyAlreadyExist_Return400()
        {
            //Arrange
            var data = A.Fake<AddDeliveryComapny>();
            int userId = 1;
            bool addResult = true;
            int logTypeId = 1;
            A.CallTo(() => _userServices.UserExist(userId)).Returns(true);
            A.CallTo(() => _deliveryService.DoesDeliveryCompanyExist(data.CompanyName)).Returns(true);
            A.CallTo(() => _deliveryService.AddDeliveryCompany(data.CompanyName)).Returns(addResult);
            A.CallTo(() => _logServices.getLogTypeId("Create")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", userId, logTypeId));
            var controller = new DeliveryController(_deliveryService, _userServices, _logServices, _notificationServices);

            //Act
            var result = await controller.AddDeliveryCompany(data, userId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<BadRequestObjectResult>();
        }
        [Fact]
        public async Task DeliveryController_GetDeliveryCompanies_ReturnOk()
        {
            //Arrange
            var data = A.Fake<IEnumerable<GetDeliveryCompany>>();
            A.CallTo(() => _deliveryService.GetDeliveryCompanies()).Returns(data);
            var controller = new DeliveryController(_deliveryService, _userServices, _logServices, _notificationServices);

            //Act
            var result = await controller.GetDeliveryCompanies();

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();
        }
        [Fact]
        public async Task DeliveryController_GetDeliveryStatuses_ReturnOk()
        {
            //Arrange
            var data = A.Fake<IEnumerable<GetDeliveryStatus>>();
            A.CallTo(() => _deliveryService.GetDeliveryStatuses()).Returns(data);
            var controller = new DeliveryController(_deliveryService, _userServices, _logServices, _notificationServices);

            //Act
            var result = await controller.GetDeliveryStatuses();

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();
        }
        [Fact]
        public async Task DeliveryController_GetUserProformas_ReturnOk()
        {
            //Arrange
            var data = A.Fake<IEnumerable<GetProformaList>>();
            int userId = 1;
            A.CallTo(() => _userServices.UserExist(userId)).Returns(true);
            A.CallTo(() => _deliveryService.GetProformaListWithoutDelivery(true, userId)).Returns(data);
            var controller = new DeliveryController(_deliveryService, _userServices, _logServices, _notificationServices);

            //Act
            var result = await controller.GetUserProformas(userId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();
        }
        [Fact]
        public async Task DeliveryController_GetUserProformas_UserNotFound_Return404()
        {
            //Arrange
            var data = A.Fake<IEnumerable<GetProformaList>>();
            int userId = 1;
            A.CallTo(() => _userServices.UserExist(userId)).Returns(false);
            A.CallTo(() => _deliveryService.GetProformaListWithoutDelivery(true, userId)).Returns(data);
            var controller = new DeliveryController(_deliveryService, _userServices, _logServices, _notificationServices);

            //Act
            var result = await controller.GetUserProformas(userId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundObjectResult>();
        }
        [Fact]
        public async Task DeliveryController_GetClientProformas_ReturnOk()
        {
            //Arrange
            var data = A.Fake<IEnumerable<GetProformaList>>();
            int userId = 1;
            A.CallTo(() => _userServices.UserExist(userId)).Returns(true);
            A.CallTo(() => _deliveryService.GetProformaListWithoutDelivery(false, userId)).Returns(data);
            var controller = new DeliveryController(_deliveryService, _userServices, _logServices, _notificationServices);

            //Act
            var result = await controller.GetClientProformas(userId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();
        }
        [Fact]
        public async Task DeliveryController_GetClientProformas_UserNotFound_Return404()
        {
            //Arrange
            var data = A.Fake<IEnumerable<GetProformaList>>();
            int userId = 1;
            A.CallTo(() => _userServices.UserExist(userId)).Returns(false);
            A.CallTo(() => _deliveryService.GetProformaListWithoutDelivery(false, userId)).Returns(data);
            var controller = new DeliveryController(_deliveryService, _userServices, _logServices, _notificationServices);

            //Act
            var result = await controller.GetClientProformas(userId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundObjectResult>();
        }
        [Fact]
        public async Task DeliveryController_DeleteDelivery_ReturnOk()
        {
            //Arrange
            int deliveryId = 1;
            int userId = 1;
            bool isDeliveryToUser = true;
            int logTypeId = 1;
            string userFull = "";
            var newNotification = A.Fake<CreateNotification>();
            A.CallTo(() => _userServices.UserExist(userId)).Returns(true);
            A.CallTo(() => _deliveryService.DeliveryExist(deliveryId)).Returns(true);
            A.CallTo(() => _deliveryService.GetDeliveryOwnerId(deliveryId));
            A.CallTo(() => _deliveryService.DeleteDelivery(deliveryId)).Returns(true);
            A.CallTo(() => _logServices.getLogTypeId("Delete")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", userId, logTypeId));
            A.CallTo(() => _userServices.GetUserFullName(userId)).Returns(userFull);
            A.CallTo(() => _notificationServices.CreateNotification(newNotification));
            var controller = new DeliveryController(_deliveryService, _userServices, _logServices, _notificationServices);

            //Act
            var result = await controller.DeleteDelivery(deliveryId, userId, isDeliveryToUser);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkResult>();
        }
        [Fact]
        public async Task DeliveryController_DeleteDelivery_DeleteFails_Return500()
        {
            //Arrange
            int deliveryId = 1;
            int userId = 1;
            bool isDeliveryToUser = true;
            int logTypeId = 1;
            string userFull = "";
            var newNotification = A.Fake<CreateNotification>();
            A.CallTo(() => _userServices.UserExist(userId)).Returns(true);
            A.CallTo(() => _deliveryService.DeliveryExist(deliveryId)).Returns(true);
            A.CallTo(() => _deliveryService.GetDeliveryOwnerId(deliveryId));
            A.CallTo(() => _deliveryService.DeleteDelivery(deliveryId)).Returns(false);
            A.CallTo(() => _logServices.getLogTypeId("Delete")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", userId, logTypeId));
            A.CallTo(() => _userServices.GetUserFullName(userId)).Returns(userFull);
            A.CallTo(() => _notificationServices.CreateNotification(newNotification));
            var controller = new DeliveryController(_deliveryService, _userServices, _logServices, _notificationServices);

            //Act
            var result = await controller.DeleteDelivery(deliveryId, userId, isDeliveryToUser);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<StatusCodeResult>();
        }
        [Fact]
        public async Task DeliveryController_DeleteDelivery_UserNotFound_Return404()
        {
            //Arrange
            int deliveryId = 1;
            int userId = 1;
            bool isDeliveryToUser = true;
            int logTypeId = 1;
            string userFull = "";
            var newNotification = A.Fake<CreateNotification>();
            A.CallTo(() => _userServices.UserExist(userId)).Returns(false);
            A.CallTo(() => _deliveryService.DeliveryExist(deliveryId)).Returns(true);
            A.CallTo(() => _deliveryService.GetDeliveryOwnerId(deliveryId));
            A.CallTo(() => _deliveryService.DeleteDelivery(deliveryId)).Returns(true);
            A.CallTo(() => _logServices.getLogTypeId("Delete")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", userId, logTypeId));
            A.CallTo(() => _userServices.GetUserFullName(userId)).Returns(userFull);
            A.CallTo(() => _notificationServices.CreateNotification(newNotification));
            var controller = new DeliveryController(_deliveryService, _userServices, _logServices, _notificationServices);

            //Act
            var result = await controller.DeleteDelivery(deliveryId, userId, isDeliveryToUser);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundObjectResult>();
        }
        [Fact]
        public async Task DeliveryController_DeleteDelivery_DeliveryNotFound_Return404()
        {
            //Arrange
            int deliveryId = 1;
            int userId = 1;
            bool isDeliveryToUser = true;
            int logTypeId = 1;
            string userFull = "";
            var newNotification = A.Fake<CreateNotification>();
            A.CallTo(() => _userServices.UserExist(userId)).Returns(true);
            A.CallTo(() => _deliveryService.DeliveryExist(deliveryId)).Returns(false);
            A.CallTo(() => _deliveryService.GetDeliveryOwnerId(deliveryId));
            A.CallTo(() => _deliveryService.DeleteDelivery(deliveryId)).Returns(true);
            A.CallTo(() => _logServices.getLogTypeId("Delete")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", userId, logTypeId));
            A.CallTo(() => _userServices.GetUserFullName(userId)).Returns(userFull);
            A.CallTo(() => _notificationServices.CreateNotification(newNotification));
            var controller = new DeliveryController(_deliveryService, _userServices, _logServices, _notificationServices);

            //Act
            var result = await controller.DeleteDelivery(deliveryId, userId, isDeliveryToUser);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundObjectResult>();
        }
        [Fact]
        public async Task DeliveryController_GetRestOfDelivery_ReturnOk()
        {
            //Arrange
            var data = A.Fake<GetRestDelivery>();
            int deliveryId = 1;
            A.CallTo(() => _deliveryService.DeliveryExist(deliveryId)).Returns(true);
            A.CallTo(() => _deliveryService.GetRestDelivery(deliveryId)).Returns(data);
            var controller = new DeliveryController(_deliveryService, _userServices, _logServices, _notificationServices);

            //Act
            var result = await controller.GetRestOfDelivery(deliveryId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();
        }
        [Fact]
        public async Task DeliveryController_GetRestOfDelivery_DeliveryNotFound_Return404()
        {
            //Arrange
            var data = A.Fake<GetRestDelivery>();
            int deliveryId = 1;
            A.CallTo(() => _deliveryService.DeliveryExist(deliveryId)).Returns(false);
            A.CallTo(() => _deliveryService.GetRestDelivery(deliveryId)).Returns(data);
            var controller = new DeliveryController(_deliveryService, _userServices, _logServices, _notificationServices);

            //Act
            var result = await controller.GetRestOfDelivery(deliveryId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundObjectResult>();
        }
        [Fact]
        public async Task DeliveryController_AddNote_ReturnOk()
        {
            //Arrange
            var data = A.Fake<AddNote>();
            A.CallTo(() => _userServices.UserExist(data.UserId)).Returns(true);
            A.CallTo(() => _deliveryService.DeliveryExist(data.DeliveryId)).Returns(true);
            A.CallTo(() => _deliveryService.AddNote(data)).Returns(true);
            var controller = new DeliveryController(_deliveryService, _userServices, _logServices, _notificationServices);

            //Act
            var result = await controller.AddNote(data);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkResult>();
        }
        [Fact]
        public async Task DeliveryController_AddNote_AddFails_Return500()
        {
            //Arrange
            var data = A.Fake<AddNote>();
            A.CallTo(() => _userServices.UserExist(data.UserId)).Returns(true);
            A.CallTo(() => _deliveryService.DeliveryExist(data.DeliveryId)).Returns(true);
            A.CallTo(() => _deliveryService.AddNote(data)).Returns(false);
            var controller = new DeliveryController(_deliveryService, _userServices, _logServices, _notificationServices);

            //Act
            var result = await controller.AddNote(data);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<StatusCodeResult>();
        }
        [Fact]
        public async Task DeliveryController_AddNote_UserNotFound_Return404()
        {
            //Arrange
            var data = A.Fake<AddNote>();
            A.CallTo(() => _userServices.UserExist(data.UserId)).Returns(false);
            A.CallTo(() => _deliveryService.DeliveryExist(data.DeliveryId)).Returns(true);
            A.CallTo(() => _deliveryService.AddNote(data)).Returns(true);
            var controller = new DeliveryController(_deliveryService, _userServices, _logServices, _notificationServices);

            //Act
            var result = await controller.AddNote(data);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundObjectResult>();
        }
        [Fact]
        public async Task DeliveryController_AddNote_DeliveryNotFound_Return400()
        {
            //Arrange
            var data = A.Fake<AddNote>();
            A.CallTo(() => _userServices.UserExist(data.UserId)).Returns(true);
            A.CallTo(() => _deliveryService.DeliveryExist(data.DeliveryId)).Returns(false);
            A.CallTo(() => _deliveryService.AddNote(data)).Returns(true);
            var controller = new DeliveryController(_deliveryService, _userServices, _logServices, _notificationServices);

            //Act
            var result = await controller.AddNote(data);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<BadRequestObjectResult>();
        }
        [Fact]
        public async Task DeliveryController_SetDeliveryStatus_ReturnOk()
        {
            //Arrange
            var data = A.Fake<SetDeliveryStatus>();
            A.CallTo(() => _deliveryService.DeliveryExist(data.DeliveryId)).Returns(true);
            A.CallTo(() => _deliveryService.SetDeliveryStatus(data)).Returns(true);
            var controller = new DeliveryController(_deliveryService, _userServices, _logServices, _notificationServices);

            //Act
            var result = await controller.SetDeliveryStatus(data);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkResult>();
        }
        [Fact]
        public async Task DeliveryController_SetDeliveryStatus_ModifyFails_Return500()
        {
            //Arrange
            var data = A.Fake<SetDeliveryStatus>();
            A.CallTo(() => _deliveryService.DeliveryExist(data.DeliveryId)).Returns(true);
            A.CallTo(() => _deliveryService.SetDeliveryStatus(data)).Returns(false);
            var controller = new DeliveryController(_deliveryService, _userServices, _logServices, _notificationServices);

            //Act
            var result = await controller.SetDeliveryStatus(data);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<StatusCodeResult>();
        }
        [Fact]
        public async Task DeliveryController_SetDeliveryStatus_DeliveryNotFound_Return404()
        {
            //Arrange
            var data = A.Fake<SetDeliveryStatus>();
            A.CallTo(() => _deliveryService.DeliveryExist(data.DeliveryId)).Returns(false);
            A.CallTo(() => _deliveryService.SetDeliveryStatus(data)).Returns(true);
            var controller = new DeliveryController(_deliveryService, _userServices, _logServices, _notificationServices);

            //Act
            var result = await controller.SetDeliveryStatus(data);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundObjectResult>();
        }
        [Fact]
        public async Task DeliveryController_ModifyDelivery_ReturnOk()
        {
            //Arrange
            var data = A.Fake<ModifyDelivery>();
            int userId = 1;
            bool isDeliveryToUser = true;
            int logTypeId = 1;
            string userFull = "";
            var newNotification = A.Fake<CreateNotification>();
            A.CallTo(() => _userServices.UserExist(userId)).Returns(true);
            A.CallTo(() => _deliveryService.DeliveryExist(data.DeliveryId)).Returns(true);
            A.CallTo(() => _deliveryService.GetDeliveryOwnerId(data.DeliveryId));
            A.CallTo(() => _deliveryService.ModifyDelivery(data)).Returns(true);
            A.CallTo(() => _logServices.getLogTypeId("Modify")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", userId, logTypeId));
            A.CallTo(() => _userServices.GetUserFullName(userId)).Returns(userFull);
            A.CallTo(() => _notificationServices.CreateNotification(newNotification));
            var controller = new DeliveryController(_deliveryService, _userServices, _logServices, _notificationServices);

            //Act
            var result = await controller.ModifyDelivery(data, userId, isDeliveryToUser);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkResult>();
        }
        [Fact]
        public async Task DeliveryController_ModifyDelivery_ModifyFails_Return500()
        {
            //Arrange
            var data = A.Fake<ModifyDelivery>();
            int userId = 1;
            bool isDeliveryToUser = true;
            int logTypeId = 1;
            string userFull = "";
            var newNotification = A.Fake<CreateNotification>();
            A.CallTo(() => _userServices.UserExist(userId)).Returns(true);
            A.CallTo(() => _deliveryService.DeliveryExist(data.DeliveryId)).Returns(true);
            A.CallTo(() => _deliveryService.GetDeliveryOwnerId(data.DeliveryId));
            A.CallTo(() => _deliveryService.ModifyDelivery(data)).Returns(false);
            A.CallTo(() => _logServices.getLogTypeId("Modify")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", userId, logTypeId));
            A.CallTo(() => _userServices.GetUserFullName(userId)).Returns(userFull);
            A.CallTo(() => _notificationServices.CreateNotification(newNotification));
            var controller = new DeliveryController(_deliveryService, _userServices, _logServices, _notificationServices);

            //Act
            var result = await controller.ModifyDelivery(data, userId, isDeliveryToUser);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<StatusCodeResult>();
        }
        [Fact]
        public async Task DeliveryController_ModifyDelivery_UserNotFound_Return404()
        {
            //Arrange
            var data = A.Fake<ModifyDelivery>();
            int userId = 1;
            bool isDeliveryToUser = true;
            int logTypeId = 1;
            string userFull = "";
            var newNotification = A.Fake<CreateNotification>();
            A.CallTo(() => _userServices.UserExist(userId)).Returns(false);
            A.CallTo(() => _deliveryService.DeliveryExist(data.DeliveryId)).Returns(true);
            A.CallTo(() => _deliveryService.GetDeliveryOwnerId(data.DeliveryId));
            A.CallTo(() => _deliveryService.ModifyDelivery(data)).Returns(true);
            A.CallTo(() => _logServices.getLogTypeId("Modify")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", userId, logTypeId));
            A.CallTo(() => _userServices.GetUserFullName(userId)).Returns(userFull);
            A.CallTo(() => _notificationServices.CreateNotification(newNotification));
            var controller = new DeliveryController(_deliveryService, _userServices, _logServices, _notificationServices);

            //Act
            var result = await controller.ModifyDelivery(data, userId, isDeliveryToUser);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundObjectResult>();
        }
        [Fact]
        public async Task DeliveryController_ModifyDelivery_DeliveryNotFound_Return404()
        {
            //Arrange
            var data = A.Fake<ModifyDelivery>();
            int userId = 1;
            bool isDeliveryToUser = true;
            int logTypeId = 1;
            string userFull = "";
            var newNotification = A.Fake<CreateNotification>();
            A.CallTo(() => _userServices.UserExist(userId)).Returns(true);
            A.CallTo(() => _deliveryService.DeliveryExist(data.DeliveryId)).Returns(false);
            A.CallTo(() => _deliveryService.GetDeliveryOwnerId(data.DeliveryId));
            A.CallTo(() => _deliveryService.ModifyDelivery(data)).Returns(true);
            A.CallTo(() => _logServices.getLogTypeId("Modify")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", userId, logTypeId));
            A.CallTo(() => _userServices.GetUserFullName(userId)).Returns(userFull);
            A.CallTo(() => _notificationServices.CreateNotification(newNotification));
            var controller = new DeliveryController(_deliveryService, _userServices, _logServices, _notificationServices);

            //Act
            var result = await controller.ModifyDelivery(data, userId, isDeliveryToUser);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundObjectResult>();
        }
    }
}
