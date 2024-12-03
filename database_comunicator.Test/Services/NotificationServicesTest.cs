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
    public class NotificationServicesTest
    {
        private readonly NotificationServices _notificationServices;
        private readonly IConfiguration _configuration;
        private readonly ILogger<NotificationServices> _logger;
        public NotificationServicesTest()
        {
            _logger = A.Fake<ILogger<NotificationServices>>();
            _configuration = new ConfigurationBuilder().AddUserSecrets("955b92d9-13d8-44b0-b618-19483bfe8b68").Build();
            var testDbConnectionString = _configuration["testDb"];
            var contextOptions = new DbContextOptionsBuilder<HandlerContext>()
                .UseSqlServer(testDbConnectionString);
            var context = new HandlerContext(contextOptions.Options, _configuration, A.Fake<IHttpContextAccessor>());
            _notificationServices = new NotificationServices(context, _logger);
        }
        [Fact]
        public async Task NotificationServices_CreateNotification_ReturnNewNotificationId()
        {
            //Arrange
            var data = new CreateNotification
            {
                UserId = 1,
                Info = "Test notification.",
                ObjectType = "User",
                Referance = "1",
            };

            //Act
            var result = await _notificationServices.CreateNotification(data);

            //Assert
            result.Should().BeGreaterThan(0);
            result.Should().BePositive();
        }
        [Fact]
        public async Task NotificationServices_CreateNotification_ObjectTypeIncorrect_ReturnFalse()
        {
            //Arrange
            var data = new CreateNotification
            {
                UserId = 1,
                Info = "Test notification.",
                ObjectType = "",
                Referance = "1",
            };

            //Act
            var result = await _notificationServices.CreateNotification(data);

            //Assert
            result.Should().Be(-1);
        }
        [Fact]
        public async Task NotificationServices_SetIsRead_ReturnTrue()
        {
            //Arrange
            int notifId = 1;
            bool isRead = true;

            //Act
            var result = await _notificationServices.SetIsRead(notifId, isRead);

            //Assert
            result.Should().BeTrue();
        }
        [Fact]
        public async Task NotificationServices_NotificationExists_ReturnTrue()
        {
            //Arrange
            int notifId = 1;

            //Act
            var result = await _notificationServices.NotificationExists(notifId);

            //Assert
            result.Should().BeTrue();
        }
    }
}
