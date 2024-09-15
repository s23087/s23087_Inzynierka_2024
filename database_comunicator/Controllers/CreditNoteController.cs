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
        public CreditNoteController(ICreditNoteServices creditNoteServices)
        {
            _creditNoteServices = creditNoteServices;
        }
        [HttpPost]
        [Route("addYoursCreditNote")]
        public async Task<IActionResult> AddYoursCreditNote(AddCreditNote data)
        {
            await _creditNoteServices.AddYoursCreditNote(data);
            return Ok();
        }
    }
}
