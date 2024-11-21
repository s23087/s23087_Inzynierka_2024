using database_communicator.Services;
using FakeItEasy;
using Xunit;
using database_communicator.Models.DTOs.Get;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;

namespace database_communicator.Controllers.Tests
{
    public class ClientControllerTests
    {
        private readonly IOrganizationServices _organizationServices;
        private readonly IUserServices _userServices;
        private readonly ILogServices _logServices;

        public ClientControllerTests()
        {
            _organizationServices = A.Fake<IOrganizationServices>();
            _userServices = A.Fake<IUserServices>();
            _logServices = A.Fake<ILogServices>();
        }
        [Fact]
        public void ClientController_GetClients_OnlyUserPassed_UserExist_ReturnOK()
        {
            // Arrange
            int userId = 1;
            bool isOrg = true;
            int orgId = 1;
            var clientList = A.Fake<IEnumerable<GetClient>>();
            A.CallTo(() => _userServices.UserExist(userId)).Returns(true);
            A.CallTo(() => _userServices.IsOrgUser(userId)).Returns(isOrg);
            A.CallTo(() => _userServices.GetOrgId(userId, isOrg)).Returns(orgId);
            var controller = new ClientController(_organizationServices, _userServices, _logServices);

            // Act
            var result = controller.GetClients(userId, null, null, null);

            // Assert
            result.Should().BeOfType<IActionResult>();
            result.Should().NotBeNull();
        }
    }
}