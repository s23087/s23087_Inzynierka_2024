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
    public class OfferControllerTest
    {
        private readonly IOfferServices _offerServices;
        private readonly IUserServices _userServices;
        private readonly ILogServices _logServices;
        public OfferControllerTest()
        {
            _offerServices = A.Fake<IOfferServices>();
            _userServices = A.Fake<IUserServices>();
            _logServices = A.Fake<ILogServices>();
        }
        [Fact]
        public async Task OfferController_AddOffer_CsvFile_ReturnOk()
        {
            //Arrange
            var data = A.Fake<AddOffer>();
            data.Path = "tmp.csv";
            data.OfferStatusId = 1;
            int deactivatedId = 2;
            int newOfferId = 1;
            int logTypeId = 1;
            A.CallTo(() => _userServices.UserExist(data.UserId)).Returns(true);
            A.CallTo(() => _offerServices.DoesPricelistExist(data.OfferName, data.UserId)).Returns(false);
            A.CallTo(() => _offerServices.CreatePricelist(data)).Returns(newOfferId);
            A.CallTo(() => _offerServices.GetDeactivatedStatusId()).Returns(deactivatedId);
            A.CallTo(() => _offerServices.CreateCsvFile(newOfferId, data.UserId, data.Path, data.MaxQty)).Returns(true);
            A.CallTo(() => _logServices.getLogTypeId("Create")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", data.UserId, logTypeId));
            var controller = new OfferController(_offerServices, _userServices, _logServices);

            //Act
            var result = await controller.AddOffer(data);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkResult>();
        }
        [Fact]
        public async Task OfferController_AddOffer_XmlFile_ReturnOk()
        {
            //Arrange
            var data = A.Fake<AddOffer>();
            data.Path = "tmp.xlsx";
            data.OfferStatusId = 1;
            int deactivatedId = 2;
            int newOfferId = 1;
            int logTypeId = 1;
            A.CallTo(() => _userServices.UserExist(data.UserId)).Returns(true);
            A.CallTo(() => _offerServices.DoesPricelistExist(data.OfferName, data.UserId)).Returns(false);
            A.CallTo(() => _offerServices.CreatePricelist(data)).Returns(newOfferId);
            A.CallTo(() => _offerServices.GetDeactivatedStatusId()).Returns(deactivatedId);
            A.CallTo(() => _offerServices.CreateXmlFile(newOfferId, data.UserId, data.Path, data.MaxQty)).Returns(true);
            A.CallTo(() => _logServices.getLogTypeId("Create")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", data.UserId, logTypeId));
            var controller = new OfferController(_offerServices, _userServices, _logServices);

            //Act
            var result = await controller.AddOffer(data);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkResult>();
        }
        [Fact]
        public async Task OfferController_AddOffer_CsvFile_ReturnOkWithError()
        {
            //Arrange
            var data = A.Fake<AddOffer>();
            data.Path = "tmp.csv";
            data.OfferStatusId = 1;
            int deactivatedId = 2;
            int newOfferId = 1;
            int logTypeId = 1;
            A.CallTo(() => _userServices.UserExist(data.UserId)).Returns(true);
            A.CallTo(() => _offerServices.DoesPricelistExist(data.OfferName, data.UserId)).Returns(false);
            A.CallTo(() => _offerServices.CreatePricelist(data)).Returns(newOfferId);
            A.CallTo(() => _offerServices.GetDeactivatedStatusId()).Returns(deactivatedId);
            A.CallTo(() => _offerServices.CreateCsvFile(newOfferId, data.UserId, data.Path, data.MaxQty)).Returns(false);
            A.CallTo(() => _logServices.getLogTypeId("Create")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", data.UserId, logTypeId));
            var controller = new OfferController(_offerServices, _userServices, _logServices);

            //Act
            var result = await controller.AddOffer(data);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();
        }
        [Fact]
        public async Task OfferController_AddOffer_XmlFile_ReturnOkWithError()
        {
            //Arrange
            var data = A.Fake<AddOffer>();
            data.Path = "tmp.xlsx";
            data.OfferStatusId = 1;
            int deactivatedId = 2;
            int newOfferId = 1;
            int logTypeId = 1;
            A.CallTo(() => _userServices.UserExist(data.UserId)).Returns(true);
            A.CallTo(() => _offerServices.DoesPricelistExist(data.OfferName, data.UserId)).Returns(false);
            A.CallTo(() => _offerServices.CreatePricelist(data)).Returns(newOfferId);
            A.CallTo(() => _offerServices.GetDeactivatedStatusId()).Returns(deactivatedId);
            A.CallTo(() => _offerServices.CreateXmlFile(newOfferId, data.UserId, data.Path, data.MaxQty)).Returns(false);
            A.CallTo(() => _logServices.getLogTypeId("Create")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", data.UserId, logTypeId));
            var controller = new OfferController(_offerServices, _userServices, _logServices);

            //Act
            var result = await controller.AddOffer(data);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();
        }
        [Fact]
        public async Task OfferController_AddOffer_CsvFileDeactivated_ReturnOk()
        {
            //Arrange
            var data = A.Fake<AddOffer>();
            data.Path = "tmp.csv";
            data.OfferStatusId = 1;
            int deactivatedId = 1;
            int newOfferId = 1;
            int logTypeId = 1;
            A.CallTo(() => _userServices.UserExist(data.UserId)).Returns(true);
            A.CallTo(() => _offerServices.DoesPricelistExist(data.OfferName, data.UserId)).Returns(false);
            A.CallTo(() => _offerServices.CreatePricelist(data)).Returns(newOfferId);
            A.CallTo(() => _offerServices.GetDeactivatedStatusId()).Returns(deactivatedId);
            A.CallTo(() => _offerServices.CreateCsvFile(newOfferId, data.UserId, data.Path, data.MaxQty)).Returns(true);
            A.CallTo(() => _logServices.getLogTypeId("Create")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", data.UserId, logTypeId));
            var controller = new OfferController(_offerServices, _userServices, _logServices);

            //Act
            var result = await controller.AddOffer(data);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkResult>();
        }
        [Fact]
        public async Task OfferController_AddOffer_XmlFileDeactivated_ReturnOk()
        {
            //Arrange
            var data = A.Fake<AddOffer>();
            data.Path = "tmp.xlsx";
            data.OfferStatusId = 1;
            int deactivatedId = 1;
            int newOfferId = 1;
            int logTypeId = 1;
            A.CallTo(() => _userServices.UserExist(data.UserId)).Returns(true);
            A.CallTo(() => _offerServices.DoesPricelistExist(data.OfferName, data.UserId)).Returns(false);
            A.CallTo(() => _offerServices.CreatePricelist(data)).Returns(newOfferId);
            A.CallTo(() => _offerServices.GetDeactivatedStatusId()).Returns(deactivatedId);
            A.CallTo(() => _offerServices.CreateXmlFile(newOfferId, data.UserId, data.Path, data.MaxQty)).Returns(true);
            A.CallTo(() => _logServices.getLogTypeId("Create")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", data.UserId, logTypeId));
            var controller = new OfferController(_offerServices, _userServices, _logServices);

            //Act
            var result = await controller.AddOffer(data);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkResult>();
        }
        [Fact]
        public async Task OfferController_AddOffer_UserNotFound_Return404()
        {
            //Arrange
            var data = A.Fake<AddOffer>();
            data.Path = "tmp.csv";
            data.OfferStatusId = 1;
            int deactivatedId = 2;
            int newOfferId = 1;
            int logTypeId = 1;
            A.CallTo(() => _userServices.UserExist(data.UserId)).Returns(false);
            A.CallTo(() => _offerServices.DoesPricelistExist(data.OfferName, data.UserId)).Returns(false);
            A.CallTo(() => _offerServices.CreatePricelist(data)).Returns(newOfferId);
            A.CallTo(() => _offerServices.GetDeactivatedStatusId()).Returns(deactivatedId);
            A.CallTo(() => _offerServices.CreateCsvFile(newOfferId, data.UserId, data.Path, data.MaxQty)).Returns(true);
            A.CallTo(() => _logServices.getLogTypeId("Create")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", data.UserId, logTypeId));
            var controller = new OfferController(_offerServices, _userServices, _logServices);

            //Act
            var result = await controller.AddOffer(data);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundObjectResult>();
        }
        [Fact]
        public async Task OfferController_AddOffer_OfferExist_Return400()
        {
            //Arrange
            var data = A.Fake<AddOffer>();
            data.Path = "tmp.csv";
            data.OfferStatusId = 1;
            int deactivatedId = 2;
            int newOfferId = 1;
            int logTypeId = 1;
            A.CallTo(() => _userServices.UserExist(data.UserId)).Returns(true);
            A.CallTo(() => _offerServices.DoesPricelistExist(data.OfferName, data.UserId)).Returns(true);
            A.CallTo(() => _offerServices.CreatePricelist(data)).Returns(newOfferId);
            A.CallTo(() => _offerServices.GetDeactivatedStatusId()).Returns(deactivatedId);
            A.CallTo(() => _offerServices.CreateCsvFile(newOfferId, data.UserId, data.Path, data.MaxQty)).Returns(true);
            A.CallTo(() => _logServices.getLogTypeId("Create")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", data.UserId, logTypeId));
            var controller = new OfferController(_offerServices, _userServices, _logServices);

            //Act
            var result = await controller.AddOffer(data);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<BadRequestObjectResult>();
        }
        [Fact]
        public async Task OfferController_AddOffer_AddFails_Return500()
        {
            //Arrange
            var data = A.Fake<AddOffer>();
            data.Path = "tmp.csv";
            data.OfferStatusId = 1;
            int deactivatedId = 2;
            int newOfferId = 0;
            int logTypeId = 1;
            A.CallTo(() => _userServices.UserExist(data.UserId)).Returns(true);
            A.CallTo(() => _offerServices.DoesPricelistExist(data.OfferName, data.UserId)).Returns(false);
            A.CallTo(() => _offerServices.CreatePricelist(data)).Returns(newOfferId);
            A.CallTo(() => _offerServices.GetDeactivatedStatusId()).Returns(deactivatedId);
            A.CallTo(() => _offerServices.CreateCsvFile(newOfferId, data.UserId, data.Path, data.MaxQty)).Returns(true);
            A.CallTo(() => _logServices.getLogTypeId("Create")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", data.UserId, logTypeId));
            var controller = new OfferController(_offerServices, _userServices, _logServices);

            //Act
            var result = await controller.AddOffer(data);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<StatusCodeResult>();
        }
        [Fact]
        public async Task OfferController_GetPricelist_ReturnOk()
        {
            //Arrange
            var data = A.Fake<IEnumerable<GetPriceList>>();
            int userId = 1;
            var filters = A.Fake<OfferFiltersTemplate>();
            A.CallTo(() => _userServices.UserExist(userId)).Returns(true);
            A.CallTo(() => _offerServices.GetPriceLists(userId, null, filters)).Returns(data);
            var controller = new OfferController(_offerServices, _userServices, _logServices);

            //Act
            var result = await controller.GetPricelist(userId, null, null, null, null, null, null, null, null, null, null, null);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();
        }
        [Fact]
        public async Task OfferController_GetPricelist_UserNotFound_Return404()
        {
            //Arrange
            var data = A.Fake<IEnumerable<GetPriceList>>();
            int userId = 1;
            var filters = A.Fake<OfferFiltersTemplate>();
            A.CallTo(() => _userServices.UserExist(userId)).Returns(false);
            A.CallTo(() => _offerServices.GetPriceLists(userId, null, filters)).Returns(data);
            var controller = new OfferController(_offerServices, _userServices, _logServices);

            //Act
            var result = await controller.GetPricelist(userId, null, null, null, null, null, null, null, null, null, null, null);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundObjectResult>();
        }
        [Fact]
        public async Task OfferController_GetPricelist_SortIsIncorrect_Return400()
        {
            //Arrange
            var data = A.Fake<IEnumerable<GetPriceList>>();
            int userId = 1;
            var filters = A.Fake<OfferFiltersTemplate>();
            string sort = "";
            A.CallTo(() => _userServices.UserExist(userId)).Returns(true);
            A.CallTo(() => _offerServices.GetPriceLists(userId, sort, filters)).Returns(data);
            var controller = new OfferController(_offerServices, _userServices, _logServices);

            //Act
            var result = await controller.GetPricelist(userId, null, sort, null, null, null, null, null, null, null, null, null);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<BadRequestObjectResult>();
        }
        [Fact]
        public async Task OfferController_DeleteOffer_ReturnOk()
        {
            //Arrange
            int userId = 1;
            int offerId = 1;
            int logTypeId = 1;
            A.CallTo(() => _userServices.UserExist(userId)).Returns(true);
            A.CallTo(() => _offerServices.DoesPricelistExist(offerId)).Returns(true);
            A.CallTo(() => _offerServices.DeletePricelist(offerId)).Returns(true);
            A.CallTo(() => _logServices.getLogTypeId("Create")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", userId, logTypeId));
            var controller = new OfferController(_offerServices, _userServices, _logServices);

            //Act
            var result = await controller.DeleteOffer(offerId, userId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkResult>();
        }
        [Fact]
        public async Task OfferController_DeleteOffer_DeleteFails_Return500()
        {
            //Arrange
            int userId = 1;
            int offerId = 1;
            int logTypeId = 1;
            A.CallTo(() => _userServices.UserExist(userId)).Returns(true);
            A.CallTo(() => _offerServices.DoesPricelistExist(offerId)).Returns(true);
            A.CallTo(() => _offerServices.DeletePricelist(offerId)).Returns(false);
            A.CallTo(() => _logServices.getLogTypeId("Create")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", userId, logTypeId));
            var controller = new OfferController(_offerServices, _userServices, _logServices);

            //Act
            var result = await controller.DeleteOffer(offerId, userId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<StatusCodeResult>();
        }
        [Fact]
        public async Task OfferController_DeleteOffer_UserNotFound_Return404()
        {
            //Arrange
            int userId = 1;
            int offerId = 1;
            int logTypeId = 1;
            A.CallTo(() => _userServices.UserExist(userId)).Returns(false);
            A.CallTo(() => _offerServices.DoesPricelistExist(offerId)).Returns(true);
            A.CallTo(() => _offerServices.DeletePricelist(offerId)).Returns(true);
            A.CallTo(() => _logServices.getLogTypeId("Create")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", userId, logTypeId));
            var controller = new OfferController(_offerServices, _userServices, _logServices);

            //Act
            var result = await controller.DeleteOffer(offerId, userId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundObjectResult>();
        }
        [Fact]
        public async Task OfferController_DeleteOffer_OfferNotFound_Return404()
        {
            //Arrange
            int userId = 1;
            int offerId = 1;
            int logTypeId = 1;
            A.CallTo(() => _userServices.UserExist(userId)).Returns(true);
            A.CallTo(() => _offerServices.DoesPricelistExist(offerId)).Returns(false);
            A.CallTo(() => _offerServices.DeletePricelist(offerId)).Returns(true);
            A.CallTo(() => _logServices.getLogTypeId("Create")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", userId, logTypeId));
            var controller = new OfferController(_offerServices, _userServices, _logServices);

            //Act
            var result = await controller.DeleteOffer(offerId, userId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundObjectResult>();
        }
        [Fact]
        public async Task OfferController_GetStatuses_ReturnOk()
        {
            //Arrange
            var data = A.Fake<IEnumerable<GetOfferStatus>>();
            A.CallTo(() => _offerServices.GetPricelistStatuses()).Returns(data);
            var controller = new OfferController(_offerServices, _userServices, _logServices);

            //Act
            var result = await controller.GetStatuses();

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();
        }
        [Fact]
        public async Task OfferController_GetItems_ReturnOk()
        {
            //Arrange
            var data = A.Fake<IEnumerable<GetItemsForOffer>>();
            int userId = 1;
            string currency = "PLN";
            A.CallTo(() => _userServices.UserExist(userId)).Returns(true);
            A.CallTo(() => _offerServices.GetItemsForPricelist(userId, currency)).Returns(data);
            var controller = new OfferController(_offerServices, _userServices, _logServices);

            //Act
            var result = await controller.GetItems(userId, currency);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();
        }
        [Fact]
        public async Task OfferController_GetItems_UserNotFound_Return404()
        {
            //Arrange
            var data = A.Fake<IEnumerable<GetItemsForOffer>>();
            int userId = 1;
            string currency = "PLN";
            A.CallTo(() => _userServices.UserExist(userId)).Returns(false);
            A.CallTo(() => _offerServices.GetItemsForPricelist(userId, currency)).Returns(data);
            var controller = new OfferController(_offerServices, _userServices, _logServices);

            //Act
            var result = await controller.GetItems(userId, currency);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundObjectResult>();
        }
        [Fact]
        public async Task OfferController_GetOfferItems_ReturnOk()
        {
            //Arrange
            var data = A.Fake<IEnumerable<GetCredtItemForTable>>();
            int userId = 1;
            int pricelistId = 1;
            A.CallTo(() => _userServices.UserExist(userId)).Returns(true);
            A.CallTo(() => _offerServices.DoesPricelistExist(pricelistId)).Returns(true);
            A.CallTo(() => _offerServices.GetPricelistItems(pricelistId, userId)).Returns(data);
            var controller = new OfferController(_offerServices, _userServices, _logServices);

            //Act
            var result = await controller.GetOfferItems(pricelistId, userId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();
        }
        [Fact]
        public async Task OfferController_GetOfferItems_UserNotFound_Return404()
        {
            //Arrange
            var data = A.Fake<IEnumerable<GetCredtItemForTable>>();
            int userId = 1;
            int pricelistId = 1;
            A.CallTo(() => _userServices.UserExist(userId)).Returns(false);
            A.CallTo(() => _offerServices.DoesPricelistExist(pricelistId)).Returns(true);
            A.CallTo(() => _offerServices.GetPricelistItems(pricelistId, userId)).Returns(data);
            var controller = new OfferController(_offerServices, _userServices, _logServices);

            //Act
            var result = await controller.GetOfferItems(pricelistId, userId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundObjectResult>();
        }
        [Fact]
        public async Task OfferController_GetOfferItems_OfferNotFound_Return404()
        {
            //Arrange
            var data = A.Fake<IEnumerable<GetCredtItemForTable>>();
            int userId = 1;
            int pricelistId = 1;
            A.CallTo(() => _userServices.UserExist(userId)).Returns(true);
            A.CallTo(() => _offerServices.DoesPricelistExist(pricelistId)).Returns(false);
            A.CallTo(() => _offerServices.GetPricelistItems(pricelistId, userId)).Returns(data);
            var controller = new OfferController(_offerServices, _userServices, _logServices);

            //Act
            var result = await controller.GetOfferItems(pricelistId, userId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundObjectResult>();
        }
        [Fact]
        public async Task OfferController_GetRestPricelist_ReturnOk()
        {
            //Arrange
            var data = A.Fake<GetRestModifyOffer>();
            int userId = 1;
            int pricelistId = 1;
            A.CallTo(() => _userServices.UserExist(userId)).Returns(true);
            A.CallTo(() => _offerServices.DoesPricelistExist(pricelistId)).Returns(true);
            A.CallTo(() => _offerServices.GetRestModifyPricelist(pricelistId, userId)).Returns(data);
            var controller = new OfferController(_offerServices, _userServices, _logServices);

            //Act
            var result = await controller.GetRestPricelist(pricelistId, userId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();
        }
        [Fact]
        public async Task OfferController_GetRestPricelist_UserNotFound_Return404()
        {
            //Arrange
            var data = A.Fake<GetRestModifyOffer>();
            int userId = 1;
            int pricelistId = 1;
            A.CallTo(() => _userServices.UserExist(userId)).Returns(false);
            A.CallTo(() => _offerServices.DoesPricelistExist(pricelistId)).Returns(true);
            A.CallTo(() => _offerServices.GetRestModifyPricelist(pricelistId, userId)).Returns(data);
            var controller = new OfferController(_offerServices, _userServices, _logServices);

            //Act
            var result = await controller.GetRestPricelist(pricelistId, userId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundObjectResult>();
        }
        [Fact]
        public async Task OfferController_GetRestPricelist_OfferNotFound_Return404()
        {
            //Arrange
            var data = A.Fake<GetRestModifyOffer>();
            int userId = 1;
            int pricelistId = 1;
            A.CallTo(() => _userServices.UserExist(userId)).Returns(true);
            A.CallTo(() => _offerServices.DoesPricelistExist(pricelistId)).Returns(false);
            A.CallTo(() => _offerServices.GetRestModifyPricelist(pricelistId, userId)).Returns(data);
            var controller = new OfferController(_offerServices, _userServices, _logServices);

            //Act
            var result = await controller.GetRestPricelist(pricelistId, userId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundObjectResult>();
        }
        [Fact]
        public async Task OfferController_ModifyPricelist_CsvActivated_ReturnOk()
        {
            //Arrange
            var data = A.Fake<ModifyPricelist>();
            data.OfferName = null;
            data.OfferStatusId = 1;
            int deactivatedStatus = 2;
            string path = ".csv";
            int maxQty = 10;
            int logTypeId = 1;
            A.CallTo(() => _userServices.UserExist(data.UserId)).Returns(true);
            A.CallTo(() => _offerServices.DoesPricelistExist(data.OfferId)).Returns(true);
            A.CallTo(() => _offerServices.ModifyPricelist(data)).Returns(true);
            A.CallTo(() => _offerServices.GetDeactivatedStatusId()).Returns(deactivatedStatus);
            A.CallTo(() => _offerServices.GetOfferMaxQty(data.OfferId)).Returns(maxQty);
            A.CallTo(() => _offerServices.GetPricelistPath(data.OfferId)).Returns(path);
            A.CallTo(() => _offerServices.CreateCsvFile(data.OfferId, data.UserId, path, maxQty));
            A.CallTo(() => _logServices.getLogTypeId("Modify")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", data.UserId, logTypeId));
            var controller = new OfferController(_offerServices, _userServices, _logServices);

            //Act
            var result = await controller.ModifyPricelist(data);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkResult>();
        }
        [Fact]
        public async Task OfferController_ModifyPricelist_XmlActivated_ReturnOk()
        {
            //Arrange
            var data = A.Fake<ModifyPricelist>();
            data.OfferName = null;
            data.OfferStatusId = 1;
            int deactivatedStatus = 2;
            string path = ".xml";
            int maxQty = 10;
            int logTypeId = 1;
            A.CallTo(() => _userServices.UserExist(data.UserId)).Returns(true);
            A.CallTo(() => _offerServices.DoesPricelistExist(data.OfferId)).Returns(true);
            A.CallTo(() => _offerServices.ModifyPricelist(data)).Returns(true);
            A.CallTo(() => _offerServices.GetDeactivatedStatusId()).Returns(deactivatedStatus);
            A.CallTo(() => _offerServices.GetOfferMaxQty(data.OfferId)).Returns(maxQty);
            A.CallTo(() => _offerServices.GetPricelistPath(data.OfferId)).Returns(path);
            A.CallTo(() => _offerServices.CreateXmlFile(data.OfferId, data.UserId, path, maxQty));
            A.CallTo(() => _logServices.getLogTypeId("Modify")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", data.UserId, logTypeId));
            var controller = new OfferController(_offerServices, _userServices, _logServices);

            //Act
            var result = await controller.ModifyPricelist(data);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkResult>();
        }
        [Fact]
        public async Task OfferController_ModifyPricelist_CsvDeactivated_ReturnOk()
        {
            //Arrange
            var data = A.Fake<ModifyPricelist>();
            data.OfferName = null;
            data.OfferStatusId = 1;
            int deactivatedStatus = 1;
            int logTypeId = 1;
            A.CallTo(() => _userServices.UserExist(data.UserId)).Returns(true);
            A.CallTo(() => _offerServices.DoesPricelistExist(data.OfferId)).Returns(true);
            A.CallTo(() => _offerServices.ModifyPricelist(data)).Returns(true);
            A.CallTo(() => _offerServices.GetDeactivatedStatusId()).Returns(deactivatedStatus);
            A.CallTo(() => _logServices.getLogTypeId("Modify")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", data.UserId, logTypeId));
            var controller = new OfferController(_offerServices, _userServices, _logServices);

            //Act
            var result = await controller.ModifyPricelist(data);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkResult>();
        }
        [Fact]
        public async Task OfferController_ModifyPricelist_XmlDeactivated_ReturnOk()
        {
            //Arrange
            var data = A.Fake<ModifyPricelist>();
            data.OfferName = null;
            data.OfferStatusId = 1;
            int deactivatedStatus = 1;
            int logTypeId = 1;
            A.CallTo(() => _userServices.UserExist(data.UserId)).Returns(true);
            A.CallTo(() => _offerServices.DoesPricelistExist(data.OfferId)).Returns(true);
            A.CallTo(() => _offerServices.ModifyPricelist(data)).Returns(true);
            A.CallTo(() => _offerServices.GetDeactivatedStatusId()).Returns(deactivatedStatus);
            A.CallTo(() => _logServices.getLogTypeId("Modify")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", data.UserId, logTypeId));
            var controller = new OfferController(_offerServices, _userServices, _logServices);

            //Act
            var result = await controller.ModifyPricelist(data);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkResult>();
        }
        [Fact]
        public async Task OfferController_ModifyPricelist_ModifyFails_Return500()
        {
            //Arrange
            var data = A.Fake<ModifyPricelist>();
            data.OfferName = null;
            data.OfferStatusId = 1;
            int deactivatedStatus = 1;
            int logTypeId = 1;
            A.CallTo(() => _userServices.UserExist(data.UserId)).Returns(true);
            A.CallTo(() => _offerServices.DoesPricelistExist(data.OfferId)).Returns(true);
            A.CallTo(() => _offerServices.ModifyPricelist(data)).Returns(false);
            A.CallTo(() => _offerServices.GetDeactivatedStatusId()).Returns(deactivatedStatus);
            A.CallTo(() => _logServices.getLogTypeId("Modify")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", data.UserId, logTypeId));
            var controller = new OfferController(_offerServices, _userServices, _logServices);

            //Act
            var result = await controller.ModifyPricelist(data);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<StatusCodeResult>();
        }
        [Fact]
        public async Task OfferController_ModifyPricelist_UserNotFound_Return404()
        {
            //Arrange
            var data = A.Fake<ModifyPricelist>();
            data.OfferName = null;
            data.OfferStatusId = 1;
            int deactivatedStatus = 1;
            int logTypeId = 1;
            A.CallTo(() => _userServices.UserExist(data.UserId)).Returns(false);
            A.CallTo(() => _offerServices.DoesPricelistExist(data.OfferId)).Returns(true);
            A.CallTo(() => _offerServices.ModifyPricelist(data)).Returns(true);
            A.CallTo(() => _offerServices.GetDeactivatedStatusId()).Returns(deactivatedStatus);
            A.CallTo(() => _logServices.getLogTypeId("Modify")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", data.UserId, logTypeId));
            var controller = new OfferController(_offerServices, _userServices, _logServices);

            //Act
            var result = await controller.ModifyPricelist(data);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundObjectResult>();
        }
        [Fact]
        public async Task OfferController_ModifyPricelist_OfferNotFound_Return404()
        {
            //Arrange
            var data = A.Fake<ModifyPricelist>();
            data.OfferName = null;
            data.OfferStatusId = 1;
            int deactivatedStatus = 1;
            int logTypeId = 1;
            A.CallTo(() => _userServices.UserExist(data.UserId)).Returns(true);
            A.CallTo(() => _offerServices.DoesPricelistExist(data.OfferId)).Returns(false);
            A.CallTo(() => _offerServices.ModifyPricelist(data)).Returns(true);
            A.CallTo(() => _offerServices.GetDeactivatedStatusId()).Returns(deactivatedStatus);
            A.CallTo(() => _logServices.getLogTypeId("Modify")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", data.UserId, logTypeId));
            var controller = new OfferController(_offerServices, _userServices, _logServices);

            //Act
            var result = await controller.ModifyPricelist(data);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundObjectResult>();
        }
        [Fact]
        public async Task OfferController_ModifyPricelist_OfferNameOk_ReturnOk()
        {
            //Arrange
            var data = A.Fake<ModifyPricelist>();
            data.OfferName = "";
            data.OfferStatusId = 1;
            int deactivatedStatus = 1;
            int logTypeId = 1;
            A.CallTo(() => _userServices.UserExist(data.UserId)).Returns(true);
            A.CallTo(() => _offerServices.DoesPricelistExist(data.OfferId)).Returns(true);
            A.CallTo(() => _offerServices.DoesPricelistExist(data.OfferName, data.UserId)).Returns(false);
            A.CallTo(() => _offerServices.ModifyPricelist(data)).Returns(true);
            A.CallTo(() => _offerServices.GetDeactivatedStatusId()).Returns(deactivatedStatus);
            A.CallTo(() => _logServices.getLogTypeId("Modify")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", data.UserId, logTypeId));
            var controller = new OfferController(_offerServices, _userServices, _logServices);

            //Act
            var result = await controller.ModifyPricelist(data);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkResult>();
        }
        [Fact]
        public async Task OfferController_ModifyPricelist_OfferNameAlreadyExist_Return400()
        {
            //Arrange
            var data = A.Fake<ModifyPricelist>();
            data.OfferName = "";
            data.OfferStatusId = 1;
            int deactivatedStatus = 1;
            int logTypeId = 1;
            A.CallTo(() => _userServices.UserExist(data.UserId)).Returns(true);
            A.CallTo(() => _offerServices.DoesPricelistExist(data.OfferId)).Returns(true);
            A.CallTo(() => _offerServices.DoesPricelistExist(data.OfferName, data.UserId)).Returns(true);
            A.CallTo(() => _offerServices.ModifyPricelist(data)).Returns(true);
            A.CallTo(() => _offerServices.GetDeactivatedStatusId()).Returns(deactivatedStatus);
            A.CallTo(() => _logServices.getLogTypeId("Modify")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", data.UserId, logTypeId));
            var controller = new OfferController(_offerServices, _userServices, _logServices);

            //Act
            var result = await controller.ModifyPricelist(data);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<BadRequestObjectResult>();
        }
        [Fact]
        public async Task OfferController_GetDeactivatedStatusId_ReturnOk()
        {
            //Arrange
            int statusId = 1;
            A.CallTo(() => _offerServices.GetDeactivatedStatusId()).Returns(statusId);
            var controller = new OfferController(_offerServices, _userServices, _logServices);

            //Act
            var result = await controller.GetDeactivatedStatusId();

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();
        }
        [Fact]
        public async Task OfferController_StartUpdate_ReturnOk()
        {
            //Arrange
            var csvIds = A.Dummy<IEnumerable<int>>();
            var xmlIds = A.Dummy<IEnumerable<int>>();
            var rejectedIds = A.Dummy<List<int>>();
            A.CallTo(() => _offerServices.GetAllActiveCsvOfferId()).Returns(csvIds);
            A.CallTo(() => _offerServices.GetAllActiveXmlOfferId()).Returns(xmlIds);
            A.CallTo(() => _offerServices.UpdateCsvFile(A<int>.Ignored)).Returns(true);
            A.CallTo(() => _offerServices.UpdateCsvFile(A<int>.Ignored)).Returns(true);
            var controller = new OfferController(_offerServices, _userServices, _logServices);

            //Act
            var result = await controller.StartUpdate();

            //Assert
            A.CallTo(() => _offerServices.UpdateCsvFile(A<int>.Ignored)).MustHaveHappenedANumberOfTimesMatching(callsQty => callsQty == csvIds.Count());
            A.CallTo(() => _offerServices.UpdateCsvFile(A<int>.Ignored)).MustHaveHappenedANumberOfTimesMatching(callsQty => callsQty == xmlIds.Count());
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();
        }
    }
}
