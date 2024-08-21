using database_comunicator.Models.DTOs;
using database_comunicator.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace database_comunicator.Controllers
{
    [Route("{db_name}/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserServices _userServices;
        public UserController(IUserServices userServices)
        {
            _userServices = userServices;
        }
        [HttpPost]
        [Route("sign_in")]
        public async Task<IActionResult> SignIn(UserLogin loginInfo)
        {
            var doExist = await _userServices.UserExist(loginInfo.Email);
            if (!doExist) return Unauthorized();
            return await _userServices.VerifyUserPassword(loginInfo.Email, loginInfo.Password) ? Ok() : Unauthorized();
        }
    }
}
