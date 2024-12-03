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
    public class RequestServicesTest
    {
        private readonly RequestServices _requestServices;
        private readonly IConfiguration _configuration;
        public RequestServicesTest()
        {
            _configuration = new ConfigurationBuilder().AddUserSecrets("955b92d9-13d8-44b0-b618-19483bfe8b68").Build();
            var testDbConnectionString = _configuration["testDb"];
            var contextOptions = new DbContextOptionsBuilder<HandlerContext>()
                .UseSqlServer(testDbConnectionString);
            var context = new HandlerContext(contextOptions.Options, _configuration, A.Fake<IHttpContextAccessor>());
            _requestServices = new RequestServices(context, A.Fake<ILogger<RequestServices>>());
        }
        [Fact]
        public async Task RequestServices_AddRequest_ReturnNewRequestId()
        {
            //Arrange
            var data = new AddRequest
            {
                CreatorId = 1,
                ReceiverId = 2,
                ObjectType = "User",
                Path = "Test",
                Note = "",
                Title = "Test",
            };

            //Act
            var result = await _requestServices.AddRequest(data);

            //Assert
            result.Should().BeGreaterThan(0);
            result.Should().BePositive();
        }
        [Fact]
        public async Task RequestServices_RequestExist_ReturnTrue()
        {
            //Arrange
            int requestId = 1;

            //Act
            var result = await _requestServices.RequestExist(requestId);

            //Assert
            result.Should().BeTrue();
        }
        [Fact]
        public async Task RequestServices_DeleteRequest_ReturnTrue()
        {
            //Arrange
            int requestId = 4;

            //Act
            var result = await _requestServices.DeleteRequest(requestId);

            //Assert
            result.Should().BeTrue();
        }
        [Fact]
        public async Task RequestServices_ModifyRequest_ReturnTrue()
        {
            //Arrange
            var data = new ModifyRequest
            {
                RequestId = 2,
                Title = "Test modify",
            };

            //Act
            var result = await _requestServices.ModifyRequest(data);

            //Assert
            result.Should().BeTrue();
        }
        [Fact]
        public async Task RequestServices_SetRequestStatus_ReturnTrue()
        {
            //Arrange
            int requestId = 3;
            int statusId = 2;
            var data = new SetRequestStatus
            {
                StatusName = "Request cancelled",
            };

            //Act
            var result = await _requestServices.SetRequestStatus(requestId, statusId, data);

            //Assert
            result.Should().BeTrue();
        }
    }
}
