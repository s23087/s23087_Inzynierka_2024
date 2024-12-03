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
    public class UserServicesTest
    {
        private readonly UserServices _userServices;
        private readonly IConfiguration _configuration;
        public UserServicesTest()
        {
            _configuration = new ConfigurationBuilder().AddUserSecrets("955b92d9-13d8-44b0-b618-19483bfe8b68").Build();
            var testDbConnectionString = _configuration["testDb"];
            var contextOptions = new DbContextOptionsBuilder<HandlerContext>()
                .UseSqlServer(testDbConnectionString);
            var context = new HandlerContext(contextOptions.Options, _configuration, A.Fake<IHttpContextAccessor>());
            _userServices = new UserServices(context, A.Fake<ILogger<UserServices>>());
        }
        [Fact]
        public async Task UserServices_AddUser_ReturnTrue()
        {
            //Arrange
            var data = new AddUser
            {
                Email = "test@test.pl",
                Username = "Test",
                Surname = "Test",
                Password = "Test",
                RoleName = null
            };
            int orgId = 1;
            int roleId = 3;
            bool isOrg = true;

            //Act
            var result = await _userServices.AddUser(data, orgId, roleId, isOrg);

            //Assert
            result.Should().BeTrue();
        }
        [Fact]
        public async Task UserServices_UserExist_EmailExist_ReturnTrue()
        {
            //Arrange
            string email = "test@handler.b2b.com";

            //Act
            var result = await _userServices.UserExist(email);

            //Assert
            result.Should().BeTrue();
        }
        [Fact]
        public async Task UserServices_UserExist_EmailNotFound_ReturnFalse()
        {
            //Arrange
            string email = "";

            //Act
            var result = await _userServices.UserExist(email);

            //Assert
            result.Should().BeFalse();
        }
        [Fact]
        public async Task UserServices_UserExist_UserIdExist_ReturnTrue()
        {
            //Arrange
            int userId = 1;

            //Act
            var result = await _userServices.UserExist(userId);

            //Assert
            result.Should().BeTrue();
        }
        [Fact]
        public async Task UserServices_UserExist_UserIdNotFound_ReturnFalse()
        {
            //Arrange
            int userId = 100;

            //Act
            var result = await _userServices.UserExist(userId);

            //Assert
            result.Should().BeFalse();
        }
        [Fact]
        public async Task UserServices_IsOrgUser_WithEmailPassed_ReturnTrue()
        {
            //Arrange
            string email = "test@handler.b2b.com";

            //Act
            var result = await _userServices.UserExist(email);

            //Assert
            result.Should().BeTrue();
        }
        [Fact]
        public async Task UserServices_IsOrgUser_WithUserIdPassed_ReturnTrue()
        {
            //Arrange
            int userId = 1;

            //Act
            var result = await _userServices.UserExist(userId);

            //Assert
            result.Should().BeTrue();
        }
        [Fact]
        public async Task UserServices_VerifyUserPassword_WithEmailPassed_ReturnTrue()
        {
            //Arrange
            string email = "test@handler.b2b.com";
            string password = "test";

            //Act
            var result = await _userServices.VerifyUserPassword(email, password);

            //Assert
            result.Should().BeTrue();
        }
        [Fact]
        public async Task UserServices_VerifyUserPassword_WithUserIdPassed_ReturnTrue()
        {
            //Arrange
            int userId = 1;
            string password = "test";

            //Act
            var result = await _userServices.VerifyUserPassword(userId, password);

            //Assert
            result.Should().BeTrue();
        }
        [Fact]
        public async Task UserServices_ModifyPassword_ReturnTrue()
        {
            //Arrange
            int userId = 2;
            string password = "test";

            //Act
            var result = await _userServices.ModifyPassword(userId, password);

            //Assert
            result.Should().BeTrue();
        }
        [Fact]
        public async Task UserServices_ModifyUserData_ReturnTrue()
        {
            //Arrange
            int userId = 3;
            string email = "test.merchant@handler.b2b.com";

            //Act
            var result = await _userServices.ModifyUserData(userId, email, null, null);

            //Assert
            result.Should().BeTrue();
        }
        [Fact]
        public async Task UserServices_EmailExist_ReturnTrue()
        {
            //Arrange
            string email = "test@handler.b2b.com";

            //Act
            var result = await _userServices.EmailExist(email);

            //Assert
            result.Should().BeTrue();
        }
        [Fact]
        public async Task UserServices_EmailExist_NotFound_ReturnFalse()
        {
            //Arrange
            string email = "";

            //Act
            var result = await _userServices.EmailExist(email);

            //Assert
            result.Should().BeFalse();
        }
        [Fact]
        public async Task UserServices_ModifyUserRole_ReturnTrue()
        {
            //Arrange
            int orgUserId = 1;
            int roleId = 1;

            //Act
            var result = await _userServices.ModifyUserRole(orgUserId, roleId);

            //Assert
            result.Should().BeTrue();
        }
    }
}
