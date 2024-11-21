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
    public class ProformasControllerTest
    {
        private readonly IProformaServices _proformaServices;
        private readonly IUserServices _userServices;
        private readonly ILogServices _logServices;
        private readonly INotificationServices _notificationServices;
        public ProformasControllerTest()
        {
            _proformaServices = A.Fake<IProformaServices>();
            _userServices = A.Fake<IUserServices>();
            _logServices = A.Fake<ILogServices>();
            _notificationServices = A.Fake<INotificationServices>();
        }
        [Fact]
        public async Task ProformasController_AddProforma_ReturnOk()
        {
            //Arrange
            var data = A.Fake<AddProforma>();
            int userId = 1;
            int newProformaId = 1;
            int logTypeId = 1;
            string userFull = "";
            var newNotification = A.Fake<CreateNotification>();
            A.CallTo(() => _userServices.UserExist(userId)).Returns(true);
            A.CallTo(() => _userServices.UserExist(data.UserId)).Returns(true);
            A.CallTo(() => _proformaServices.ProformaExist(data.ProformaNumber, data.SellerId, data.BuyerId)).Returns(false);
            A.CallTo(() => _proformaServices.AddProforma(data)).Returns(newProformaId);
            A.CallTo(() => _logServices.getLogTypeId("Create")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", userId, logTypeId));
            A.CallTo(() => _userServices.GetUserFullName(userId)).Returns(userFull);
            A.CallTo(() => _notificationServices.CreateNotification(newNotification));
            var controller = new ProformasController(_proformaServices, _userServices, _logServices, _notificationServices);

            //Act
            var result = await controller.AddProforma(data, userId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkResult>();
        }
        [Fact]
        public async Task ProformasController_AddProforma_AddFails_Return500()
        {
            //Arrange
            var data = A.Fake<AddProforma>();
            int userId = 1;
            int newProformaId = 0;
            int logTypeId = 1;
            string userFull = "";
            var newNotification = A.Fake<CreateNotification>();
            A.CallTo(() => _userServices.UserExist(userId)).Returns(true);
            A.CallTo(() => _userServices.UserExist(data.UserId)).Returns(true);
            A.CallTo(() => _proformaServices.ProformaExist(data.ProformaNumber, data.SellerId, data.BuyerId)).Returns(false);
            A.CallTo(() => _proformaServices.AddProforma(data)).Returns(newProformaId);
            A.CallTo(() => _logServices.getLogTypeId("Create")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", userId, logTypeId));
            A.CallTo(() => _userServices.GetUserFullName(userId)).Returns(userFull);
            A.CallTo(() => _notificationServices.CreateNotification(newNotification));
            var controller = new ProformasController(_proformaServices, _userServices, _logServices, _notificationServices);

            //Act
            var result = await controller.AddProforma(data, userId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<StatusCodeResult>();
        }
        [Fact]
        public async Task ProformasController_AddProforma_UserNotFound_Return404()
        {
            //Arrange
            var data = A.Fake<AddProforma>();
            int userId = 1;
            int newProformaId = 1;
            int logTypeId = 1;
            string userFull = "";
            var newNotification = A.Fake<CreateNotification>();
            A.CallTo(() => _userServices.UserExist(userId)).Returns(false);
            A.CallTo(() => _userServices.UserExist(data.UserId)).Returns(true);
            A.CallTo(() => _proformaServices.ProformaExist(data.ProformaNumber, data.SellerId, data.BuyerId)).Returns(false);
            A.CallTo(() => _proformaServices.AddProforma(data)).Returns(newProformaId);
            A.CallTo(() => _logServices.getLogTypeId("Create")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", userId, logTypeId));
            A.CallTo(() => _userServices.GetUserFullName(userId)).Returns(userFull);
            A.CallTo(() => _notificationServices.CreateNotification(newNotification));
            var controller = new ProformasController(_proformaServices, _userServices, _logServices, _notificationServices);

            //Act
            var result = await controller.AddProforma(data, userId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundObjectResult>();
        }
        [Fact]
        public async Task ProformasController_AddProforma_ProformaUserNotFound_Return404()
        {
            //Arrange
            var data = A.Fake<AddProforma>();
            int userId = 1;
            int newProformaId = 1;
            int logTypeId = 1;
            string userFull = "";
            var newNotification = A.Fake<CreateNotification>();
            A.CallTo(() => _userServices.UserExist(userId)).Returns(true);
            A.CallTo(() => _userServices.UserExist(data.UserId)).Returns(false);
            A.CallTo(() => _proformaServices.ProformaExist(data.ProformaNumber, data.SellerId, data.BuyerId)).Returns(false);
            A.CallTo(() => _proformaServices.AddProforma(data)).Returns(newProformaId);
            A.CallTo(() => _logServices.getLogTypeId("Create")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", userId, logTypeId));
            A.CallTo(() => _userServices.GetUserFullName(userId)).Returns(userFull);
            A.CallTo(() => _notificationServices.CreateNotification(newNotification));
            var controller = new ProformasController(_proformaServices, _userServices, _logServices, _notificationServices);

            //Act
            var result = await controller.AddProforma(data, userId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundObjectResult>();
        }
        [Fact]
        public async Task ProformasController_AddProforma_ProformaAlreadyExist_Return400()
        {
            //Arrange
            var data = A.Fake<AddProforma>();
            int userId = 1;
            int newProformaId = 1;
            int logTypeId = 1;
            string userFull = "";
            var newNotification = A.Fake<CreateNotification>();
            A.CallTo(() => _userServices.UserExist(userId)).Returns(true);
            A.CallTo(() => _userServices.UserExist(data.UserId)).Returns(true);
            A.CallTo(() => _proformaServices.ProformaExist(data.ProformaNumber, data.SellerId, data.BuyerId)).Returns(true);
            A.CallTo(() => _proformaServices.AddProforma(data)).Returns(newProformaId);
            A.CallTo(() => _logServices.getLogTypeId("Create")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", userId, logTypeId));
            A.CallTo(() => _userServices.GetUserFullName(userId)).Returns(userFull);
            A.CallTo(() => _notificationServices.CreateNotification(newNotification));
            var controller = new ProformasController(_proformaServices, _userServices, _logServices, _notificationServices);

            //Act
            var result = await controller.AddProforma(data, userId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<BadRequestObjectResult>();
        }
        [Fact]
        public async Task ProformasController_GetYoursProformas_ReturnOk()
        {
            //Arrange
            var data = A.Fake<IEnumerable<GetProforma>>();
            int userId = 1;
            var filters = A.Fake<ProformaFiltersTemplate>();
            A.CallTo(() => _userServices.UserExist(userId)).Returns(true);
            A.CallTo(() => _proformaServices.GetProformas(true, userId, null, filters)).Returns(data);
            var controller = new ProformasController(_proformaServices, _userServices, _logServices, _notificationServices);

            //Act
            var result = await controller.GetYoursProformas(userId, null, null, null, null, null, null, null, null, null, null);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();
        }
        [Fact]
        public async Task ProformasController_GetYoursProformas_UserNotFound_Return404()
        {
            //Arrange
            var data = A.Fake<IEnumerable<GetProforma>>();
            int userId = 1;
            var filters = A.Fake<ProformaFiltersTemplate>();
            A.CallTo(() => _userServices.UserExist(userId)).Returns(false);
            A.CallTo(() => _proformaServices.GetProformas(true, userId, null, filters)).Returns(data);
            var controller = new ProformasController(_proformaServices, _userServices, _logServices, _notificationServices);

            //Act
            var result = await controller.GetYoursProformas(userId, null, null, null, null, null, null, null, null, null, null);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundObjectResult>();
        }
        [Fact]
        public async Task ProformasController_GetYoursProformas_SortIncorrect_Return400()
        {
            //Arrange
            var data = A.Fake<IEnumerable<GetProforma>>();
            int userId = 1;
            var filters = A.Fake<ProformaFiltersTemplate>();
            string sort = "";
            A.CallTo(() => _userServices.UserExist(userId)).Returns(true);
            A.CallTo(() => _proformaServices.GetProformas(true, userId, sort, filters)).Returns(data);
            var controller = new ProformasController(_proformaServices, _userServices, _logServices, _notificationServices);

            //Act
            var result = await controller.GetYoursProformas(userId, null, sort, null, null, null, null, null, null, null, null);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<BadRequestObjectResult>();
        }
        [Fact]
        public async Task ProformasController_GetYoursProformas_NoUserPassed_ReturnOk()
        {
            //Arrange
            var data = A.Fake<IEnumerable<GetProforma>>();
            var filters = A.Fake<ProformaFiltersTemplate>();
            A.CallTo(() => _proformaServices.GetProformas(true, null, filters)).Returns(data);
            var controller = new ProformasController(_proformaServices, _userServices, _logServices, _notificationServices);

            //Act
            var result = await controller.GetYoursProformas(null, null, null, null, null, null, null, null, null, null);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();
        }
        [Fact]
        public async Task ProformasController_GetYoursProformas_NoUserPassedSortIncorrect_Return400()
        {
            //Arrange
            var data = A.Fake<IEnumerable<GetProforma>>();
            var filters = A.Fake<ProformaFiltersTemplate>();
            string sort = "";
            A.CallTo(() => _proformaServices.GetProformas(true, sort, filters)).Returns(data);
            var controller = new ProformasController(_proformaServices, _userServices, _logServices, _notificationServices);

            //Act
            var result = await controller.GetYoursProformas(null, sort, null, null, null, null, null, null, null, null);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<BadRequestObjectResult>();
        }
        [Fact]
        public async Task ProformasControllerGetClientProformas_ReturnOk()
        {
            //Arrange
            var data = A.Fake<IEnumerable<GetProforma>>();
            int userId = 1;
            var filters = A.Fake<ProformaFiltersTemplate>();
            A.CallTo(() => _userServices.UserExist(userId)).Returns(true);
            A.CallTo(() => _proformaServices.GetProformas(false, userId, null, filters)).Returns(data);
            var controller = new ProformasController(_proformaServices, _userServices, _logServices, _notificationServices);

            //Act
            var result = await controller.GetClientProformas(userId, null, null, null, null, null, null, null, null, null, null);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();
        }
        [Fact]
        public async Task ProformasController_GetClientProformas_UserNotFound_Return404()
        {
            //Arrange
            var data = A.Fake<IEnumerable<GetProforma>>();
            int userId = 1;
            var filters = A.Fake<ProformaFiltersTemplate>();
            A.CallTo(() => _userServices.UserExist(userId)).Returns(false);
            A.CallTo(() => _proformaServices.GetProformas(false, userId, null, filters)).Returns(data);
            var controller = new ProformasController(_proformaServices, _userServices, _logServices, _notificationServices);

            //Act
            var result = await controller.GetClientProformas(userId, null, null, null, null, null, null, null, null, null, null);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundObjectResult>();
        }
        [Fact]
        public async Task ProformasController_GetClientProformas_SortIncorrect_Return400()
        {
            //Arrange
            var data = A.Fake<IEnumerable<GetProforma>>();
            int userId = 1;
            var filters = A.Fake<ProformaFiltersTemplate>();
            string sort = "";
            A.CallTo(() => _userServices.UserExist(userId)).Returns(true);
            A.CallTo(() => _proformaServices.GetProformas(false, userId, sort, filters)).Returns(data);
            var controller = new ProformasController(_proformaServices, _userServices, _logServices, _notificationServices);

            //Act
            var result = await controller.GetClientProformas(userId, null, sort, null, null, null, null, null, null, null, null);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<BadRequestObjectResult>();
        }
        [Fact]
        public async Task ProformasController_GetClientProformas_NoUserPassed_ReturnOk()
        {
            //Arrange
            var data = A.Fake<IEnumerable<GetProforma>>();
            var filters = A.Fake<ProformaFiltersTemplate>();
            A.CallTo(() => _proformaServices.GetProformas(false, null, filters)).Returns(data);
            var controller = new ProformasController(_proformaServices, _userServices, _logServices, _notificationServices);

            //Act
            var result = await controller.GetClientProformas(null, null, null, null, null, null, null, null, null, null);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();
        }
        [Fact]
        public async Task ProformasController_GetClientProformas_NoUserPassedSortIncorrect_Return400()
        {
            //Arrange
            var data = A.Fake<IEnumerable<GetProforma>>();
            var filters = A.Fake<ProformaFiltersTemplate>();
            string sort = "";
            A.CallTo(() => _proformaServices.GetProformas(false, sort, filters)).Returns(data);
            var controller = new ProformasController(_proformaServices, _userServices, _logServices, _notificationServices);

            //Act
            var result = await controller.GetClientProformas(null, sort, null, null, null, null, null, null, null, null);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<BadRequestObjectResult>();
        }
        [Fact]
        public async Task ProformasController_GetRestClientProforma_ReturnOk()
        {
            //Arrange
            var data = A.Fake<GetRestProforma>();
            int proformaId = 1;
            A.CallTo(() => _proformaServices.ProformaExist(proformaId)).Returns(true);
            A.CallTo(() => _proformaServices.GetRestProforma(false, proformaId)).Returns(data);
            var controller = new ProformasController(_proformaServices, _userServices, _logServices, _notificationServices);

            //Act
            var result = await controller.GetRestClientProforma(proformaId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();
        }
        [Fact]
        public async Task ProformasController_GetRestClientProforma_UserNotFound_Return404()
        {
            //Arrange
            var data = A.Fake<GetRestProforma>();
            int proformaId = 1;
            A.CallTo(() => _proformaServices.ProformaExist(proformaId)).Returns(false);
            A.CallTo(() => _proformaServices.GetRestProforma(false, proformaId)).Returns(data);
            var controller = new ProformasController(_proformaServices, _userServices, _logServices, _notificationServices);

            //Act
            var result = await controller.GetRestClientProforma(proformaId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundObjectResult>();
        }
        [Fact]
        public async Task ProformasController_GetRestYourProforma_ReturnOk()
        {
            //Arrange
            var data = A.Fake<GetRestProforma>();
            int proformaId = 1;
            A.CallTo(() => _proformaServices.ProformaExist(proformaId)).Returns(true);
            A.CallTo(() => _proformaServices.GetRestProforma(true, proformaId)).Returns(data);
            var controller = new ProformasController(_proformaServices, _userServices, _logServices, _notificationServices);

            //Act
            var result = await controller.GetRestYourProforma(proformaId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();
        }
        [Fact]
        public async Task ProformasController_GetRestYourProforma_UserNotFound_Return404()
        {
            //Arrange
            var data = A.Fake<GetRestProforma>();
            int proformaId = 1;
            A.CallTo(() => _proformaServices.ProformaExist(proformaId)).Returns(false);
            A.CallTo(() => _proformaServices.GetRestProforma(true, proformaId)).Returns(data);
            var controller = new ProformasController(_proformaServices, _userServices, _logServices, _notificationServices);

            //Act
            var result = await controller.GetRestYourProforma(proformaId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundObjectResult>();
        }
        [Fact]
        public async Task ProformasController_DeleteYourProforma_ReturnOk()
        {
            //Arrange
            int proformaId = 1;
            int userId = 1;
            int owner = 2;
            int logTypeId = 1;
            string userFull = "";
            var newNotification = A.Fake<CreateNotification>();
            A.CallTo(() => _userServices.UserExist(userId)).Returns(true);
            A.CallTo(() => _proformaServices.ProformaExist(proformaId)).Returns(true);
            A.CallTo(() => _proformaServices.DoesDeliveryExist(proformaId)).Returns(false);
            A.CallTo(() => _proformaServices.GetProformaUserId(proformaId)).Returns(owner);
            A.CallTo(() => _proformaServices.GetProformaNumber(proformaId));
            A.CallTo(() => _proformaServices.DeleteProforma(true, proformaId)).Returns(true);
            A.CallTo(() => _logServices.getLogTypeId("Delete")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", userId, logTypeId));
            A.CallTo(() => _userServices.GetUserFullName(userId)).Returns(userFull);
            A.CallTo(() => _notificationServices.CreateNotification(newNotification));
            var controller = new ProformasController(_proformaServices, _userServices, _logServices, _notificationServices);

            //Act
            var result = await controller.DeleteYourProforma(proformaId, userId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkResult>();
        }
        [Fact]
        public async Task ProformasController_DeleteYourProforma_DeleteFails_Return500()
        {
            //Arrange
            int proformaId = 1;
            int userId = 1;
            int owner = 2;
            int logTypeId = 1;
            string userFull = "";
            var newNotification = A.Fake<CreateNotification>();
            A.CallTo(() => _userServices.UserExist(userId)).Returns(true);
            A.CallTo(() => _proformaServices.ProformaExist(proformaId)).Returns(true);
            A.CallTo(() => _proformaServices.DoesDeliveryExist(proformaId)).Returns(false);
            A.CallTo(() => _proformaServices.GetProformaUserId(proformaId)).Returns(owner);
            A.CallTo(() => _proformaServices.GetProformaNumber(proformaId));
            A.CallTo(() => _proformaServices.DeleteProforma(true, proformaId)).Returns(false);
            A.CallTo(() => _logServices.getLogTypeId("Delete")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", userId, logTypeId));
            A.CallTo(() => _userServices.GetUserFullName(userId)).Returns(userFull);
            A.CallTo(() => _notificationServices.CreateNotification(newNotification));
            var controller = new ProformasController(_proformaServices, _userServices, _logServices, _notificationServices);

            //Act
            var result = await controller.DeleteYourProforma(proformaId, userId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<StatusCodeResult>();
        }
        [Fact]
        public async Task ProformasController_DeleteYourProforma_UserNotFound_Return404()
        {
            //Arrange
            int proformaId = 1;
            int userId = 1;
            int owner = 2;
            int logTypeId = 1;
            string userFull = "";
            var newNotification = A.Fake<CreateNotification>();
            A.CallTo(() => _userServices.UserExist(userId)).Returns(false);
            A.CallTo(() => _proformaServices.ProformaExist(proformaId)).Returns(true);
            A.CallTo(() => _proformaServices.DoesDeliveryExist(proformaId)).Returns(false);
            A.CallTo(() => _proformaServices.GetProformaUserId(proformaId)).Returns(owner);
            A.CallTo(() => _proformaServices.GetProformaNumber(proformaId));
            A.CallTo(() => _proformaServices.DeleteProforma(true, proformaId)).Returns(true);
            A.CallTo(() => _logServices.getLogTypeId("Delete")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", userId, logTypeId));
            A.CallTo(() => _userServices.GetUserFullName(userId)).Returns(userFull);
            A.CallTo(() => _notificationServices.CreateNotification(newNotification));
            var controller = new ProformasController(_proformaServices, _userServices, _logServices, _notificationServices);

            //Act
            var result = await controller.DeleteYourProforma(proformaId, userId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundObjectResult>();
        }
        [Fact]
        public async Task ProformasController_DeleteYourProforma_ProformaNotFound_Return404()
        {
            //Arrange
            int proformaId = 1;
            int userId = 1;
            int owner = 2;
            int logTypeId = 1;
            string userFull = "";
            var newNotification = A.Fake<CreateNotification>();
            A.CallTo(() => _userServices.UserExist(userId)).Returns(true);
            A.CallTo(() => _proformaServices.ProformaExist(proformaId)).Returns(false);
            A.CallTo(() => _proformaServices.DoesDeliveryExist(proformaId)).Returns(false);
            A.CallTo(() => _proformaServices.GetProformaUserId(proformaId)).Returns(owner);
            A.CallTo(() => _proformaServices.GetProformaNumber(proformaId));
            A.CallTo(() => _proformaServices.DeleteProforma(true, proformaId)).Returns(true);
            A.CallTo(() => _logServices.getLogTypeId("Delete")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", userId, logTypeId));
            A.CallTo(() => _userServices.GetUserFullName(userId)).Returns(userFull);
            A.CallTo(() => _notificationServices.CreateNotification(newNotification));
            var controller = new ProformasController(_proformaServices, _userServices, _logServices, _notificationServices);

            //Act
            var result = await controller.DeleteYourProforma(proformaId, userId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundObjectResult>();
        }
        [Fact]
        public async Task ProformasController_DeleteYourProforma_DeliveryExistForThatProforma_Return400()
        {
            //Arrange
            int proformaId = 1;
            int userId = 1;
            int owner = 2;
            int logTypeId = 1;
            string userFull = "";
            var newNotification = A.Fake<CreateNotification>();
            A.CallTo(() => _userServices.UserExist(userId)).Returns(true);
            A.CallTo(() => _proformaServices.ProformaExist(proformaId)).Returns(true);
            A.CallTo(() => _proformaServices.DoesDeliveryExist(proformaId)).Returns(true);
            A.CallTo(() => _proformaServices.GetProformaUserId(proformaId)).Returns(owner);
            A.CallTo(() => _proformaServices.GetProformaNumber(proformaId));
            A.CallTo(() => _proformaServices.DeleteProforma(true, proformaId)).Returns(true);
            A.CallTo(() => _logServices.getLogTypeId("Delete")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", userId, logTypeId));
            A.CallTo(() => _userServices.GetUserFullName(userId)).Returns(userFull);
            A.CallTo(() => _notificationServices.CreateNotification(newNotification));
            var controller = new ProformasController(_proformaServices, _userServices, _logServices, _notificationServices);

            //Act
            var result = await controller.DeleteYourProforma(proformaId, userId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<BadRequestObjectResult>();
        }
        [Fact]
        public async Task ProformasController_DeleteClientProforma_ReturnOk()
        {
            //Arrange
            int proformaId = 1;
            int userId = 1;
            int owner = 2;
            int logTypeId = 1;
            string userFull = "";
            var newNotification = A.Fake<CreateNotification>();
            A.CallTo(() => _userServices.UserExist(userId)).Returns(true);
            A.CallTo(() => _proformaServices.ProformaExist(proformaId)).Returns(true);
            A.CallTo(() => _proformaServices.DoesDeliveryExist(proformaId)).Returns(false);
            A.CallTo(() => _proformaServices.GetProformaUserId(proformaId)).Returns(owner);
            A.CallTo(() => _proformaServices.GetProformaNumber(proformaId));
            A.CallTo(() => _proformaServices.DeleteProforma(false, proformaId)).Returns(true);
            A.CallTo(() => _logServices.getLogTypeId("Delete")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", userId, logTypeId));
            A.CallTo(() => _userServices.GetUserFullName(userId)).Returns(userFull);
            A.CallTo(() => _notificationServices.CreateNotification(newNotification));
            var controller = new ProformasController(_proformaServices, _userServices, _logServices, _notificationServices);

            //Act
            var result = await controller.DeleteClientProforma(proformaId, userId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkResult>();
        }
        [Fact]
        public async Task ProformasController_DeleteClientProforma_DeleteFails_Return500()
        {
            //Arrange
            int proformaId = 1;
            int userId = 1;
            int owner = 2;
            int logTypeId = 1;
            string userFull = "";
            var newNotification = A.Fake<CreateNotification>();
            A.CallTo(() => _userServices.UserExist(userId)).Returns(true);
            A.CallTo(() => _proformaServices.ProformaExist(proformaId)).Returns(true);
            A.CallTo(() => _proformaServices.DoesDeliveryExist(proformaId)).Returns(false);
            A.CallTo(() => _proformaServices.GetProformaUserId(proformaId)).Returns(owner);
            A.CallTo(() => _proformaServices.GetProformaNumber(proformaId));
            A.CallTo(() => _proformaServices.DeleteProforma(false, proformaId)).Returns(false);
            A.CallTo(() => _logServices.getLogTypeId("Delete")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", userId, logTypeId));
            A.CallTo(() => _userServices.GetUserFullName(userId)).Returns(userFull);
            A.CallTo(() => _notificationServices.CreateNotification(newNotification));
            var controller = new ProformasController(_proformaServices, _userServices, _logServices, _notificationServices);

            //Act
            var result = await controller.DeleteClientProforma(proformaId, userId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<StatusCodeResult>();
        }
        [Fact]
        public async Task ProformasController_DeleteClientProforma_UserNotFound_Return404()
        {
            //Arrange
            int proformaId = 1;
            int userId = 1;
            int owner = 2;
            int logTypeId = 1;
            string userFull = "";
            var newNotification = A.Fake<CreateNotification>();
            A.CallTo(() => _userServices.UserExist(userId)).Returns(false);
            A.CallTo(() => _proformaServices.ProformaExist(proformaId)).Returns(true);
            A.CallTo(() => _proformaServices.DoesDeliveryExist(proformaId)).Returns(false);
            A.CallTo(() => _proformaServices.GetProformaUserId(proformaId)).Returns(owner);
            A.CallTo(() => _proformaServices.GetProformaNumber(proformaId));
            A.CallTo(() => _proformaServices.DeleteProforma(false, proformaId)).Returns(true);
            A.CallTo(() => _logServices.getLogTypeId("Delete")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", userId, logTypeId));
            A.CallTo(() => _userServices.GetUserFullName(userId)).Returns(userFull);
            A.CallTo(() => _notificationServices.CreateNotification(newNotification));
            var controller = new ProformasController(_proformaServices, _userServices, _logServices, _notificationServices);

            //Act
            var result = await controller.DeleteClientProforma(proformaId, userId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundObjectResult>();
        }
        [Fact]
        public async Task ProformasController_DeleteClientProforma_ProformaNotFound_Return404()
        {
            //Arrange
            int proformaId = 1;
            int userId = 1;
            int owner = 2;
            int logTypeId = 1;
            string userFull = "";
            var newNotification = A.Fake<CreateNotification>();
            A.CallTo(() => _userServices.UserExist(userId)).Returns(true);
            A.CallTo(() => _proformaServices.ProformaExist(proformaId)).Returns(false);
            A.CallTo(() => _proformaServices.DoesDeliveryExist(proformaId)).Returns(false);
            A.CallTo(() => _proformaServices.GetProformaUserId(proformaId)).Returns(owner);
            A.CallTo(() => _proformaServices.GetProformaNumber(proformaId));
            A.CallTo(() => _proformaServices.DeleteProforma(false, proformaId)).Returns(true);
            A.CallTo(() => _logServices.getLogTypeId("Delete")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", userId, logTypeId));
            A.CallTo(() => _userServices.GetUserFullName(userId)).Returns(userFull);
            A.CallTo(() => _notificationServices.CreateNotification(newNotification));
            var controller = new ProformasController(_proformaServices, _userServices, _logServices, _notificationServices);

            //Act
            var result = await controller.DeleteClientProforma(proformaId, userId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundObjectResult>();
        }
        [Fact]
        public async Task ProformasController_DeleteClientProforma_DeliveryExistForThatProforma_Return400()
        {
            //Arrange
            int proformaId = 1;
            int userId = 1;
            int owner = 2;
            int logTypeId = 1;
            string userFull = "";
            var newNotification = A.Fake<CreateNotification>();
            A.CallTo(() => _userServices.UserExist(userId)).Returns(true);
            A.CallTo(() => _proformaServices.ProformaExist(proformaId)).Returns(true);
            A.CallTo(() => _proformaServices.DoesDeliveryExist(proformaId)).Returns(true);
            A.CallTo(() => _proformaServices.GetProformaUserId(proformaId)).Returns(owner);
            A.CallTo(() => _proformaServices.GetProformaNumber(proformaId));
            A.CallTo(() => _proformaServices.DeleteProforma(false, proformaId)).Returns(true);
            A.CallTo(() => _logServices.getLogTypeId("Delete")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", userId, logTypeId));
            A.CallTo(() => _userServices.GetUserFullName(userId)).Returns(userFull);
            A.CallTo(() => _notificationServices.CreateNotification(newNotification));
            var controller = new ProformasController(_proformaServices, _userServices, _logServices, _notificationServices);

            //Act
            var result = await controller.DeleteClientProforma(proformaId, userId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<BadRequestObjectResult>();
        }
        [Fact]
        public async Task ProformasController_GetProformaPath_ReturnOk()
        {
            //Arrange
            int proformaId = 1;
            A.CallTo(() => _proformaServices.ProformaExist(proformaId)).Returns(true);
            A.CallTo(() => _proformaServices.GetProformaPath(proformaId)).Returns("");
            var controller = new ProformasController(_proformaServices, _userServices, _logServices, _notificationServices);

            //Act
            var result = await controller.GetProformaPath(proformaId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();
        }
        [Fact]
        public async Task ProformasController_GetProformaPath_ProformaNotFound_Return404()
        {
            //Arrange
            int proformaId = 1;
            A.CallTo(() => _proformaServices.ProformaExist(proformaId)).Returns(false);
            A.CallTo(() => _proformaServices.GetProformaPath(proformaId)).Returns("");
            var controller = new ProformasController(_proformaServices, _userServices, _logServices, _notificationServices);

            //Act
            var result = await controller.GetProformaPath(proformaId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundObjectResult>();
        }
        [Fact]
        public async Task ProformasController_GetRestModifyProforma_ReturnOk()
        {
            //Arrange
            var data = A.Fake<GetRestModifyProforma>();
            int proformaId = 1;
            A.CallTo(() => _proformaServices.ProformaExist(proformaId)).Returns(true);
            A.CallTo(() => _proformaServices.GetRestModifyProforma(proformaId)).Returns(data);
            var controller = new ProformasController(_proformaServices, _userServices, _logServices, _notificationServices);

            //Act
            var result = await controller.GetRestModifyProforma(proformaId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();
        }
        [Fact]
        public async Task ProformasController_GetRestModifyProforma_ProformaNotFound_Return404()
        {
            //Arrange
            var data = A.Fake<GetRestModifyProforma>();
            int proformaId = 1;
            A.CallTo(() => _proformaServices.ProformaExist(proformaId)).Returns(false);
            A.CallTo(() => _proformaServices.GetRestModifyProforma(proformaId)).Returns(data);
            var controller = new ProformasController(_proformaServices, _userServices, _logServices, _notificationServices);

            //Act
            var result = await controller.GetRestModifyProforma(proformaId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundObjectResult>();
        }
        [Fact]
        public async Task ProformasController_ModifyProforma_ReturnOk()
        {
            //Arrange
            var data = A.Fake<ModifyProforma>();
            data.UserId = 2;
            int userId = 1;
            int logTypeId = 1;
            string userFull = "";
            var newNotification = A.Fake<CreateNotification>();
            A.CallTo(() => _userServices.UserExist(userId)).Returns(true);
            A.CallTo(() => _userServices.UserExist((int)data.UserId)).Returns(true);
            A.CallTo(() => _proformaServices.GetProformaNumber(data.ProformaId)).Returns("");
            A.CallTo(() => _proformaServices.ModifyProforma(data)).Returns(true);
            A.CallTo(() => _logServices.getLogTypeId("Modify")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", userId, logTypeId));
            A.CallTo(() => _userServices.GetUserFullName(userId)).Returns(userFull);
            A.CallTo(() => _notificationServices.CreateNotification(newNotification));
            var controller = new ProformasController(_proformaServices, _userServices, _logServices, _notificationServices);

            //Act
            var result = await controller.ModifyProforma(data, userId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkResult>();
        }
        [Fact]
        public async Task ProformasController_ModifyProforma_ModifyFails_Return500()
        {
            //Arrange
            var data = A.Fake<ModifyProforma>();
            data.UserId = 2;
            int userId = 1;
            int logTypeId = 1;
            string userFull = "";
            var newNotification = A.Fake<CreateNotification>();
            A.CallTo(() => _userServices.UserExist(userId)).Returns(true);
            A.CallTo(() => _userServices.UserExist((int)data.UserId)).Returns(true);
            A.CallTo(() => _proformaServices.GetProformaNumber(data.ProformaId)).Returns("");
            A.CallTo(() => _proformaServices.ModifyProforma(data)).Returns(false);
            A.CallTo(() => _logServices.getLogTypeId("Modify")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", userId, logTypeId));
            A.CallTo(() => _userServices.GetUserFullName(userId)).Returns(userFull);
            A.CallTo(() => _notificationServices.CreateNotification(newNotification));
            var controller = new ProformasController(_proformaServices, _userServices, _logServices, _notificationServices);

            //Act
            var result = await controller.ModifyProforma(data, userId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<StatusCodeResult>();
        }
        [Fact]
        public async Task ProformasController_ModifyProforma_UserNotFound_Return404()
        {
            //Arrange
            var data = A.Fake<ModifyProforma>();
            data.UserId = 2;
            int userId = 1;
            int logTypeId = 1;
            string userFull = "";
            var newNotification = A.Fake<CreateNotification>();
            A.CallTo(() => _userServices.UserExist(userId)).Returns(false);
            A.CallTo(() => _userServices.UserExist((int)data.UserId)).Returns(true);
            A.CallTo(() => _proformaServices.GetProformaNumber(data.ProformaId)).Returns("");
            A.CallTo(() => _proformaServices.ModifyProforma(data)).Returns(true);
            A.CallTo(() => _logServices.getLogTypeId("Modify")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", userId, logTypeId));
            A.CallTo(() => _userServices.GetUserFullName(userId)).Returns(userFull);
            A.CallTo(() => _notificationServices.CreateNotification(newNotification));
            var controller = new ProformasController(_proformaServices, _userServices, _logServices, _notificationServices);

            //Act
            var result = await controller.ModifyProforma(data, userId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundObjectResult>();
        }
        [Fact]
        public async Task ProformasController_ModifyProforma_ProformaUserNotFound_Return404()
        {
            //Arrange
            var data = A.Fake<ModifyProforma>();
            data.UserId = 2;
            int userId = 1;
            int logTypeId = 1;
            string userFull = "";
            var newNotification = A.Fake<CreateNotification>();
            A.CallTo(() => _userServices.UserExist(userId)).Returns(true);
            A.CallTo(() => _userServices.UserExist((int)data.UserId)).Returns(false);
            A.CallTo(() => _proformaServices.GetProformaNumber(data.ProformaId)).Returns("");
            A.CallTo(() => _proformaServices.ModifyProforma(data)).Returns(true);
            A.CallTo(() => _logServices.getLogTypeId("Modify")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", userId, logTypeId));
            A.CallTo(() => _userServices.GetUserFullName(userId)).Returns(userFull);
            A.CallTo(() => _notificationServices.CreateNotification(newNotification));
            var controller = new ProformasController(_proformaServices, _userServices, _logServices, _notificationServices);

            //Act
            var result = await controller.ModifyProforma(data, userId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundObjectResult>();
        }
    }
}
