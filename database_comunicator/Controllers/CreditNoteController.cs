using database_communicator.FilterClass;
using database_communicator.Models.DTOs.Create;
using database_communicator.Models.DTOs.Modify;
using database_communicator.Services;
using Microsoft.AspNetCore.Mvc;

namespace database_communicator.Controllers
{
    /// <summary>
    /// This controller works on Credit note table, credit note items table and few other related tables. Used for receiving, modifying and creating data 
    /// that holds credit notes and credit note items information. Use db_name parameter to pass the name of database that you want ot connect.
    /// </summary>
    [Route("{db_name}/[controller]")]
    [ApiController]
    public class CreditNoteController(ICreditNoteServices creditNoteServices, IUserServices userServices, ILogServices logServices, INotificationServices notificationServices) : ControllerBase
    {
        private readonly ICreditNoteServices _creditNoteServices = creditNoteServices;
        private readonly IUserServices _userServices = userServices;
        private readonly ILogServices _logServices = logServices;
        private readonly INotificationServices _notificationServices = notificationServices;

        /// <summary>
        /// Create new credit note from provided data. This action will also create new log entry and notification.
        /// </summary>
        /// <param name="data">Information about new credit note wrapped in <see cref="Models.DTOs.Create.AddCreditNote"/></param>
        /// <param name="userId">Id of user that's activating this action.</param>
        /// <returns>200 code if action was success, 400 code if given credit number exist or item can be deduced from user or 404 if user is not found.</returns>
        [HttpPost]
        [Route("add/{userId}")]
        public async Task<IActionResult> AddCreditNote(AddCreditNote data, int userId)
        {
            var exist = await _userServices.UserExist(userId);
            if (!exist) return NotFound("This user does not exists.");
            var creditExist = await _creditNoteServices.CreditNoteExist(data.CreditNoteNumber, data.InvoiceId);
            if (creditExist) return BadRequest("Credit note with that number already exist.");
            foreach (var item in data.CreditNoteItems.Select(e => new { e.ItemId, e.InvoiceId, e.UserId, e.Qty }))
            {
                if (item.Qty > 0)
                {
                    continue;
                }
                var canBeApplied = await _creditNoteServices.CreditDeductionCanBeApplied(item.UserId, item.InvoiceId, item.ItemId, item.Qty);
                if (!canBeApplied)
                {
                    return BadRequest("This user do not posses enough qty of this item to be deducted.");
                }
            }
            var result = await _creditNoteServices.AddCreditNote(data);
            if (result == 0) return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            var logId = await _logServices.getLogTypeId("Create");
            var logDescription = $"User with id {userId} has created the credit note {data.CreditNoteNumber} for user with id {data.CreditNoteItems.Select(e => e.UserId).First()}.";
            await _logServices.CreateActionLog(logDescription, userId, logId);
            if (data.CreditNoteItems.Select(e => e.UserId).First() != userId)
            {
                var userFull = await _userServices.GetUserFullName(userId);
                await _notificationServices.CreateNotification(new CreateNotification
                {
                    UserId = data.CreditNoteItems.Select(e => e.UserId).First(),
                    Info = $"The credit note with number {data.CreditNoteNumber} has been added by {userFull}.",
                    ObjectType = data.IsYourCreditNote ? "Yours credit notes" : "Client credit notes",
                    Referance = $"{result}"
                });
            }
            return Ok();
        }
        /// <summary>
        /// Tries to receive basic information about users credit notes.
        /// </summary>
        /// <param name="isYourCredit">True if you want to receive credit note with user as recipient, false if client is recipient.</param>
        /// <param name="userId">User id that data is specially filtered on.</param>
        /// <param name="search">>The phrase searched in credit note information. It will check if phrase exist in organization name or invoice number.</param>
        /// <param name="sort">Contains parameter that object will be sorted by. Must start with D or A to determine ascending order. Then is follow by name of property.</param>
        /// <param name="dateL">Filter that search for date that is lower then given value</param>
        /// <param name="dateG">Filter that search for date that is greater then given value</param>
        /// <param name="qtyL">Filter that search for qty that is lower then given value</param>
        /// <param name="qtyG">Filter that search for qty that is greater then given value</param>
        /// <param name="totalL">Filter that search for total that is lower then given value</param>
        /// <param name="totalG">Filter that search for total that is greater then given value</param>
        /// <param name="recipient">Filter that search for recipient with given value</param>
        /// <param name="currency">Filter that search for currency with given value</param>
        /// <param name="paymentStatus">Filter that search for payment status with given value</param>
        /// <param name="status">Filter that search for status with given value</param>
        /// <returns>200 code with list of <see cref="Models.DTOs.Get.GetCreditNote"/>, 400 when sort if given sort is incorrect or 404 if user do not exist.</returns>
        [HttpGet]
        [Route("get/{isYourCredit}/{userId}")]
        public async Task<IActionResult> GetUserCreditNote(bool isYourCredit, int userId, string? search, string? sort, string? dateL, string? dateG,
            int? qtyL, int? qtyG, int? totalL, int? totalG, int? recipient, string? currency, bool? paymentStatus, bool? status)
        {
            if (sort != null)
            {
                bool isSortOk = sort.StartsWith('A') || sort.StartsWith('D');
                if (!isSortOk) return BadRequest("Sort value is incorrect.");
            }
            var filters = new CreditNoteFiltersTemplate
            {
                DateL = dateL,
                DateG = dateG,
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
            if (!exist) return NotFound("This user does not exists.");
            if (search != null)
            {
                var result = await _creditNoteServices.GetCreditNotes(isYourCredit, search, userId, sort: sort, filters);
                return Ok(result);
            }
            else
            {
                var result = await _creditNoteServices.GetCreditNotes(isYourCredit, userId, sort: sort, filters);
                return Ok(result);
            }
        }
        /// <summary>
        /// Tries to receive basic information about credit notes.
        /// </summary>
        /// <param name="isYourCredit">True if you want to receive credit note with user as recipient, false if client is recipient.</param>
        /// <param name="search">>The phrase searched in credit note information. It will check if phrase exist in organization name or invoice number.</param>
        /// <param name="sort">Contains parameter that object will be sorted by. Must start with D or A to determine ascending order. Then is follow by name of property.</param>
        /// <param name="dateL">Filter that search for date that is lower then given value</param>
        /// <param name="dateG">Filter that search for date that is greater then given value</param>
        /// <param name="qtyL">Filter that search for qty that is lower then given value</param>
        /// <param name="qtyG">Filter that search for qty that is greater then given value</param>
        /// <param name="totalL">Filter that search for total that is lower then given value</param>
        /// <param name="totalG">Filter that search for total that is greater then given value</param>
        /// <param name="recipient">Filter that search for recipient with given value</param>
        /// <param name="currency">Filter that search for currency with given value</param>
        /// <param name="paymentStatus">Filter that search for payment status with given value</param>
        /// <param name="status">Filter that search for status with given value</param>
        /// <returns>200 code with list of <see cref="Models.DTOs.Get.GetCreditNote"/>, 400 when sort if given sort is incorrect or 404 if user do not exist.</returns>
        [HttpGet]
        [Route("get/{isYourCredit}")]
        public async Task<IActionResult> GetCreditNote(bool isYourCredit, string? search, string? sort, string? dateL, string? dateG,
            int? qtyL, int? qtyG, int? totalL, int? totalG, int? recipient, string? currency, bool? paymentStatus, bool? status)
        {
            if (sort != null)
            {
                bool isSortOk = sort.StartsWith('A') || sort.StartsWith('D');
                if (!isSortOk) return BadRequest("Sort value is incorrect.");
            }
            var filters = new CreditNoteFiltersTemplate
            {
                DateL = dateL,
                DateG = dateG,
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
                var result = await _creditNoteServices.GetCreditNotes(isYourCredit, search, sort: sort, filters);
                return Ok(result);
            }
            else
            {
                var result = await _creditNoteServices.GetCreditNotes(isYourCredit, sort: sort, filters);
                return Ok(result);
            }
        }
        /// <summary>
        /// Tries to receive information that was not passed as basic information in <see cref="GetCreditNote"/>
        /// or <see cref="GetUserCreditNote"/> 
        /// function and are needed to showcase object for user.
        /// </summary>
        /// <param name="creditId">Credit note id</param>
        /// <returns>200 code with list of <see cref="Models.DTOs.Get.GetRestCreditNote"/> or 404 when credit note do not exist.</returns>
        [HttpGet]
        [Route("rest/{creditId}")]
        public async Task<IActionResult> GetRestCreditNote(int creditId)
        {
            var exist = await _creditNoteServices.CreditNoteExist(creditId);
            if (!exist) return NotFound();
            var result = await _creditNoteServices.GetRestCreditNote(creditId);
            return Ok(result);
        }
        /// <summary>
        /// Tries to receive credit note path value.
        /// </summary>
        /// <param name="creditId">Credit note id.</param>
        /// <returns>200 code with string that holds path value or 404 when credit note is not found.</returns>
        [HttpGet]
        [Route("get/path/{creditId}")]
        public async Task<IActionResult> GetCreditFilePath(int creditId)
        {
            var exist = await _creditNoteServices.CreditNoteExist(creditId);
            if (!exist) return NotFound();
            var result = await _creditNoteServices.GetCreditFilePath(creditId);
            return Ok(result);
        }
        /// <summary>
        /// Delete chosen credit note from database. This action will also create new log entry and notification.
        /// </summary>
        /// <param name="creditId">Credit note id to delete.</param>
        /// <param name="userId">Id of user that's activating this action.</param>
        /// <param name="isYourCredit">True if credit note has user as recipient, false if client is recipient.</param>
        /// <returns>200 code when action is successful, 404 when credit note or user id is not found or 500 when delete action failed.</returns>
        [HttpDelete]
        [Route("delete/{creditId}/{isYourCredit}/{userId}")]
        public async Task<IActionResult> DeleteCreditNote(int creditId, int userId, bool isYourCredit)
        {
            var userExist = await _userServices.UserExist(userId);
            if (!userExist) return NotFound("User not found.");
            var exist = await _creditNoteServices.CreditNoteExist(creditId);
            if (!exist) return NotFound("This credit note do not exist.");
            var creditUser = await _creditNoteServices.GetCreditNoteUser(creditId);
            var creditNumber = await _creditNoteServices.GetCreditNumber(creditId);
            var result = await _creditNoteServices.DeleteCreditNote(creditId);
            if (!result) return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            var logId = await _logServices.getLogTypeId("Delete");
            var logDescription = $"User with id {userId} has deleted the credit note with id {creditId} and number {creditNumber}.";
            await _logServices.CreateActionLog(logDescription, userId, logId);
            if (creditUser != userId)
            {
                var userFull = await _userServices.GetUserFullName(userId);
                await _notificationServices.CreateNotification(new CreateNotification
                {
                    UserId = creditUser,
                    Info = $"The credit note with number {creditNumber} has been deleted by {userFull}.",
                    ObjectType = isYourCredit ? "Yours credit notes" : "Client credit notes",
                    Referance = $"{creditId}"
                });
            }
            return Ok(result);
        }
        /// <summary>
        /// Tries to receive information that was not passed as basic information in <see cref="GetCreditNote"/>
        /// or <see cref="GetUserCreditNote"/> 
        /// function and are needed to showcase object for modifying.
        /// </summary>
        /// <param name="creditId">Credit note id.</param>
        /// <param name="isYourCredit">True if credit note has user as recipient, false if client is recipient.</param>
        /// <returns>200 code with object of <see cref="Models.DTOs.Get.GetRestModifyCredit"/> or 404 when credit note is not found.</returns>
        [HttpGet]
        [Route("get/rest/modify/{isYourCredit}/{creditId}")]
        public async Task<IActionResult> GetRestModifyCredit(int creditId, bool isYourCredit)
        {
            var exist = await _creditNoteServices.CreditNoteExist(creditId);
            if (!exist) return NotFound();
            var result = await _creditNoteServices.GetRestModifyCredit(creditId, isYourCredit);
            return Ok(result);
        }
        /// <summary>
        /// Overwrite credit data with new one. This action will also create new log entry.
        /// </summary>
        /// <param name="data">Object of <see cref="Models.DTOs.Modify.ModifyCreditNote"/> containing changed credit note properties.</param>
        /// <param name="userId">Id of user that's activating this action.</param>
        /// <returns>200 code when action is successful, 404 when user or credit note id do not exist, 400 when new credit note number already exist or 500 when modification procedure failed.</returns>
        [HttpPost]
        [Route("modify/{userId}")]
        public async Task<IActionResult> ModifyCreditNote(ModifyCreditNote data, int userId)
        {
            var exist = await _userServices.UserExist(userId);
            if (!exist) return NotFound("User not found.");
            var cdExist = await _creditNoteServices.CreditNoteExist(data.Id);
            if (!cdExist) return NotFound("Credit note does not exist.");
            if (data.CreditNumber != null)
            {
                var invoiceId = await _creditNoteServices.GetCreditNoteInvoiceId(data.Id);
                var newNumberExist = await _creditNoteServices.CreditNoteExist(data.CreditNumber, invoiceId);
                if (newNumberExist) return BadRequest("Credit note with this number and invoice already exist.");
            }
            var creditUser = await _creditNoteServices.GetCreditNoteUser(data.Id);
            var result = await _creditNoteServices.ModifyCreditNote(data);
            var creditNumber = await _creditNoteServices.GetCreditNumber(data.Id);
            if (!result) return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            var logId = await _logServices.getLogTypeId("Modify");
            var logDescription = $"User with id {userId} has modified the credit note with id {data.Id} and number {creditNumber}.";
            await _logServices.CreateActionLog(logDescription, userId, logId);
            if (creditUser != userId)
            {
                var userFull = await _userServices.GetUserFullName(userId);
                await _notificationServices.CreateNotification(new CreateNotification
                {
                    UserId = creditUser,
                    Info = $"The credit note with number {creditNumber} has been modified by {userFull}.",
                    ObjectType = data.IsYourCredit ? "Yours credit notes" : "Client credit notes",
                    Referance = $"{data.Id}"
                });
            }
            return Ok(result);
        }
    }
}
