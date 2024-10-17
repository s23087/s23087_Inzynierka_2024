using database_communicator.Models.DTOs;
using database_communicator.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace database_communicator.Controllers
{
    [Route("{db_name}/[controller]")]
    [ApiController]
    public class ClientController : ControllerBase
    {
        private readonly IOrganizationServices _organizationServices;
        private readonly IUserServices _userServices;
        private readonly ILogServices _logServices;
        public ClientController(IOrganizationServices organizationServices, IUserServices userServices, ILogServices logServices)
        {
            _organizationServices = organizationServices;
            _userServices = userServices;
            _logServices = logServices;

        }
        [HttpGet]
        [Route("get/{userId}")]
        public async Task<IActionResult> GetClients(int userId, string? search, string? sort, int? country)
        {
            var exist = await _userServices.UserExist(userId);
            if (!exist) return NotFound();
            var isOrg = await _userServices.IsOrgUser(userId);
            var orgId = await _userServices.GetOrgId(userId, isOrg);
            IEnumerable<GetClient> result;
            if (search == null)
            {
                result = await _organizationServices.GetClients(userId, orgId, sort: sort, country);
                return Ok(result);
            }
            result = await _organizationServices.GetClients(userId, orgId, search, sort: sort, country);
            return Ok(result);
        }
        [HttpGet]
        [Route("get/org/{userId}")]
        public async Task<IActionResult> GetOrgClients(int userId, string? search, string? sort, int? country)
        {
            var exist = await _userServices.UserExist(userId);
            if (!exist) return NotFound();
            var orgId = await _userServices.GetOrgId(userId, true);
            IEnumerable<GetOrgClient> result;
            if (search == null)
            {
                result = await _organizationServices.GetOrgClients(orgId, sort: sort, country);
                return Ok(result);
            }
            result = await _organizationServices.GetOrgClients(orgId, search, sort: sort, country);
            return Ok(result);
        }
        [HttpGet]
        [Route("get/rest/{orgId}")]
        public async Task<IActionResult> GetRestInfoOrg(int orgId)
        {
            var orgExist = await _organizationServices.OrgExist(orgId);
            if (!orgExist) return NotFound("Org do not exist.");
            var result = await _organizationServices.GetClientsRestInfo(orgId);
            return Ok(result);
        }
        [HttpGet]
        [Route("get/availability_statuses")]
        public async Task<IActionResult> GetAvailabilityStatuses()
        {
            var result = await _organizationServices.GetAvailabilityStatuses();
            return Ok(result);
        }
        [HttpPost]
        [Route("add/availability_status/{userId}")]
        public async Task<IActionResult> AddAvailabilityStatuses(AddAvailabilityStatus data, int userId)
        {
            var exist = await _userServices.UserExist(userId);
            if (!exist) return NotFound();
            var logTypeId = await _logServices.getLogTypeId("Create");
            await _logServices.CreateActionLog($"Availability Status with name {data.StatusName} and days {data.DaysForRealization} had been added by user with id {userId}.", userId, logTypeId);
            await _organizationServices.AddAvailabilityStatus(data);
            return Ok();
        }
        [HttpPost]
        [Route("modify/user_client_bindings/{userId}")]
        public async Task<IActionResult> SetUserClientBindings(SetUserClientBindings data, int userId)
        {
            var exist = await _userServices.UserExist(userId);
            if (!exist) return NotFound("Your account does not exists.");
            var orgExist = await _organizationServices.OrgExist(data.OrgId);
            if (!orgExist) return NotFound("Org do not exist.");
            var usersExist = await _organizationServices.ManyUserExist(data.UsersId);
            if (!usersExist) return NotFound("One of user id do not exist.");
            var result = await _organizationServices.SetClientUserBindings(data);
            if (!result) return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            var logTypeId = await _logServices.getLogTypeId("Modify");
            await _logServices.CreateActionLog($"User bindings for client with id {data.OrgId} has been changed by user id {userId}.", userId, logTypeId);
            return Ok();
        }
        [HttpPost]
        [Route("setAvailabilityStatusesToClient/{userId}")]
        public async Task<IActionResult> SetAvailabilityStatusesToClient(SetAvailabilityStatusesToClient data, int userId)
        {
            var exist = await _userServices.UserExist(userId);
            if (!exist) return NotFound("Your account does not exists.");
            var orgExist = await _organizationServices.OrgExist(data.OrgId);
            if (!orgExist) return NotFound("Organization does not exists.");
            var statusExists = await _organizationServices.StatusExist(data.StatusId);
            if (!statusExists) return NotFound("Status does not exists.");
            var result = await _organizationServices.SetClientAvailabilityStatus(data.OrgId, data.StatusId);
            if (!result) return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            var logTypeId = await _logServices.getLogTypeId("Modify");
            await _logServices.CreateActionLog($"Availability Status for client with id {data.OrgId} has been changed by user id {userId}.", userId, logTypeId);
            return Ok();
        }
        [HttpPost]
        [Route("modify/{userId}")]
        public async Task<IActionResult> ModifyOrg(ModifyOrg data, int userId)
        {
            var exist = await _userServices.UserExist(userId);
            if (!exist) return NotFound("Your account does not exists.");
            var orgExist = await _organizationServices.OrgExist(data.OrgId);
            if (!orgExist) return NotFound("Organization does not exists.");
            var result = await _organizationServices.ModifyOrg(data);
            if (!result) return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            var logTypeId = await _logServices.getLogTypeId("Modify");
            await _logServices.CreateActionLog($"Organization with id {data.OrgId} had been modified by user with id {userId}.", userId, logTypeId);
            return Ok();
        }
        [HttpPost]
        [Route("add/{userId}")]
        public async Task<IActionResult> AddOrganization(AddOrganization data, int userId)
        {
            var exist = await _userServices.UserExist(userId);
            if (!exist) return NotFound();
            var orgId = await _organizationServices.AddOrganization(data, userId);
            if (orgId == 0) return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            var logTypeId = await _logServices.getLogTypeId("Create");
            await _logServices.CreateActionLog($"Organization with name {data.OrgName} and id {orgId} had been added by user with id {userId}.", userId, logTypeId);
            return Ok(orgId);
        }
        [HttpGet]
        [Route("get/user_client_bindings/{orgId}")]
        public async Task<IActionResult> GetUserClientBindings(int orgId)
        {
            var orgExist = await _organizationServices.OrgExist(orgId);
            if (!orgExist) return NotFound();
            var result = await _organizationServices.GetClientBindings(orgId);
            return Ok(result);
        }
        [HttpDelete]
        [Route("delete/{orgId}/{userId}")]
        public async Task<IActionResult> DeleteClient(int orgId, int userId)
        {
            var exist = await _userServices.UserExist(userId);
            if (!exist) return NotFound("User not found.");
            var clientExist = await _organizationServices.OrgExist(orgId);
            if (!clientExist) return NotFound("Client not found.");
            var relationExist = await _organizationServices.OrgHaveRelations(orgId, userId);
            if (relationExist) return BadRequest("This organization is included in existing objects (Invoice, proforma etc.).");
            var result = await _organizationServices.DeleteOrg(orgId);
            if (!result) return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            var logTypeId = await _logServices.getLogTypeId("Delete");
            await _logServices.CreateActionLog($"Organization with  id {orgId} had been deleted by user with id {userId}.", userId, logTypeId);
            return Ok();
        }

    }
}
