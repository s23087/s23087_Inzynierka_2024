using database_communicator.Models;
using database_communicator.Models.DTOs;
using database_communicator.Services;
using database_comunicator.FilterClass;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace database_communicator.Controllers
{
    [Route("{db_name}/[controller]")]
    [ApiController]
    public class InvoicesController : ControllerBase
    {
        private const string invoiceNotFoundMessage = "Invoice not found.";
        private const string userNotFoundMessage = "User not found.";
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
        [Route("get/taxes")]
        public async Task<IActionResult> GetTaxes() { 
            var result = await _invoicesService.GetTaxes();
            return Ok(result);
        }
        [HttpGet]
        [Route("get/payment/statuses")]
        public async Task<IActionResult> GetPaymentStatuses()
        {
            var result = await _invoicesService.GetPaymentStatuses();
            return Ok(result);
        }
        [HttpGet]
        [Route("get/purchase/list")]
        public async Task<IActionResult> GetPurchaseList()
        {
            var result = await _invoicesService.GetPurchaseInvoicesList();
            return Ok(result);
        }
        [HttpGet]
        [Route("get/sales/list")]
        public async Task<IActionResult> GetSalesList()
        {
            var result = await _invoicesService.GetSalesInvoicesList();
            return Ok(result);
        }
        [HttpGet]
        [Route("get/payment/methods")]
        public async Task<IActionResult> GetPaymentMethods()
        {
            var result = await _invoicesService.GetPaymentMethods();
            return Ok(result);
        }
        [HttpGet]
        [Route("get/purchase/items/{invoiceId}")]
        public async Task<IActionResult> GetPurchaseInvoiceItems(int invoiceId)
        {
            var invoiceExist = await _invoicesService.InvoiceExist(invoiceId);
            if (!invoiceExist) return NotFound(invoiceNotFoundMessage);
            var result = await _invoicesService.GetInvoiceItems(invoiceId, true);
            return Ok(result);
        }
        [HttpGet]
        [Route("get/sales/items/{invoiceId}")]
        public async Task<IActionResult> GetSalesInvoiceItems(int invoiceId)
        {
            var invoiceExist = await _invoicesService.InvoiceExist(invoiceId);
            if (!invoiceExist) return NotFound(invoiceNotFoundMessage);
            var result = await _invoicesService.GetInvoiceItems(invoiceId, false);
            return Ok(result);
        }
        [HttpGet]
        [Route("get/orgs/{userId}")]
        public async Task<IActionResult> GetOrgs(int userId)
        {
            var exist = await _userServices.UserExist(userId);
            if (!exist) return NotFound(userNotFoundMessage);
            var result = await _invoicesService.GetOrgsForInvoice(userId);
            return Ok(result);
        }
        [HttpGet]
        [Route("get/purchase/{userId}")]
        public async Task<IActionResult> GetPurchaseInvoices(int userId, string? search, string? sort, string? dateL, string? dateG,
            string? dueL, string? dueG, int? qtyL, int? qtyG, int? totalL, int? totalG, int? recipient, string? currency, int? paymentStatus, bool? status)
        {
            IEnumerable<GetInvoices> result;
            var filters = new InvoiceFiltersTemplate
            {
                DateL = dateL,
                DateG = dateG,
                DueL = dueL,
                DueG = dueG,
                QtyL = qtyL,
                QtyG = qtyG,
                TotalL = totalL,
                TotalG = totalG,
                Recipient = recipient,
                Currency = currency,
                PaymentStatus = paymentStatus,
                Status = status
            };
            var exist = await _userServices.UserExist(userId);
            if (!exist) return NotFound(userNotFoundMessage);
            if (search != null)
            {
                result = await _invoicesService.GetPurchaseInvoices(userId, search, sort: sort, filters);
                return Ok(result);
            }
            result = await _invoicesService.GetPurchaseInvoices(userId, sort: sort, filters);
            return Ok(result);
        }
        [HttpGet]
        [Route("get/purchase/org")]
        public async Task<IActionResult> GetPurchaseInvoicesOrg(string? search, string? sort, string? dateL, string? dateG,
            string? dueL, string? dueG, int? qtyL, int? qtyG, int? totalL, int? totalG, int? recipient, string? currency, int? paymentStatus, bool? status)
        {
            IEnumerable<GetInvoices> result;
            var filters = new InvoiceFiltersTemplate
            {
                DateL = dateL,
                DateG = dateG,
                DueL = dueL,
                DueG = dueG,
                QtyL = qtyL,
                QtyG = qtyG,
                TotalL = totalL,
                TotalG = totalG,
                Recipient = recipient,
                Currency = currency,
                PaymentStatus = paymentStatus,
                Status = status
            };
            if (search != null)
            {
                result = await _invoicesService.GetPurchaseInvoices(search, sort: sort, filters);
                return Ok(result);
            }
            result = await _invoicesService.GetPurchaseInvoices(sort: sort, filters);
            return Ok(result);
        }
        [HttpGet]
        [Route("get/sales/{userId}")]
        public async Task<IActionResult> GetSalesInvoices(int userId, string? search, string? sort, string? dateL, string? dateG,
            string? dueL, string? dueG, int? qtyL, int? qtyG, int? totalL, int? totalG, int? recipient, string? currency, int? paymentStatus, bool? status)
        {
            IEnumerable<GetInvoices> result;
            var filters = new InvoiceFiltersTemplate
            {
                DateL = dateL,
                DateG = dateG,
                DueL = dueL,
                DueG = dueG,
                QtyL = qtyL,
                QtyG = qtyG,
                TotalL = totalL,
                TotalG = totalG,
                Recipient = recipient,
                Currency = currency,
                PaymentStatus = paymentStatus,
                Status = status
            };
            var exist = await _userServices.UserExist(userId);
            if (!exist) return NotFound(userNotFoundMessage);
            if (search != null)
            {
                result = await _invoicesService.GetSalesInvoices(userId, search, sort: sort, filters);
                return Ok(result);
            }
            result = await _invoicesService.GetSalesInvoices(userId, sort: sort, filters);
            return Ok(result);
        }
        [HttpGet]
        [Route("get/sales/org")]
        public async Task<IActionResult> GetSalesInvoicesOrg(string? search, string? sort, string? dateL, string? dateG,
            string? dueL, string? dueG, int? qtyL, int? qtyG, int? totalL, int? totalG, int? recipient, string? currency, int? paymentStatus, bool? status)
        {
            IEnumerable<GetInvoices> result;
            var filters = new InvoiceFiltersTemplate
            {
                DateL = dateL,
                DateG = dateG,
                DueL = dueL,
                DueG = dueG,
                QtyL = qtyL,
                QtyG = qtyG,
                TotalL = totalL,
                TotalG = totalG,
                Recipient = recipient,
                Currency = currency,
                PaymentStatus = paymentStatus,
                Status = status
            };
            if (search != null)
            {
                result = await _invoicesService.GetSalesInvoices(search, sort: sort, filters);
                return Ok(result);
            }
            result = await _invoicesService.GetSalesInvoices(sort: sort, filters);
            return Ok(result);
        }
        [HttpPost]
        [Route("add/purchase/{userId}")]
        public async Task<IActionResult> AddPurchaseInvoice(AddPurchaseInvoice data, int userId)
        {
            var exist = await _userServices.UserExist(data.UserId);
            if (!exist) return NotFound(userNotFoundMessage);
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
        [Route("add/sales/{userId}")]
        public async Task<IActionResult> AddSalesInvoice(AddSalesInvoice data, int userId)
        {
            var exist = await _userServices.UserExist(data.UserId);
            if (!exist) return NotFound(userNotFoundMessage);
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
        [Route("get/items")]
        public async Task<IActionResult> GetAllItems()
        {
            var result = await _itemServices.GetItemList();
            return Ok(result);
        }
        [HttpGet]
        [Route("get/sales/items/{userId}/currency/{currency}")]
        public async Task<IActionResult> GetAllSalesItems(int userId, string currency)
        {
            var exist = await _userServices.UserExist(userId);
            if (!exist) return NotFound(userNotFoundMessage);
            var result = await _itemServices.GetSalesItemList(userId, currency);
            return Ok(result);
        }
        [HttpDelete]
        [Route("delete/{invoiceId}/{isYourInvoice}/{userId}")]
        public async Task<IActionResult> DeleteInvoice(int invoiceId, int userId, bool isYourInvoice)
        {
            var exist = await _userServices.UserExist(userId);
            if (!exist) return NotFound(userNotFoundMessage);
            var invoiceExist = await _invoicesService.InvoiceExist(invoiceId);
            if (!invoiceExist) return NotFound(invoiceNotFoundMessage);
            var cantBeDeleted = await _invoicesService.CheckIfCreditNoteExist(invoiceId) || await _invoicesService.CheckIfSellingPriceExist(invoiceId);
            if (cantBeDeleted) return BadRequest("That invoice has references to other document. Please delete them first.");
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
        [Route("get/path/{invoiceId}")]
        public async Task<IActionResult> GetInvoicePath(int invoiceId)
        {
            var invoiceExist = await _invoicesService.InvoiceExist(invoiceId);
            if (!invoiceExist) return NotFound(invoiceNotFoundMessage);
            var result = await _invoicesService.GetInvoicePath(invoiceId);
            return Ok(result);
        }
        [HttpGet]
        [Route("rest/purchase/{invoiceId}")]
        public async Task<IActionResult> GetRestPurchaseInvoice(int invoiceId)
        {
            var invoiceExist = await _invoicesService.InvoiceExist(invoiceId);
            if (!invoiceExist) return NotFound(invoiceNotFoundMessage);
            var result = await _invoicesService.GetRestPurchaseInvoice(invoiceId);
            return Ok(result);
        }
        [HttpGet]
        [Route("rest/sales/{invoiceId}")]
        public async Task<IActionResult> GetRestSalesInvoice(int invoiceId)
        {
            var invoiceExist = await _invoicesService.InvoiceExist(invoiceId);
            if (!invoiceExist) return NotFound(invoiceNotFoundMessage);
            var result = await _invoicesService.GetRestSalesInvoice(invoiceId);
            return Ok(result);
        }
        [HttpGet]
        [Route("get/rest/modify/{invoiceId}")]
        public async Task<IActionResult> GetRestModifyInvoice(int invoiceId)
        {
            var invoiceExist = await _invoicesService.InvoiceExist(invoiceId);
            if (!invoiceExist) return NotFound(invoiceNotFoundMessage);
            var result = await _invoicesService.GetRestModifyInvoice(invoiceId);
            return Ok(result);
        }
        [HttpPost]
        [Route("modify/{userId}")]
        public async Task<IActionResult> ModifyInvoice(ModifyInvoice data, int userId)
        {
            var exist = await _userServices.UserExist(userId);
            if (!exist) return NotFound(userNotFoundMessage);
            var invoiceExist = await _invoicesService.InvoiceExist(data.InvoiceId);
            if (!invoiceExist) return NotFound(invoiceNotFoundMessage);
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
        [HttpPost]
        [Route("update/status")]
        public async Task<IActionResult> UpdateStatus()
        {
            await _invoicesService.UpdateInvoiceStatus();
            return Ok();
        }
    }
}
