using database_comunicator.Models.DTOs;
using database_comunicator.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace database_comunicator.Controllers
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
        [Route("clients")]
        public async Task<IActionResult> GetClients(int userId, string? search, string? sort, int? country)
        {
            var isOrg = await _userServices.IsOrgUser(userId);
            var orgId = await _userServices.GetOrgId(userId, isOrg);
            IEnumerable<GetClient> result;
            if (search == null)
            {
                result = await _organizationServices.GetClients(orgId, sort: sort, country);
                return Ok(result);
            }
            result = await _organizationServices.GetClients(orgId, search, sort: sort, country);
            return Ok(result);
        }
        [HttpGet]
        [Route("orgClients")]
        public async Task<IActionResult> GetOrgClients(int userId, string? search, string? sort, int? country)
        {
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
        [Route("restInfoOrg")]
        public async Task<IActionResult> GetRestInfoOrg(int orgId)
        {
            var result = await _organizationServices.GetClientsRestInfo(orgId);
            return Ok(result);
        }
        [HttpGet]
        [Route("availabilityStatuses")]
        public async Task<IActionResult> GetAvailabilityStatuses()
        {
            var result = await _organizationServices.GetAvailabilityStatuses();
            return Ok(result);
        }
        [HttpPost]
        [Route("addAvailabilityStatuses")]
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
        [Route("setUserClientBindings")]
        public async Task<IActionResult> SetUserClientBindings(SetUserClientBindings data, int userId)
        {
            var exist = await _userServices.UserExist(userId);
            if (!exist) return NotFound("User do not exist.");
            var orgExist = await _organizationServices.OrgExist(data.OrgId);
            if (!orgExist) return NotFound("Org do not exist.");
            var usersExist = await _organizationServices.ManyUserExist(data.UsersId);
            if (!usersExist) return NotFound("One of user id do not exist.");
            var logTypeId = await _logServices.getLogTypeId("Modify");
            await _logServices.CreateActionLog($"User bindings for client with id {data.OrgId} has been changed by user id {userId}.", userId, logTypeId);
            var result = await _organizationServices.SetClientUserBindings(data);
            return result ? Ok() : new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
        [HttpPost]
        [Route("setAvailabilityStatusesToClient")]
        public async Task<IActionResult> SetAvailabilityStatusesToClient(SetAvailabilityStatusesToClient data, int userId)
        {
            var exist = await _userServices.UserExist(userId);
            if (!exist) return NotFound();
            var orgExist = await _organizationServices.OrgExist(data.OrgId);
            if (!orgExist) return NotFound();
            var statusExsits = await _organizationServices.StatusExist(data.StatusId);
            if (!statusExsits) return NotFound();
            var logTypeId = await _logServices.getLogTypeId("Modify");
            await _logServices.CreateActionLog($"Availability Status for client with id {data.OrgId} has been changed by user id {userId}.", userId, logTypeId);
            await _organizationServices.SetClientAvailabilityStatus(data.OrgId, data.StatusId);
            return Ok();
        }
        [HttpPost]
        [Route("modifyOrg")]
        public async Task<IActionResult> ModifyOrg(ModifyOrg data, int userId)
        {
            var exist = await _userServices.UserExist(userId);
            if (!exist) return NotFound();
            var orgExist = await _organizationServices.OrgExist(data.OrgId);
            if (!orgExist) return NotFound();
            await _organizationServices.ModifyOrg(data);
            var logTypeId = await _logServices.getLogTypeId("Modify");
            await _logServices.CreateActionLog($"Organization with id {data.OrgId} had been modified by user with id {userId}.", userId, logTypeId);
            return Ok();
        }
        [HttpPost]
        [Route("addOrg")]
        public async Task<IActionResult> AddOrganization(AddOrganization data, int userId)
        {
            var exist = await _userServices.UserExist(userId);
            if (!exist) return NotFound();
            var orgId = await _organizationServices.AddOrganization(data);
            var logTypeId = await _logServices.getLogTypeId("Create");
            await _logServices.CreateActionLog($"Organization with name {data.OrgName} and id {orgId} had been added by user with id {userId}.", userId, logTypeId);
            return Ok(orgId);
        }
        [HttpGet]
        [Route("getUserClientBindings")]
        public async Task<IActionResult> GetUserClientBindings(int orgId)
        {
            var orgExist = await _organizationServices.OrgExist(orgId);
            if (!orgExist) return NotFound();
            var result = await _organizationServices.GetClientBindings(orgId);
            return Ok(result);
        }
        [HttpDelete]
        [Route("deleteOrg/{orgId}")]
        public async Task<IActionResult> DeleteOrg(int orgId, int userId)
        {
            var exist = await _userServices.UserExist(userId);
            if (!exist) return NotFound();
            var relationExist = await _organizationServices.OrgHaveRelations(orgId);
            if (relationExist) return BadRequest();
            await _organizationServices.DeleteOrg(orgId);
            var logTypeId = await _logServices.getLogTypeId("Delete");
            await _logServices.CreateActionLog($"Organization with  id {orgId} had been deleted by user with id {userId}.", userId, logTypeId);
            return Ok();
        }

    }
}
