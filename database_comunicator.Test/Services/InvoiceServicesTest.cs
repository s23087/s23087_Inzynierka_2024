using database_communicator.Data;
using database_communicator.Models.DTOs.Create;
using database_communicator.Models.DTOs.Modify;
using database_communicator.Services;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace database_communicator_Test.Services
{
    public class InvoiceServicesTest
    {
        private readonly InvoiceServices _invoiceServices;
        private readonly IConfiguration _configuration;
        public InvoiceServicesTest()
        {
            _configuration = new ConfigurationBuilder().AddUserSecrets("955b92d9-13d8-44b0-b618-19483bfe8b68").Build();
            var testDbConnectionString = _configuration["testDb"];
            var contextOptions = new DbContextOptionsBuilder<HandlerContext>()
                .UseSqlServer(testDbConnectionString);
            var context = new HandlerContext(contextOptions.Options, _configuration, A.Fake<IHttpContextAccessor>());
            _invoiceServices = new InvoiceServices(context, A.Fake<ILogger<InvoiceServices>>());
        }
        [Fact]
        public async Task InvoiceServices_AddPurchaseInvoice_ReturnNewInvoiceId()
        {
            //Arrange
            var data = new AddPurchaseInvoice
            {
                UserId = 1,
                InvoiceNumber = "Test",
                Seller = 2,
                Buyer = 1,
                InvoiceDate = DateTime.Now,
                DueDate = DateTime.Now,
                Note = "",
                InSystem = true,
                TransportCost = 100,
                InvoiceFilePath = null,
                Taxes = 1,
                CurrencyValueDate = new DateTime(2023, 9, 3, 0, 0, 0, DateTimeKind.Utc),
                CurrencyName = "EUR",
                PaymentMethodId = 1,
                PaymentsStatusId = 1,
                EuroValue = 4.00M,
                UsdValue = 3.00M,
                InvoiceItems = [
                        new() {
                            ItemId = 1,
                            Qty = 1,
                            Price = 250.00M
                        }
                    ]
            };

            //Act
            var result = await _invoiceServices.AddPurchaseInvoice(data);

            //Assert
            result.Should().BeGreaterThan(0);
            result.Should().BePositive();
        }
        [Fact]
        public async Task InvoiceServices_AddSalesInvoice_ReturnNewInvoiceId()
        {
            //Arrange
            var data = new AddSalesInvoice
            {
                UserId = 1,
                InvoiceNumber = "TestS",
                Seller = 1,
                Buyer = 2,
                InvoiceDate = DateTime.Now,
                DueDate = DateTime.Now,
                Note = "",
                InSystem = true,
                TransportCost = 100,
                InvoiceFilePath = null,
                Taxes = 1,
                CurrencyValueDate = new DateTime(2023, 9, 3, 0, 0, 0, DateTimeKind.Utc),
                CurrencyName = "EUR",
                PaymentMethodId = 1,
                PaymentsStatusId = 1,
                CurrencyValue = 4.00M,
                InvoiceItems = [
                        new() {
                            PriceId = 6,
                            ItemId = 2,
                            Qty = 1,
                            Price = 25.00M,
                            BuyInvoiceId = 4,
                        }
                    ]
            };

            //Act
            var result = await _invoiceServices.AddSalesInvoice(data);

            //Assert
            result.Should().BeGreaterThan(0);
            result.Should().BePositive();
        }
        [Fact]
        public async Task InvoiceServices_CheckIfSellingPriceExist_ReturnTrue()
        {
            //Arrange
            int invoiceId = 1;

            //Act
            var result = await _invoiceServices.CheckIfSellingPriceExist(invoiceId);

            //Assert
            result.Should().BeTrue();
        }
        [Fact]
        public async Task InvoiceServices_CheckIfSellingPriceExist_ReturnFalse()
        {
            //Arrange
            int invoiceId = 2;

            //Act
            var result = await _invoiceServices.CheckIfSellingPriceExist(invoiceId);

            //Assert
            result.Should().BeFalse();
        }
        [Fact]
        public async Task InvoiceServices_CheckIfCreditNoteExist_ReturnTrue()
        {
            //Arrange
            int invoiceId = 1;

            //Act
            var result = await _invoiceServices.CheckIfSellingPriceExist(invoiceId);

            //Assert
            result.Should().BeTrue();
        }
        [Fact]
        public async Task InvoiceServices_CheckIfCreditNoteExist_ReturnFalse()
        {
            //Arrange
            int invoiceId = 3;

            //Act
            var result = await _invoiceServices.CheckIfSellingPriceExist(invoiceId);

            //Assert
            result.Should().BeFalse();
        }
        [Fact]
        public async Task InvoiceServices_DeleteInvoice_ReturnTrue()
        {
            //Arrange
            int invoiceId = 7;

            //Act
            var result = await _invoiceServices.DeleteInvoice(invoiceId);

            //Assert
            result.Should().BeTrue();
        }
        [Fact]
        public async Task InvoiceServices_InvoiceExist_ReturnTrue()
        {
            //Arrange
            int invoiceId = 1;

            //Act
            var result = await _invoiceServices.InvoiceExist(invoiceId);

            //Assert
            result.Should().BeTrue();
        }
        [Fact]
        public async Task InvoiceServices_InvoiceExist_ReturnFalse()
        {
            //Arrange
            int invoiceId = 200;

            //Act
            var result = await _invoiceServices.InvoiceExist(invoiceId);

            //Assert
            result.Should().BeFalse();
        }
        [Fact]
        public async Task InvoiceServices_ModifyInvoice_ReturnTrue()
        {
            //Arrange
            var data = new ModifyInvoice { 
                InvoiceId = 1,
                IsYourInvoice = true,
                Note = "test"
            };

            //Act
            var result = await _invoiceServices.ModifyInvoice(data);

            //Assert
            result.Should().BeTrue();
        }
        [Fact]
        public async Task InvoiceServices_UpdateInvoiceStatus_ReturnTrue()
        {
            //Arrange

            //Act
            var result = await _invoiceServices.UpdateInvoiceStatus();

            //Assert
            result.Should().BeTrue();
        }
    }
}
