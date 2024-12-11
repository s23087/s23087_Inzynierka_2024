using database_communicator.Controllers;
using database_communicator.Models.DTOs.Create;
using database_communicator.Models.DTOs.Get;
using database_communicator.Models.DTOs.Modify;
using database_communicator.Services;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;

namespace database_communicator_Test.Controllers
{
    public class RequestsControllerTest
    {
        private readonly IRequestServices _requestServices;
        private readonly IUserServices _userServices;
        private readonly ILogServices _logServices;
        private readonly INotificationServices _notificationServices;
        public RequestsControllerTest()
        {
            _requestServices = A.Fake<IRequestServices>();
            _userServices = A.Fake<IUserServices>();
            _logServices = A.Fake<ILogServices>();
            _notificationServices = A.Fake<INotificationServices>();
        }
        [Fact]
        public async Task RequestsController_GetReceivers_ReturnOk()
        {
            //Arrange
            var data = A.Fake<IEnumerable<GetUsers>>();
            A.CallTo(() => _userServices.GetAccountantUser()).Returns(data);
            var controller = new RequestsController(_requestServices, _userServices, _logServices, _notificationServices);

            //Act
            var result = await controller.GetReceivers();

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();
        }
        [Fact]
        public async Task RequestsController_AddRequest_ReturnOk()
        {
            //Arrange
            var data = A.Fake<AddRequest>();
            int requestId = 1;
            data.ReceiverId = 1;
            data.CreatorId = 2;
            int logTypeId = 1;
            string userFull = "";
            A.CallTo(() => _userServices.UserExist(data.CreatorId)).Returns(true);
            A.CallTo(() => _userServices.UserExist(data.ReceiverId)).Returns(true);
            A.CallTo(() => _requestServices.AddRequest(data)).Returns(requestId);
            A.CallTo(() => _logServices.getLogTypeId("Create")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", data.CreatorId, logTypeId));
            A.CallTo(() => _userServices.GetUserFullName(data.CreatorId)).Returns(userFull);
            A.CallTo(() => _notificationServices.CreateNotification(A<CreateNotification>.Ignored));
            var controller = new RequestsController(_requestServices, _userServices, _logServices, _notificationServices);

            //Act
            var result = await controller.AddRequest(data);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkResult>();
        }
        [Fact]
        public async Task RequestsController_AddRequest_AddFails_Return500()
        {
            //Arrange
            var data = A.Fake<AddRequest>();
            int requestId = 0;
            data.ReceiverId = 1;
            data.CreatorId = 2;
            int logTypeId = 1;
            string userFull = "";
            A.CallTo(() => _userServices.UserExist(data.CreatorId)).Returns(true);
            A.CallTo(() => _userServices.UserExist(data.ReceiverId)).Returns(true);
            A.CallTo(() => _requestServices.AddRequest(data)).Returns(requestId);
            A.CallTo(() => _logServices.getLogTypeId("Create")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", data.CreatorId, logTypeId));
            A.CallTo(() => _userServices.GetUserFullName(data.CreatorId)).Returns(userFull);
            A.CallTo(() => _notificationServices.CreateNotification(A<CreateNotification>.Ignored));
            var controller = new RequestsController(_requestServices, _userServices, _logServices, _notificationServices);

            //Act
            var result = await controller.AddRequest(data);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<StatusCodeResult>();
        }
        [Fact]
        public async Task RequestsController_AddRequest_CreatorNotFound_Return404()
        {
            //Arrange
            var data = A.Fake<AddRequest>();
            int requestId = 1;
            data.ReceiverId = 1;
            data.CreatorId = 2;
            int logTypeId = 1;
            string userFull = "";
            A.CallTo(() => _userServices.UserExist(data.CreatorId)).Returns(false);
            A.CallTo(() => _userServices.UserExist(data.ReceiverId)).Returns(true);
            A.CallTo(() => _requestServices.AddRequest(data)).Returns(requestId);
            A.CallTo(() => _logServices.getLogTypeId("Create")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", data.CreatorId, logTypeId));
            A.CallTo(() => _userServices.GetUserFullName(data.CreatorId)).Returns(userFull);
            A.CallTo(() => _notificationServices.CreateNotification(A<CreateNotification>.Ignored));
            var controller = new RequestsController(_requestServices, _userServices, _logServices, _notificationServices);

            //Act
            var result = await controller.AddRequest(data);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundObjectResult>();
        }
        [Fact]
        public async Task RequestsController_AddRequest_ReceiverNotFound_Return404()
        {
            //Arrange
            var data = A.Fake<AddRequest>();
            int requestId = 1;
            data.ReceiverId = 1;
            data.CreatorId = 2;
            int logTypeId = 1;
            string userFull = "";
            A.CallTo(() => _userServices.UserExist(data.CreatorId)).Returns(true);
            A.CallTo(() => _userServices.UserExist(data.ReceiverId)).Returns(false);
            A.CallTo(() => _requestServices.AddRequest(data)).Returns(requestId);
            A.CallTo(() => _logServices.getLogTypeId("Create")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", data.CreatorId, logTypeId));
            A.CallTo(() => _userServices.GetUserFullName(data.CreatorId)).Returns(userFull);
            A.CallTo(() => _notificationServices.CreateNotification(A<CreateNotification>.Ignored));
            var controller = new RequestsController(_requestServices, _userServices, _logServices, _notificationServices);

            //Act
            var result = await controller.AddRequest(data);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundObjectResult>();
        }
        [Fact]
        public async Task RequestsController_GetMyRequests_ReturnOk()
        {
            //Arrange
            var data = A.Fake<IEnumerable<GetRequest>>();
            int userId = 1;
            A.CallTo(() => _userServices.UserExist(userId)).Returns(true);
            A.CallTo(() => _requestServices.GetMyRequests(userId, null, null, null, null, null)).Returns(data);
            var controller = new RequestsController(_requestServices, _userServices, _logServices, _notificationServices);

            //Act
            var result = await controller.GetMyRequests(userId, null, null, null, null, null, null);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();
        }
        [Fact]
        public async Task RequestsController_GetMyRequests_UserNotFound_Return404()
        {
            //Arrange
            var data = A.Fake<IEnumerable<GetRequest>>();
            int userId = 1;
            A.CallTo(() => _userServices.UserExist(userId)).Returns(false);
            A.CallTo(() => _requestServices.GetMyRequests(userId, null, null, null, null, null)).Returns(data);
            var controller = new RequestsController(_requestServices, _userServices, _logServices, _notificationServices);

            //Act
            var result = await controller.GetMyRequests(userId, null, null, null, null, null, null);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundObjectResult>();
        }
        [Fact]
        public async Task RequestsController_GetMyRequests_SortIncorrect_Return400()
        {
            //Arrange
            var data = A.Fake<IEnumerable<GetRequest>>();
            int userId = 1;
            string sort = "";
            A.CallTo(() => _userServices.UserExist(userId)).Returns(true);
            A.CallTo(() => _requestServices.GetMyRequests(userId, null, null, null, null, null)).Returns(data);
            var controller = new RequestsController(_requestServices, _userServices, _logServices, _notificationServices);

            //Act
            var result = await controller.GetMyRequests(userId, null, sort, null, null, null, null);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<BadRequestObjectResult>();
        }
        [Fact]
        public async Task RequestsController_GetReceivedRequests_ReturnOk()
        {
            //Arrange
            var data = A.Fake<IEnumerable<GetRequest>>();
            int userId = 1;
            A.CallTo(() => _userServices.UserExist(userId)).Returns(true);
            A.CallTo(() => _requestServices.GetReceivedRequests(userId, null, null, null, null, null)).Returns(data);
            var controller = new RequestsController(_requestServices, _userServices, _logServices, _notificationServices);

            //Act
            var result = await controller.GetReceivedRequests(userId, null, null, null, null, null, null);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();
        }
        [Fact]
        public async Task RequestsController_GetReceivedRequests_UserNotFound_Return404()
        {
            //Arrange
            var data = A.Fake<IEnumerable<GetRequest>>();
            int userId = 1;
            A.CallTo(() => _userServices.UserExist(userId)).Returns(false);
            A.CallTo(() => _requestServices.GetReceivedRequests(userId, null, null, null, null, null)).Returns(data);
            var controller = new RequestsController(_requestServices, _userServices, _logServices, _notificationServices);

            //Act
            var result = await controller.GetReceivedRequests(userId, null, null, null, null, null, null);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundObjectResult>();
        }
        [Fact]
        public async Task RequestsController_GetReceivedRequests_SortIncorrect_Return400()
        {
            //Arrange
            var data = A.Fake<IEnumerable<GetRequest>>();
            int userId = 1;
            string sort = "";
            A.CallTo(() => _userServices.UserExist(userId)).Returns(true);
            A.CallTo(() => _requestServices.GetReceivedRequests(userId, null, null, null, null, null)).Returns(data);
            var controller = new RequestsController(_requestServices, _userServices, _logServices, _notificationServices);

            //Act
            var result = await controller.GetReceivedRequests(userId, null, sort, null, null, null, null);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<BadRequestObjectResult>();
        }
        [Fact]
        public async Task RequestsController_GetRestRequest_ReturnOk()
        {
            //Arrange
            var data = A.Fake<GetRestRequest>();
            int requestId = 1;
            A.CallTo(() => _requestServices.RequestExist(requestId)).Returns(true);
            A.CallTo(() => _requestServices.GetRestRequest(requestId)).Returns(data);
            var controller = new RequestsController(_requestServices, _userServices, _logServices, _notificationServices);

            //Act
            var result = await controller.GetRestRequest(requestId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();
        }
        [Fact]
        public async Task RequestsController_GetRestRequest_RequestNotFound_Return404()
        {
            //Arrange
            var data = A.Fake<GetRestRequest>();
            int requestId = 1;
            A.CallTo(() => _requestServices.RequestExist(requestId)).Returns(false);
            A.CallTo(() => _requestServices.GetRestRequest(requestId)).Returns(data);
            var controller = new RequestsController(_requestServices, _userServices, _logServices, _notificationServices);

            //Act
            var result = await controller.GetRestRequest(requestId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundObjectResult>();
        }
        [Fact]
        public async Task RequestsController_DeleteRequest_ReturnOk()
        {
            //Arrange
            int userId = 1;
            int receiver = 2;
            int requestId = 1;
            int logTypeId = 1;
            string userFull = "";
            A.CallTo(() => _userServices.UserExist(userId)).Returns(true);
            A.CallTo(() => _requestServices.RequestExist(requestId)).Returns(true);
            A.CallTo(() => _requestServices.GetReceiverId(requestId)).Returns(receiver);
            A.CallTo(() => _requestServices.DeleteRequest(requestId)).Returns(true);
            A.CallTo(() => _logServices.getLogTypeId("Delete")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", userId, logTypeId));
            A.CallTo(() => _userServices.GetUserFullName(userId)).Returns(userFull);
            A.CallTo(() => _notificationServices.CreateNotification(A<CreateNotification>.Ignored));
            var controller = new RequestsController(_requestServices, _userServices, _logServices, _notificationServices);

            //Act
            var result = await controller.DeleteRequest(requestId, userId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkResult>();
        }
        [Fact]
        public async Task RequestsController_DeleteRequest_DeleteFails_Return500()
        {
            //Arrange
            int userId = 1;
            int receiver = 2;
            int requestId = 1;
            int logTypeId = 1;
            string userFull = "";
            A.CallTo(() => _userServices.UserExist(userId)).Returns(true);
            A.CallTo(() => _requestServices.RequestExist(requestId)).Returns(true);
            A.CallTo(() => _requestServices.GetReceiverId(requestId)).Returns(receiver);
            A.CallTo(() => _requestServices.DeleteRequest(requestId)).Returns(false);
            A.CallTo(() => _logServices.getLogTypeId("Delete")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", userId, logTypeId));
            A.CallTo(() => _userServices.GetUserFullName(userId)).Returns(userFull);
            A.CallTo(() => _notificationServices.CreateNotification(A<CreateNotification>.Ignored));
            var controller = new RequestsController(_requestServices, _userServices, _logServices, _notificationServices);

            //Act
            var result = await controller.DeleteRequest(requestId, userId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<StatusCodeResult>();
        }
        [Fact]
        public async Task RequestsController_DeleteRequest_UserNotFound_Return404()
        {
            //Arrange
            int userId = 1;
            int receiver = 2;
            int requestId = 1;
            int logTypeId = 1;
            string userFull = "";
            A.CallTo(() => _userServices.UserExist(userId)).Returns(false);
            A.CallTo(() => _requestServices.RequestExist(requestId)).Returns(true);
            A.CallTo(() => _requestServices.GetReceiverId(requestId)).Returns(receiver);
            A.CallTo(() => _requestServices.DeleteRequest(requestId)).Returns(true);
            A.CallTo(() => _logServices.getLogTypeId("Delete")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", userId, logTypeId));
            A.CallTo(() => _userServices.GetUserFullName(userId)).Returns(userFull);
            A.CallTo(() => _notificationServices.CreateNotification(A<CreateNotification>.Ignored));
            var controller = new RequestsController(_requestServices, _userServices, _logServices, _notificationServices);

            //Act
            var result = await controller.DeleteRequest(requestId, userId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundObjectResult>();
        }
        [Fact]
        public async Task RequestsController_DeleteRequest_RequestNotFound_Return404()
        {
            //Arrange
            int userId = 1;
            int receiver = 2;
            int requestId = 1;
            int logTypeId = 1;
            string userFull = "";
            A.CallTo(() => _userServices.UserExist(userId)).Returns(true);
            A.CallTo(() => _requestServices.RequestExist(requestId)).Returns(false);
            A.CallTo(() => _requestServices.GetReceiverId(requestId)).Returns(receiver);
            A.CallTo(() => _requestServices.DeleteRequest(requestId)).Returns(true);
            A.CallTo(() => _logServices.getLogTypeId("Delete")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", userId, logTypeId));
            A.CallTo(() => _userServices.GetUserFullName(userId)).Returns(userFull);
            A.CallTo(() => _notificationServices.CreateNotification(A<CreateNotification>.Ignored));
            var controller = new RequestsController(_requestServices, _userServices, _logServices, _notificationServices);

            //Act
            var result = await controller.DeleteRequest(requestId, userId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundObjectResult>();
        }
        [Fact]
        public async Task RequestsController_ModifyRequest_ReturnOk()
        {
            //Arrange
            var data = A.Fake<ModifyRequest>();
            int userId = 1;
            data.ReceiverId = 2;
            int logTypeId = 1;
            string userFull = "";
            A.CallTo(() => _userServices.UserExist(userId)).Returns(true);
            A.CallTo(() => _requestServices.RequestExist(data.RequestId)).Returns(true);
            A.CallTo(() => _userServices.UserExist((int)data.ReceiverId)).Returns(true);
            A.CallTo(() => _requestServices.ModifyRequest(data)).Returns(true);
            A.CallTo(() => _logServices.getLogTypeId("Modify")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", userId, logTypeId));
            A.CallTo(() => _userServices.GetUserFullName(userId)).Returns(userFull);
            A.CallTo(() => _notificationServices.CreateNotification(A<CreateNotification>.Ignored));
            var controller = new RequestsController(_requestServices, _userServices, _logServices, _notificationServices);

            //Act
            var result = await controller.ModifyRequest(data, userId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkResult>();
        }
        [Fact]
        public async Task RequestsController_ModifyRequest_ModifyFails_Return500()
        {
            //Arrange
            var data = A.Fake<ModifyRequest>();
            int userId = 1;
            data.ReceiverId = 2;
            int logTypeId = 1;
            string userFull = "";
            A.CallTo(() => _userServices.UserExist(userId)).Returns(true);
            A.CallTo(() => _requestServices.RequestExist(data.RequestId)).Returns(true);
            A.CallTo(() => _userServices.UserExist((int)data.ReceiverId)).Returns(true);
            A.CallTo(() => _requestServices.ModifyRequest(data)).Returns(false);
            A.CallTo(() => _logServices.getLogTypeId("Modify")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", userId, logTypeId));
            A.CallTo(() => _userServices.GetUserFullName(userId)).Returns(userFull);
            A.CallTo(() => _notificationServices.CreateNotification(A<CreateNotification>.Ignored));
            var controller = new RequestsController(_requestServices, _userServices, _logServices, _notificationServices);

            //Act
            var result = await controller.ModifyRequest(data, userId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<StatusCodeResult>();
        }
        [Fact]
        public async Task RequestsController_ModifyRequest_UserNotFound_Return404()
        {
            //Arrange
            var data = A.Fake<ModifyRequest>();
            int userId = 1;
            data.ReceiverId = 2;
            int logTypeId = 1;
            string userFull = "";
            A.CallTo(() => _userServices.UserExist(userId)).Returns(false);
            A.CallTo(() => _requestServices.RequestExist(data.RequestId)).Returns(true);
            A.CallTo(() => _userServices.UserExist((int)data.ReceiverId)).Returns(true);
            A.CallTo(() => _requestServices.ModifyRequest(data)).Returns(true);
            A.CallTo(() => _logServices.getLogTypeId("Modify")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", userId, logTypeId));
            A.CallTo(() => _userServices.GetUserFullName(userId)).Returns(userFull);
            A.CallTo(() => _notificationServices.CreateNotification(A<CreateNotification>.Ignored));
            var controller = new RequestsController(_requestServices, _userServices, _logServices, _notificationServices);

            //Act
            var result = await controller.ModifyRequest(data, userId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundObjectResult>();
        }
        [Fact]
        public async Task RequestsController_ModifyRequest_RequestNotFound_Return404()
        {
            //Arrange
            var data = A.Fake<ModifyRequest>();
            int userId = 1;
            data.ReceiverId = 2;
            int logTypeId = 1;
            string userFull = "";
            A.CallTo(() => _userServices.UserExist(userId)).Returns(true);
            A.CallTo(() => _requestServices.RequestExist(data.RequestId)).Returns(false);
            A.CallTo(() => _userServices.UserExist((int)data.ReceiverId)).Returns(true);
            A.CallTo(() => _requestServices.ModifyRequest(data)).Returns(true);
            A.CallTo(() => _logServices.getLogTypeId("Modify")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", userId, logTypeId));
            A.CallTo(() => _userServices.GetUserFullName(userId)).Returns(userFull);
            A.CallTo(() => _notificationServices.CreateNotification(A<CreateNotification>.Ignored));
            var controller = new RequestsController(_requestServices, _userServices, _logServices, _notificationServices);

            //Act
            var result = await controller.ModifyRequest(data, userId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundObjectResult>();
        }
        [Fact]
        public async Task RequestsController_ModifyRequest_NewReceiverNotFound_Return404()
        {
            //Arrange
            var data = A.Fake<ModifyRequest>();
            int userId = 1;
            data.ReceiverId = 2;
            int logTypeId = 1;
            string userFull = "";
            A.CallTo(() => _userServices.UserExist(userId)).Returns(true);
            A.CallTo(() => _requestServices.RequestExist(data.RequestId)).Returns(true);
            A.CallTo(() => _userServices.UserExist((int)data.ReceiverId)).Returns(false);
            A.CallTo(() => _requestServices.ModifyRequest(data)).Returns(true);
            A.CallTo(() => _logServices.getLogTypeId("Modify")).Returns(logTypeId);
            A.CallTo(() => _logServices.CreateActionLog("", userId, logTypeId));
            A.CallTo(() => _userServices.GetUserFullName(userId)).Returns(userFull);
            A.CallTo(() => _notificationServices.CreateNotification(A<CreateNotification>.Ignored));
            var controller = new RequestsController(_requestServices, _userServices, _logServices, _notificationServices);

            //Act
            var result = await controller.ModifyRequest(data, userId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundObjectResult>();
        }
        [Fact]
        public async Task RequestsController_SetStatus_ReturnOk()
        {
            //Arrange
            var data = A.Fake<SetRequestStatus>();
            int statusId = 1;
            int requestId = 1;
            A.CallTo(() => _requestServices.RequestExist(requestId)).Returns(true);
            A.CallTo(() => _requestServices.GetStatusId(data.StatusName)).Returns(statusId);
            A.CallTo(() => _requestServices.SetRequestStatus(requestId, statusId, data)).Returns(true);
            var controller = new RequestsController(_requestServices, _userServices, _logServices, _notificationServices);

            //Act
            var result = await controller.SetStatus(requestId, data);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkResult>();
        }
        [Fact]
        public async Task RequestsController_SetStatus_SetFails_Return400()
        {
            //Arrange
            var data = A.Fake<SetRequestStatus>();
            int statusId = 1;
            int requestId = 1;
            A.CallTo(() => _requestServices.RequestExist(requestId)).Returns(true);
            A.CallTo(() => _requestServices.GetStatusId(data.StatusName)).Returns(statusId);
            A.CallTo(() => _requestServices.SetRequestStatus(requestId, statusId, data)).Returns(false);
            var controller = new RequestsController(_requestServices, _userServices, _logServices, _notificationServices);

            //Act
            var result = await controller.SetStatus(requestId, data);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<BadRequestResult>();
        }
        [Fact]
        public async Task RequestsController_SetStatus_RequestNotFound_Return404()
        {
            //Arrange
            var data = A.Fake<SetRequestStatus>();
            int statusId = 1;
            int requestId = 1;
            A.CallTo(() => _requestServices.RequestExist(requestId)).Returns(false);
            A.CallTo(() => _requestServices.GetStatusId(data.StatusName)).Returns(statusId);
            A.CallTo(() => _requestServices.SetRequestStatus(requestId, statusId, data)).Returns(true);
            var controller = new RequestsController(_requestServices, _userServices, _logServices, _notificationServices);

            //Act
            var result = await controller.SetStatus(requestId, data);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundObjectResult>();
        }
        [Fact]
        public async Task RequestsController_GetRequestPath_ReturnOk()
        {
            //Arrange
            int requestId = 1;
            A.CallTo(() => _requestServices.RequestExist(requestId)).Returns(true);
            A.CallTo(() => _requestServices.GetRequestPath(requestId)).Returns("");
            var controller = new RequestsController(_requestServices, _userServices, _logServices, _notificationServices);

            //Act
            var result = await controller.GetRequestPath(requestId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();
        }
        [Fact]
        public async Task RequestsController_GetRequestPath_RequestNotFound_Return404()
        {
            //Arrange
            int requestId = 1;
            A.CallTo(() => _requestServices.RequestExist(requestId)).Returns(false);
            A.CallTo(() => _requestServices.GetRequestPath(requestId)).Returns("");
            var controller = new RequestsController(_requestServices, _userServices, _logServices, _notificationServices);

            //Act
            var result = await controller.GetRequestPath(requestId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundObjectResult>();
        }
        [Fact]
        public async Task RequestsController_GetRestModifyRequest_ReturnOk()
        {
            //Arrange
            var data = A.Fake<GetRestModifyRequest>();
            int requestId = 1;
            A.CallTo(() => _requestServices.RequestExist(requestId)).Returns(true);
            A.CallTo(() => _requestServices.GetRestModifyRequest(requestId)).Returns(data);
            var controller = new RequestsController(_requestServices, _userServices, _logServices, _notificationServices);

            //Act
            var result = await controller.GetRestModifyRequest(requestId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();
        }
        [Fact]
        public async Task RequestsController_GetRestModifyRequest_RequestNotFound_Return404()
        {
            //Arrange
            var data = A.Fake<GetRestModifyRequest>();
            int requestId = 1;
            A.CallTo(() => _requestServices.RequestExist(requestId)).Returns(false);
            A.CallTo(() => _requestServices.GetRestModifyRequest(requestId)).Returns(data);
            var controller = new RequestsController(_requestServices, _userServices, _logServices, _notificationServices);

            //Act
            var result = await controller.GetRestModifyRequest(requestId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundObjectResult>();
        }
        [Fact]
        public async Task RequestsController_GetStatuses_ReturnOk()
        {
            //Arrange
            var data = A.Fake<IEnumerable<GetRequestStatus>>();
            A.CallTo(() => _requestServices.GetRequestStatuses()).Returns(data);
            var controller = new RequestsController(_requestServices, _userServices, _logServices, _notificationServices);

            //Act
            var result = await controller.GetStatuses();

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();
        }
    }
}
