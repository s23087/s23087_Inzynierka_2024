﻿using database_comunicator.Models.DTOs;
using database_comunicator.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace database_comunicator.Controllers
{
    [Route("{db_name}/[controller]")]
    [ApiController]
    public class RegistrationController : ControllerBase
    {
        private readonly IRegistrationServices _registrationServices;
        public RegistrationController(IRegistrationServices registrationServices)
        {
            _registrationServices = registrationServices;
        }

        [HttpGet]
        [Route("countries")]
        public async Task<IActionResult> GetCountries()
        {
            var countries = await _registrationServices.getCountriesNames();

            return Ok(countries.Select(e => new GetCountries
            {
                CountryName = e.CountryName
            }));
        }
    }
}