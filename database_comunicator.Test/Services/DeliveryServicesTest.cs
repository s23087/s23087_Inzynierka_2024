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
    public class DeliveryServicesTest
    {
        private readonly DeliveryServices _deliveryServices;
        private readonly IConfiguration _configuration;
        public DeliveryServicesTest()
        {
            _configuration = new ConfigurationBuilder().AddUserSecrets("955b92d9-13d8-44b0-b618-19483bfe8b68").Build();
            var testDbConnectionString = _configuration["testDb"];
            var contextOptions = new DbContextOptionsBuilder<HandlerContext>()
                .UseSqlServer(testDbConnectionString);
            var context = new HandlerContext(contextOptions.Options, _configuration, A.Fake<IHttpContextAccessor>());
            _deliveryServices = new DeliveryServices(context, A.Fake<ILogger<DeliveryServices>>());
        }
        [Fact]
        public async Task DeliveryServices_DoesDeliveryCompanyExist_ReturnTrue()
        {
            //Arrange
            string companyName = "DPD";

            //Act
            var result = await _deliveryServices.DoesDeliveryCompanyExist(companyName);

            //Assert
            result.Should().BeTrue();
        }
        [Fact]
        public async Task DeliveryServices_DoesDeliveryCompanyExist_ReturnFalse()
        {
            //Arrange
            string companyName = "abc";

            //Act
            var result = await _deliveryServices.DoesDeliveryCompanyExist(companyName);

            //Assert
            result.Should().BeFalse();
        }
        [Fact]
        public async Task DeliveryServices_AddDeliveryCompany_ReturnTrue()
        {
            //Arrange
            string companyName = "DHL";

            //Act
            var result = await _deliveryServices.AddDeliveryCompany(companyName);

            //Assert
            result.Should().BeTrue();
        }
        [Fact]
        public async Task DeliveryServices_AddDelivery_ReturnNewDeliveryId()
        {
            //Arrange
            var data = new AddDelivery
            {
                UserId = 1,
                IsDeliveryToUser = true,
                EstimatedDeliveryDate = DateTime.UtcNow,
                ProformaId = 4,
                CompanyId = 1,
                Waybills = ["4235235253"],
            };

            //Act
            var result = await _deliveryServices.AddDelivery(data);

            //Assert
            result.Should().BeGreaterThan(0);
            result.Should().BePositive();
        }
        [Fact]
        public async Task DeliveryServices_DeliveryProformaExist_ReturnTrue()
        {
            //Arrange
            int proformaId = 1;

            //Act
            var result = await _deliveryServices.DeliveryProformaExist(proformaId);

            //Assert
            result.Should().BeTrue();
        }
        [Fact]
        public async Task DeliveryServices_DeliveryProformaExist_ReturnFalse()
        {
            //Arrange
            int proformaId = 20;

            //Act
            var result = await _deliveryServices.DeliveryProformaExist(proformaId);

            //Assert
            result.Should().BeFalse();
        }
        [Fact]
        public async Task DeliveryServices_DeliveryExist_ReturnTrue()
        {
            //Arrange
            int deliveryId = 1;

            //Act
            var result = await _deliveryServices.DeliveryExist(deliveryId);

            //Assert
            result.Should().BeTrue();
        }
        [Fact]
        public async Task DeliveryServices_DeliveryExist_ReturnFalse()
        {
            //Arrange
            int deliveryId = 200;

            //Act
            var result = await _deliveryServices.DeliveryExist(deliveryId);

            //Assert
            result.Should().BeFalse();
        }
        [Fact]
        public async Task DeliveryServices_DeleteDelivery_ReturnTrue()
        {
            //Arrange
            int deliveryId = 3;

            //Act
            var result = await _deliveryServices.DeleteDelivery(deliveryId);

            //Assert
            result.Should().BeTrue();
        }
        [Fact]
        public async Task DeliveryServices_AddNote_ReturnTrue()
        {
            //Arrange
            var data = new AddNote
            {
                DeliveryId = 1,
                UserId = 1,
                Note = "test",
            };

            //Act
            var result = await _deliveryServices.AddNote(data);

            //Assert
            result.Should().BeTrue();
        }
        [Fact]
        public async Task DeliveryServices_SetDeliveryStatus_ChangeToFulfilled_ReturnTrue()
        {
            //Arrange
            var data = new SetDeliveryStatus
            {
                StatusName = "Fulfilled",
                DeliveryId = 1
            };

            //Act
            var result = await _deliveryServices.SetDeliveryStatus(data);

            //Assert
            result.Should().BeTrue();
        }
        [Fact]
        public async Task DeliveryServices_SetDeliveryStatus_ChangeToInTransport_ReturnTrue()
        {
            //Arrange
            var data = new SetDeliveryStatus
            {
                StatusName = "In transport",
                DeliveryId = 1
            };

            //Act
            var result = await _deliveryServices.SetDeliveryStatus(data);

            //Assert
            result.Should().BeTrue();
        }
        [Fact]
        public async Task DeliveryServices_ModifyDelivery_ReturnTrue()
        {
            //Arrange
            var data = new ModifyDelivery
            {
                DeliveryId = 1,
                Waybills = ["654564564"]
            };

            //Act
            var result = await _deliveryServices.ModifyDelivery(data);

            //Assert
            result.Should().BeTrue();
        }
    }
}
