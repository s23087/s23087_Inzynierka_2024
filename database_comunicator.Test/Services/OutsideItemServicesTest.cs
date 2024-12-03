using database_communicator.Data;
using database_communicator.Models.DTOs.Create;
using database_communicator.Services;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace database_communicator_Test.Services
{
    public class OutsideItemServicesTest
    {
        private readonly OutsideItemServices _outsideItemServices;
        private readonly IConfiguration _configuration;
        public OutsideItemServicesTest()
        {
            _configuration = new ConfigurationBuilder().AddUserSecrets("955b92d9-13d8-44b0-b618-19483bfe8b68").Build();
            var testDbConnectionString = _configuration["testDb"];
            var contextOptions = new DbContextOptionsBuilder<HandlerContext>()
                .UseSqlServer(testDbConnectionString);
            var context = new HandlerContext(contextOptions.Options, _configuration, A.Fake<IHttpContextAccessor>());
            _outsideItemServices = new OutsideItemServices(context, A.Fake<ILogger<OutsideItemServices>>());
        }
        [Fact]
        public async Task OutsideItemServices_ItemExist_ReturnTrue()
        {
            //Arrange
            int itemId = 6;
            int orgId = 2;

            //Act
            var result = await _outsideItemServices.ItemExist(itemId, orgId);

            //Assert
            result.Should().BeTrue();
        }
        [Fact]
        public async Task OutsideItemServices_DeleteItem_ReturnTrue()
        {
            //Arrange
            int itemId = 10;
            int orgId = 2;

            //Act
            var result = await _outsideItemServices.DeleteItem(itemId, orgId);

            //Assert
            result.Should().BeTrue();
        }
        [Fact]
        public async Task OutsideItemServices_AddItems_ReturnEmptyArray()
        {
            //Arrange
            var data = new CreateOutsideItems 
            { 
                OrgId = 3,
                Currency = "PLN",
                Items = 
                [
                    new(){
                        Partnumber = "itemA",
                        ItemName = "Item A",
                        ItemDesc = "Laptop.",
                        Eans = ["1256156153132"],
                        Qty = 2,
                        Price = 1999.99M
                    }
                ]
            };

            //Act
            var result = await _outsideItemServices.AddItems(data);

            //Assert
            result.Should().BeEmpty();
        }
    }
}
