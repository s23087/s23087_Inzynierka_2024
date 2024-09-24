using database_comunicator.Data;
using database_comunicator.Models;
using database_comunicator.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace database_comunicator.Services
{
    public interface ICreditNoteServices
    {
        public Task<int> AddCreditNote(AddCreditNote data);
        public Task<IEnumerable<GetCreditNote>> GetCreditNotes(bool yourCreditNotes);
        public Task<IEnumerable<GetCreditNote>> GetCreditNotes(bool yourCreditNotes, string search);
        public Task<IEnumerable<GetCreditNote>> GetCreditNotes(bool yourCreditNotes, int userId);
        public Task<IEnumerable<GetCreditNote>> GetCreditNotes(bool yourCreditNotes, string search, int userId);
        public Task<bool> CreditDeductionCanBeApplied(int userId, int invoiceid, int itemId, int qty);
        public Task<bool> CreditNoteExist(string creditNoteNumber, int invoiceId);
        public Task<bool> CreditNoteExist(int creditNoteId);
        public Task<GetRestCreditNote> GetRestCreditNote(int creditNoteId);
        public Task<bool> DeleteCreditNote(int creditNoteId);
        public Task<int> GetCreditNoteUser(int creditNoteId);
        public Task<string> GetCreditNumber(int creditNoteId);
        public Task<string?> GetCreditFilePath(int creditNoteId);
    }
    public class CreditNoteServices : ICreditNoteServices
    {
        private readonly HandlerContext _handlerContext;
        public CreditNoteServices(HandlerContext handlerContext)
        {
            _handlerContext = handlerContext;
        }
        public async Task<int> AddCreditNote(AddCreditNote data)
        {
            using var trans = await _handlerContext.Database.BeginTransactionAsync();
            try
            {
                var creditNote = new CreditNote
                {
                    CreditNoteDate = data.CreditNoteDate,
                    CreditNoteNumber = data.CreditNotenumber,
                    InSystem = data.InSystem,
                    Note = data.Note,
                    InvoiceId = data.InvoiceId,
                    IsPaid = data.IsPaid,
                    CreditFilePath = data.FilePath,
                    IdUser = data.CreditNoteItems.Select(e => e.UserId).First()
                };
                await _handlerContext.AddAsync<CreditNote>(creditNote);
                await _handlerContext.SaveChangesAsync();

                var currencyName = await _handlerContext.Invoices.Where(e => e.InvoiceId == data.InvoiceId).Select(e => e.CurrencyName).FirstAsync();
                var invoiceCurrencydate = await _handlerContext.Invoices.Where(e => e.InvoiceId == data.InvoiceId).Select(e => e.CurrencyValueDate).FirstAsync();
                if (currencyName != "PLN")
                {
                    var currencyVal = await _handlerContext.Invoices
                        .Where(e => e.InvoiceId == data.InvoiceId)
                        .Select(e => e.Currency)
                        .Where(e => e.CurrencyName == currencyName)
                        .Select(e => e.CurrencyValue1).FirstAsync();

                    var creditItems = data.CreditNoteItems.Select(e => new CreditNoteItem
                    {
                        CreditNoteId = creditNote.IdCreditNote,
                        PurchasePriceId = e.PurchasePriceId,
                        Qty = e.Qty,
                        NewPrice = e.NewPrice * currencyVal
                    }).ToList();

                    await _handlerContext.CreditNoteItems.AddRangeAsync(creditItems);
                    await _handlerContext.SaveChangesAsync();

                    var calculatedItems = creditItems.Select(e => new CalculatedCreditNotePrice
                    {
                        CurrencyName = currencyName,
                        UpdateDate = invoiceCurrencydate,
                        CreditItemId = e.CreditItemId,
                        Price = e.NewPrice / currencyVal
                    }).ToList();

                    await _handlerContext.CalculatedCreditNotePrices.AddRangeAsync(calculatedItems);

                    if (data.IsYourCreditNote)
                    {
                        var secVal = await _handlerContext.Invoices
                            .Where(e => e.InvoiceId == data.InvoiceId)
                            .Select(e => e.Currency)
                            .Where(e => e.CurrencyName != currencyName && e.CurrencyName != "PLN")
                            .Select(e => e.CurrencyValue1).FirstAsync();

                        var secCalculatedItems = creditItems.Select(e => new CalculatedCreditNotePrice
                        {
                            CurrencyName = currencyName,
                            UpdateDate = invoiceCurrencydate,
                            CreditItemId = e.CreditItemId,
                            Price = e.NewPrice / currencyVal
                        }).ToList();

                        await _handlerContext.CalculatedCreditNotePrices.AddRangeAsync(secCalculatedItems);
                    }
                } else
                {
                    var creditItems = data.CreditNoteItems.Select(e => new CreditNoteItem
                    {
                        CreditNoteId = creditNote.IdCreditNote,
                        PurchasePriceId = e.PurchasePriceId,
                        Qty = e.Qty,
                        NewPrice = e.NewPrice,
                    }).ToList();

                    await _handlerContext.CreditNoteItems.AddRangeAsync(creditItems);
                    await _handlerContext.SaveChangesAsync();

                    if (data.IsYourCreditNote)
                    {
                        var updateDate = await _handlerContext.Invoices.Where(e => e.InvoiceId == data.InvoiceId).Select(e => e.InvoiceDate).FirstAsync();
                        var usdVal = await _handlerContext.CurrencyValues
                            .Where(e => e.CurrencyName == "USD" && e.UpdateDate == updateDate)
                            .Select(e => e.CurrencyValue1).FirstAsync();
                        var eurVal = await _handlerContext.CurrencyValues
                            .Where(e => e.CurrencyName == "EUR" && e.UpdateDate == updateDate)
                            .Select(e => e.CurrencyValue1).FirstAsync();

                        var usdItems = creditItems.Select(e => new CalculatedCreditNotePrice
                        {
                            CurrencyName = "USD",
                            UpdateDate = updateDate,
                            CreditItemId = e.CreditItemId,
                            Price = e.NewPrice / usdVal
                        }).ToList();

                        var eurItems = creditItems.Select(e => new CalculatedCreditNotePrice
                        {
                            CurrencyName = "EUR",
                            UpdateDate = updateDate,
                            CreditItemId = e.CreditItemId,
                            Price = e.NewPrice / eurVal
                        }).ToList();

                        await _handlerContext.CalculatedCreditNotePrices.AddRangeAsync(usdItems);
                        await _handlerContext.CalculatedCreditNotePrices.AddRangeAsync(eurItems);
                    }
                }

                foreach (var item in data.CreditNoteItems)
                {
                    await _handlerContext.ItemOwners
                            .Where(e => e.InvoiceId == item.InvoiceId && e.OwnedItemId == item.ItemId && e.IdUser == item.UserId)
                            .ExecuteUpdateAsync(setters => setters.SetProperty(s => s.Qty, s => s.Qty + item.Qty));
                }
                await _handlerContext.SaveChangesAsync();
                await trans.CommitAsync();
                return creditNote.IdCreditNote;
            } catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                await trans.RollbackAsync();
                return 0;
            }
        }
        public async Task<IEnumerable<GetCreditNote>> GetCreditNotes(bool yourCreditNotes)
        {
            return await _handlerContext.CreditNotes
                .Where(e => yourCreditNotes ? e.Invoice.OwnedItems.Any() : e.Invoice.SellingPrices.Any())
                .Select(e => new GetCreditNote
                {
                    User = e.User.Username + " " + e.User.Surname,
                    CreditNoteId = e.IdCreditNote,
                    InvoiceNumber = e.Invoice.InvoiceNumber,
                    Date = e.CreditNoteDate,
                    Qty = e.CreditNoteItems.Any(d => d.Qty > 0) ? e.CreditNoteItems.Where(d => d.Qty > 0).Select(d => d.Qty).Sum() : e.CreditNoteItems.Sum(d => d.Qty),
                    Total = e.CreditNoteItems.Sum(d => d.NewPrice * d.Qty),
                    ClientName = yourCreditNotes ? e.Invoice.SellerNavigation.OrgName : e.Invoice.BuyerNavigation.OrgName,
                    InSystem = e.InSystem,
                    IsPaid = e.IsPaid
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
                    User = obj.User.Username + " " + obj.User.Surname,
                    CreditNoteId = obj.IdCreditNote,
                    InvoiceNumber = obj.Invoice.InvoiceNumber,
                    Date = obj.CreditNoteDate,
                    Qty = obj.CreditNoteItems.Any(d => d.Qty > 0) ? obj.CreditNoteItems.Where(d => d.Qty > 0).Select(d => d.Qty).Sum() : obj.CreditNoteItems.Sum(d => d.Qty),
                    Total = obj.CreditNoteItems.Sum(e => e.NewPrice * e.Qty),
                    ClientName = yourCreditNotes ? obj.Invoice.SellerNavigation.OrgName : obj.Invoice.BuyerNavigation.OrgName,
                    InSystem = obj.InSystem,
                    IsPaid = obj.IsPaid
                }).ToListAsync();
        }
        public async Task<IEnumerable<GetCreditNote>> GetCreditNotes(bool yourCreditNotes, int userId)
        {
            return await _handlerContext.CreditNotes
                .Where(e => e.IdUser == userId)
                .Where(e => yourCreditNotes ? e.Invoice.OwnedItems.Any() : e.Invoice.SellingPrices.Any())
                .Select(inst => new GetCreditNote
                {
                    CreditNoteId = inst.IdCreditNote,
                    InvoiceNumber = inst.Invoice.InvoiceNumber,
                    Date = inst.CreditNoteDate,
                    Qty = inst.CreditNoteItems.Any(d => d.Qty > 0) ? inst.CreditNoteItems.Where(d => d.Qty > 0).Select(d => d.Qty).Sum() : inst.CreditNoteItems.Sum(d => d.Qty),
                    Total = inst.CreditNoteItems.Sum(e => e.NewPrice * e.Qty),
                    ClientName = yourCreditNotes ? inst.Invoice.SellerNavigation.OrgName : inst.Invoice.BuyerNavigation.OrgName,
                    InSystem = inst.InSystem,
                    IsPaid = inst.IsPaid
                }).ToListAsync();
        }
        public async Task<IEnumerable<GetCreditNote>> GetCreditNotes(bool yourCreditNotes, string search, int userId)
        {
            return await _handlerContext.CreditNotes
                .Where(e => e.IdUser == userId)
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
                    Qty = ent.CreditNoteItems.Any(d => d.Qty > 0) ? ent.CreditNoteItems.Where(d => d.Qty > 0).Select(d => d.Qty).Sum() : ent.CreditNoteItems.Sum(d => d.Qty),
                    Total = ent.CreditNoteItems.Sum(e => e.NewPrice * e.Qty),
                    ClientName = yourCreditNotes ? ent.Invoice.SellerNavigation.OrgName : ent.Invoice.BuyerNavigation.OrgName,
                    InSystem = ent.InSystem,
                    IsPaid = ent.IsPaid,
                }).ToListAsync();

        }
        public async Task<bool> CreditDeductionCanBeApplied(int userId, int invoiceid, int itemId, int qty)
        {
            var currentResult = await _handlerContext.ItemOwners
                .Where(e => e.IdUser == userId && e.InvoiceId == invoiceid && e.OwnedItemId == itemId)
                .Select(e => e.Qty).FirstAsync();
            return currentResult - qty >= 0;
        }
        public async Task<bool> CreditNoteExist(string creditNoteNumber, int invoiceId)
        {
            return await _handlerContext.CreditNotes.AnyAsync(x => x.CreditNoteNumber == creditNoteNumber && x.InvoiceId == invoiceId);
        }
        public async Task<bool> CreditNoteExist(int creditNoteId)
        {
            return await _handlerContext.CreditNotes.AnyAsync(x => x.IdCreditNote == creditNoteId);
        }
        public async Task<GetRestCreditNote> GetRestCreditNote(int creditNoteId)
        {
            var creditCurrency = await _handlerContext.CreditNotes.Where(e => e.IdCreditNote == creditNoteId).Select(e => e.Invoice.CurrencyName).FirstAsync();
            return await _handlerContext.CreditNotes
                .Where(e => e.IdCreditNote == creditNoteId)
                .Select(e => new GetRestCreditNote
                {
                    CreditNoteNumber = e.CreditNoteNumber,
                    CurrencyName = creditCurrency,
                    Note = e.Note,
                    Path = e.CreditFilePath ?? "",
                    CreditItems = e.CreditNoteItems.Select(x => new GetCredtItemForTable
                    {
                        CreditItemId = x.CreditItemId,
                        Partnumber = x.PurchasePrice.OwnedItem.OriginalItem.PartNumber,
                        ItemName = x.PurchasePrice.OwnedItem.OriginalItem.ItemName,
                        Qty = e.CreditNoteItems.All(d => d.Qty > 0 && d.CreditItemId == x.CreditItemId) ? x.Qty * -1 : x.Qty,
                        Price = creditCurrency == "PLN" ? x.NewPrice : x.CalculatedCreditNotePrices.Where(d => d.CurrencyName == creditCurrency).Select(d => d.Price).First(),
                    }).ToList(),
                }).FirstAsync();
        }
        public async Task<bool> DeleteCreditNote(int creditNoteId)
        {
            using var trans = await _handlerContext.Database.BeginTransactionAsync();
            try
            {
                var creditItems = await _handlerContext.CreditNoteItems
                    .Where(e => e.CreditNoteId == creditNoteId)
                    .Select(e => new
                    {
                        e.CreditItemId, e.PurchasePrice.InvoiceId, e.PurchasePrice.OwnedItemId, e.CreditNote.IdUser, e.Qty
                    }).ToListAsync();
                foreach (var creditItem in creditItems)
                {
                    await _handlerContext.ItemOwners
                        .Where(e => e.InvoiceId == creditItem.InvoiceId && e.IdUser == creditItem.IdUser && e.OwnedItemId == creditItem.OwnedItemId)
                        .ExecuteUpdateAsync(setter =>
                            setter.SetProperty(s => s.Qty, s => s.Qty - creditItem.Qty)
                        );
                    await _handlerContext.CalculatedCreditNotePrices.Where(e => e.CreditItemId == creditItem.CreditItemId).ExecuteDeleteAsync();
                }
                await _handlerContext.CreditNoteItems.Where(e => e.CreditNoteId == creditNoteId).ExecuteDeleteAsync();
                await _handlerContext.CreditNotes.Where(e => e.IdCreditNote == creditNoteId).ExecuteDeleteAsync();
                await _handlerContext.SaveChangesAsync();
                await trans.CommitAsync();
                return true;
            } catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                await trans.RollbackAsync();
                return false;
            }
        }
        public async Task<int> GetCreditNoteUser(int creditNoteId)
        {
            return await _handlerContext.CreditNotes.Where(e => e.IdCreditNote == creditNoteId).Select(e => e.IdUser).FirstAsync();
        }
        public async Task<string> GetCreditNumber(int creditNoteId)
        {
            return await _handlerContext.CreditNotes.Where(e => e.IdCreditNote == creditNoteId).Select(e => e.CreditNoteNumber).FirstAsync();
        }
        public async Task<string?> GetCreditFilePath(int creditNoteId)
        {
            return await _handlerContext.CreditNotes.Where(e => e.IdCreditNote == creditNoteId).Select(e => e.CreditFilePath).FirstAsync();
        }
    }
}
