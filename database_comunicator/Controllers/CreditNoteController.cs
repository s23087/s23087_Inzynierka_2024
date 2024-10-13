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
    public class CreditNoteController : ControllerBase
    {
        private readonly ICreditNoteServices _creditNoteServices;
        private readonly IUserServices _userServices;
        private readonly ILogServices _logServices;
        private readonly INotificationServices _notificationServices;
        public CreditNoteController(ICreditNoteServices creditNoteServices, IUserServices userServices, ILogServices logServices, INotificationServices notificationServices)
        {
            _creditNoteServices = creditNoteServices;
            _userServices = userServices;
            _logServices = logServices;
            _notificationServices = notificationServices;

        }
        [HttpPost]
        [Route("add/{userId}")]
        public async Task<IActionResult> AddCreditNote(AddCreditNote data, int userId)
        {
            var exist = await _userServices.UserExist(userId);
            if (!exist) return NotFound("This user does not exists.");
            var creditExist = await _creditNoteServices.CreditNoteExist(data.CreditNotenumber, data.InvoiceId);
            if (creditExist) return BadRequest("Credit note with that number alredy exist.");
            foreach (var item in data.CreditNoteItems.Select(e => new {e.ItemId, e.InvoiceId, e.UserId, e.Qty}))
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
            var desc = $"User with id {userId} has created the credit note {data.CreditNotenumber} for user with id {data.CreditNoteItems.Select(e => e.UserId).First()}.";
            await _logServices.CreateActionLog(desc, userId, logId);
            if (data.CreditNoteItems.Select(e => e.UserId).First() != userId)
            {
                var userFull = await _userServices.GetUserFullName(userId);
                await _notificationServices.CreateNotification(new CreateNotification
                {
                    UserId = data.CreditNoteItems.Select(e => e.UserId).First(),
                    Info = $"The credit note with number {data.CreditNotenumber} has been added by {userFull}.",
                    ObjectType = data.IsYourCreditNote ? "Yours credit notes" : "Client credit notes",
                    Referance = $"{result}"
                });
            }
            return Ok();
        }
        [HttpGet]
        [Route("get/{isYourCredit}/{userId}")]
        public async Task<IActionResult> GetUserCreditNote(bool isYourCredit, int userId, string? search, string? sort, string? dateL, string? dateG,
            int? qtyL, int? qtyG, int? totalL, int? totalG, int? recipient, string? currency, bool? paymentStatus, bool? status)
        {
            var exist = await _userServices.UserExist(userId);
            if (!exist) return NotFound();
            if (search != null)
            {
                var result = await _creditNoteServices.GetCreditNotes(isYourCredit, search, userId, sort: sort, dateL, dateG, qtyL, qtyG, totalL, totalG, recipient,
                    currency, paymentStatus, status);
                return Ok(result);
            } else
            {
                var result = await _creditNoteServices.GetCreditNotes(isYourCredit, userId, sort: sort, dateL, dateG, qtyL, qtyG, totalL, totalG, recipient,
                    currency, paymentStatus, status);
                return Ok(result);
            }
        }
        [HttpGet]
        [Route("get/{isYourCredit}")]
        public async Task<IActionResult> GetCreditNote(bool isYourCredit, string? search, string? sort, string? dateL, string? dateG,
            int? qtyL, int? qtyG, int? totalL, int? totalG, int? recipient, string? currency, bool? paymentStatus, bool? status)
        {
            if (search != null)
            {
                var result = await _creditNoteServices.GetCreditNotes(isYourCredit, search, sort: sort, dateL, dateG, qtyL, qtyG, totalL, totalG, recipient,
                    currency, paymentStatus, status);
                return Ok(result);
            }
            else
            {
                var result = await _creditNoteServices.GetCreditNotes(isYourCredit, sort: sort, dateL, dateG, qtyL, qtyG, totalL, totalG, recipient,
                    currency, paymentStatus, status);
                return Ok(result);
            }
        }
        [HttpGet]
        [Route("rest/{creditId}")]
        public async Task<IActionResult> GetRestCreditNote(int creditId)
        {
            var exist = await _creditNoteServices.CreditNoteExist(creditId);
            if (!exist) return NotFound();
            var result = await _creditNoteServices.GetRestCreditNote(creditId);
            return Ok(result);
        }
        [HttpGet]
        [Route("path/{creditId}")]
        public async Task<IActionResult> GetCreditFilePath(int creditId)
        {
            var exist = await _creditNoteServices.CreditNoteExist(creditId);
            if (!exist) return NotFound();
            var result = await _creditNoteServices.GetCreditFilePath(creditId);
            return Ok(result);
        }
        [HttpDelete]
        [Route("delete/{creditId}/{isYourCredit}/{userId}")]
        public async Task<IActionResult> DeleteCreditNote(int creditId, int userId, bool isYourCredit)
        {
            var exist = await _creditNoteServices.CreditNoteExist(creditId);
            if (!exist) return NotFound();
            var creditUser = await _creditNoteServices.GetCreditNoteUser(creditId);
            var creditNumber = await _creditNoteServices.GetCreditNumber(creditId);
            var result = await _creditNoteServices.DeleteCreditNote(creditId);
            if (!result) return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            var logId = await _logServices.getLogTypeId("Delete");
            var desc = $"User with id {userId} has deleted the credit note with id {creditId} and number {creditNumber}.";
            await _logServices.CreateActionLog(desc, userId, logId);
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
        [HttpGet]
        [Route("modify/rest/{isYourCredit}/{creditId}")]
        public async Task<IActionResult> GetRestModifyCredit(int creditId, bool isYourCredit)
        {
            var exist = await _creditNoteServices.CreditNoteExist(creditId);
            if (!exist) return NotFound();
            var result = await _creditNoteServices.GetRestModifyCredit(creditId, isYourCredit);
            return Ok(result);
        }
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
                if (newNumberExist) return BadRequest("Credit note with this number and invoice alredy exist.");
            }
            var creditUser = await _creditNoteServices.GetCreditNoteUser(data.Id);
            var result = await _creditNoteServices.ModifyCreditNote(data);
            var creditNumber = await _creditNoteServices.GetCreditNumber(data.Id);
            if (!result) return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            var logId = await _logServices.getLogTypeId("Delete");
            var desc = $"User with id {userId} has modified the credit note with id {data.Id} and number {creditNumber}.";
            await _logServices.CreateActionLog(desc, userId, logId);
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
