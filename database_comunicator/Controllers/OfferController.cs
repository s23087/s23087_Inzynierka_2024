using database_comunicator.Models.DTOs;
using database_comunicator.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace database_comunicator.Controllers
{
    [Route("{db_name}/[controller]")]
    [ApiController]
    public class OfferController : ControllerBase
    {
        private readonly IOfferServices _offerServices;
        private readonly IUserServices _userServices;
        private readonly ILogServices _logServices;
        public OfferController(IOfferServices offerServices, IUserServices userServices, ILogServices logServices)
        {
            _logServices = logServices;
            _offerServices = offerServices;
            _userServices = userServices;
        }
        [HttpPost]
        [Route("add")]
        public async Task<IActionResult> AddOffer(AddOffer data)
        {
            var exist = await _userServices.UserExist(data.UserId);
            if (!exist) return NotFound("User not found.");
            var proExist = await _offerServices.DoesPricelistExist(data.OfferName, data.UserId);
            if (proExist) return BadRequest("This pricelist already exist.");
            var offerId = await _offerServices.CreateOffer(data);
            if (offerId == 0) return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            var logId = await _logServices.getLogTypeId("Create");
            var desc = $"User with id {data.UserId} has created the pricelsit with name {data.OfferName} and id {offerId}.";
            await _logServices.CreateActionLog(desc, data.UserId, logId);
            return Ok();
        }
        [HttpGet]
        [Route("get/{userId}")]
        public async Task<IActionResult> GetYoursProformas(int userId, string? search)
        {
            var exist = await _userServices.UserExist(userId);
            if (!exist) return NotFound("User not found.");
            if (search != null)
            {
                var sResult = await _offerServices.GetPriceLists(userId, search);
                return Ok(sResult);
            }
            var result = await _offerServices.GetPriceLists(userId);
            return Ok(result);
        }
        [HttpDelete]
        [Route("delete/{offerId}")]
        public async Task<IActionResult> DeleteYourProforma(int offerId, int userId)
        {
            var userExist = await _userServices.UserExist(userId);
            if (!userExist) return NotFound("User not found.");
            var exist = await _offerServices.DoesPricelistExist(offerId);
            if (!exist) return NotFound("Pricelist not found.");
            var result = await _offerServices.DeletePricelist(offerId);
            if (!result) return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            var logId = await _logServices.getLogTypeId("Delete");
            var desc = $"User with id {userId} has deleted the pricelist with id {offerId}.";
            await _logServices.CreateActionLog(desc, userId, logId);
            return Ok();
        }
        [HttpGet]
        [Route("get/statues")]
        public async Task<IActionResult> GetStatuses()
        {
            var result = await _offerServices.GetOfferStatuses();
            return Ok(result);
        }
    }
}
