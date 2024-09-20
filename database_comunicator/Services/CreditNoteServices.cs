using database_comunicator.Data;
using database_comunicator.Models;
using database_comunicator.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace database_comunicator.Services
{
    public interface ICreditNoteServices
    {
        public Task AddYoursCreditNote(AddCreditNote data);
        public Task<IEnumerable<GetCreditNote>> GetCreditNotes(bool yourCreditNotes);
        public Task<IEnumerable<GetCreditNote>> GetCreditNotes(bool yourCreditNotes, string search);
        public Task<IEnumerable<GetCreditNote>> GetCreditNotes(bool yourCreditNotes, int userId);
        public Task<IEnumerable<GetCreditNote>> GetCreditNotes(bool yourCreditNotes, string search, int userId);
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
        public async Task<IEnumerable<GetCreditNote>> GetCreditNotes(bool yourCreditNotes)
        {
            return await _handlerContext.CreditNotes
                .Where(e => yourCreditNotes ? e.Invoice.OwnedItems.Any() : e.Invoice.SellingPrices.Any())
                .Select(e => new GetCreditNote
                {
                    CreditNoteId = e.IdCreditNote,
                    InvoiceNumber = e.Invoice.InvoiceNumber,
                    Date = e.CreditNoteDate,
                    Qty = e.CreditNoteItems.Sum(d => d.Qty),
                    Total = e.CreditNoteItems.Sum(d => d.NewPrice),
                    ClientName = yourCreditNotes ? e.Invoice.SellerNavigation.OrgName : e.Invoice.BuyerNavigation.OrgName,
                    InSystem = e.InSystem
                }).ToListAsync();
        }
        public async Task<IEnumerable<GetCreditNote>> GetCreditNotes(bool yourCreditNotes, string search)
        {
            return await _handlerContext.CreditNotes
                .Where(e => yourCreditNotes ? e.Invoice.OwnedItems.Any() : e.Invoice.SellingPrices.Any())
                .Where(e => e.Invoice.InvoiceNumber.ToLower().Contains(search.ToLower()) 
                    || 
                    (yourCreditNotes ? 
                        e.Invoice.SellerNavigation.OrgName.ToLower().Contains(search.ToLower())
                        :
                        e.Invoice.BuyerNavigation.OrgName.ToLower().Contains(search.ToLower())
                    )
                )
                .Select(obj => new GetCreditNote
                {
                    CreditNoteId = obj.IdCreditNote,
                    InvoiceNumber = obj.Invoice.InvoiceNumber,
                    Date = obj.CreditNoteDate,
                    Qty = obj.CreditNoteItems.Sum(e => e.Qty),
                    Total = obj.CreditNoteItems.Sum(e => e.NewPrice),
                    ClientName = yourCreditNotes ? obj.Invoice.SellerNavigation.OrgName : obj.Invoice.BuyerNavigation.OrgName,
                    InSystem = obj.InSystem
                }).ToListAsync();
        }
        public async Task<IEnumerable<GetCreditNote>> GetCreditNotes(bool yourCreditNotes, int userId)
        {
            return await _handlerContext.CreditNotes
                .Where(e => yourCreditNotes ? 
                    e.Invoice.OwnedItems.SelectMany(e => e.ItemOwners).Any(x => x.IdUser == userId) 
                    : 
                    e.Invoice.SellingPrices.Any(x => x.IdUser == userId)
                 )
                .Where(e => yourCreditNotes ? e.Invoice.OwnedItems.Any() : e.Invoice.SellingPrices.Any())
                .Select(inst => new GetCreditNote
                {
                    CreditNoteId = inst.IdCreditNote,
                    InvoiceNumber = inst.Invoice.InvoiceNumber,
                    Date = inst.CreditNoteDate,
                    Qty = inst.CreditNoteItems.Sum(e => e.Qty),
                    Total = inst.CreditNoteItems.Sum(e => e.NewPrice),
                    ClientName = yourCreditNotes ? inst.Invoice.SellerNavigation.OrgName : inst.Invoice.BuyerNavigation.OrgName,
                    InSystem = inst.InSystem
                }).ToListAsync();
        }
        public async Task<IEnumerable<GetCreditNote>> GetCreditNotes(bool yourCreditNotes, string search, int userId)
        {
            return await _handlerContext.CreditNotes
                .Where(e => yourCreditNotes ?
                    e.Invoice.OwnedItems.SelectMany(e => e.ItemOwners).Any(x => x.IdUser == userId)
                    :
                    e.Invoice.SellingPrices.Any(x => x.IdUser == userId)
                 )
                .Where(e => yourCreditNotes ? e.Invoice.OwnedItems.Any() : e.Invoice.SellingPrices.Any())
                .Where(e => e.Invoice.InvoiceNumber.ToLower().Contains(search.ToLower())
                    ||
                    (yourCreditNotes ?
                        e.Invoice.SellerNavigation.OrgName.ToLower().Contains(search.ToLower())
                        :
                        e.Invoice.BuyerNavigation.OrgName.ToLower().Contains(search.ToLower())
                    )
                )
                .Select(ent => new GetCreditNote
                {
                    CreditNoteId = ent.IdCreditNote,
                    InvoiceNumber = ent.Invoice.InvoiceNumber,
                    Date = ent.CreditNoteDate,
                    Qty = ent.CreditNoteItems.Sum(e => e.Qty),
                    Total = ent.CreditNoteItems.Sum(e => e.NewPrice),
                    ClientName = yourCreditNotes ? ent.Invoice.SellerNavigation.OrgName : ent.Invoice.BuyerNavigation.OrgName,
                    InSystem = ent.InSystem
                }).ToListAsync();

        }
    }
}
