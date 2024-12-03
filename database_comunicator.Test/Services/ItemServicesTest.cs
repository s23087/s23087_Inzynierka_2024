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
    public class ItemServicesTest
    {
        private readonly ItemServices _itemServices;
        private readonly IConfiguration _configuration;
        public ItemServicesTest()
        {
            _configuration = new ConfigurationBuilder().AddUserSecrets("955b92d9-13d8-44b0-b618-19483bfe8b68").Build();
            var testDbConnectionString = _configuration["testDb"];
            var contextOptions = new DbContextOptionsBuilder<HandlerContext>()
                .UseSqlServer(testDbConnectionString);
            var context = new HandlerContext(contextOptions.Options, _configuration, A.Fake<IHttpContextAccessor>());
            _itemServices = new ItemServices(context, A.Fake<ILogger<ItemServices>>());
        }
        [Fact]
        public async Task ItemServices_AddItem_ReturnNewItemId()
        {
            //Arrange
            var data = new AddItem
            {
                UserId = 1,
                ItemName = "Test item",
                ItemDescription = "",
                PartNumber = "test_item",
                Eans = ["215124532"]
            };

            //Act
            var result = await _itemServices.AddItem(data);

            //Assert
            result.Should().BeGreaterThan(0);
            result.Should().BePositive();
        }
        [Fact]
        public async Task ItemServices_UpdateItem_ReturnTrue()
        {
            //Arrange
            var data = new UpdateItem
            {
                UserId = 1,
                Id = 1,
                ItemDescription = "Test.",
                Eans = ["1256156153132"]
            };

            //Act
            var result = await _itemServices.UpdateItem(data);

            //Assert
            result.Should().BeTrue();
        }
        [Fact]
        public async Task ItemServices_ChangeBindings_ReturnTrue()
        {
            //Arrange
            var data = new List<ModifyBinding>
            {
                new()
                {
                    UserId = 1,
                    InvoiceId = 4,
                    ItemId = 2,
                    Qty = -2
                },
                new()
                {
                    UserId = 3,
                    InvoiceId = 4,
                    ItemId = 2,
                    Qty = 2
                }
            };

            //Act
            var result = await _itemServices.ChangeBindings(data);

            //Assert
            result.Should().BeTrue();
        }
        [Fact]
        public async Task ItemServices_ItemHaveRelations_ItemHaveRelation_ReturnTrue()
        {
            //Arrange
            int itemId = 1;

            //Act
            var result = await _itemServices.ItemHaveRelations(itemId);

            //Assert
            result.Should().BeTrue();
        }
        [Fact]
        public async Task ItemServices_ItemHaveRelations_ItemDoNotHaveRelation_ReturnFalse()
        {
            //Arrange
            int itemId = 11;

            //Act
            var result = await _itemServices.ItemHaveRelations(itemId);

            //Assert
            result.Should().BeFalse();
        }
    }
}
