using database_comunicator.Models;
using database_comunicator.Models.DTOs;
using database_comunicator.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace database_comunicator.Controllers
{
    [Route("{db_name}/[controller]")]
    [ApiController]
    public class InvoicesController : ControllerBase
    {
        private readonly IInvoiceServices _invoicesService;
        private readonly IUserServices _userServices;
        private readonly IOrganizationServices _organizationServices;
        private readonly IItemServices _itemServices;
        private readonly ILogServices _logServices;
        private readonly INotificationServices _notificationServices;
        public InvoicesController(
            IInvoiceServices invoiceServices, 
            IUserServices userServices,
            IOrganizationServices organizationServices,
            IItemServices itemServices,
            ILogServices logServices,
            INotificationServices notificationServices
            )
        {
            _invoicesService = invoiceServices;
            _userServices = userServices;
            _organizationServices = organizationServices;
            _itemServices = itemServices;
            _logServices = logServices;
            _notificationServices = notificationServices;
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
        [Route("getPurchaseList")]
        public async Task<IActionResult> GetPurchaseList()
        {
            var result = await _invoicesService.GetPurchaseInvoicesList();
            return Ok(result);
        }
        [HttpGet]
        [Route("getSalesList")]
        public async Task<IActionResult> GetSalesList()
        {
            var result = await _invoicesService.GetSalesInvoicesList();
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
        [Route("getPurchaseInvoiceItems/{invoiceId}")]
        public async Task<IActionResult> GetPurchaseInvoiceItems(int invoiceId)
        {
            var invoiceExist = await _invoicesService.InvoiceExist(invoiceId);
            if (!invoiceExist) return NotFound("Invoice not found.");
            var result = await _invoicesService.GetInvoiceItems(invoiceId, true);
            return Ok(result);
        }
        [HttpGet]
        [Route("getSalesInvoiceItems/{invoiceId}")]
        public async Task<IActionResult> GetSalesInvoiceItems(int invoiceId)
        {
            var invoiceExist = await _invoicesService.InvoiceExist(invoiceId);
            if (!invoiceExist) return NotFound("Invoice not found.");
            var result = await _invoicesService.GetInvoiceItems(invoiceId, false);
            return Ok(result);
        }
        [HttpGet]
        [Route("getOrgs/{userId}")]
        public async Task<IActionResult> GetOrgs(int userId)
        {
            var exist = await _userServices.UserExist(userId);
            if (!exist) return NotFound("User not found.");
            var result = await _invoicesService.GetOrgsForInvocie(userId);
            return Ok(result);
        }
        [HttpGet]
        [Route("getPurchaseInvoices")]
        public async Task<IActionResult> GetPurchaseInvoices(int userId, string? search)
        {
            IEnumerable<GetInvoices> result;
            var exist = await _userServices.UserExist(userId);
            if (!exist) return NotFound("User not found.");
            if (search != null)
            {
                result = await _invoicesService.GetPurchaseInvoices(userId, search);
                return Ok(result);
            }
            result = await _invoicesService.GetPurchaseInvoices(userId);
            return Ok(result);
        }
        [HttpGet]
        [Route("getPurchaseInvoicesOrg")]
        public async Task<IActionResult> GetPurchaseInvoicesOrg(string? search)
        {
            IEnumerable<GetInvoices> result;
            if (search != null)
            {
                result = await _invoicesService.GetPurchaseInvoices(search);
                return Ok(result);
            }
            result = await _invoicesService.GetPurchaseInvoices();
            return Ok(result);
        }
        [HttpGet]
        [Route("getSalesInvoices")]
        public async Task<IActionResult> GetSalesInvoices(int userId, string? search)
        {
            IEnumerable<GetInvoices> result;
            var exist = await _userServices.UserExist(userId);
            if (!exist) return NotFound("User not found.");
            if (search != null)
            {
                result = await _invoicesService.GetSalesInvocies(userId, search);
                return Ok(result);
            }
            result = await _invoicesService.GetSalesInvocies(userId);
            return Ok(result);
        }
        [HttpGet]
        [Route("getSalesInvoicesOrg")]
        public async Task<IActionResult> GetSalesInvoicesOrg(string? search)
        {
            IEnumerable<GetInvoices> result;
            if (search != null)
            {
                result = await _invoicesService.GetSalesInvocies(search);
                return Ok(result);
            }
            result = await _invoicesService.GetSalesInvocies();
            return Ok(result);
        }
        [HttpPost]
        [Route("addPurchaseInvoice")]
        public async Task<IActionResult> AddPurchaseInvoice(AddPurchaseInvoice data, int userId)
        {
            var exist = await _userServices.UserExist(data.UserId);
            if (!exist) return NotFound("User not found.");
            var orgExist = await _organizationServices.OrgExist(data.Seller);
            if (!orgExist) return NotFound("Seller not found.");
            foreach (var itemId in data.InvoiceItems.Select(e => e.ItemId))
            {
                var itemExist = await _itemServices.ItemExist(itemId);
                if (!itemExist)
                {
                    return NotFound($"Item with id {itemId} do not exist,");
                }
            }
            var invoiceId = await _invoicesService.AddPurchaseInvoice(data);
            if (invoiceId == 0) return BadRequest("Could not create the document.");
            var logId = await _logServices.getLogTypeId("Create");
            var desc = $"User with id {userId} has created the invoice {data.InvoiceNumber} for user with id {data.UserId}.";
            await _logServices.CreateActionLog(desc, userId, logId);
            if (data.UserId != userId)
            {
                var userFull = await _userServices.GetUserFullName(userId);
                await _notificationServices.CreateNotification(new CreateNotification
                {
                    UserId = data.UserId,
                    Info = $"The purchase invoice with number {data.InvoiceNumber} has been added by {userFull}.",
                    ObjectType = "Yours invoices",
                    Referance = $"{invoiceId}"
                });
            }
            return Ok();
        }
        [HttpPost]
        [Route("addSalesInvoice")]
        public async Task<IActionResult> AddSalesInvoice(AddSalesInvoice data, int userId)
        {
            var exist = await _userServices.UserExist(data.UserId);
            if (!exist) return NotFound("User not found.");
            var orgExist = await _organizationServices.OrgExist(data.Buyer);
            if (!orgExist) return NotFound("Buyer not found.");
            foreach (var itemId in data.InvoiceItems.Select(e => e.ItemId))
            {
                var itemExist = await _itemServices.ItemExist(itemId);
                if (!itemExist)
                {
                    return NotFound($"Item with id {itemId} do not exist,");
                }
            }
            var invoiceId = await _invoicesService.AddSalesInvoice(data);
            if (invoiceId == 0) return BadRequest("Could not create the document.");
            var logId = await _logServices.getLogTypeId("Create");
            var desc = $"User with id {userId} has created the invoice {data.InvoiceNumber} for user with id {data.UserId}.";
            await _logServices.CreateActionLog(desc, userId, logId);
            if (data.UserId != userId)
            {
                var userFull = await _userServices.GetUserFullName(userId);
                await _notificationServices.CreateNotification(new CreateNotification
                {
                    UserId = data.UserId,
                    Info = $"The sales invoice with number {data.InvoiceNumber} has been added by {userFull}.",
                    ObjectType = "Sales invoices",
                    Referance = $"{invoiceId}"
                });
            }
            return Ok();
        }
        [HttpGet]
        [Route("allItems")]
        public async Task<IActionResult> GetAllItems()
        {
            var result = await _itemServices.GetItemList();
            return Ok(result);
        }
        [HttpGet]
        [Route("allSalesItems/{userId}")]
        public async Task<IActionResult> GetAllSalesItems(int userId, string currency)
        {
            var exist = await _userServices.UserExist(userId);
            if (!exist) return NotFound("User not found.");
            var result = await _itemServices.GetSalesItemList(userId, currency);
            return Ok(result);
        }
        [HttpDelete]
        [Route("deleteInvoice/{invoiceId}")]
        public async Task<IActionResult> DeleteInvoice(int invoiceId, int userId, bool isYourInvoice)
        {
            var exist = await _userServices.UserExist(userId);
            if (!exist) return NotFound("User not found.");
            var invoiceExist = await _invoicesService.InvoiceExist(invoiceId);
            if (!invoiceExist) return NotFound("Invoice not found.");
            var cantBeDeleted = await _invoicesService.CheckIfCreditNoteExist(invoiceId) || await _invoicesService.CheckIfSellingPriceExist(invoiceId);
            if (cantBeDeleted) return BadRequest("That invoice has refereances to other document. Please delete them first.");
            var invoiceUsers = await _invoicesService.GetInvoiceUser(invoiceId);
            var invoiceNumber = await _invoicesService.GetInvoiceNumber(invoiceId);
            var result = await _invoicesService.DeleteInvoice(invoiceId);
            if (result)
            {
                var logId = await _logServices.getLogTypeId("Delete");
                var desc = $"User with id {userId} has deleted the invoice {invoiceNumber}.";
                await _logServices.CreateActionLog(desc, userId, logId);
                var userFull = await _userServices.GetUserFullName(userId);
                foreach (var user in invoiceUsers)
                {
                    if (user == userId)
                    {
                        continue;
                    }
                    await _notificationServices.CreateNotification(new CreateNotification
                    {
                        UserId = user,
                        Info = $"The invoice with number {invoiceNumber} has been deleted by {userFull}.",
                        ObjectType = isYourInvoice ? "Yours invoices" : "Sales invoices",
                        Referance = $"{invoiceId}"
                    });
                }
            }
            return result ? Ok() : new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
        [HttpGet]
        [Route("invoicePath/{invoiceId}")]
        public async Task<IActionResult> GetInvoicePath(int invoiceId)
        {
            var invoiceExist = await _invoicesService.InvoiceExist(invoiceId);
            if (!invoiceExist) return NotFound("Invoice not found.");
            var result = await _invoicesService.GetInvoicePath(invoiceId);
            return Ok(result);
        }
        [HttpGet]
        [Route("rest/purchase/{invoiceId}")]
        public async Task<IActionResult> GetRestPurchaseInvoice(int invoiceId)
        {
            var invoiceExist = await _invoicesService.InvoiceExist(invoiceId);
            if (!invoiceExist) return NotFound("Invoice not found.");
            var result = await _invoicesService.GetRestPurchaseInvoice(invoiceId);
            return Ok(result);
        }
        [HttpGet]
        [Route("rest/sales/{invoiceId}")]
        public async Task<IActionResult> GetRestSalesInvoice(int invoiceId)
        {
            var invoiceExist = await _invoicesService.InvoiceExist(invoiceId);
            if (!invoiceExist) return NotFound("Invoice not found.");
            var result = await _invoicesService.GetRestSalesInvoice(invoiceId);
            return Ok(result);
        }
        [HttpGet]
        [Route("rest/modify/{invoiceId}")]
        public async Task<IActionResult> GetRestModifyInvoice(int invoiceId)
        {
            var invoiceExist = await _invoicesService.InvoiceExist(invoiceId);
            if (!invoiceExist) return NotFound("Invoice not found.");
            var result = await _invoicesService.GetRestModifyInvoice(invoiceId);
            return Ok(result);
        }
        [HttpPost]
        [Route("modify/{userId}")]
        public async Task<IActionResult> ModifyInvoice(ModifyInvoice data, int userId)
        {
            var exist = await _userServices.UserExist(userId);
            if (!exist) return NotFound("User not found.");
            var invoiceExist = await _invoicesService.InvoiceExist(data.InvoiceId);
            if (!invoiceExist) return NotFound("Invoice not found.");
            var result = await _invoicesService.ModifyInvoice(data);
            if (result)
            {
                var invoiceUsers = await _invoicesService.GetInvoiceUser(data.InvoiceId);
                var invoiceNumber = await _invoicesService.GetInvoiceNumber(data.InvoiceId);
                var logId = await _logServices.getLogTypeId("Modify");
                var desc = $"User with id {userId} has modify the invoice {invoiceNumber}.";
                await _logServices.CreateActionLog(desc, userId, logId);
                var userFull = await _userServices.GetUserFullName(userId);
                foreach (var user in invoiceUsers)
                {
                    if (user == userId)
                    {
                        continue;
                    }
                    await _notificationServices.CreateNotification(new CreateNotification
                    {
                        UserId = user,
                        Info = $"The invoice with number {invoiceNumber} has been deleted by {userFull}.",
                        ObjectType = data.IsYourInvoice ? "Yours invoices" : "Sales invoices",
                        Referance = $"{data.InvoiceId}"
                    });
                }
            }
            return result ? Ok() : new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }
}
