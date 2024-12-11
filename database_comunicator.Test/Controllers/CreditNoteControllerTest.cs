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
    public class CreditNoteControllerTest
    {
        private readonly ICreditNoteServices _creditNoteServices;
        private readonly IUserServices _userServices;
        private readonly ILogServices _logServices;
        private readonly INotificationServices _notificationServices;
        public CreditNoteControllerTest()
        {
            _creditNoteServices = A.Fake<ICreditNoteServices>();
            _userServices = A.Fake<IUserServices>();
            _logServices = A.Fake<ILogServices>();
            _notificationServices = A.Fake<INotificationServices>();
        }
        [Fact]
        public async Task CreditNoteController_AddCreditNote_ReturnOk()
        {
            //Arrange
            var data = A.Dummy<AddCreditNote>();
            data.CreditNoteItems = A.CollectionOfDummy<NewCreditNoteItems>(2);
            int userId = 1;
            int addResult = 1;
            int logTypeId = 1;
            string userFull = "";
            var newNotification = A.Fake<CreateNotification>();
            A.CallTo(() => _userServices.UserExist(userId)).Returns(true);
            A.CallTo(() => _creditNoteServices.CreditNoteExist(data.CreditNoteNumber, data.InvoiceId)).Returns(false);
            A.CallTo(() => _creditNoteServices.CreditDeductionCanBeApplied(A<int>.Ignored, A<int>.Ignored, A<int>.Ignored, A<int>.Ignored)).Returns(true);
            A.CallTo(() => _creditNoteServices.AddCreditNote(data)).Returns(addResult);
            A.CallTo(() => _logServices.getLogTypeId("Create")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", userId, logTypeId));
            A.CallTo(() => _userServices.GetUserFullName(userId)).Returns(userFull);
            A.CallTo(() => _notificationServices.CreateNotification(newNotification));
            var controller = new CreditNoteController(_creditNoteServices, _userServices, _logServices, _notificationServices);

            //Act
            var result = await controller.AddCreditNote(data, userId);

            //Assert
            A.CallTo(() => _creditNoteServices.CreditDeductionCanBeApplied(A<int>.Ignored, A<int>.Ignored, A<int>.Ignored, A<int>.Ignored)).MustHaveHappenedTwiceExactly();
            result.Should().NotBeNull();
            result.Should().BeOfType<OkResult>();
        }
        [Fact]
        public async Task CreditNoteController_AddCreditNote_AddFailed_Return500()
        {
            //Arrange
            var data = A.Dummy<AddCreditNote>();
            data.CreditNoteItems = A.CollectionOfDummy<NewCreditNoteItems>(2);
            int userId = 1;
            int addResult = 0;
            int logTypeId = 1;
            string userFull = "";
            var newNotification = A.Fake<CreateNotification>();
            A.CallTo(() => _userServices.UserExist(userId)).Returns(true);
            A.CallTo(() => _creditNoteServices.CreditNoteExist(data.CreditNoteNumber, data.InvoiceId)).Returns(false);
            A.CallTo(() => _creditNoteServices.CreditDeductionCanBeApplied(A<int>.Ignored, A<int>.Ignored, A<int>.Ignored, A<int>.Ignored)).Returns(true);
            A.CallTo(() => _creditNoteServices.AddCreditNote(data)).Returns(addResult);
            A.CallTo(() => _logServices.getLogTypeId("Create")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", userId, logTypeId));
            A.CallTo(() => _userServices.GetUserFullName(userId)).Returns(userFull);
            A.CallTo(() => _notificationServices.CreateNotification(newNotification));
            var controller = new CreditNoteController(_creditNoteServices, _userServices, _logServices, _notificationServices);

            //Act
            var result = await controller.AddCreditNote(data, userId);

            //Assert
            A.CallTo(() => _creditNoteServices.CreditDeductionCanBeApplied(A<int>.Ignored, A<int>.Ignored, A<int>.Ignored, A<int>.Ignored)).MustHaveHappenedTwiceExactly();
            result.Should().NotBeNull();
            result.Should().BeOfType<StatusCodeResult>();
        }
        [Fact]
        public async Task CreditNoteController_AddCreditNote_UserNotFound_Return404()
        {
            //Arrange
            var data = A.Dummy<AddCreditNote>();
            data.CreditNoteItems = A.CollectionOfDummy<NewCreditNoteItems>(2);
            int userId = 1;
            int addResult = 1;
            int logTypeId = 1;
            string userFull = "";
            var newNotification = A.Fake<CreateNotification>();
            A.CallTo(() => _userServices.UserExist(userId)).Returns(false);
            A.CallTo(() => _creditNoteServices.CreditNoteExist(data.CreditNoteNumber, data.InvoiceId)).Returns(false);
            A.CallTo(() => _creditNoteServices.CreditDeductionCanBeApplied(A<int>.Ignored, A<int>.Ignored, A<int>.Ignored, A<int>.Ignored)).Returns(true);
            A.CallTo(() => _creditNoteServices.AddCreditNote(data)).Returns(addResult);
            A.CallTo(() => _logServices.getLogTypeId("Create")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", userId, logTypeId));
            A.CallTo(() => _userServices.GetUserFullName(userId)).Returns(userFull);
            A.CallTo(() => _notificationServices.CreateNotification(newNotification));
            var controller = new CreditNoteController(_creditNoteServices, _userServices, _logServices, _notificationServices);

            //Act
            var result = await controller.AddCreditNote(data, userId);

            //Assert
            A.CallTo(() => _creditNoteServices.CreditDeductionCanBeApplied(A<int>.Ignored, A<int>.Ignored, A<int>.Ignored, A<int>.Ignored)).MustNotHaveHappened();
            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundObjectResult>();
        }
        [Fact]
        public async Task CreditNoteController_AddCreditNote_CreditAlreadyExist_Return400()
        {
            //Arrange
            var data = A.Dummy<AddCreditNote>();
            data.CreditNoteItems = A.CollectionOfDummy<NewCreditNoteItems>(2);
            int userId = 1;
            int addResult = 1;
            int logTypeId = 1;
            string userFull = "";
            var newNotification = A.Fake<CreateNotification>();
            A.CallTo(() => _userServices.UserExist(userId)).Returns(true);
            A.CallTo(() => _creditNoteServices.CreditNoteExist(data.CreditNoteNumber, data.InvoiceId)).Returns(true);
            A.CallTo(() => _creditNoteServices.CreditDeductionCanBeApplied(A<int>.Ignored, A<int>.Ignored, A<int>.Ignored, A<int>.Ignored)).Returns(true);
            A.CallTo(() => _creditNoteServices.AddCreditNote(data)).Returns(addResult);
            A.CallTo(() => _logServices.getLogTypeId("Create")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", userId, logTypeId));
            A.CallTo(() => _userServices.GetUserFullName(userId)).Returns(userFull);
            A.CallTo(() => _notificationServices.CreateNotification(newNotification));
            var controller = new CreditNoteController(_creditNoteServices, _userServices, _logServices, _notificationServices);

            //Act
            var result = await controller.AddCreditNote(data, userId);

            //Assert
            A.CallTo(() => _creditNoteServices.CreditDeductionCanBeApplied(A<int>.Ignored, A<int>.Ignored, A<int>.Ignored, A<int>.Ignored)).MustNotHaveHappened();
            result.Should().NotBeNull();
            result.Should().BeOfType<BadRequestObjectResult>();
        }
        [Fact]
        public async Task CreditNoteController_AddCreditNote_AtLeastOneItemCantBeDeduced_Return400()
        {
            //Arrange
            var data = A.Dummy<AddCreditNote>();
            data.CreditNoteItems = A.CollectionOfDummy<NewCreditNoteItems>(1);
            int userId = 1;
            int addResult = 1;
            int logTypeId = 1;
            string userFull = "";
            var newNotification = A.Fake<CreateNotification>();
            A.CallTo(() => _userServices.UserExist(userId)).Returns(true);
            A.CallTo(() => _creditNoteServices.CreditNoteExist(data.CreditNoteNumber, data.InvoiceId)).Returns(false);
            A.CallTo(() => _creditNoteServices.CreditDeductionCanBeApplied(A<int>.Ignored, A<int>.Ignored, A<int>.Ignored, A<int>.Ignored)).Returns(false);
            A.CallTo(() => _creditNoteServices.AddCreditNote(data)).Returns(addResult);
            A.CallTo(() => _logServices.getLogTypeId("Create")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", userId, logTypeId));
            A.CallTo(() => _userServices.GetUserFullName(userId)).Returns(userFull);
            A.CallTo(() => _notificationServices.CreateNotification(newNotification));
            var controller = new CreditNoteController(_creditNoteServices, _userServices, _logServices, _notificationServices);

            //Act
            var result = await controller.AddCreditNote(data, userId);

            //Assert
            A.CallTo(() => _creditNoteServices.CreditDeductionCanBeApplied(A<int>.Ignored, A<int>.Ignored, A<int>.Ignored, A<int>.Ignored)).MustHaveHappenedOnceExactly();
            result.Should().NotBeNull();
            result.Should().BeOfType<BadRequestObjectResult>();
        }
        [Fact]
        public async Task CreditNoteController_GetUserCreditNote_ReturnOk()
        {
            //Arrange
            var data = A.Fake<IEnumerable<GetCreditNote>>();
            bool isYourCredit = true;
            int userId = 1;
            var filters = A.Fake<CreditNoteFiltersTemplate>();
            A.CallTo(() => _userServices.UserExist(userId)).Returns(true);
            A.CallTo(() => _creditNoteServices.GetCreditNotes(isYourCredit, userId, null, filters)).Returns(data);
            var controller = new CreditNoteController(_creditNoteServices, _userServices, _logServices, _notificationServices);

            //Act
            var result = await controller.GetUserCreditNote(isYourCredit, userId, null, null, null, null, null, null, null, null, null, null, null, null);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();
        }
        [Fact]
        public async Task CreditNoteController_GetUserCreditNote_SortIncorrect_Return400()
        {
            //Arrange
            var data = A.Fake<IEnumerable<GetCreditNote>>();
            bool isYourCredit = true;
            int userId = 1;
            var filters = A.Fake<CreditNoteFiltersTemplate>();
            A.CallTo(() => _userServices.UserExist(userId)).Returns(true);
            A.CallTo(() => _creditNoteServices.GetCreditNotes(isYourCredit, userId, null, filters)).Returns(data);
            var controller = new CreditNoteController(_creditNoteServices, _userServices, _logServices, _notificationServices);

            //Act
            var result = await controller.GetUserCreditNote(isYourCredit, userId, null, "", null, null, null, null, null, null, null, null, null, null);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<BadRequestObjectResult>();
        }
        [Fact]
        public async Task CreditNoteController_GetUserCreditNote_UserNotFound_Return404()
        {
            //Arrange
            var data = A.Fake<IEnumerable<GetCreditNote>>();
            bool isYourCredit = true;
            int userId = 1;
            var filters = A.Fake<CreditNoteFiltersTemplate>();
            A.CallTo(() => _userServices.UserExist(userId)).Returns(false);
            A.CallTo(() => _creditNoteServices.GetCreditNotes(isYourCredit, userId, null, filters)).Returns(data);
            var controller = new CreditNoteController(_creditNoteServices, _userServices, _logServices, _notificationServices);

            //Act
            var result = await controller.GetUserCreditNote(isYourCredit, userId, null, null, null, null, null, null, null, null, null, null, null, null);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundObjectResult>();
        }
        [Fact]
        public async Task CreditNoteController_GetCreditNote_ReturnOk()
        {
            //Arrange
            var data = A.Fake<IEnumerable<GetCreditNote>>();
            bool isYourCredit = true;
            var filters = A.Fake<CreditNoteFiltersTemplate>();
            A.CallTo(() => _creditNoteServices.GetCreditNotes(isYourCredit, null, filters)).Returns(data);
            var controller = new CreditNoteController(_creditNoteServices, _userServices, _logServices, _notificationServices);

            //Act
            var result = await controller.GetCreditNote(isYourCredit, null, null, null, null, null, null, null, null, null, null, null, null);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();
        }
        [Fact]
        public async Task CreditNoteController_GetCreditNote_SortIncorrect_Return400()
        {
            //Arrange
            var data = A.Fake<IEnumerable<GetCreditNote>>();
            bool isYourCredit = true;
            var filters = A.Fake<CreditNoteFiltersTemplate>();
            A.CallTo(() => _creditNoteServices.GetCreditNotes(isYourCredit, null, filters)).Returns(data);
            var controller = new CreditNoteController(_creditNoteServices, _userServices, _logServices, _notificationServices);

            //Act
            var result = await controller.GetCreditNote(isYourCredit, null, "", null, null, null, null, null, null, null, null, null, null);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<BadRequestObjectResult>();
        }
        [Fact]
        public async Task CreditNoteController_GetRestCreditNote_ReturnOk()
        {
            //Arrange
            var data = A.Fake<GetRestCreditNote>();
            int creditId = 1;
            A.CallTo(() => _creditNoteServices.CreditNoteExist(creditId)).Returns(true);
            A.CallTo(() => _creditNoteServices.GetRestCreditNote(creditId)).Returns(data);
            var controller = new CreditNoteController(_creditNoteServices, _userServices, _logServices, _notificationServices);

            //Act
            var result = await controller.GetRestCreditNote(creditId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();
        }
        [Fact]
        public async Task CreditNoteController_GetRestCreditNote_CreditNoteNotFound_Return404()
        {
            //Arrange
            var data = A.Fake<GetRestCreditNote>();
            int creditId = 1;
            A.CallTo(() => _creditNoteServices.CreditNoteExist(creditId)).Returns(false);
            A.CallTo(() => _creditNoteServices.GetRestCreditNote(creditId)).Returns(data);
            var controller = new CreditNoteController(_creditNoteServices, _userServices, _logServices, _notificationServices);

            //Act
            var result = await controller.GetRestCreditNote(creditId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundResult>();
        }
        [Fact]
        public async Task CreditNoteController_GetCreditFilePath_ReturnOk()
        {
            //Arrange
            var data = "";
            int creditId = 1;
            A.CallTo(() => _creditNoteServices.CreditNoteExist(creditId)).Returns(true);
            A.CallTo(() => _creditNoteServices.GetCreditFilePath(creditId)).Returns(data);
            var controller = new CreditNoteController(_creditNoteServices, _userServices, _logServices, _notificationServices);

            //Act
            var result = await controller.GetCreditFilePath(creditId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();
        }
        [Fact]
        public async Task CreditNoteController_GetCreditFilePath_CreditNoteNotFound_Return404()
        {
            //Arrange
            var data = "";
            int creditId = 1;
            A.CallTo(() => _creditNoteServices.CreditNoteExist(creditId)).Returns(false);
            A.CallTo(() => _creditNoteServices.GetCreditFilePath(creditId)).Returns(data);
            var controller = new CreditNoteController(_creditNoteServices, _userServices, _logServices, _notificationServices);

            //Act
            var result = await controller.GetCreditFilePath(creditId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundResult>();
        }
        [Fact]
        public async Task CreditNoteController_DeleteCreditNote_ReturnOk()
        {
            //Arrange
            int creditId = 1;
            int userId = 1;
            bool isYourCredit = true;
            int creditUser = 1;
            string creditNumber = "";
            int logTypeId = 1;
            string userFull = "";
            var newNotification = A.Fake<CreateNotification>();
            A.CallTo(() => _userServices.UserExist(userId)).Returns(true);
            A.CallTo(() => _creditNoteServices.CreditNoteExist(creditId)).Returns(true);
            A.CallTo(() => _creditNoteServices.GetCreditNoteUser(creditId)).Returns(creditUser);
            A.CallTo(() => _creditNoteServices.GetCreditNumber(creditId)).Returns(creditNumber);
            A.CallTo(() => _creditNoteServices.DeleteCreditNote(creditId)).Returns(true);
            A.CallTo(() => _logServices.getLogTypeId("Delete")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", userId, logTypeId));
            A.CallTo(() => _userServices.GetUserFullName(userId)).Returns(userFull);
            A.CallTo(() => _notificationServices.CreateNotification(newNotification));
            var controller = new CreditNoteController(_creditNoteServices, _userServices, _logServices, _notificationServices);

            //Act
            var result = await controller.DeleteCreditNote(creditId, userId, isYourCredit);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();
        }
        [Fact]
        public async Task CreditNoteController_DeleteCreditNote_DeleteFails_Return500()
        {
            //Arrange
            int creditId = 1;
            int userId = 1;
            bool isYourCredit = true;
            int creditUser = 1;
            string creditNumber = "";
            int logTypeId = 1;
            string userFull = "";
            var newNotification = A.Fake<CreateNotification>();
            A.CallTo(() => _userServices.UserExist(userId)).Returns(true);
            A.CallTo(() => _creditNoteServices.CreditNoteExist(creditId)).Returns(true);
            A.CallTo(() => _creditNoteServices.GetCreditNoteUser(creditId)).Returns(creditUser);
            A.CallTo(() => _creditNoteServices.GetCreditNumber(creditId)).Returns(creditNumber);
            A.CallTo(() => _creditNoteServices.DeleteCreditNote(creditId)).Returns(false);
            A.CallTo(() => _logServices.getLogTypeId("Delete")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", userId, logTypeId));
            A.CallTo(() => _userServices.GetUserFullName(userId)).Returns(userFull);
            A.CallTo(() => _notificationServices.CreateNotification(newNotification));
            var controller = new CreditNoteController(_creditNoteServices, _userServices, _logServices, _notificationServices);

            //Act
            var result = await controller.DeleteCreditNote(creditId, userId, isYourCredit);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<StatusCodeResult>();
        }
        [Fact]
        public async Task CreditNoteController_DeleteCreditNote_UserNotFound_Return404()
        {
            //Arrange
            int creditId = 1;
            int userId = 1;
            bool isYourCredit = true;
            int creditUser = 1;
            string creditNumber = "";
            int logTypeId = 1;
            string userFull = "";
            var newNotification = A.Fake<CreateNotification>();
            A.CallTo(() => _userServices.UserExist(userId)).Returns(false);
            A.CallTo(() => _creditNoteServices.CreditNoteExist(creditId)).Returns(true);
            A.CallTo(() => _creditNoteServices.GetCreditNoteUser(creditId)).Returns(creditUser);
            A.CallTo(() => _creditNoteServices.GetCreditNumber(creditId)).Returns(creditNumber);
            A.CallTo(() => _creditNoteServices.DeleteCreditNote(creditId)).Returns(true);
            A.CallTo(() => _logServices.getLogTypeId("Delete")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", userId, logTypeId));
            A.CallTo(() => _userServices.GetUserFullName(userId)).Returns(userFull);
            A.CallTo(() => _notificationServices.CreateNotification(newNotification));
            var controller = new CreditNoteController(_creditNoteServices, _userServices, _logServices, _notificationServices);

            //Act
            var result = await controller.DeleteCreditNote(creditId, userId, isYourCredit);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundObjectResult>();
        }
        [Fact]
        public async Task CreditNoteController_DeleteCreditNote_CreditNoteNotFound_Return404()
        {
            //Arrange
            int creditId = 1;
            int userId = 1;
            bool isYourCredit = true;
            int creditUser = 1;
            string creditNumber = "";
            int logTypeId = 1;
            string userFull = "";
            var newNotification = A.Fake<CreateNotification>();
            A.CallTo(() => _userServices.UserExist(userId)).Returns(true);
            A.CallTo(() => _creditNoteServices.CreditNoteExist(creditId)).Returns(false);
            A.CallTo(() => _creditNoteServices.GetCreditNoteUser(creditId)).Returns(creditUser);
            A.CallTo(() => _creditNoteServices.GetCreditNumber(creditId)).Returns(creditNumber);
            A.CallTo(() => _creditNoteServices.DeleteCreditNote(creditId)).Returns(true);
            A.CallTo(() => _logServices.getLogTypeId("Delete")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", userId, logTypeId));
            A.CallTo(() => _userServices.GetUserFullName(userId)).Returns(userFull);
            A.CallTo(() => _notificationServices.CreateNotification(newNotification));
            var controller = new CreditNoteController(_creditNoteServices, _userServices, _logServices, _notificationServices);

            //Act
            var result = await controller.DeleteCreditNote(creditId, userId, isYourCredit);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundObjectResult>();
        }
        [Fact]
        public async Task CreditNoteController_GetRestModifyCredit_ReturnOk()
        {
            //Arrange
            var data = A.Fake<GetRestModifyCredit>();
            int creditId = 1;
            bool isYourCredit = true;
            A.CallTo(() => _creditNoteServices.CreditNoteExist(creditId)).Returns(true);
            A.CallTo(() => _creditNoteServices.GetRestModifyCredit(creditId, isYourCredit)).Returns(data);
            var controller = new CreditNoteController(_creditNoteServices, _userServices, _logServices, _notificationServices);

            //Act
            var result = await controller.GetRestModifyCredit(creditId, isYourCredit);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();
        }
        [Fact]
        public async Task CreditNoteController_GetRestModifyCredit_CreditNoteNotFound_Return404()
        {
            //Arrange
            var data = A.Fake<GetRestModifyCredit>();
            int creditId = 1;
            bool isYourCredit = true;
            A.CallTo(() => _creditNoteServices.CreditNoteExist(creditId)).Returns(false);
            A.CallTo(() => _creditNoteServices.GetRestModifyCredit(creditId, isYourCredit)).Returns(data);
            var controller = new CreditNoteController(_creditNoteServices, _userServices, _logServices, _notificationServices);

            //Act
            var result = await controller.GetRestModifyCredit(creditId, isYourCredit);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundResult>();
        }
        [Fact]
        public async Task CreditNoteController_ModifyCreditNote_ReturnOk()
        {
            //Arrange
            var data = A.Fake<ModifyCreditNote>();
            int userId = 1;
            int invoiceId = 1;
            int logTypeId = 1;
            string userFull = "";
            var newNotification = A.Fake<CreateNotification>();
            A.CallTo(() => _userServices.UserExist(userId)).Returns(true);
            A.CallTo(() => _creditNoteServices.CreditNoteExist(data.Id)).Returns(true);
            A.CallTo(() => _creditNoteServices.GetCreditNoteInvoiceId(data.Id)).Returns(invoiceId);
            A.CallTo(() => _creditNoteServices.CreditNoteExist(data.CreditNumber ?? "", invoiceId)).Returns(false);
            A.CallTo(() => _creditNoteServices.GetCreditNoteUser(data.Id));
            A.CallTo(() => _creditNoteServices.ModifyCreditNote(data)).Returns(true);
            A.CallTo(() => _creditNoteServices.GetCreditNumber(data.Id));
            A.CallTo(() => _logServices.getLogTypeId("Modify")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", userId, logTypeId));
            A.CallTo(() => _userServices.GetUserFullName(userId)).Returns(userFull);
            A.CallTo(() => _notificationServices.CreateNotification(newNotification));
            var controller = new CreditNoteController(_creditNoteServices, _userServices, _logServices, _notificationServices);

            //Act
            var result = await controller.ModifyCreditNote(data, userId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();
        }
        [Fact]
        public async Task CreditNoteController_ModifyCreditNote_ModifyFails_Return500()
        {
            //Arrange
            var data = A.Fake<ModifyCreditNote>();
            int userId = 1;
            int invoiceId = 1;
            int logTypeId = 1;
            string userFull = "";
            var newNotification = A.Fake<CreateNotification>();
            A.CallTo(() => _userServices.UserExist(userId)).Returns(true);
            A.CallTo(() => _creditNoteServices.CreditNoteExist(data.Id)).Returns(true);
            A.CallTo(() => _creditNoteServices.GetCreditNoteInvoiceId(data.Id)).Returns(invoiceId);
            A.CallTo(() => _creditNoteServices.CreditNoteExist(data.CreditNumber ?? "", invoiceId)).Returns(false);
            A.CallTo(() => _creditNoteServices.GetCreditNoteUser(data.Id));
            A.CallTo(() => _creditNoteServices.ModifyCreditNote(data)).Returns(false);
            A.CallTo(() => _creditNoteServices.GetCreditNumber(data.Id));
            A.CallTo(() => _logServices.getLogTypeId("Modify")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", userId, logTypeId));
            A.CallTo(() => _userServices.GetUserFullName(userId)).Returns(userFull);
            A.CallTo(() => _notificationServices.CreateNotification(newNotification));
            var controller = new CreditNoteController(_creditNoteServices, _userServices, _logServices, _notificationServices);

            //Act
            var result = await controller.ModifyCreditNote(data, userId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<StatusCodeResult>();
        }
        [Fact]
        public async Task CreditNoteController_ModifyCreditNote_UserNotFound_Return404()
        {
            //Arrange
            var data = A.Fake<ModifyCreditNote>();
            int userId = 1;
            int invoiceId = 1;
            int logTypeId = 1;
            string userFull = "";
            var newNotification = A.Fake<CreateNotification>();
            A.CallTo(() => _userServices.UserExist(userId)).Returns(false);
            A.CallTo(() => _creditNoteServices.CreditNoteExist(data.Id)).Returns(true);
            A.CallTo(() => _creditNoteServices.GetCreditNoteInvoiceId(data.Id)).Returns(invoiceId);
            A.CallTo(() => _creditNoteServices.CreditNoteExist(data.CreditNumber ?? "", invoiceId)).Returns(false);
            A.CallTo(() => _creditNoteServices.GetCreditNoteUser(data.Id));
            A.CallTo(() => _creditNoteServices.ModifyCreditNote(data)).Returns(true);
            A.CallTo(() => _creditNoteServices.GetCreditNumber(data.Id));
            A.CallTo(() => _logServices.getLogTypeId("Modify")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", userId, logTypeId));
            A.CallTo(() => _userServices.GetUserFullName(userId)).Returns(userFull);
            A.CallTo(() => _notificationServices.CreateNotification(newNotification));
            var controller = new CreditNoteController(_creditNoteServices, _userServices, _logServices, _notificationServices);

            //Act
            var result = await controller.ModifyCreditNote(data, userId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundObjectResult>();
        }
        [Fact]
        public async Task CreditNoteController_ModifyCreditNote_CreditNoteNotFound_Return404()
        {
            //Arrange
            var data = A.Fake<ModifyCreditNote>();
            int userId = 1;
            int invoiceId = 1;
            int logTypeId = 1;
            string userFull = "";
            var newNotification = A.Fake<CreateNotification>();
            A.CallTo(() => _userServices.UserExist(userId)).Returns(true);
            A.CallTo(() => _creditNoteServices.CreditNoteExist(data.Id)).Returns(false);
            A.CallTo(() => _creditNoteServices.GetCreditNoteInvoiceId(data.Id)).Returns(invoiceId);
            A.CallTo(() => _creditNoteServices.CreditNoteExist(data.CreditNumber ?? "", invoiceId)).Returns(false);
            A.CallTo(() => _creditNoteServices.GetCreditNoteUser(data.Id));
            A.CallTo(() => _creditNoteServices.ModifyCreditNote(data)).Returns(true);
            A.CallTo(() => _creditNoteServices.GetCreditNumber(data.Id));
            A.CallTo(() => _logServices.getLogTypeId("Modify")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", userId, logTypeId));
            A.CallTo(() => _userServices.GetUserFullName(userId)).Returns(userFull);
            A.CallTo(() => _notificationServices.CreateNotification(newNotification));
            var controller = new CreditNoteController(_creditNoteServices, _userServices, _logServices, _notificationServices);

            //Act
            var result = await controller.ModifyCreditNote(data, userId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundObjectResult>();
        }
        [Fact]
        public async Task CreditNoteController_ModifyCreditNote_NewCreditNumberAlreadyExist_Return400()
        {
            //Arrange
            var data = A.Fake<ModifyCreditNote>();
            data.CreditNumber = "";
            int userId = 1;
            int invoiceId = 1;
            int logTypeId = 1;
            string userFull = "";
            var newNotification = A.Fake<CreateNotification>();
            A.CallTo(() => _userServices.UserExist(userId)).Returns(true);
            A.CallTo(() => _creditNoteServices.CreditNoteExist(data.Id)).Returns(true);
            A.CallTo(() => _creditNoteServices.GetCreditNoteInvoiceId(data.Id)).Returns(invoiceId);
            A.CallTo(() => _creditNoteServices.CreditNoteExist(data.CreditNumber, invoiceId)).Returns(true);
            A.CallTo(() => _creditNoteServices.GetCreditNoteUser(data.Id));
            A.CallTo(() => _creditNoteServices.ModifyCreditNote(data)).Returns(true);
            A.CallTo(() => _creditNoteServices.GetCreditNumber(data.Id));
            A.CallTo(() => _logServices.getLogTypeId("Modify")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", userId, logTypeId));
            A.CallTo(() => _userServices.GetUserFullName(userId)).Returns(userFull);
            A.CallTo(() => _notificationServices.CreateNotification(newNotification));
            var controller = new CreditNoteController(_creditNoteServices, _userServices, _logServices, _notificationServices);

            //Act
            var result = await controller.ModifyCreditNote(data, userId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<BadRequestObjectResult>();
        }
    }
}
