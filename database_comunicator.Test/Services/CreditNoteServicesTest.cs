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
    public class CreditNoteServicesTest
    {
        private readonly CreditNoteServices _creditNoteServices;
        private readonly IConfiguration _configuration;
        public CreditNoteServicesTest()
        {
            _configuration = new ConfigurationBuilder().AddUserSecrets("955b92d9-13d8-44b0-b618-19483bfe8b68").Build();
            var testDbConnectionString = _configuration["testDb"];
            var contextOptions = new DbContextOptionsBuilder<HandlerContext>()
                .UseSqlServer(testDbConnectionString);
            var context = new HandlerContext(contextOptions.Options, _configuration, A.Fake<IHttpContextAccessor>());
            _creditNoteServices = new CreditNoteServices(context, A.Fake<ILogger<CreditNoteServices>>());
        }
        [Fact]
        public async Task CreditNoteServices_AddCreditNote_ReturnTrue()
        {
            //Arrange
            int creditNoteId = 8;

            //Act
            var result = await _creditNoteServices.CreditNoteExist(creditNoteId);

            //Assert
            result.Should().BeTrue();
        }
        [Fact]
        public async Task CreditNoteServices_CreditNoteExist_ReturnFalse()
        {
            //Arrange
            int creditNoteId = 20;

            //Act
            var result = await _creditNoteServices.CreditNoteExist(creditNoteId);

            //Assert
            result.Should().BeFalse();
        }
        [Fact]
        public async Task CreditNoteServices_AddCreditNote_ReturnNewCreditId()
        {
            //Arrange
            var data = new AddCreditNote
            {
                CreditNoteDate = DateTime.Now,
                InSystem = false,
                CreditNoteNumber = "CreditS T",
                IsYourCreditNote = false,
                FilePath = "test",
                Note = "",
                IsPaid = true,
                InvoiceId = 5,
                CreditNoteItems =
                [
                    new() {
                        UserId = 1,
                        ItemId = 1,
                        InvoiceId = 4,
                        PurchasePriceId = 5,
                        Qty = 1,
                        NewPrice = 65.10M
                    }
                ]
            };

            //Act
            var result = await _creditNoteServices.AddCreditNote(data);

            //Assert
            result.Should().BeGreaterThan(0);
            result.Should().BePositive();
        }
        [Fact]
        public async Task CreditNoteServices_AddCreditNote_NullNote_Return0()
        {
            //Arrange
            var data = new AddCreditNote
            {
                CreditNoteDate = DateTime.Now,
                InSystem = false,
                CreditNoteNumber = "CreditS T",
                IsYourCreditNote = false,
                FilePath = "test",
                IsPaid = true,
                InvoiceId = 5,
                CreditNoteItems =
                [
                    new() {
                        UserId = 1,
                        ItemId = 1,
                        InvoiceId = 4,
                        PurchasePriceId = 5,
                        Qty = 1,
                        NewPrice = 65.10M
                    }
                ]
            };

            //Act
            var result = await _creditNoteServices.AddCreditNote(data);

            //Assert
            result.Should().Be(0);
        }
        [Fact]
        public async Task CreditNoteServices_CreditDeductionCanBeApplied_ReturnTrue()
        {
            //Arrange
            int userId = 1;
            int invoiceId = 4;
            int itemId = 2;
            int qty = -3;

            //Act
            var result = await _creditNoteServices.CreditDeductionCanBeApplied(userId, invoiceId, itemId, qty);

            //Assert
            result.Should().BeTrue();
        }
        [Fact]
        public async Task CreditNoteServices_CreditDeductionCanBeApplied_ReturnFalse()
        {
            //Arrange
            int userId = 1;
            int invoiceId = 2;
            int itemId = 5;
            int qty = -3;

            //Act
            var result = await _creditNoteServices.CreditDeductionCanBeApplied(userId, invoiceId, itemId, qty);

            //Assert
            result.Should().BeFalse();
        }
        [Fact]
        public async Task CreditNoteServices_DeleteCreditNote_ReturnTrue()
        {
            //Arrange
            int creditId = 15;

            //Act
            var result = await _creditNoteServices.DeleteCreditNote(creditId);

            //Assert
            result.Should().BeTrue();
        }
        [Fact]
        public async Task CreditNoteServices_ModifyCreditNote_ReturnTrue()
        {
            //Arrange
            var data = new ModifyCreditNote
            {
                Id = 8,
                IsYourCredit = true,
                Note = "a"
            };

            //Act
            var result = await _creditNoteServices.ModifyCreditNote(data);

            //Assert
            result.Should().BeTrue();
        }
    }
}
