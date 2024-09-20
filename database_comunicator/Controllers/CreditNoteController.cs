using database_comunicator.Models.DTOs;
using database_comunicator.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace database_comunicator.Controllers
{
    [Route("{db_name}/[controller]")]
    [ApiController]
    public class CreditNoteController : ControllerBase
    {
        private readonly ICreditNoteServices _creditNoteServices;
        private readonly IUserServices _userServices;
        public CreditNoteController(ICreditNoteServices creditNoteServices, IUserServices userServices)
        {
            _creditNoteServices = creditNoteServices;
            _userServices = userServices;

        }
        [HttpPost]
        [Route("addYoursCreditNote")]
        public async Task<IActionResult> AddYoursCreditNote(AddCreditNote data)
        {
            await _creditNoteServices.AddYoursCreditNote(data);
            return Ok();
        }
        [HttpGet]
        [Route("getCreditNote/{isYourInvoice}/{userId}")]
        public async Task<IActionResult> GetUserCreditNote(bool isYourInvoice, int userId, string? search)
        {
            var exist = await _userServices.UserExist(userId);
            if (!exist) return NotFound();
            if (search != null)
            {
                var result = await _creditNoteServices.GetCreditNotes(isYourInvoice, search, userId);
                return Ok(result);
            } else
            {
                var result = await _creditNoteServices.GetCreditNotes(isYourInvoice, userId);
                return Ok(result);
            }
        }
        [HttpGet]
        [Route("getCreditNote/{isYourInvoice}")]
        public async Task<IActionResult> GeCreditNote(bool isYourInvoice, string? search)
        {
            if (search != null)
            {
                var result = await _creditNoteServices.GetCreditNotes(isYourInvoice, search);
                return Ok(result);
            }
            else
            {
                var result = await _creditNoteServices.GetCreditNotes(isYourInvoice);
                return Ok(result);
            }
        }
    }
}
