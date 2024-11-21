using database_communicator.FilterClass;
using database_communicator.Models.DTOs.Create;
using database_communicator.Models.DTOs.Get;
using database_communicator.Models.DTOs.Modify;
using database_communicator.Services;
using Microsoft.AspNetCore.Mvc;

namespace database_communicator.Controllers
{
    /// <summary>
    /// This controller works on Invoice table and few other related tables. Used for receiving, modifying and creating data 
    /// that holds invoices information. Use db_name parameter to pass the name of database that you want ot connect.
    /// </summary>
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
        /// <summary>
        /// Tries to receive taxes information from database.
        /// </summary>
        /// <returns>200 code with list of <see cref="Models.DTOs.Get.GetTaxes"/>.</returns>
        [HttpGet]
        [Route("get/taxes")]
        public async Task<IActionResult> GetTaxes()
        {
            var result = await _invoicesService.GetTaxes();
            return Ok(result);
        }
        /// <summary>
        /// Tries to receive payment status information from database.
        /// </summary>
        /// <returns>200 code with list of <see cref="Models.DTOs.Get.GetPaymentStatuses"/>.</returns>
        [HttpGet]
        [Route("get/payment/statuses")]
        public async Task<IActionResult> GetPaymentStatuses()
        {
            var result = await _invoicesService.GetPaymentStatuses();
            return Ok(result);
        }
        /// <summary>
        /// Tries to receive purchase invoice information from database.
        /// </summary>
        /// <returns>200 code with list of <see cref="GetInvoicesList"/>.</returns>
        [HttpGet]
        [Route("get/purchase/list")]
        public async Task<IActionResult> GetPurchaseList()
        {
            var result = await _invoicesService.GetPurchaseInvoicesList();
            return Ok(result);
        }
        /// <summary>
        /// Tries to receive sales invoice information from database.
        /// </summary>
        /// <returns>200 code with list of <see cref="GetInvoicesList"/>.</returns>
        [HttpGet]
        [Route("get/sales/list")]
        public async Task<IActionResult> GetSalesList()
        {
            var result = await _invoicesService.GetSalesInvoicesList();
            return Ok(result);
        }
        /// <summary>
        /// Tries to receive payment methods information from database.
        /// </summary>
        /// <returns>200 code with list of <see cref="Models.DTOs.Get.GetPaymentMethods"/>.</returns>
        [HttpGet]
        [Route("get/payment/methods")]
        public async Task<IActionResult> GetPaymentMethods()
        {
            var result = await _invoicesService.GetPaymentMethods();
            return Ok(result);
        }
        /// <summary>
        /// Tries to receive chosen purchase invoice items available for adding to credit note from database.
        /// </summary>
        /// <param name="invoiceId">Chosen invoice id</param>
        /// <returns>200 code wit list of <see cref="GetInvoiceItems"/> or 404 if invoice is not found.</returns>
        [HttpGet]
        [Route("get/purchase/items/{invoiceId}")]
        public async Task<IActionResult> GetPurchaseInvoiceItems(int invoiceId)
        {
            var invoiceExist = await _invoicesService.InvoiceExist(invoiceId);
            if (!invoiceExist) return NotFound(invoiceNotFoundMessage);
            var result = await _invoicesService.GetInvoiceItems(invoiceId, true);
            return Ok(result);
        }
        /// <summary>
        /// Tries to receive chosen sales invoice items available for adding to credit note from database.
        /// </summary>
        /// <param name="invoiceId"></param>
        /// <returns>200 code wit list of <see cref="GetInvoiceItems"/> or 404 if invoice is not found.</returns>
        [HttpGet]
        [Route("get/sales/items/{invoiceId}")]
        public async Task<IActionResult> GetSalesInvoiceItems(int invoiceId)
        {
            var invoiceExist = await _invoicesService.InvoiceExist(invoiceId);
            if (!invoiceExist) return NotFound(invoiceNotFoundMessage);
            var result = await _invoicesService.GetInvoiceItems(invoiceId, false);
            return Ok(result);
        }
        /// <summary>
        /// Tries to receive organization information from database for purpose of creating or modifying invoice.
        /// </summary>
        /// <param name="userId">Id of user that the invoice is created for.</param>
        /// <returns>200 code wit list of <see cref="GetOrgsForInvocie"/> or 404 if user is not found.</returns>
        [HttpGet]
        [Route("get/orgs/{userId}")]
        public async Task<IActionResult> GetOrgs(int userId)
        {
            var exist = await _userServices.UserExist(userId);
            if (!exist) return NotFound(userNotFoundMessage);
            var result = await _invoicesService.GetOrgsForInvoice(userId);
            return Ok(result);
        }
        /// <summary>
        /// Tries to receive purchase invoices basic information from database for given user. Can be filtered using parameters. 
        /// </summary>
        /// <param name="userId">Id of user that returned invoices will belong to.</param>
        /// <param name="search">Phrase that will be search across invoices numbers.</param>
        /// <param name="sort">Contains parameter that object will be sorted by. Must start with D or A to determine ascending order. Then is follow by name of property.</param>
        /// <param name="dateL">Filter that search for date that is lower then given value</param>
        /// <param name="dateG">Filter that search for date that is greater then given value</param>
        /// <param name="dueL">Filter that search for due date that is lower then given value</param>
        /// <param name="dueG">>Filter that search for due date that is greater then given value</param>
        /// <param name="qtyL">Filter that search for qty that is lower then given value</param>
        /// <param name="qtyG">Filter that search for qty that is greater then given value</param>
        /// <param name="totalL">Filter that search for total that is lower then given value</param>
        /// <param name="totalG">Filter that search for total that is greater then given value</param>
        /// <param name="recipient">Filter that search for recipient with given value</param>
        /// <param name="currency">Filter that search for currency with given value</param>
        /// <param name="paymentStatus">Filter that search for payment status with given value</param>
        /// <param name="status">Filter that search for status with given value</param>
        /// <returns>200 code with list of <see cref="GetInvoices"/>, 400 when sort if given sort is incorrect or 404 when user is not found.</returns>
        [HttpGet]
        [Route("get/purchase/{userId}")]
        public async Task<IActionResult> GetPurchaseInvoices(int userId, string? search, string? sort, string? dateL, string? dateG,
            string? dueL, string? dueG, int? qtyL, int? qtyG, int? totalL, int? totalG, int? recipient, string? currency, int? paymentStatus, bool? status)
        {
            if (sort != null)
            {
                bool isSortOk = sort.StartsWith("A") || sort.StartsWith("D");
                if (!isSortOk) return BadRequest("Sort value is incorrect.");
            }
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
        /// <summary>
        /// Tries to receive purchase invoices basic information from database. Can be filtered using parameters. 
        /// </summary>
        /// <param name="search">Phrase that will be search across invoices numbers.</param>
        /// <param name="sort">Contains parameter that object will be sorted by. Must start with D or A to determine ascending order. Then is follow by name of property.</param>
        /// <param name="dateL">Filter that search for date that is lower then given value</param>
        /// <param name="dateG">Filter that search for date that is greater then given value</param>
        /// <param name="dueL">Filter that search for due date that is lower then given value</param>
        /// <param name="dueG">>Filter that search for due date that is greater then given value</param>
        /// <param name="qtyL">Filter that search for qty that is lower then given value</param>
        /// <param name="qtyG">Filter that search for qty that is greater then given value</param>
        /// <param name="totalL">Filter that search for total that is lower then given value</param>
        /// <param name="totalG">Filter that search for total that is greater then given value</param>
        /// <param name="recipient">Filter that search for recipient with given value</param>
        /// <param name="currency">Filter that search for currency with given value</param>
        /// <param name="paymentStatus">Filter that search for payment status with given value</param>
        /// <param name="status">Filter that search for status with given value</param>
        /// <returns>200 code with list of <see cref="GetInvoices"/> or 400 when sort if given sort is incorrect.</returns>
        [HttpGet]
        [Route("get/purchase/org")]
        public async Task<IActionResult> GetPurchaseInvoicesOrg(string? search, string? sort, string? dateL, string? dateG,
            string? dueL, string? dueG, int? qtyL, int? qtyG, int? totalL, int? totalG, int? recipient, string? currency, int? paymentStatus, bool? status)
        {
            if (sort != null)
            {
                bool isSortOk = sort.StartsWith("A") || sort.StartsWith("D");
                if (!isSortOk) return BadRequest("Sort value is incorrect.");
            }
            IEnumerable<GetInvoices> result;
            var filters = new InvoiceFiltersTemplate
            {
                DateL = dateL,
                DateG = dateG,
                DueL = dueL,
                TotalG = totalG,
                Recipient = recipient,
                Currency = currency,
                PaymentStatus = paymentStatus,
                Status = status,
                DueG = dueG,
                QtyL = qtyL,
                QtyG = qtyG,
                TotalL = totalL,
            };
            if (search != null)
            {
                result = await _invoicesService.GetPurchaseInvoices(search, sort: sort, filters);
                return Ok(result);
            }
            result = await _invoicesService.GetPurchaseInvoices(sort: sort, filters);
            return Ok(result);
        }
        /// <summary>
        /// Tries to receive sales invoices basic information from database for given user. Can be filtered using parameters. 
        /// </summary>
        /// <param name="userId">Id of user that returned invoices will belong to.</param>
        /// <param name="search">Phrase that will be search across invoices numbers.</param>
        /// <param name="sort">Contains parameter that object will be sorted by. Must start with D or A to determine ascending order. Then is follow by name of property.</param>
        /// <param name="dateL">Filter that search for date that is lower then given value</param>
        /// <param name="dateG">Filter that search for date that is greater then given value</param>
        /// <param name="dueL">Filter that search for due date that is lower then given value</param>
        /// <param name="dueG">>Filter that search for due date that is greater then given value</param>
        /// <param name="qtyL">Filter that search for qty that is lower then given value</param>
        /// <param name="qtyG">Filter that search for qty that is greater then given value</param>
        /// <param name="totalL">Filter that search for total that is lower then given value</param>
        /// <param name="totalG">Filter that search for total that is greater then given value</param>
        /// <param name="recipient">Filter that search for recipient with given value</param>
        /// <param name="currency">Filter that search for currency with given value</param>
        /// <param name="paymentStatus">Filter that search for payment status with given value</param>
        /// <param name="status">Filter that search for status with given value</param>
        /// <returns>200 code with list of <see cref="GetInvoices"/>, 400 when sort if given sort is incorrect or 404 when user is not found.</returns>
        [HttpGet]
        [Route("get/sales/{userId}")]
        public async Task<IActionResult> GetSalesInvoices(int userId, string? search, string? sort, string? dateL, string? dateG,
            string? dueL, string? dueG, int? qtyL, int? qtyG, int? totalL, int? totalG, int? recipient, string? currency, int? paymentStatus, bool? status)
        {
            if (sort != null)
            {
                bool isSortOk = sort.StartsWith("A") || sort.StartsWith("D");
                if (!isSortOk) return BadRequest("Sort value is incorrect.");
            }
            IEnumerable<GetInvoices> result;
            var filters = new InvoiceFiltersTemplate
            {
                DateL = dateL,
                DateG = dateG,
                DueL = dueL,
                DueG = dueG,
                Recipient = recipient,
                Currency = currency,
                PaymentStatus = paymentStatus,
                Status = status,
                QtyL = qtyL,
                QtyG = qtyG,
                TotalL = totalL,
                TotalG = totalG,
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
        /// <summary>
        /// Tries to receive sales invoices basic information from database. Can be filtered using parameters. 
        /// </summary>
        /// <param name="search">Phrase that will be search across invoices numbers.</param>
        /// <param name="sort">Contains parameter that object will be sorted by. Must start with D or A to determine ascending order. Then is follow by name of property.</param>
        /// <param name="dateL">Filter that search for date that is lower then given value</param>
        /// <param name="dateG">Filter that search for date that is greater then given value</param>
        /// <param name="dueL">Filter that search for due date that is lower then given value</param>
        /// <param name="dueG">>Filter that search for due date that is greater then given value</param>
        /// <param name="qtyL">Filter that search for qty that is lower then given value</param>
        /// <param name="qtyG">Filter that search for qty that is greater then given value</param>
        /// <param name="totalL">Filter that search for total that is lower then given value</param>
        /// <param name="totalG">Filter that search for total that is greater then given value</param>
        /// <param name="recipient">Filter that search for recipient with given value</param>
        /// <param name="currency">Filter that search for currency with given value</param>
        /// <param name="paymentStatus">Filter that search for payment status with given value</param>
        /// <param name="status">Filter that search for status with given value</param>
        /// <returns>200 code with list of <see cref="GetInvoices"/> or 400 when sort if given sort is incorrect.</returns>
        [HttpGet]
        [Route("get/sales/org")]
        public async Task<IActionResult> GetSalesInvoicesOrg(string? search, string? sort, string? dateL, string? dateG,
            string? dueL, string? dueG, int? qtyL, int? qtyG, int? totalL, int? totalG, int? recipient, string? currency, int? paymentStatus, bool? status)
        {
            if (sort != null)
            {
                bool isSortOk = sort.StartsWith("A") || sort.StartsWith("D");
                if (!isSortOk) return BadRequest("Sort value is incorrect.");
            }
            IEnumerable<GetInvoices> result;
            var filters = new InvoiceFiltersTemplate
            {
                DateL = dateL,
                QtyG = qtyG,
                TotalL = totalL,
                TotalG = totalG,
                Recipient = recipient,
                Currency = currency,
                DateG = dateG,
                DueL = dueL,
                DueG = dueG,
                QtyL = qtyL,
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
        /// <summary>
        /// Create new purchase invoice in database using given information. This action will also create new log entry and notification.
        /// </summary>
        /// <param name="data">New purchase invoice data wrapped in <see cref="Models.DTOs.Create.AddPurchaseInvoice"/> object.</param>
        /// <param name="userId">Id of user that's activating this action.</param>
        /// <returns>200 code when success, 400 when failure or 404 when user, organization or item is not found.</returns>
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
        /// <summary>
        /// Create new sales invoice in database using given information. This action will also create new log entry and notification.
        /// </summary>
        /// <param name="data">New sales invoice data wrapped in <see cref="Models.DTOs.Create.AddSalesInvoice"/> object.</param>
        /// <param name="userId">Id of user that's activating this action.</param>
        /// <returns>200 code when success, 400 when failure or 404 when user, organization or item is not found.</returns>
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
        /// <summary>
        /// Tries to receive information about available items for adding to purchase invoice.
        /// </summary>
        /// <returns>200 code with list of <see cref="GetItemList"/></returns>
        [HttpGet]
        [Route("get/items")]
        public async Task<IActionResult> GetAllItems()
        {
            var result = await _itemServices.GetItemList();
            return Ok(result);
        }
        /// <summary>
        /// Tries to receive information about available items for adding to sales invoice.
        /// </summary>
        /// <param name="userId">Id of user that's items are needed for.</param>
        /// <param name="currency">Shortcut name of currency that product price will be taken.</param>
        /// <returns>200 code with list of <see cref="GetSalesItemList"/> or 404 when user is not found.</returns>
        [HttpGet]
        [Route("get/sales/items/{userId}/currency/{currency}")]
        public async Task<IActionResult> GetAllSalesItems(int userId, string currency)
        {
            var exist = await _userServices.UserExist(userId);
            if (!exist) return NotFound(userNotFoundMessage);
            var result = await _itemServices.GetSalesItemList(userId, currency);
            return Ok(result);
        }
        /// <summary>
        /// Delete chosen invoice from database. This action will also create new log entry and notification.
        /// </summary>
        /// <param name="invoiceId">Chosen id of invoice to delete</param>
        /// <param name="userId">Id of user that's activating this action.</param>
        /// <param name="isYourInvoice">True if invoice recipient is user, false if client.</param>
        /// <returns>200 code when success, 500 code when failure, 404 when user or invoice is not found or 400 when invoice have relations to other documents.</returns>
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
        /// <summary>
        /// Retrieves invoice file path from database.
        /// </summary>
        /// <param name="invoiceId">Invoice id.</param>
        /// <returns>200 with string containing file path or 404 when invoice is not found.</returns>
        [HttpGet]
        [Route("get/path/{invoiceId}")]
        public async Task<IActionResult> GetInvoicePath(int invoiceId)
        {
            var invoiceExist = await _invoicesService.InvoiceExist(invoiceId);
            if (!invoiceExist) return NotFound(invoiceNotFoundMessage);
            var result = await _invoicesService.GetInvoicePath(invoiceId);
            return Ok(result);
        }
        /// <summary>
        /// Tries to receive information that was not passed as basic information in <see cref="GetPurchaseInvoices"/> or <see cref="GetPurchaseInvoicesOrg"/> 
        /// function and are needed to showcase object for user.
        /// </summary>
        /// <param name="invoiceId">Invoice id.</param>
        /// <returns>>200 with object of <see cref="GetRestInvoice"/> or 404 when invoice is not found.</returns>
        [HttpGet]
        [Route("rest/purchase/{invoiceId}")]
        public async Task<IActionResult> GetRestPurchaseInvoice(int invoiceId)
        {
            var invoiceExist = await _invoicesService.InvoiceExist(invoiceId);
            if (!invoiceExist) return NotFound(invoiceNotFoundMessage);
            var result = await _invoicesService.GetRestPurchaseInvoice(invoiceId);
            return Ok(result);
        }
        /// <summary>
        /// Tries to receive information that was not passed as basic information in <see cref="GetSalesInvoices"/> or <see cref="GetSalesInvoicesOrg"/> 
        /// function and are needed to showcase object for user.
        /// </summary>
        /// <param name="invoiceId">Invoice id.</param>
        /// <returns>>200 with object of <see cref="GetRestInvoice"/> or 404 when invoice is not found.</returns>
        [HttpGet]
        [Route("rest/sales/{invoiceId}")]
        public async Task<IActionResult> GetRestSalesInvoice(int invoiceId)
        {
            var invoiceExist = await _invoicesService.InvoiceExist(invoiceId);
            if (!invoiceExist) return NotFound(invoiceNotFoundMessage);
            var result = await _invoicesService.GetRestSalesInvoice(invoiceId);
            return Ok(result);
        }
        /// <summary>
        /// Tries to receive information that was not passed as basic information in <see cref="GetSalesInvoices"/> or <see cref="GetSalesInvoicesOrg"/> 
        /// function and are needed to modify object.
        /// </summary>
        /// <param name="invoiceId">Invoice id.</param>
        /// <returns>>200 with object of <see cref="Models.DTOs.Get.GetRestModifyInvoice"/> or 404 when invoice is not found.</returns>
        [HttpGet]
        [Route("get/rest/modify/{invoiceId}")]
        public async Task<IActionResult> GetRestModifyInvoice(int invoiceId)
        {
            var invoiceExist = await _invoicesService.InvoiceExist(invoiceId);
            if (!invoiceExist) return NotFound(invoiceNotFoundMessage);
            var result = await _invoicesService.GetRestModifyInvoice(invoiceId);
            return Ok(result);
        }
        /// <summary>
        /// Overwrite chosen invoice given properties. This action will also create new log entry and notification.
        /// </summary>
        /// <param name="data">New invoice data wrapped in <see cref="Models.DTOs.Modify.ModifyInvoice"/> object.</param>
        /// <param name="userId">Id of user that's activating this action.</param>
        /// <returns>200 code when success, 500 code when failure or 404 when user or invoice is not found,</returns>
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
        /// <summary>
        /// Modify all invoice status that due date has passed and payment status was unpaid to due to.
        /// </summary>
        /// <returns>200 code when success or 500 when failure</returns>
        [HttpPost]
        [Route("update/status")]
        public async Task<IActionResult> UpdateStatus()
        {
            var result = await _invoicesService.UpdateInvoiceStatus();
            if (!result) return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            return Ok();
        }
    }
}
