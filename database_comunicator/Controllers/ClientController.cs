using database_communicator.Models.DTOs.Create;
using database_communicator.Models.DTOs.Get;
using database_communicator.Models.DTOs.Modify;
using database_communicator.Services;
using Microsoft.AspNetCore.Mvc;

namespace database_communicator.Controllers
{
    /// <summary>
    /// This controller works on Organization table and few other related tables. Used for receiving, modifying and creating data 
    /// that holds clients and user organization information. Use db_name parameter to pass the name of database that you want ot connect.
    /// </summary>
    [Route("{db_name}/[controller]")]
    [ApiController]
    public class ClientController(IOrganizationServices organizationServices, IUserServices userServices, ILogServices logServices) : ControllerBase
    {
        private readonly IOrganizationServices _organizationServices = organizationServices;
        private readonly IUserServices _userServices = userServices;
        private readonly ILogServices _logServices = logServices;

        /// <summary>
        /// Tries to receive basic information about chosen user clients.
        /// </summary>
        /// <param name="userId">User id that data is specially filtered on.</param>
        /// <param name="search">The phrase searched in clients information. It will check if phrase exist in organization name, city or street.</param>
        /// <param name="sort">Contains parameter that object will be sorted by. Must start with D or A to determine ascending order. Then is follow by name of property.</param>
        /// <param name="country">Id of country that objects will be filtered by.</param>
        /// <returns>200 code with list of <see cref="GetClient"/> describing clients, 400 when sort if given sort is incorrect or 404 error if user was not found.</returns>
        [HttpGet]
        [Route("get/{userId}")]
        public async Task<IActionResult> GetClients(int userId, string? search, string? sort, int? country)
        {
            if (sort != null)
            {
                bool isSortOk = sort.StartsWith('A') || sort.StartsWith('D');
                if (!isSortOk) return BadRequest();
            }
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
        /// <summary>
        /// Tries to receive basic information about clients.
        /// </summary>
        /// <param name="userId">User id for filtering out theirs organization.</param>
        /// <param name="search">The phrase searched in clients information. It will check if phrase exist in organization name, city or street.</param>
        /// <param name="sort">Contains parameter that object will be sorted by. Must start with D or A to determine ascending order. Then is follow by name of property.</param>
        /// <param name="country">Id of country that objects will be filtered by.</param>
        /// <returns>200 code with list of <see cref="GetOrgClient"/> describing clients, 400 when sort if given sort is incorrect or 404 error if user was not found.</returns>
        [HttpGet]
        [Route("get/org/{userId}")]
        public async Task<IActionResult> GetOrgClients(int userId, string? search, string? sort, int? country)
        {
            if (sort != null)
            {
                bool isSortOk = sort.StartsWith('A') || sort.StartsWith('D');
                if (!isSortOk) return BadRequest();
            }
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
        /// <summary>
        /// Tries to receive information that was not passed as basic information in <see cref="GetClients"/> or <see cref="GetOrgClients"/> 
        /// function and are needed to showcase object for user.
        /// </summary>
        /// <param name="orgId">Organization id.</param>
        /// <returns>200 code with object of <see cref="GetClientRestInfo"/> containing information or 404 code if organization was not found</returns>
        [HttpGet]
        [Route("get/rest/{orgId}")]
        public async Task<IActionResult> GetRestInfoOrg(int orgId)
        {
            var orgExist = await _organizationServices.OrgExist(orgId);
            if (!orgExist) return NotFound("Org do not exist.");
            var result = await _organizationServices.GetClientsRestInfo(orgId);
            return Ok(result);
        }
        /// <summary>
        /// Receive information about organization availability statuses.
        /// </summary>
        /// <returns>200 code with list of <see cref="Models.DTOs.Get.GetAvailabilityStatuses"/></returns>
        [HttpGet]
        [Route("get/availability_statuses")]
        public async Task<IActionResult> GetAvailabilityStatuses()
        {
            var result = await _organizationServices.GetAvailabilityStatuses();
            return Ok(result);
        }
        /// <summary>
        /// Create new organization availability status. This action will also create new log entry.
        /// </summary>
        /// <param name="data">New status data wrapped in <see cref="AddAvailabilityStatus"/> object.</param>
        /// <param name="userId">Id of user that's activating this action.</param>
        /// <returns>200 code if status has been created, 500 when not created or 404 code when user was not found.</returns>
        [HttpPost]
        [Route("add/availability_status/{userId}")]
        public async Task<IActionResult> AddAvailabilityStatuses(AddAvailabilityStatus data, int userId)
        {
            var exist = await _userServices.UserExist(userId);
            if (!exist) return NotFound();
            var logTypeId = await _logServices.getLogTypeId("Create");
            await _logServices.CreateActionLog($"Availability Status with name {data.StatusName} and days {data.DaysForRealization} had been added by user with id {userId}.", userId, logTypeId);
            var result = await _organizationServices.AddAvailabilityStatus(data);
            return result ? Ok() : new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
        /// <summary>
        /// Create or delete relationships between client and user. This action will also create new log entry.
        /// </summary>
        /// <param name="data">Contains modified bindings information wrapped in <see cref="odels.DTOs.Modify.SetUserClientBindings"/> object.</param>
        /// <param name="userId">Id of user that's activating this action.</param>
        /// <returns>200 if bindings has been modified or 404 if users or organization do not exist.</returns>
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
        /// <summary>
        /// Change organization availability status. This action will also create new log entry.
        /// </summary>
        /// <param name="data">Data containing new status information wrapped in <see cref="Models.DTOs.Modify.SetAvailabilityStatusesToClient"/> object.</param>
        /// <param name="userId">Id of user that's activating this action.</param>
        /// <returns>200 if action was successful or 404 if user, status or organization was not found.</returns>
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
        /// <summary>
        /// Overwrite organization data with new one. This action will also create new log entry.
        /// </summary>
        /// <param name="data">Object of <see cref="Models.DTOs.Modify.ModifyOrg"/> containing changed organization properties.</param>
        /// <param name="userId">Id of user that's activating this action.</param>
        /// <returns>200 code if modification was successful, 500 if not or 404 if user or organization was not found.</returns>
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
        /// <summary>
        /// Create organization entry in database. This action will also create new log entry.
        /// </summary>
        /// <param name="data">New organization information wrapped in <see cref="Models.DTOs.Create.AddOrganization"/></param>
        /// <param name="userId">Id of user that's activating this action.</param>
        /// <returns>200 code with organization id or 404 when user is not found.</returns>
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
        /// <summary>
        /// Tries to receive information about user - client relations from database.
        /// </summary>
        /// <param name="orgId">Chosen organization id</param>
        /// <returns>200 code with list of <see cref="GetClientBindings"/> or 404 code when organization is not found.</returns>
        [HttpGet]
        [Route("get/user_client_bindings/{orgId}")]
        public async Task<IActionResult> GetUserClientBindings(int orgId)
        {
            var orgExist = await _organizationServices.OrgExist(orgId);
            if (!orgExist) return NotFound();
            var result = await _organizationServices.GetClientBindings(orgId);
            return Ok(result);
        }
        /// <summary>
        /// Delete the chosen organization from database. This action will also create new log entry.
        /// </summary>
        /// <param name="orgId">Organization id to delete.</param>
        /// <param name="userId">Id of user that's activating this action.</param>
        /// <returns>200 code if success, 400 if organization have important existing relations like documents or 404 if organization or user is not found.</returns>
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
