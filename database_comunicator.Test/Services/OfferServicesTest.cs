using database_communicator.Data;
using database_communicator.Models.DTOs.Create;
using database_communicator.Models.DTOs.Modify;
using database_communicator.Services;
using FakeItEasy;
using FluentAssertions;
using LINQtoCSV;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace database_communicator_Test.Services
{
    public class OfferServicesTest
    {
        private readonly OfferServices _offerServices;
        private readonly IConfiguration _configuration;
        public OfferServicesTest()
        {
            _configuration = new ConfigurationBuilder().AddUserSecrets("955b92d9-13d8-44b0-b618-19483bfe8b68").Build();
            var testDbConnectionString = _configuration["testDb"];
            var contextOptions = new DbContextOptionsBuilder<HandlerContext>()
                .UseSqlServer(testDbConnectionString);
            var context = new HandlerContext(contextOptions.Options, _configuration, A.Fake<IHttpContextAccessor>());
            _offerServices = new OfferServices(context, A.Fake<ILogger<OfferServices>>());
        }
        [Fact]
        public async Task OfferServices_DoesPricelistExist_OnlyId_ReturnTrue()
        {
            //Arrange
            int offerId = 1;

            //Act
            var result = await _offerServices.DoesPricelistExist(offerId);

            //Assert
            result.Should().BeTrue();
        }
        [Fact]
        public async Task OfferServices_DoesPricelistExist_OnlyId_ReturnFalse()
        {
            //Arrange
            int offerId = 100;

            //Act
            var result = await _offerServices.DoesPricelistExist(offerId);

            //Assert
            result.Should().BeFalse();
        }
        [Fact]
        public async Task OfferServices_DoesPricelistExist_NameAndUser_ReturnTrue()
        {
            //Arrange
            string name = "Pricelist A";
            int userId = 1;

            //Act
            var result = await _offerServices.DoesPricelistExist(name, userId);

            //Assert
            result.Should().BeTrue();
        }
        [Fact]
        public async Task OfferServices_DoesPricelistExist_NameNotFound_ReturnFalse()
        {
            //Arrange
            string name = "";
            int userId = 1;

            //Act
            var result = await _offerServices.DoesPricelistExist(name, userId);

            //Assert
            result.Should().BeFalse();
        }
        [Fact]
        public async Task OfferServices_DeletePricelist_ReturnTrue()
        {
            //Arrange
            int offerId = 2;

            //Act
            var result = await _offerServices.DeletePricelist(offerId);

            //Assert
            result.Should().BeTrue();
        }
        [Fact]
        public async Task OfferServices_CreatePricelist_ReturnNewPricelistId()
        {
            //Arrange
            var data = new AddOffer
            {
                UserId = 1,
                OfferName = "Test",
                Path = "test",
                OfferStatusId = 1,
                MaxQty = 10,
                Currency = "PLN",
                Items = [
                        new(){
                            ItemId = 1,
                            Price = 2500
                        }
                    ]
            };

            //Act
            var result = await _offerServices.CreatePricelist(data);

            //Assert
            result.Should().BeGreaterThan(0);
            result.Should().BePositive();
        }
        [Fact]
        public async Task OfferServices_ModifyPricelist_ReturnTrue()
        {
            //Arrange
            var data = new ModifyPricelist
            {
                UserId = 1,
                OfferId = 3,
                OfferName = "Test"
            };

            //Act
            var result = await _offerServices.ModifyPricelist(data);

            //Assert
            result.Should().BeTrue();
        }
    }
}
