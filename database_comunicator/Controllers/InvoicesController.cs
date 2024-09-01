using database_comunicator.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace database_comunicator.Controllers
{
    [Route("{db_name}/[controller]")]
    [ApiController]
    public class InvoicesController : ControllerBase
    {
        private readonly IInvoiceServices _invoicesService;
        private readonly IUserServices _userServices;
        public InvoicesController(IInvoiceServices invoiceServices, IUserServices userServices)
        {
            _invoicesService = invoiceServices;
            _userServices = userServices;
        }
        [HttpGet]
        [Route("getTaxes")]
        public async Task<IActionResult> GetTaxes() { 
            var result = await _invoicesService.GetTaxes();
            return Ok(result);
        }
        [HttpGet]
        [Route("getPaymentStatuses")]
        public async Task<IActionResult> GetPaymentStatuses()
        {
            var result = await _invoicesService.GetPaymentStatuses();
            return Ok(result);
        }
        [HttpGet]
        [Route("getPaymentMethods")]
        public async Task<IActionResult> GetPaymentMethods()
        {
            var result = await _invoicesService.GetPaymentMethods();
            return Ok(result);
        }
        [HttpGet]
        [Route("getOrgs/{userId}")]
        public async Task<IActionResult> GetOrgs(int userId)
        {
            var exist = await _userServices.UserExist(userId);
            if (!exist) return NotFound();
            var result = await _invoicesService.GetOrgsForInvocie(userId);
            return Ok(result);
        }
    }
}
