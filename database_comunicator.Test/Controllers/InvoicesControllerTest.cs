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
    public class InvoicesControllerTest
    {
        private readonly IInvoiceServices _invoicesService;
        private readonly IUserServices _userServices;
        private readonly IOrganizationServices _organizationServices;
        private readonly IItemServices _itemServices;
        private readonly ILogServices _logServices;
        private readonly INotificationServices _notificationServices;
        public InvoicesControllerTest()
        {
            _invoicesService = A.Fake<IInvoiceServices>();
            _userServices = A.Fake<IUserServices>();
            _organizationServices = A.Fake<IOrganizationServices>();
            _itemServices = A.Fake<IItemServices>();
            _logServices = A.Fake<ILogServices>();
            _notificationServices = A.Fake<INotificationServices>();
        }
        [Fact]
        public async Task InvoicesController_GetTaxes_ReturnOk()
        {
            //Arrange
            var data = A.Fake<IEnumerable<GetTaxes>>();
            A.CallTo(() => _invoicesService.GetTaxes()).Returns(data);
            var controller = new InvoicesController(_invoicesService, _userServices, _organizationServices, _itemServices, _logServices, _notificationServices);

            //Act
            var result = await controller.GetTaxes();

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();
        }
        [Fact]
        public async Task InvoicesController_GetPaymentStatuses_ReturnOk()
        {
            //Arrange
            var data = A.Fake<IEnumerable<GetPaymentStatuses>>();
            A.CallTo(() => _invoicesService.GetPaymentStatuses()).Returns(data);
            var controller = new InvoicesController(_invoicesService, _userServices, _organizationServices, _itemServices, _logServices, _notificationServices);

            //Act
            var result = await controller.GetPaymentStatuses();

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();
        }
        [Fact]
        public async Task InvoicesController_GetPurchaseList_ReturnOk()
        {
            //Arrange
            var data = A.Fake<IEnumerable<GetInvoicesList>>();
            A.CallTo(() => _invoicesService.GetPurchaseInvoicesList()).Returns(data);
            var controller = new InvoicesController(_invoicesService, _userServices, _organizationServices, _itemServices, _logServices, _notificationServices);

            //Act
            var result = await controller.GetPurchaseList();

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();
        }
        [Fact]
        public async Task InvoicesController_GetSalesList_ReturnOk()
        {
            //Arrange
            var data = A.Fake<IEnumerable<GetInvoicesList>>();
            A.CallTo(() => _invoicesService.GetSalesInvoicesList()).Returns(data);
            var controller = new InvoicesController(_invoicesService, _userServices, _organizationServices, _itemServices, _logServices, _notificationServices);

            //Act
            var result = await controller.GetSalesList();

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();
        }
        [Fact]
        public async Task InvoicesController_GetPaymentMethods_ReturnOk()
        {
            //Arrange
            var data = A.Fake<IEnumerable<GetPaymentMethods>>();
            A.CallTo(() => _invoicesService.GetPaymentMethods()).Returns(data);
            var controller = new InvoicesController(_invoicesService, _userServices, _organizationServices, _itemServices, _logServices, _notificationServices);

            //Act
            var result = await controller.GetPaymentMethods();

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();
        }
        [Fact]
        public async Task InvoicesController_GetPurchaseInvoiceItems_ReturnOk()
        {
            //Arrange
            var data = A.Fake<IEnumerable<GetInvoiceItems>>();
            int invoiceId = 1;
            A.CallTo(() => _invoicesService.InvoiceExist(invoiceId)).Returns(true);
            A.CallTo(() => _invoicesService.GetInvoiceItems(invoiceId, true)).Returns(data);
            var controller = new InvoicesController(_invoicesService, _userServices, _organizationServices, _itemServices, _logServices, _notificationServices);

            //Act
            var result = await controller.GetPurchaseInvoiceItems(invoiceId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();
        }
        [Fact]
        public async Task InvoicesController_GetPurchaseInvoiceItems_InvoiceNotFound_Return404()
        {
            //Arrange
            var data = A.Fake<IEnumerable<GetInvoiceItems>>();
            int invoiceId = 1;
            A.CallTo(() => _invoicesService.InvoiceExist(invoiceId)).Returns(false);
            A.CallTo(() => _invoicesService.GetInvoiceItems(invoiceId, true)).Returns(data);
            var controller = new InvoicesController(_invoicesService, _userServices, _organizationServices, _itemServices, _logServices, _notificationServices);

            //Act
            var result = await controller.GetPurchaseInvoiceItems(invoiceId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundObjectResult>();
        }
        [Fact]
        public async Task InvoicesController_GetSalesInvoiceItems_ReturnOk()
        {
            //Arrange
            var data = A.Fake<IEnumerable<GetInvoiceItems>>();
            int invoiceId = 1;
            A.CallTo(() => _invoicesService.InvoiceExist(invoiceId)).Returns(true);
            A.CallTo(() => _invoicesService.GetInvoiceItems(invoiceId, false)).Returns(data);
            var controller = new InvoicesController(_invoicesService, _userServices, _organizationServices, _itemServices, _logServices, _notificationServices);

            //Act
            var result = await controller.GetSalesInvoiceItems(invoiceId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();
        }
        [Fact]
        public async Task InvoicesController_GetSalesInvoiceItems_InvoiceNotFound_Return404()
        {
            //Arrange
            var data = A.Fake<IEnumerable<GetInvoiceItems>>();
            int invoiceId = 1;
            A.CallTo(() => _invoicesService.InvoiceExist(invoiceId)).Returns(false);
            A.CallTo(() => _invoicesService.GetInvoiceItems(invoiceId, false)).Returns(data);
            var controller = new InvoicesController(_invoicesService, _userServices, _organizationServices, _itemServices, _logServices, _notificationServices);

            //Act
            var result = await controller.GetSalesInvoiceItems(invoiceId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundObjectResult>();
        }
        [Fact]
        public async Task InvoicesController_GetOrgs_ReturnOk()
        {
            //Arrange
            var data = A.Fake<GetOrgsForInvocie>();
            int userId = 1;
            A.CallTo(() => _userServices.UserExist(userId)).Returns(true);
            A.CallTo(() => _invoicesService.GetOrgsForInvoice(userId)).Returns(data);
            var controller = new InvoicesController(_invoicesService, _userServices, _organizationServices, _itemServices, _logServices, _notificationServices);

            //Act
            var result = await controller.GetOrgs(userId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();
        }
        [Fact]
        public async Task InvoicesController_GetOrgs_UserNotFound_Return404()
        {
            //Arrange
            var data = A.Fake<GetOrgsForInvocie>();
            int userId = 1;
            A.CallTo(() => _userServices.UserExist(userId)).Returns(false);
            A.CallTo(() => _invoicesService.GetOrgsForInvoice(userId)).Returns(data);
            var controller = new InvoicesController(_invoicesService, _userServices, _organizationServices, _itemServices, _logServices, _notificationServices);

            //Act
            var result = await controller.GetOrgs(userId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundObjectResult>();
        }
        [Fact]
        public async Task InvoicesController_GetPurchaseInvoices_ReturnOk()
        {
            //Arrange
            var data = A.Fake<IEnumerable<GetInvoices>>();
            int userId = 1;
            var filters = A.Fake<InvoiceFiltersTemplate>();
            A.CallTo(() => _userServices.UserExist(userId)).Returns(true);
            A.CallTo(() => _invoicesService.GetPurchaseInvoices(userId, null, filters)).Returns(data);
            var controller = new InvoicesController(_invoicesService, _userServices, _organizationServices, _itemServices, _logServices, _notificationServices);

            //Act
            var result = await controller.GetPurchaseInvoices(userId, null, null, null, null, null, null, null, null, null, null, null, null, null, null);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();
        }
        [Fact]
        public async Task InvoicesController_GetPurchaseInvoices_UserNotFound_Return404()
        {
            //Arrange
            var data = A.Fake<IEnumerable<GetInvoices>>();
            int userId = 1;
            var filters = A.Fake<InvoiceFiltersTemplate>();
            A.CallTo(() => _userServices.UserExist(userId)).Returns(false);
            A.CallTo(() => _invoicesService.GetPurchaseInvoices(userId, null, filters)).Returns(data);
            var controller = new InvoicesController(_invoicesService, _userServices, _organizationServices, _itemServices, _logServices, _notificationServices);

            //Act
            var result = await controller.GetPurchaseInvoices(userId, null, null, null, null, null, null, null, null, null, null, null, null, null, null);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundObjectResult>();
        }
        [Fact]
        public async Task InvoicesController_GetPurchaseInvoices_SortIsIncorrect_Return400()
        {
            //Arrange
            var data = A.Fake<IEnumerable<GetInvoices>>();
            int userId = 1;
            var filters = A.Fake<InvoiceFiltersTemplate>();
            string sort = "";
            A.CallTo(() => _userServices.UserExist(userId)).Returns(true);
            A.CallTo(() => _invoicesService.GetPurchaseInvoices(userId, null, filters)).Returns(data);
            var controller = new InvoicesController(_invoicesService, _userServices, _organizationServices, _itemServices, _logServices, _notificationServices);

            //Act
            var result = await controller.GetPurchaseInvoices(userId, null, sort, null, null, null, null, null, null, null, null, null, null, null, null);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<BadRequestObjectResult>();
        }
        [Fact]
        public async Task InvoicesController_GetPurchaseInvoicesOrg_ReturnOk()
        {
            //Arrange
            var data = A.Fake<IEnumerable<GetInvoices>>();
            var filters = A.Fake<InvoiceFiltersTemplate>();
            A.CallTo(() => _invoicesService.GetPurchaseInvoices(null, filters)).Returns(data);
            var controller = new InvoicesController(_invoicesService, _userServices, _organizationServices, _itemServices, _logServices, _notificationServices);

            //Act
            var result = await controller.GetPurchaseInvoicesOrg(null, null, null, null, null, null, null, null, null, null, null, null, null, null);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();
        }
        [Fact]
        public async Task InvoicesController_GetPurchaseInvoicesOrg_SortIsIncorrect_Return400()
        {
            //Arrange
            var data = A.Fake<IEnumerable<GetInvoices>>();
            var filters = A.Fake<InvoiceFiltersTemplate>();
            string sort = "";
            A.CallTo(() => _invoicesService.GetPurchaseInvoices(null, filters)).Returns(data);
            var controller = new InvoicesController(_invoicesService, _userServices, _organizationServices, _itemServices, _logServices, _notificationServices);

            //Act
            var result = await controller.GetPurchaseInvoicesOrg(null, sort, null, null, null, null, null, null, null, null, null, null, null, null);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<BadRequestObjectResult>();
        }
        [Fact]
        public async Task InvoicesController_GetSalesInvoices_ReturnOk()
        {
            //Arrange
            var data = A.Fake<IEnumerable<GetInvoices>>();
            int userId = 1;
            var filters = A.Fake<InvoiceFiltersTemplate>();
            A.CallTo(() => _userServices.UserExist(userId)).Returns(true);
            A.CallTo(() => _invoicesService.GetSalesInvoices(userId, null, filters)).Returns(data);
            var controller = new InvoicesController(_invoicesService, _userServices, _organizationServices, _itemServices, _logServices, _notificationServices);

            //Act
            var result = await controller.GetSalesInvoices(userId, null, null, null, null, null, null, null, null, null, null, null, null, null, null);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();
        }
        [Fact]
        public async Task InvoicesController_GetSalesInvoices_UserNotFound_Return404()
        {
            //Arrange
            var data = A.Fake<IEnumerable<GetInvoices>>();
            int userId = 1;
            var filters = A.Fake<InvoiceFiltersTemplate>();
            A.CallTo(() => _userServices.UserExist(userId)).Returns(false);
            A.CallTo(() => _invoicesService.GetSalesInvoices(userId, null, filters)).Returns(data);
            var controller = new InvoicesController(_invoicesService, _userServices, _organizationServices, _itemServices, _logServices, _notificationServices);

            //Act
            var result = await controller.GetSalesInvoices(userId, null, null, null, null, null, null, null, null, null, null, null, null, null, null);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundObjectResult>();
        }
        [Fact]
        public async Task InvoicesController_GetSalesInvoices_SortIsIncorrect_Return400()
        {
            //Arrange
            var data = A.Fake<IEnumerable<GetInvoices>>();
            int userId = 1;
            var filters = A.Fake<InvoiceFiltersTemplate>();
            string sort = "";
            A.CallTo(() => _userServices.UserExist(userId)).Returns(true);
            A.CallTo(() => _invoicesService.GetSalesInvoices(userId, null, filters)).Returns(data);
            var controller = new InvoicesController(_invoicesService, _userServices, _organizationServices, _itemServices, _logServices, _notificationServices);

            //Act
            var result = await controller.GetSalesInvoices(userId, null, sort, null, null, null, null, null, null, null, null, null, null, null, null);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<BadRequestObjectResult>();
        }
        [Fact]
        public async Task InvoicesController_GetSalesInvoicesOrg_ReturnOk()
        {
            //Arrange
            var data = A.Fake<IEnumerable<GetInvoices>>();
            var filters = A.Fake<InvoiceFiltersTemplate>();
            A.CallTo(() => _invoicesService.GetSalesInvoices(null, filters)).Returns(data);
            var controller = new InvoicesController(_invoicesService, _userServices, _organizationServices, _itemServices, _logServices, _notificationServices);

            //Act
            var result = await controller.GetSalesInvoicesOrg(null, null, null, null, null, null, null, null, null, null, null, null, null, null);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();
        }
        [Fact]
        public async Task InvoicesController_GetSalesInvoicesOrg_SortIsIncorrect_Return400()
        {
            //Arrange
            var data = A.Fake<IEnumerable<GetInvoices>>();
            var filters = A.Fake<InvoiceFiltersTemplate>();
            string sort = "";
            A.CallTo(() => _invoicesService.GetSalesInvoices(null, filters)).Returns(data);
            var controller = new InvoicesController(_invoicesService, _userServices, _organizationServices, _itemServices, _logServices, _notificationServices);

            //Act
            var result = await controller.GetSalesInvoicesOrg(null, sort, null, null, null, null, null, null, null, null, null, null, null, null);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<BadRequestObjectResult>();
        }
        [Fact]
        public async Task InvoicesController_AddPurchaseInvoice_ReturnOk()
        {
            //Arrange
            var data = A.Dummy<AddPurchaseInvoice>();
            data.InvoiceItems = A.CollectionOfDummy<InvoiceItems>(2);
            int newInvoiceId = 1;
            int userId = 1;
            int logTypeId = 1;
            string userFull = "";
            A.CallTo(() => _userServices.UserExist(data.UserId)).Returns(true);
            A.CallTo(() => _organizationServices.OrgExist(data.Seller)).Returns(true);
            A.CallTo(() => _itemServices.ItemExist(A<int>._)).Returns(true);
            A.CallTo(() => _invoicesService.AddPurchaseInvoice(data)).Returns(newInvoiceId);
            A.CallTo(() => _logServices.getLogTypeId("Create")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", data.UserId, logTypeId));
            A.CallTo(() => _userServices.GetUserFullName(data.UserId)).Returns(userFull);
            A.CallTo(() => _notificationServices.CreateNotification(A<CreateNotification>.Ignored));
            var controller = new InvoicesController(_invoicesService, _userServices, _organizationServices, _itemServices, _logServices, _notificationServices);

            //Act
            var result = await controller.AddPurchaseInvoice(data, userId);

            //Assert
            A.CallTo(() => _itemServices.ItemExist(A<int>._)).MustHaveHappenedTwiceExactly();
            result.Should().NotBeNull();
            result.Should().BeOfType<OkResult>();
        }
        [Fact]
        public async Task InvoicesController_AddPurchaseInvoice_AddFails_Return400()
        {
            //Arrange
            var data = A.Dummy<AddPurchaseInvoice>();
            data.InvoiceItems = A.CollectionOfDummy<InvoiceItems>(2);
            int newInvoiceId = 0;
            int userId = 1;
            int logTypeId = 1;
            string userFull = "";
            A.CallTo(() => _userServices.UserExist(data.UserId)).Returns(true);
            A.CallTo(() => _organizationServices.OrgExist(data.Seller)).Returns(true);
            A.CallTo(() => _itemServices.ItemExist(A<int>._)).Returns(true);
            A.CallTo(() => _invoicesService.AddPurchaseInvoice(data)).Returns(newInvoiceId);
            A.CallTo(() => _logServices.getLogTypeId("Create")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", data.UserId, logTypeId));
            A.CallTo(() => _userServices.GetUserFullName(data.UserId)).Returns(userFull);
            A.CallTo(() => _notificationServices.CreateNotification(A<CreateNotification>.Ignored));
            var controller = new InvoicesController(_invoicesService, _userServices, _organizationServices, _itemServices, _logServices, _notificationServices);

            //Act
            var result = await controller.AddPurchaseInvoice(data, userId);

            //Assert
            A.CallTo(() => _itemServices.ItemExist(A<int>._)).MustHaveHappenedTwiceExactly();
            result.Should().NotBeNull();
            result.Should().BeOfType<BadRequestObjectResult>();
        }
        [Fact]
        public async Task InvoicesController_AddPurchaseInvoice_UserNotFound_Return404()
        {
            //Arrange
            var data = A.Dummy<AddPurchaseInvoice>();
            data.InvoiceItems = A.CollectionOfDummy<InvoiceItems>(2);
            int newInvoiceId = 1;
            int userId = 1;
            int logTypeId = 1;
            string userFull = "";
            A.CallTo(() => _userServices.UserExist(data.UserId)).Returns(false);
            A.CallTo(() => _organizationServices.OrgExist(data.Seller)).Returns(true);
            A.CallTo(() => _itemServices.ItemExist(A<int>._)).Returns(true);
            A.CallTo(() => _invoicesService.AddPurchaseInvoice(data)).Returns(newInvoiceId);
            A.CallTo(() => _logServices.getLogTypeId("Create")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", data.UserId, logTypeId));
            A.CallTo(() => _userServices.GetUserFullName(data.UserId)).Returns(userFull);
            A.CallTo(() => _notificationServices.CreateNotification(A<CreateNotification>.Ignored));
            var controller = new InvoicesController(_invoicesService, _userServices, _organizationServices, _itemServices, _logServices, _notificationServices);

            //Act
            var result = await controller.AddPurchaseInvoice(data, userId);

            //Assert
            A.CallTo(() => _itemServices.ItemExist(A<int>._)).MustNotHaveHappened();
            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundObjectResult>();
        }
        [Fact]
        public async Task InvoicesController_AddPurchaseInvoice_OrgNotFound_Return404()
        {
            //Arrange
            var data = A.Dummy<AddPurchaseInvoice>();
            data.InvoiceItems = A.CollectionOfDummy<InvoiceItems>(2);
            int newInvoiceId = 1;
            int userId = 1;
            int logTypeId = 1;
            string userFull = "";
            A.CallTo(() => _userServices.UserExist(data.UserId)).Returns(true);
            A.CallTo(() => _organizationServices.OrgExist(data.Seller)).Returns(false);
            A.CallTo(() => _itemServices.ItemExist(A<int>._)).Returns(true);
            A.CallTo(() => _invoicesService.AddPurchaseInvoice(data)).Returns(newInvoiceId);
            A.CallTo(() => _logServices.getLogTypeId("Create")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", data.UserId, logTypeId));
            A.CallTo(() => _userServices.GetUserFullName(data.UserId)).Returns(userFull);
            A.CallTo(() => _notificationServices.CreateNotification(A<CreateNotification>.Ignored));
            var controller = new InvoicesController(_invoicesService, _userServices, _organizationServices, _itemServices, _logServices, _notificationServices);

            //Act
            var result = await controller.AddPurchaseInvoice(data, userId);

            //Assert
            A.CallTo(() => _itemServices.ItemExist(A<int>._)).MustNotHaveHappened();
            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundObjectResult>();
        }
        [Fact]
        public async Task InvoicesController_AddPurchaseInvoice_ItemNotFound_Return404()
        {
            //Arrange
            var data = A.Dummy<AddPurchaseInvoice>();
            data.InvoiceItems = A.CollectionOfDummy<InvoiceItems>(1);
            int newInvoiceId = 1;
            int userId = 1;
            int logTypeId = 1;
            string userFull = "";
            A.CallTo(() => _userServices.UserExist(data.UserId)).Returns(true);
            A.CallTo(() => _organizationServices.OrgExist(data.Seller)).Returns(true);
            A.CallTo(() => _itemServices.ItemExist(A<int>._)).Returns(false);
            A.CallTo(() => _invoicesService.AddPurchaseInvoice(data)).Returns(newInvoiceId);
            A.CallTo(() => _logServices.getLogTypeId("Create")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", data.UserId, logTypeId));
            A.CallTo(() => _userServices.GetUserFullName(data.UserId)).Returns(userFull);
            A.CallTo(() => _notificationServices.CreateNotification(A<CreateNotification>.Ignored));
            var controller = new InvoicesController(_invoicesService, _userServices, _organizationServices, _itemServices, _logServices, _notificationServices);

            //Act
            var result = await controller.AddPurchaseInvoice(data, userId);

            //Assert
            A.CallTo(() => _itemServices.ItemExist(A<int>._)).MustHaveHappenedOnceExactly();
            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundObjectResult>();
        }
        [Fact]
        public async Task InvoicesController_AddSalesInvoice_ReturnOk()
        {
            //Arrange
            var data = A.Dummy<AddSalesInvoice>();
            data.InvoiceItems = A.CollectionOfDummy<SalesInvoiceItems>(2);
            int newInvoiceId = 1;
            int userId = 1;
            int logTypeId = 1;
            string userFull = "";
            A.CallTo(() => _userServices.UserExist(data.UserId)).Returns(true);
            A.CallTo(() => _organizationServices.OrgExist(data.Seller)).Returns(true);
            A.CallTo(() => _itemServices.ItemExist(A<int>._)).Returns(true);
            A.CallTo(() => _invoicesService.AddSalesInvoice(data)).Returns(newInvoiceId);
            A.CallTo(() => _logServices.getLogTypeId("Create")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", data.UserId, logTypeId));
            A.CallTo(() => _userServices.GetUserFullName(data.UserId)).Returns(userFull);
            A.CallTo(() => _notificationServices.CreateNotification(A<CreateNotification>.Ignored));
            var controller = new InvoicesController(_invoicesService, _userServices, _organizationServices, _itemServices, _logServices, _notificationServices);

            //Act
            var result = await controller.AddSalesInvoice(data, userId);

            //Assert
            A.CallTo(() => _itemServices.ItemExist(A<int>._)).MustHaveHappenedTwiceExactly();
            result.Should().NotBeNull();
            result.Should().BeOfType<OkResult>();
        }
        [Fact]
        public async Task InvoicesController_AddSalesInvoice_AddFails_Return400()
        {
            //Arrange
            var data = A.Dummy<AddSalesInvoice>();
            data.InvoiceItems = A.CollectionOfDummy<SalesInvoiceItems>(2);
            int newInvoiceId = 0;
            int userId = 1;
            int logTypeId = 1;
            string userFull = "";
            A.CallTo(() => _userServices.UserExist(data.UserId)).Returns(true);
            A.CallTo(() => _organizationServices.OrgExist(data.Seller)).Returns(true);
            A.CallTo(() => _itemServices.ItemExist(A<int>._)).Returns(true);
            A.CallTo(() => _invoicesService.AddSalesInvoice(data)).Returns(newInvoiceId);
            A.CallTo(() => _logServices.getLogTypeId("Create")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", data.UserId, logTypeId));
            A.CallTo(() => _userServices.GetUserFullName(data.UserId)).Returns(userFull);
            A.CallTo(() => _notificationServices.CreateNotification(A<CreateNotification>.Ignored));
            var controller = new InvoicesController(_invoicesService, _userServices, _organizationServices, _itemServices, _logServices, _notificationServices);

            //Act
            var result = await controller.AddSalesInvoice(data, userId);

            //Assert
            A.CallTo(() => _itemServices.ItemExist(A<int>._)).MustHaveHappenedTwiceExactly();
            result.Should().NotBeNull();
            result.Should().BeOfType<BadRequestObjectResult>();
        }
        [Fact]
        public async Task InvoicesController_AddSalesInvoice_UserNotFound_Return404()
        {
            //Arrange
            var data = A.Dummy<AddSalesInvoice>();
            data.InvoiceItems = A.CollectionOfDummy<SalesInvoiceItems>(2);
            int newInvoiceId = 1;
            int userId = 1;
            int logTypeId = 1;
            string userFull = "";
            A.CallTo(() => _userServices.UserExist(data.UserId)).Returns(false);
            A.CallTo(() => _organizationServices.OrgExist(data.Seller)).Returns(true);
            A.CallTo(() => _itemServices.ItemExist(A<int>._)).Returns(true);
            A.CallTo(() => _invoicesService.AddSalesInvoice(data)).Returns(newInvoiceId);
            A.CallTo(() => _logServices.getLogTypeId("Create")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", data.UserId, logTypeId));
            A.CallTo(() => _userServices.GetUserFullName(data.UserId)).Returns(userFull);
            A.CallTo(() => _notificationServices.CreateNotification(A<CreateNotification>.Ignored));
            var controller = new InvoicesController(_invoicesService, _userServices, _organizationServices, _itemServices, _logServices, _notificationServices);

            //Act
            var result = await controller.AddSalesInvoice(data, userId);

            //Assert
            A.CallTo(() => _itemServices.ItemExist(A<int>._)).MustNotHaveHappened();
            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundObjectResult>();
        }
        [Fact]
        public async Task InvoicesController_AddSalesInvoice_OrgNotFound_Return404()
        {
            //Arrange
            var data = A.Dummy<AddSalesInvoice>();
            data.InvoiceItems = A.CollectionOfDummy<SalesInvoiceItems>(2);
            int newInvoiceId = 1;
            int userId = 1;
            int logTypeId = 1;
            string userFull = "";
            A.CallTo(() => _userServices.UserExist(data.UserId)).Returns(true);
            A.CallTo(() => _organizationServices.OrgExist(data.Seller)).Returns(false);
            A.CallTo(() => _itemServices.ItemExist(A<int>._)).Returns(true);
            A.CallTo(() => _invoicesService.AddSalesInvoice(data)).Returns(newInvoiceId);
            A.CallTo(() => _logServices.getLogTypeId("Create")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", data.UserId, logTypeId));
            A.CallTo(() => _userServices.GetUserFullName(data.UserId)).Returns(userFull);
            A.CallTo(() => _notificationServices.CreateNotification(A<CreateNotification>.Ignored));
            var controller = new InvoicesController(_invoicesService, _userServices, _organizationServices, _itemServices, _logServices, _notificationServices);

            //Act
            var result = await controller.AddSalesInvoice(data, userId);

            //Assert
            A.CallTo(() => _itemServices.ItemExist(A<int>._)).MustNotHaveHappened();
            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundObjectResult>();
        }
        [Fact]
        public async Task InvoicesController_AddSalesInvoice_ItemNotFound_Return404()
        {
            //Arrange
            var data = A.Dummy<AddSalesInvoice>();
            data.InvoiceItems = A.CollectionOfDummy<SalesInvoiceItems>(1);
            int newInvoiceId = 1;
            int userId = 1;
            int logTypeId = 1;
            string userFull = "";
            A.CallTo(() => _userServices.UserExist(data.UserId)).Returns(true);
            A.CallTo(() => _organizationServices.OrgExist(data.Seller)).Returns(true);
            A.CallTo(() => _itemServices.ItemExist(A<int>._)).Returns(false);
            A.CallTo(() => _invoicesService.AddSalesInvoice(data)).Returns(newInvoiceId);
            A.CallTo(() => _logServices.getLogTypeId("Create")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", data.UserId, logTypeId));
            A.CallTo(() => _userServices.GetUserFullName(data.UserId)).Returns(userFull);
            A.CallTo(() => _notificationServices.CreateNotification(A<CreateNotification>.Ignored));
            var controller = new InvoicesController(_invoicesService, _userServices, _organizationServices, _itemServices, _logServices, _notificationServices);

            //Act
            var result = await controller.AddSalesInvoice(data, userId);

            //Assert
            A.CallTo(() => _itemServices.ItemExist(A<int>._)).MustHaveHappenedOnceExactly();
            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundObjectResult>();
        }
        [Fact]
        public async Task InvoicesController_GetAllItems_ReturnOk()
        {
            //Arrange
            var data = A.Fake<IEnumerable<GetItemList>>();
            A.CallTo(() => _itemServices.GetItemList()).Returns(data);
            var controller = new InvoicesController(_invoicesService, _userServices, _organizationServices, _itemServices, _logServices, _notificationServices);

            //Act
            var result = await controller.GetAllItems();

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();
        }
        [Fact]
        public async Task InvoicesController_GetAllSalesItems_ReturnOk()
        {
            //Arrange
            var data = A.Fake<IEnumerable<GetSalesItemList>>();
            string currency = "PLN";
            int userId = 1;
            A.CallTo(() => _userServices.UserExist(userId)).Returns(true);
            A.CallTo(() => _itemServices.GetSalesItemList(userId, currency)).Returns(data);
            var controller = new InvoicesController(_invoicesService, _userServices, _organizationServices, _itemServices, _logServices, _notificationServices);

            //Act
            var result = await controller.GetAllSalesItems(userId, currency);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();
        }
        [Fact]
        public async Task InvoicesController_GetAllSalesItems_UserNotFound_Return404()
        {
            //Arrange
            var data = A.Fake<IEnumerable<GetSalesItemList>>();
            string currency = "PLN";
            int userId = 1;
            A.CallTo(() => _userServices.UserExist(userId)).Returns(false);
            A.CallTo(() => _itemServices.GetSalesItemList(userId, currency)).Returns(data);
            var controller = new InvoicesController(_invoicesService, _userServices, _organizationServices, _itemServices, _logServices, _notificationServices);

            //Act
            var result = await controller.GetAllSalesItems(userId, currency);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundObjectResult>();
        }
        [Fact]
        public async Task InvoicesController_DeleteInvoice_ReturnOk()
        {
            //Arrange
            int invoiceId = 1;
            int userId = 1;
            bool isYourInvoice = true;
            int logTypeId = 1;
            string userFull = "";
            A.CallTo(() => _userServices.UserExist(userId)).Returns(true);
            A.CallTo(() => _invoicesService.InvoiceExist(invoiceId)).Returns(true);
            A.CallTo(() => _invoicesService.CheckIfCreditNoteExist(invoiceId)).Returns(false);
            A.CallTo(() => _invoicesService.CheckIfSellingPriceExist(invoiceId)).Returns(false);
            A.CallTo(() => _invoicesService.GetInvoiceUser(invoiceId));
            A.CallTo(() => _invoicesService.GetInvoiceNumber(invoiceId));
            A.CallTo(() => _invoicesService.DeleteInvoice(invoiceId)).Returns(true);
            A.CallTo(() => _logServices.getLogTypeId("Delete")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", userId, logTypeId));
            A.CallTo(() => _userServices.GetUserFullName(userId)).Returns(userFull);
            A.CallTo(() => _notificationServices.CreateNotification(A<CreateNotification>.Ignored));
            var controller = new InvoicesController(_invoicesService, _userServices, _organizationServices, _itemServices, _logServices, _notificationServices);

            //Act
            var result = await controller.DeleteInvoice(invoiceId, userId, isYourInvoice);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkResult>();
        }
        [Fact]
        public async Task InvoicesController_DeleteInvoice_DeleteFails_Return500()
        {
            //Arrange
            int invoiceId = 1;
            int userId = 1;
            bool isYourInvoice = true;
            int logTypeId = 1;
            string userFull = "";
            A.CallTo(() => _userServices.UserExist(userId)).Returns(true);
            A.CallTo(() => _invoicesService.InvoiceExist(invoiceId)).Returns(true);
            A.CallTo(() => _invoicesService.CheckIfCreditNoteExist(invoiceId)).Returns(false);
            A.CallTo(() => _invoicesService.CheckIfSellingPriceExist(invoiceId)).Returns(false);
            A.CallTo(() => _invoicesService.GetInvoiceUser(invoiceId));
            A.CallTo(() => _invoicesService.GetInvoiceNumber(invoiceId));
            A.CallTo(() => _invoicesService.DeleteInvoice(invoiceId)).Returns(false);
            A.CallTo(() => _logServices.getLogTypeId("Delete")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", userId, logTypeId));
            A.CallTo(() => _userServices.GetUserFullName(userId)).Returns(userFull);
            A.CallTo(() => _notificationServices.CreateNotification(A<CreateNotification>.Ignored));
            var controller = new InvoicesController(_invoicesService, _userServices, _organizationServices, _itemServices, _logServices, _notificationServices);

            //Act
            var result = await controller.DeleteInvoice(invoiceId, userId, isYourInvoice);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<StatusCodeResult>();
        }
        [Fact]
        public async Task InvoicesController_DeleteInvoice_UserNotFound_Return404()
        {
            //Arrange
            int invoiceId = 1;
            int userId = 1;
            bool isYourInvoice = true;
            int logTypeId = 1;
            string userFull = "";
            A.CallTo(() => _userServices.UserExist(userId)).Returns(false);
            A.CallTo(() => _invoicesService.InvoiceExist(invoiceId)).Returns(true);
            A.CallTo(() => _invoicesService.CheckIfCreditNoteExist(invoiceId)).Returns(false);
            A.CallTo(() => _invoicesService.CheckIfSellingPriceExist(invoiceId)).Returns(false);
            A.CallTo(() => _invoicesService.GetInvoiceUser(invoiceId));
            A.CallTo(() => _invoicesService.GetInvoiceNumber(invoiceId));
            A.CallTo(() => _invoicesService.DeleteInvoice(invoiceId)).Returns(true);
            A.CallTo(() => _logServices.getLogTypeId("Delete")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", userId, logTypeId));
            A.CallTo(() => _userServices.GetUserFullName(userId)).Returns(userFull);
            A.CallTo(() => _notificationServices.CreateNotification(A<CreateNotification>.Ignored));
            var controller = new InvoicesController(_invoicesService, _userServices, _organizationServices, _itemServices, _logServices, _notificationServices);

            //Act
            var result = await controller.DeleteInvoice(invoiceId, userId, isYourInvoice);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundObjectResult>();
        }
        [Fact]
        public async Task InvoicesController_DeleteInvoice_InvoiceNotFound_Return404()
        {
            //Arrange
            int invoiceId = 1;
            int userId = 1;
            bool isYourInvoice = true;
            int logTypeId = 1;
            string userFull = "";
            A.CallTo(() => _userServices.UserExist(userId)).Returns(true);
            A.CallTo(() => _invoicesService.InvoiceExist(invoiceId)).Returns(false);
            A.CallTo(() => _invoicesService.CheckIfCreditNoteExist(invoiceId)).Returns(false);
            A.CallTo(() => _invoicesService.CheckIfSellingPriceExist(invoiceId)).Returns(false);
            A.CallTo(() => _invoicesService.GetInvoiceUser(invoiceId));
            A.CallTo(() => _invoicesService.GetInvoiceNumber(invoiceId));
            A.CallTo(() => _invoicesService.DeleteInvoice(invoiceId)).Returns(true);
            A.CallTo(() => _logServices.getLogTypeId("Delete")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", userId, logTypeId));
            A.CallTo(() => _userServices.GetUserFullName(userId)).Returns(userFull);
            A.CallTo(() => _notificationServices.CreateNotification(A<CreateNotification>.Ignored));
            var controller = new InvoicesController(_invoicesService, _userServices, _organizationServices, _itemServices, _logServices, _notificationServices);

            //Act
            var result = await controller.DeleteInvoice(invoiceId, userId, isYourInvoice);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundObjectResult>();
        }
        [Fact]
        public async Task InvoicesController_DeleteInvoice_CreditNoteExist_Return400()
        {
            //Arrange
            int invoiceId = 1;
            int userId = 1;
            bool isYourInvoice = true;
            int logTypeId = 1;
            string userFull = "";
            A.CallTo(() => _userServices.UserExist(userId)).Returns(true);
            A.CallTo(() => _invoicesService.InvoiceExist(invoiceId)).Returns(true);
            A.CallTo(() => _invoicesService.CheckIfCreditNoteExist(invoiceId)).Returns(true);
            A.CallTo(() => _invoicesService.CheckIfSellingPriceExist(invoiceId)).Returns(false);
            A.CallTo(() => _invoicesService.GetInvoiceUser(invoiceId));
            A.CallTo(() => _invoicesService.GetInvoiceNumber(invoiceId));
            A.CallTo(() => _invoicesService.DeleteInvoice(invoiceId)).Returns(true);
            A.CallTo(() => _logServices.getLogTypeId("Delete")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", userId, logTypeId));
            A.CallTo(() => _userServices.GetUserFullName(userId)).Returns(userFull);
            A.CallTo(() => _notificationServices.CreateNotification(A<CreateNotification>.Ignored));
            var controller = new InvoicesController(_invoicesService, _userServices, _organizationServices, _itemServices, _logServices, _notificationServices);

            //Act
            var result = await controller.DeleteInvoice(invoiceId, userId, isYourInvoice);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<BadRequestObjectResult>();
        }
        [Fact]
        public async Task InvoicesController_DeleteInvoice_InvoiceItemHaveRelations_Return400()
        {
            //Arrange
            int invoiceId = 1;
            int userId = 1;
            bool isYourInvoice = true;
            int logTypeId = 1;
            string userFull = "";
            A.CallTo(() => _userServices.UserExist(userId)).Returns(true);
            A.CallTo(() => _invoicesService.InvoiceExist(invoiceId)).Returns(true);
            A.CallTo(() => _invoicesService.CheckIfCreditNoteExist(invoiceId)).Returns(true);
            A.CallTo(() => _invoicesService.CheckIfSellingPriceExist(invoiceId)).Returns(false);
            A.CallTo(() => _invoicesService.GetInvoiceUser(invoiceId));
            A.CallTo(() => _invoicesService.GetInvoiceNumber(invoiceId));
            A.CallTo(() => _invoicesService.DeleteInvoice(invoiceId)).Returns(true);
            A.CallTo(() => _logServices.getLogTypeId("Delete")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", userId, logTypeId));
            A.CallTo(() => _userServices.GetUserFullName(userId)).Returns(userFull);
            A.CallTo(() => _notificationServices.CreateNotification(A<CreateNotification>.Ignored));
            var controller = new InvoicesController(_invoicesService, _userServices, _organizationServices, _itemServices, _logServices, _notificationServices);

            //Act
            var result = await controller.DeleteInvoice(invoiceId, userId, isYourInvoice);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<BadRequestObjectResult>();
        }
        [Fact]
        public async Task InvoicesController_GetInvoicePath_ReturnOk()
        {
            //Arrange
            var data = "";
            int invoiceId = 1;
            A.CallTo(() => _invoicesService.InvoiceExist(invoiceId)).Returns(true);
            A.CallTo(() => _invoicesService.GetInvoicePath(invoiceId)).Returns(data);
            var controller = new InvoicesController(_invoicesService, _userServices, _organizationServices, _itemServices, _logServices, _notificationServices);

            //Act
            var result = await controller.GetInvoicePath(invoiceId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();
        }
        [Fact]
        public async Task InvoicesController_GetInvoicePath_InvoiceNotFound_Return404()
        {
            //Arrange
            var data = "";
            int invoiceId = 1;
            A.CallTo(() => _invoicesService.InvoiceExist(invoiceId)).Returns(false);
            A.CallTo(() => _invoicesService.GetInvoicePath(invoiceId)).Returns(data);
            var controller = new InvoicesController(_invoicesService, _userServices, _organizationServices, _itemServices, _logServices, _notificationServices);

            //Act
            var result = await controller.GetInvoicePath(invoiceId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundObjectResult>();
        }
        [Fact]
        public async Task InvoicesController_GetRestPurchaseInvoice_ReturnOk()
        {
            //Arrange
            var data = A.Fake<GetRestInvoice>();
            int invoiceId = 1;
            A.CallTo(() => _invoicesService.InvoiceExist(invoiceId)).Returns(true);
            A.CallTo(() => _invoicesService.GetRestPurchaseInvoice(invoiceId)).Returns(data);
            var controller = new InvoicesController(_invoicesService, _userServices, _organizationServices, _itemServices, _logServices, _notificationServices);

            //Act
            var result = await controller.GetRestPurchaseInvoice(invoiceId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();
        }
        [Fact]
        public async Task InvoicesController_GetRestPurchaseInvoice_InvoiceNotFound_Return404()
        {
            //Arrange
            var data = A.Fake<GetRestInvoice>();
            int invoiceId = 1;
            A.CallTo(() => _invoicesService.InvoiceExist(invoiceId)).Returns(false);
            A.CallTo(() => _invoicesService.GetRestPurchaseInvoice(invoiceId)).Returns(data);
            var controller = new InvoicesController(_invoicesService, _userServices, _organizationServices, _itemServices, _logServices, _notificationServices);

            //Act
            var result = await controller.GetRestPurchaseInvoice(invoiceId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundObjectResult>();
        }
        [Fact]
        public async Task InvoicesController_GetRestSalesInvoice_ReturnOk()
        {
            //Arrange
            var data = A.Fake<GetRestInvoice>();
            int invoiceId = 1;
            A.CallTo(() => _invoicesService.InvoiceExist(invoiceId)).Returns(true);
            A.CallTo(() => _invoicesService.GetRestSalesInvoice(invoiceId)).Returns(data);
            var controller = new InvoicesController(_invoicesService, _userServices, _organizationServices, _itemServices, _logServices, _notificationServices);

            //Act
            var result = await controller.GetRestSalesInvoice(invoiceId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();
        }
        [Fact]
        public async Task InvoicesController_GetRestSalesInvoice_InvoiceNotFound_Return404()
        {
            //Arrange
            var data = A.Fake<GetRestInvoice>();
            int invoiceId = 1;
            A.CallTo(() => _invoicesService.InvoiceExist(invoiceId)).Returns(false);
            A.CallTo(() => _invoicesService.GetRestSalesInvoice(invoiceId)).Returns(data);
            var controller = new InvoicesController(_invoicesService, _userServices, _organizationServices, _itemServices, _logServices, _notificationServices);

            //Act
            var result = await controller.GetRestSalesInvoice(invoiceId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundObjectResult>();
        }
        [Fact]
        public async Task InvoicesController_GetRestModifyInvoice_ReturnOk()
        {
            //Arrange
            var data = A.Fake<GetRestModifyInvoice>();
            int invoiceId = 1;
            A.CallTo(() => _invoicesService.InvoiceExist(invoiceId)).Returns(true);
            A.CallTo(() => _invoicesService.GetRestModifyInvoice(invoiceId)).Returns(data);
            var controller = new InvoicesController(_invoicesService, _userServices, _organizationServices, _itemServices, _logServices, _notificationServices);

            //Act
            var result = await controller.GetRestModifyInvoice(invoiceId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();
        }
        [Fact]
        public async Task InvoicesController_GetRestModifyInvoice_InvoiceNotFound_Return404()
        {
            //Arrange
            var data = A.Fake<GetRestModifyInvoice>();
            int invoiceId = 1;
            A.CallTo(() => _invoicesService.InvoiceExist(invoiceId)).Returns(false);
            A.CallTo(() => _invoicesService.GetRestModifyInvoice(invoiceId)).Returns(data);
            var controller = new InvoicesController(_invoicesService, _userServices, _organizationServices, _itemServices, _logServices, _notificationServices);

            //Act
            var result = await controller.GetRestModifyInvoice(invoiceId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundObjectResult>();
        }
        [Fact]
        public async Task InvoicesControllerModifyInvoice_ReturnOk()
        {
            //Arrange
            var data = A.Fake<ModifyInvoice>();
            int userId = 1;
            var users = new List<int>
            {
                2
            };
            int logTypeId = 1;
            string userFull = "";
            A.CallTo(() => _userServices.UserExist(userId)).Returns(true);
            A.CallTo(() => _invoicesService.InvoiceExist(data.InvoiceId)).Returns(true);
            A.CallTo(() => _invoicesService.ModifyInvoice(data)).Returns(true);
            A.CallTo(() => _invoicesService.GetInvoiceUser(data.InvoiceId)).Returns(users);
            A.CallTo(() => _invoicesService.GetInvoiceNumber(data.InvoiceId));
            A.CallTo(() => _logServices.getLogTypeId("Modify")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", userId, logTypeId));
            A.CallTo(() => _userServices.GetUserFullName(A<int>._)).Returns(userFull);
            A.CallTo(() => _notificationServices.CreateNotification(A<CreateNotification>.Ignored));
            var controller = new InvoicesController(_invoicesService, _userServices, _organizationServices, _itemServices, _logServices, _notificationServices);

            //Act
            var result = await controller.ModifyInvoice(data, userId);

            //Assert
            A.CallTo(() => _userServices.GetUserFullName(A<int>._)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _notificationServices.CreateNotification(A<CreateNotification>.Ignored)).MustHaveHappenedOnceExactly();
            result.Should().NotBeNull();
            result.Should().BeOfType<OkResult>();
        }
        [Fact]
        public async Task InvoicesControllerModifyInvoice_ModifyFails_Return500()
        {
            //Arrange
            var data = A.Fake<ModifyInvoice>();
            int userId = 1;
            var users = new List<int>
            {
                2
            };
            int logTypeId = 1;
            string userFull = "";
            A.CallTo(() => _userServices.UserExist(userId)).Returns(true);
            A.CallTo(() => _invoicesService.InvoiceExist(data.InvoiceId)).Returns(true);
            A.CallTo(() => _invoicesService.ModifyInvoice(data)).Returns(false);
            A.CallTo(() => _invoicesService.GetInvoiceUser(data.InvoiceId)).Returns(users);
            A.CallTo(() => _invoicesService.GetInvoiceNumber(data.InvoiceId));
            A.CallTo(() => _logServices.getLogTypeId("Modify")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", userId, logTypeId));
            A.CallTo(() => _userServices.GetUserFullName(A<int>._)).Returns(userFull);
            A.CallTo(() => _notificationServices.CreateNotification(A<CreateNotification>.Ignored));
            var controller = new InvoicesController(_invoicesService, _userServices, _organizationServices, _itemServices, _logServices, _notificationServices);

            //Act
            var result = await controller.ModifyInvoice(data, userId);

            //Assert
            A.CallTo(() => _userServices.GetUserFullName(A<int>._)).MustNotHaveHappened();
            A.CallTo(() => _notificationServices.CreateNotification(A<CreateNotification>.Ignored)).MustNotHaveHappened();
            result.Should().NotBeNull();
            result.Should().BeOfType<StatusCodeResult>();
        }
        [Fact]
        public async Task InvoicesControllerModifyInvoice_UserNotFound_Return404()
        {
            //Arrange
            var data = A.Fake<ModifyInvoice>();
            int userId = 1;
            var users = new List<int>
            {
                2
            };
            int logTypeId = 1;
            string userFull = "";
            A.CallTo(() => _userServices.UserExist(userId)).Returns(false);
            A.CallTo(() => _invoicesService.InvoiceExist(data.InvoiceId)).Returns(true);
            A.CallTo(() => _invoicesService.ModifyInvoice(data)).Returns(true);
            A.CallTo(() => _invoicesService.GetInvoiceUser(data.InvoiceId)).Returns(users);
            A.CallTo(() => _invoicesService.GetInvoiceNumber(data.InvoiceId));
            A.CallTo(() => _logServices.getLogTypeId("Modify")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", userId, logTypeId));
            A.CallTo(() => _userServices.GetUserFullName(A<int>._)).Returns(userFull);
            A.CallTo(() => _notificationServices.CreateNotification(A<CreateNotification>.Ignored));
            var controller = new InvoicesController(_invoicesService, _userServices, _organizationServices, _itemServices, _logServices, _notificationServices);

            //Act
            var result = await controller.ModifyInvoice(data, userId);

            //Assert
            A.CallTo(() => _userServices.GetUserFullName(A<int>._)).MustNotHaveHappened();
            A.CallTo(() => _notificationServices.CreateNotification(A<CreateNotification>.Ignored)).MustNotHaveHappened();
            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundObjectResult>();
        }
        [Fact]
        public async Task InvoicesControllerModifyInvoice_InvoiceNotFound_Return404()
        {
            //Arrange
            var data = A.Fake<ModifyInvoice>();
            int userId = 1;
            var users = new List<int>
            {
                2
            };
            int logTypeId = 1;
            string userFull = "";
            A.CallTo(() => _userServices.UserExist(userId)).Returns(true);
            A.CallTo(() => _invoicesService.InvoiceExist(data.InvoiceId)).Returns(false);
            A.CallTo(() => _invoicesService.ModifyInvoice(data)).Returns(true);
            A.CallTo(() => _invoicesService.GetInvoiceUser(data.InvoiceId)).Returns(users);
            A.CallTo(() => _invoicesService.GetInvoiceNumber(data.InvoiceId));
            A.CallTo(() => _logServices.getLogTypeId("Modify")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", userId, logTypeId));
            A.CallTo(() => _userServices.GetUserFullName(A<int>._)).Returns(userFull);
            A.CallTo(() => _notificationServices.CreateNotification(A<CreateNotification>.Ignored));
            var controller = new InvoicesController(_invoicesService, _userServices, _organizationServices, _itemServices, _logServices, _notificationServices);

            //Act
            var result = await controller.ModifyInvoice(data, userId);

            //Assert
            A.CallTo(() => _userServices.GetUserFullName(A<int>._)).MustNotHaveHappened();
            A.CallTo(() => _notificationServices.CreateNotification(A<CreateNotification>.Ignored)).MustNotHaveHappened();
            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundObjectResult>();
        }
        [Fact]
        public async Task InvoicesController_UpdateStatus_ReturnOk()
        {
            //Arrange
            A.CallTo(() => _invoicesService.UpdateInvoiceStatus()).Returns(true);
            var controller = new InvoicesController(_invoicesService, _userServices, _organizationServices, _itemServices, _logServices, _notificationServices);

            //Act
            var result = await controller.UpdateStatus();

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkResult>();
        }
        [Fact]
        public async Task InvoicesController_UpdateStatus_UpdateFails_Return500()
        {
            //Arrange
            A.CallTo(() => _invoicesService.UpdateInvoiceStatus()).Returns(false);
            var controller = new InvoicesController(_invoicesService, _userServices, _organizationServices, _itemServices, _logServices, _notificationServices);

            //Act
            var result = await controller.UpdateStatus();

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<StatusCodeResult>();
        }
    }
}
