﻿using database_comunicator.Models;
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
        [Route("addCreditNote")]
        public async Task<IActionResult> AddCreditNote(AddCreditNote data, int userId)
        {
            var exist = await _userServices.UserExist(userId);
            if (!exist) return NotFound("This user does not exists.");
            var creditExist = await _creditNoteServices.CreditNoteExist(data.CreditNotenumber, data.InvoiceId);
            if (creditExist) return BadRequest("This invoice exist.");
            foreach (var item in data.CreditNoteItems.Select(e => new {e.ItemId, e.InvoiceId, e.UserId, e.Qty}))
            {
                if (item.Qty > 0)
                {
                    continue;
                }
                var canBeApplied = await _creditNoteServices.CreditDeductionCanBeApplied(item.UserId, item.InvoiceId, item.ItemId, item.Qty);
                if (!canBeApplied)
                {
                    return BadRequest("This user do not posses enought item to be deducted.");
                }
            }
            var result = await _creditNoteServices.AddCreditNote(data);
            if (result == 0) return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            var logId = await _logServices.getLogTypeId("Create");
            var desc = $"User with id {userId} has created the credit note {data.CreditNotenumber} for user with id {data.CreditNoteItems.Select(e => e.UserId).First()}.";
            await _logServices.CreateActionLog(desc, userId, logId);
            if (data.CreditNoteItems.Select(e => e.UserId).First() != userId)
            {
                await _notificationServices.CreateNotification(new CreateNotification
                {
                    UserId = data.CreditNoteItems.Select(e => e.UserId).First(),
                    Info = $"The credit note with number {data.CreditNotenumber} has been added.",
                    ObjectType = "Invoice",
                    Referance = $"{result}"
                });
            }
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
