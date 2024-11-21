using database_communicator.Models.DTOs.Create;
using database_communicator.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace database_communicator.Controllers
{
    /// <summary>
    /// This controller allow to register new organization, first user for database, receive data needed for registration and set up new database. Use db_name parameter to pass the name of database that you want ot connect.
    /// </summary>
    [Route("{db_name}/[controller]")]
    [ApiController]
    public class RegistrationController(IRegistrationServices registrationServices, IOrganizationServices organizationServices, IUserServices userServices, IRolesServices rolesServices) : ControllerBase
    {
        private readonly IRegistrationServices _registrationServices = registrationServices;
        private readonly IOrganizationServices _organizationServices = organizationServices;
        private readonly IUserServices _userServices = userServices;
        private readonly IRolesServices _rolesServices = rolesServices;

        /// <summary>
        /// Tries to receive list of countries from Template database.
        /// </summary>
        /// <returns>200 with the list of <see cref="Models.DTOs.Get.GetCountries"/></returns>
        [HttpGet]
        [Route("countries")]
        public async Task<IActionResult> GetCountries()
        {
            var countries = await _registrationServices.GetCountries();
            return Ok(countries);
        }
        /// <summary>
        /// Create new database using Template database and createDB.sql file.
        /// </summary>
        /// <param name="orgName">New organization name</param>
        /// <returns>200 code when success or 400 when failure</returns>
        [HttpPost]
        [Route("/template/[controller]/createDb/{orgName}")]
        public async Task<IActionResult> CreateDb(string orgName)
        {
            if (orgName.IsNullOrEmpty()) return BadRequest();
            bool result = await _registrationServices.CreateNewDatabase(orgName);

            if (result)
            {
                return Ok();
            }

            return BadRequest();
        }
        /// <summary>
        /// Set up newly created database, by creating tables, indexes and etc.
        /// </summary>
        /// <returns>200 code when success, 400 when failure.</returns>
        [HttpPost]
        [Route("setupDb")]
        public async Task<IActionResult> SetupDb()
        {

            bool result = await _registrationServices.SetupDatabase();

            if (result)
            {
                return Ok();
            }

            return BadRequest();
        }
        /// <summary>
        /// Create user and user organization in new database that has been set up.
        /// </summary>
        /// <param name="newUser">New user and organization data wrapped in <see cref="Models.DTOs.Create.RegisterUser"/> object.</param>
        /// <returns>200 code when success, 400 when failure or 404 when country is not found.</returns>
        [HttpPost]
        [Route("registerUser")]
        public async Task<IActionResult> RegisterUser(RegisterUser newUser)
        {
            bool countryExist = await _organizationServices.CountryExist(newUser.Country);
            if (!countryExist) return NotFound();
            int countryId = await _organizationServices.GetCountryId(newUser.Country);
            var org = new AddOrganization
            {
                OrgName = newUser.OrgName,
                Nip = newUser.Nip,
                Street = newUser.Street,
                City = newUser.City,
                PostalCode = newUser.PostalCode,
                CreditLimit = null,
                CountryId = countryId
            };
            int orgId = await _organizationServices.AddOrganization(org, null);

            if (orgId == 0) return BadRequest();

            int roleId = -1;

            if (newUser.IsOrg)
            {
                roleId = await _rolesServices.GetRoleId("Admin");
            }

            bool result = await _userServices.AddUser(new AddUser
            {
                Email = newUser.Email,
                Username = newUser.Username,
                Surname = newUser.Surname,
                Password = newUser.Password
            }, orgId, roleId, newUser.IsOrg);

            return result ? Ok() : BadRequest();

        }
    }
}
