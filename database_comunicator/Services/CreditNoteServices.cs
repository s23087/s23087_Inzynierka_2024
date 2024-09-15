using database_comunicator.Data;
using database_comunicator.Models;
using database_comunicator.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace database_comunicator.Services
{
    public interface ICreditNoteServices
    {
        public Task AddYoursCreditNote(AddCreditNote data);
    }
    public class CreditNoteServices : ICreditNoteServices
    {
        private readonly HandlerContext _handlerContext;
        public CreditNoteServices(HandlerContext handlerContext)
        {
            _handlerContext = handlerContext;
        }
        public async Task AddYoursCreditNote(AddCreditNote data)
        {
            using var trans = await _handlerContext.Database.BeginTransactionAsync();
            try
            {
                var currencyName = await _handlerContext.Invoices.Where(e => e.InvoiceId == data.InvoiceId).Select(e => e.CurrencyName).FirstAsync();
                if (currencyName != "PLN")
                {
                    var currencyVal = await _handlerContext.Invoices
                        .Where(e => e.InvoiceId == data.InvoiceId)
                        .Include(e => e.Currency)
                        .Select(e => e.Currency)
                        .Where(e => e.CurrencyName == currencyName)
                        .Select(e => e.CurrencyValue1).FirstAsync();

                    data.CreditNoteItems = data.CreditNoteItems.Select(e => new CreditNoteItems
                    {
                        UserId = e.UserId,
                        ItemId = e.ItemId,
                        InvoiceId = e.InvoiceId,
                        PurchasePriceId = e.PurchasePriceId,
                        Qty = e.Qty,
                        NewPrice = e.NewPrice * currencyVal
                    }).ToList();
                }
                var creditNote = new CreditNote
                {
                    CreditNoteDate = data.CreditNoteDate,
                    CreditNoteNumber = data.CreditNotenumber,
                    InSystem = data.InSystem,
                    Note = data.Note,
                    InvoiceId = data.InvoiceId
                };
                _handlerContext.Add<CreditNote>(creditNote);
                await _handlerContext.SaveChangesAsync();

                var creditItems = data.CreditNoteItems.Select(e => new CreditNoteItem
                {
                    CreditNoteId = creditNote.IdCreditNote,
                    PurchasePriceId = e.PurchasePriceId,
                    Qty = e.Qty,
                    NewPrice = e.NewPrice,
                    IncludeQty = e.IncludeQty,
                }).ToList();
                await _handlerContext.CreditNoteItems.AddRangeAsync(creditItems);
                await _handlerContext.SaveChangesAsync();

                var invoiceCurrencydate = await _handlerContext.Invoices.Where(e => e.InvoiceId == data.InvoiceId).Select(e => e.CurrencyValueDate).FirstAsync();
                var eurVal = await _handlerContext.CurrencyValues
                    .Where(e => e.UpdateDate == invoiceCurrencydate && e.CurrencyName == "EUR")
                    .Select(e => e.CurrencyValue1).FirstAsync();
                var usdVal = await _handlerContext.CurrencyValues
                    .Where(e => e.UpdateDate == invoiceCurrencydate && e.CurrencyName == "USD")
                    .Select(e => e.CurrencyValue1).FirstAsync();
                if(usdVal == 0 || eurVal == 0)
                {
                    Console.WriteLine("Could not get currency values.");
                    await trans.RollbackAsync();
                }

                foreach (var item in data.CreditNoteItems)
                {
                    if(item.IncludeQty)
                    {
                        await _handlerContext.ItemOwners
                            .Where(e => e.InvoiceId == item.InvoiceId && e.OwnedItemId == item.ItemId && e.IdUser == item.UserId)
                            .ExecuteUpdateAsync(setters => setters.SetProperty(s => s.Qty, s => s.Qty + item.Qty));
                    }
                }
                foreach (var item in creditItems)
                {
                    _handlerContext.CalculatedCreditNotePrices.Add(new CalculatedCreditNotePrice
                    {
                        CurrencyName = "USD",
                        UpdateDate = invoiceCurrencydate,
                        CreditItemId = item.CreditItemId,
                        Price = item.NewPrice / usdVal
                    });
                    _handlerContext.CalculatedCreditNotePrices.Add(new CalculatedCreditNotePrice
                    {
                        CurrencyName = "EUR",
                        UpdateDate = invoiceCurrencydate,
                        CreditItemId = item.CreditItemId,
                        Price = item.NewPrice / eurVal
                    });
                }
                await _handlerContext.SaveChangesAsync();
                await trans.CommitAsync();

            } catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                await trans.RollbackAsync();
            }
        }
    }
}
