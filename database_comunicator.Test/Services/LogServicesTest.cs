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
    public class LogServicesTest
    {
        private readonly LogServices _logServices;
        private readonly IConfiguration _configuration;
        private readonly ILogger<LogServices> _logger;
        public LogServicesTest()
        {
            _logger = A.Fake<ILogger<LogServices>>();
            _configuration = new ConfigurationBuilder().AddUserSecrets("955b92d9-13d8-44b0-b618-19483bfe8b68").Build();
            var testDbConnectionString = _configuration["testDb"];
            var contextOptions = new DbContextOptionsBuilder<HandlerContext>()
                .UseSqlServer(testDbConnectionString);
            var context = new HandlerContext(contextOptions.Options, _configuration, A.Fake<IHttpContextAccessor>());
            _logServices = new LogServices(context, _logger);
        }
        [Fact]
        public async Task LogServices_CreateActionLog_NoErrorThrown()
        {
            //Arrange
            string description = "test log";
            int userId = 1;
            int typeId = 1;

            //Act
            await _logServices.CreateActionLog(description, userId, typeId);

            //Assert
            A.CallTo(_logger).MustNotHaveHappened();
        }
        [Fact]
        public async Task LogServices_CreateActionLog_UserNotFound_ErrorThrown()
        {
            //Arrange
            string description = "test log";
            int userId = 200;
            int typeId = 1;

            //Act
            await _logServices.CreateActionLog(description, userId, typeId);

            //Assert
            A.CallTo(_logger).MustHaveHappenedOnceExactly();
        }
    }
}
