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
    public class ProformaServicesTest
    {
        private readonly ProformaServices _proformaServices;
        private readonly IConfiguration _configuration;
        public ProformaServicesTest()
        {
            _configuration = new ConfigurationBuilder().AddUserSecrets("955b92d9-13d8-44b0-b618-19483bfe8b68").Build();
            var testDbConnectionString = _configuration["testDb"];
            var contextOptions = new DbContextOptionsBuilder<HandlerContext>()
                .UseSqlServer(testDbConnectionString);
            var context = new HandlerContext(contextOptions.Options, _configuration, A.Fake<IHttpContextAccessor>());
            _proformaServices = new ProformaServices(context, A.Fake<ILogger<ProformaServices>>());
        }
        [Fact]
        public async Task ProformaServices_AddProforma_ReturnNewPricelistId()
        {
            //Arrange
            var data = new AddProforma
            {
                IsYourProforma = true,
                ProformaNumber = "Test",
                SellerId = 2,
                BuyerId = 1,
                Date = DateTime.Now,
                TransportCost = 100,
                Note = "",
                InSystem = true,
                Path = "test",
                TaxId = 4,
                PaymentId = 1,
                CurrencyDate = DateTime.Now,
                CurrencyName = "PLN",
                CurrencyValue = 1,
                UserId = 1,
                ProformaItems =
                [
                    new(){
                        ItemId = 1,
                        Qty = 10,
                        Price = 159.99M
                    }
                ]
            };

            //Act
            var result = await _proformaServices.AddProforma(data);

            //Assert
            result.Should().BeGreaterThan(0);
            result.Should().BePositive();
        }
        [Fact]
        public async Task ProformaServices_ProformaExist_ReturnTrue()
        {
            //Arrange
            string number = "Proforma A";
            int sellerId = 2;
            int buyerId = 1;

            //Act
            var result = await _proformaServices.ProformaExist(number, sellerId, buyerId);

            //Assert
            result.Should().BeTrue();
        }
        [Fact]
        public async Task ProformaServices_ProformaExist_NumberNotFound_ReturnFalse()
        {
            //Arrange
            string number = "A";
            int sellerId = 2;
            int buyerId = 1;

            //Act
            var result = await _proformaServices.ProformaExist(number, sellerId, buyerId);

            //Assert
            result.Should().BeFalse();
        }
        [Fact]
        public async Task ProformaServices_ProformaExist_IdVersion_ReturnTrue()
        {
            //Arrange
            int orgId = 1;

            //Act
            var result = await _proformaServices.ProformaExist(orgId);

            //Assert
            result.Should().BeTrue();
        }
        [Fact]
        public async Task ProformaServices_ProformaExist_IdVersion_NumberNotFound_ReturnFalse()
        {
            //Arrange
            int orgId = 100;

            //Act
            var result = await _proformaServices.ProformaExist(orgId);

            //Assert
            result.Should().BeFalse();
        }
        [Fact]
        public async Task ProformaServices_DeleteProforma_ReturnTrue()
        {
            //Arrange
            bool isYourProforma = true;
            int proformaId = 5;

            //Act
            var result = await _proformaServices.DeleteProforma(isYourProforma, proformaId);

            //Assert
            result.Should().BeTrue();
        }
        [Fact]
        public async Task ProformaServices_DoesDeliveryExist_ReturnTrue()
        {
            //Arrange
            int proformaId = 2;

            //Act
            var result = await _proformaServices.DoesDeliveryExist(proformaId);

            //Assert
            result.Should().BeTrue();
        }
        [Fact]
        public async Task ProformaServices_ModifyProforma_ReturnTrue()
        {
            //Arrange
            var data = new ModifyProforma
            {
                IsYourProforma = true,
                ProformaId = 2,
                ProformaNumber = "Proforma yours B"
            };

            //Act
            var result = await _proformaServices.ModifyProforma(data);

            //Assert
            result.Should().BeTrue();
        }
    }
}
