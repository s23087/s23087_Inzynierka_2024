using database_comunicator.Models.DTOs;
using database_comunicator.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace database_comunicator.Controllers
{
    [Route("{db_name}/[controller]")]
    [ApiController]
    public class RegistrationController : ControllerBase
    {
        private readonly IRegistrationServices _registrationServices;
        private readonly IOrganizationServices _organizationServices;
        private readonly IUserServices _userServices;
        private readonly IRolesServices _rolesServices;
        public RegistrationController(IRegistrationServices registrationServices, IOrganizationServices organizationServices, IUserServices userServices, IRolesServices rolesServices)
        {
            _registrationServices = registrationServices;
            _organizationServices = organizationServices;
            _userServices = userServices;
            _rolesServices = rolesServices;
        }

        [HttpGet]
        [Route("countries")]
        public async Task<IActionResult> GetCountries()
        {
            var countries = await _registrationServices.getCountries();

            return Ok(countries.Select(e => new GetCountries
            {
                Id = e.CountryId,
                CountryName = e.CountryName
            }));
        }

        [HttpPost]
        [Route("/template/[controller]/createDb")]
        public async Task<IActionResult> CreateDb(string orgName)
        {
            bool result = await _registrationServices.CreateNewDatabase(orgName);

            if (result)
            {
                return Ok();
            }

            return BadRequest();
        }

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
        [HttpPost]
        [Route("registerUser")]
        public async Task<IActionResult> RegisterUser(RegisterUser newUser)
        {
            bool countryExist = await _organizationServices.CountryExist(newUser.Country);
            if (!countryExist) { return BadRequest(); }
            int countryId = await _organizationServices.GetCountryId(newUser.Country);
            int orgId = await _organizationServices.AddOrganization(new AddOrganization
            {
                OrgName = newUser.OrgName,
                Nip = newUser.Nip,
                Street = newUser.Street,
                City = newUser.City,
                PostalCode = newUser.PostalCode,
                CreditLimit = null,
                CountryId = countryId
            });

            int roleId = -1;

            if (newUser.IsOrg)
            {
                roleId = await _rolesServices.GetRoleId("Admin");
            }

            var result = await _userServices.AddUser(new AddUser
            {
                Email = newUser.Email,
                Username = newUser.Username,
                Surname = newUser.Username,
                Password = newUser.Password
            }, orgId, roleId, newUser.IsOrg);

            return result ? Ok() : BadRequest();



        }
    }
}
