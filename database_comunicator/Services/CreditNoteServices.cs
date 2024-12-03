using database_communicator.Data;
using database_communicator.FilterClass;
using database_communicator.Models;
using database_communicator.Models.DTOs.Create;
using database_communicator.Models.DTOs.Get;
using database_communicator.Models.DTOs.Modify;
using database_communicator.Utils;
using Microsoft.EntityFrameworkCore;

namespace database_communicator.Services
{
    /// <summary>
    /// Interface that interact with database and contains functions allowing to work on credit notes.
    /// </summary>
    public interface ICreditNoteServices
    {
        /// <summary>
        /// Using transactions add credit note and it's items to database.
        /// </summary>
        /// <param name="data">Credit note data wrapped in <see cref="Models.DTOs.Create.AddCreditNote"/></param>
        /// <returns>Id of new credit note or 0 if creation was not successful.</returns>
        public Task<int> AddCreditNote(AddCreditNote data);
        /// <summary>
        /// Do select query with given filter and sort to receive credit note information.
        /// </summary>
        /// <param name="yourCreditNotes">True if credit note was given by client to user, otherwise false.</param>
        /// <param name="sort">Contains parameter that object will be sorted by. Must start with D or A to determine ascending order. Then is follow by name of property.</param>
        /// <param name="filters">Object with filter values wrapped in <see cref="CreditNoteFiltersTemplate"/></param>
        /// <returns>List of <see cref="GetCreditNote"/>.</returns>
        public Task<IEnumerable<GetCreditNote>> GetCreditNotes(bool yourCreditNotes, string? sort, CreditNoteFiltersTemplate filters);
        /// <summary>
        /// Do select query with given search, filter and sort to receive credit note information.
        /// </summary>
        /// <param name="yourCreditNotes">True if credit note was given by client to user, otherwise false.</param>
        /// <param name="search">The phrase searched in credit note information. It will check if phrase exist in organization name or invoice number.</param>
        /// <param name="sort">Contains parameter that object will be sorted by. Must start with D or A to determine ascending order. Then is follow by name of property.</param>
        /// <param name="filters">Object with filter values wrapped in <see cref="CreditNoteFiltersTemplate"/></param>
        /// <returns>List of <see cref="GetCreditNote"/>.</returns>
        public Task<IEnumerable<GetCreditNote>> GetCreditNotes(bool yourCreditNotes, string search, string? sort, CreditNoteFiltersTemplate filters);
        /// <summary>
        /// Do select query with given filter and sort to receive credit note information for given user.
        /// </summary>
        /// <param name="yourCreditNotes">True if credit note was given by client to user, otherwise false.</param>
        /// <param name="userId">Id of user.</param>
        /// <param name="sort">Contains parameter that object will be sorted by. Must start with D or A to determine ascending order. Then is follow by name of property.</param>
        /// <param name="filters">Object with filter values wrapped in <see cref="CreditNoteFiltersTemplate"/></param>
        /// <returns>List of <see cref="GetCreditNote"/>.</returns>
        public Task<IEnumerable<GetCreditNote>> GetCreditNotes(bool yourCreditNotes, int userId, string? sort, CreditNoteFiltersTemplate filters);
        /// <summary>
        /// Do select query with given search, filter and sort to receive credit note information for given user.
        /// </summary>
        /// <param name="yourCreditNotes">True if proforma is form client to user, false if it's from user to his client.</param>
        /// <param name="search">The phrase searched in credit note information. It will check if phrase exist in organization name or invoice number.</param>
        /// <param name="userId">Id of user.</param>
        /// <param name="sort">Contains parameter that object will be sorted by. Must start with D or A to determine ascending order. Then is follow by name of property.</param>
        /// <param name="filters">Object with filter values wrapped in <see cref="CreditNoteFiltersTemplate"/></param>
        /// <returns>List of <see cref="GetCreditNote"/>.</returns>
        public Task<IEnumerable<GetCreditNote>> GetCreditNotes(bool yourCreditNotes, string search, int userId, string? sort, CreditNoteFiltersTemplate filters);
        /// <summary>
        /// Check if deduction that comes with credit note can be applied to the user with given item.
        /// </summary>
        /// <param name="userId">Id of user that owns credit note.</param>
        /// <param name="invoiceId">Id of invoice that credit note will be applied.</param>
        /// <param name="itemId">Id of item that quantity will be deduced from.</param>
        /// <param name="qty">Quantity that will be deduced.</param>
        /// <returns>True if can be deduced or false if can't</returns>
        public Task<bool> CreditDeductionCanBeApplied(int userId, int invoiceId, int itemId, int qty);
        /// <summary>
        /// Checks if credit note with given invoice id and credit note number exist.
        /// </summary>
        /// <param name="creditNoteNumber">Credit note number</param>
        /// <param name="invoiceId">Id of invoice bounded to credit note.</param>
        /// <returns>True if exist, false if don't</returns>
        public Task<bool> CreditNoteExist(string creditNoteNumber, int invoiceId);
        /// <summary>
        /// Checks if credit note with given id exist.
        /// </summary>
        /// <param name="creditNoteId">Credit note id</param>
        /// <returns>True if exist, false if not.</returns>
        public Task<bool> CreditNoteExist(int creditNoteId);
        /// <summary>
        /// Do select query using given id to receive credit note information that was not given in bulk query.
        /// </summary>
        /// <param name="creditNoteId">Credit note id.</param>
        /// <returns>Credit note information wrapped in <see cref="Models.DTOs.Get.GetRestCreditNote"/>.</returns>
        public Task<GetRestCreditNote> GetRestCreditNote(int creditNoteId);
        /// <summary>
        /// Do delete query on credit note and credit note items table. Also deduces item qty from item-owner table.
        /// </summary>
        /// <param name="creditNoteId">Credit note id to delete.</param>
        /// <returns>True if action was success, false if failure.</returns>
        public Task<bool> DeleteCreditNote(int creditNoteId);
        /// <summary>
        /// Do select query on credit note table to get user id bounded to credit note.
        /// </summary>
        /// <param name="creditNoteId">Credit note id.</param>
        /// <returns>Return id of user.</returns>
        public Task<int> GetCreditNoteUser(int creditNoteId);
        /// <summary>
        /// Do select query on credit note table to get credit note number.
        /// </summary>
        /// <param name="creditNoteId">Credit note id.</param>
        /// <returns>String containing credit note number.</returns>
        public Task<string> GetCreditNumber(int creditNoteId);
        /// <summary>
        /// Do select query on credit note table to get credit note file path.
        /// </summary>
        /// <param name="creditNoteId">Credit note id.</param>
        /// <returns>String containing file path.</returns>
        public Task<string?> GetCreditFilePath(int creditNoteId);
        /// <summary>
        /// Do select query to receive credit note information needed to modify it, but not passed in bulk query.
        /// </summary>
        /// <param name="creditNoteId">Credit note id.</param>
        /// <param name="isYourCredit">True if credit note was given by client to user, otherwise false.</param>
        /// <returns>Chosen credit note information wrapped in  <see cref="Models.DTOs.Get.GetRestModifyCredit"/>.</returns>
        public Task<GetRestModifyCredit> GetRestModifyCredit(int creditNoteId, bool isYourCredit);
        /// <summary>
        /// Do select query to receive credit note invoice id.
        /// </summary>
        /// <param name="creditNoteId">Credit note id.</param>
        /// <returns>Invoice id that's bound to credit note.</returns>
        public Task<int> GetCreditNoteInvoiceId(int creditNoteId);
        /// <summary>
        /// Overwrites chosen credit note properties given in data parameter.
        /// </summary>
        /// <param name="data">Object of <see cref="Models.DTOs.Modify.ModifyCreditNote"/>.</param>
        /// <returns>True if success, false if failure.</returns>
        public Task<bool> ModifyCreditNote(ModifyCreditNote data);
    }
    /// <remarks>
    /// Class for Credit note services.
    /// </remarks>
    /// <param name="handlerContext">Database context</param>
    /// <param name="logger">Log interface</param>
    public class CreditNoteServices(HandlerContext handlerContext, ILogger<CreditNoteServices> logger) : ICreditNoteServices
    {
        private readonly HandlerContext _handlerContext = handlerContext;
        private readonly ILogger<CreditNoteServices> _logger = logger;
        /// <summary>
        /// Using transactions add credit note and it's items to database.
        /// </summary>
        /// <param name="data">Credit note data wrapped in <see cref="Models.DTOs.Create.AddCreditNote"/></param>
        /// <returns>Id of new credit note or 0 if creation was not successful.</returns>
        public async Task<int> AddCreditNote(AddCreditNote data)
        {
            using var trans = await _handlerContext.Database.BeginTransactionAsync();
            try
            {
                var creditNote = new CreditNote
                {
                    CreditNoteDate = data.CreditNoteDate,
                    CreditNoteNumber = data.CreditNoteNumber,
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
                var invoiceCurrencyDate = await _handlerContext.Invoices.Where(e => e.InvoiceId == data.InvoiceId).Select(e => e.CurrencyValueDate).FirstAsync();
                if (currencyName != "PLN")
                {
                    var currencyVal = await _handlerContext.CurrencyValues
                        .Where(e => e.CurrencyName == currencyName && e.UpdateDate.Equals(invoiceCurrencyDate))
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
                        UpdateDate = invoiceCurrencyDate,
                        CreditItemId = e.CreditItemId,
                        Price = e.NewPrice / currencyVal
                    }).ToList();

                    await _handlerContext.CalculatedCreditNotePrices.AddRangeAsync(calculatedItems);

                    if (data.IsYourCreditNote)
                    {
                        var secVal = await _handlerContext.CurrencyValues
                            .Where(e => e.CurrencyName != currencyName && e.CurrencyName != "PLN" && e.UpdateDate == invoiceCurrencyDate)
                            .Select(e => new { e.CurrencyValue1, e.CurrencyName}).FirstAsync();

                        var secCalculatedItems = creditItems.Select(e => new CalculatedCreditNotePrice
                        {
                            CurrencyName = secVal.CurrencyName,
                            UpdateDate = invoiceCurrencyDate,
                            CreditItemId = e.CreditItemId,
                            Price = e.NewPrice / secVal.CurrencyValue1
                        }).ToList();

                        await _handlerContext.CalculatedCreditNotePrices.AddRangeAsync(secCalculatedItems);
                    }
                }
                else
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
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Create credit note error.");
                await trans.RollbackAsync();
                return 0;
            }
        }
        /// <summary>
        /// Do select query with given filter and sort to receive credit note information.
        /// </summary>
        /// <param name="yourCreditNotes">True if credit note was given by client to user, otherwise false.</param>
        /// <param name="sort">Contains parameter that object will be sorted by. Must start with D or A to determine ascending order. Then is follow by name of property.</param>
        /// <param name="filters">Object with filter values wrapped in <see cref="CreditNoteFiltersTemplate"/></param>
        /// <returns>List of <see cref="GetCreditNote"/>.</returns>
        public async Task<IEnumerable<GetCreditNote>> GetCreditNotes(bool yourCreditNotes, string? sort, CreditNoteFiltersTemplate filters)
        {
            var sortFunc = SortFilterUtils.GetCreditNoteSort(sort);
            bool direction;
            if (sort == null)
            {
                direction = true;
            }
            else
            {
                direction = sort.StartsWith('D');
            }
            var dateLCond = CreditNoteFilters.GetDateLowerFilter(filters.DateL);
            var dateGCond = CreditNoteFilters.GetDateGreaterFilter(filters.DateL);
            var qtyLCond = CreditNoteFilters.GetQtyLowerFilter(filters.QtyL);
            var qtyGCond = CreditNoteFilters.GetQtyGreaterFilter(filters.QtyG);
            var totalLCond = CreditNoteFilters.GetPriceLowerFilter(filters.TotalL);
            var totalGCond = CreditNoteFilters.GetPriceGreaterFilter(filters.TotalG);
            var recipientCond = CreditNoteFilters.GetRecipientFilter(filters.Recipient, yourCreditNotes);
            var currencyCond = CreditNoteFilters.GetCurrencyFilter(filters.Currency);
            var paymentStatusCond = CreditNoteFilters.GetPaymentStatusFilter(filters.PaymentStatus);
            var statusCond = CreditNoteFilters.GetStatusFilter(filters.Status);

            return await _handlerContext.CreditNotes
                .Where(e => yourCreditNotes ? e.Invoice.OwnedItems.Any() : e.Invoice.SellingPrices.Any())
                .Where(dateLCond)
                .Where(dateGCond)
                .Where(qtyLCond)
                .Where(qtyGCond)
                .Where(totalLCond)
                .Where(totalGCond)
                .Where(recipientCond)
                .Where(currencyCond)
                .Where(paymentStatusCond)
                .Where(statusCond)
                .OrderByWithDirection(sortFunc, direction)
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
        /// <summary>
        /// Do select query with given search, filter and sort to receive credit note information.
        /// </summary>
        /// <param name="yourCreditNotes">True if credit note was given by client to user, otherwise false.</param>
        /// <param name="search">The phrase searched in credit note information. It will check if phrase exist in organization name or invoice number.</param>
        /// <param name="sort">Contains parameter that object will be sorted by. Must start with D or A to determine ascending order. Then is follow by name of property.</param>
        /// <param name="filters">Object with filter values wrapped in <see cref="CreditNoteFiltersTemplate"/></param>
        /// <returns>List of <see cref="GetCreditNote"/>.</returns>
        public async Task<IEnumerable<GetCreditNote>> GetCreditNotes(bool yourCreditNotes, string search, string? sort, CreditNoteFiltersTemplate filters)
        {
            var sortFunc = SortFilterUtils.GetCreditNoteSort(sort);
            bool direction;
            if (sort == null)
            {
                direction = true;
            }
            else
            {
                direction = sort.StartsWith('D');
            }
            var dateLCond = CreditNoteFilters.GetDateLowerFilter(filters.DateL);
            var dateGCond = CreditNoteFilters.GetDateGreaterFilter(filters.DateL);
            var qtyLCond = CreditNoteFilters.GetQtyLowerFilter(filters.QtyL);
            var qtyGCond = CreditNoteFilters.GetQtyGreaterFilter(filters.QtyG);
            var totalLCond = CreditNoteFilters.GetPriceLowerFilter(filters.TotalL);
            var totalGCond = CreditNoteFilters.GetPriceGreaterFilter(filters.TotalG);
            var recipientCond = CreditNoteFilters.GetRecipientFilter(filters.Recipient, yourCreditNotes);
            var currencyCond = CreditNoteFilters.GetCurrencyFilter(filters.Currency);
            var paymentStatusCond = CreditNoteFilters.GetPaymentStatusFilter(filters.PaymentStatus);
            var statusCond = CreditNoteFilters.GetStatusFilter(filters.Status);

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
                .Where(dateLCond)
                .Where(dateGCond)
                .Where(qtyLCond)
                .Where(qtyGCond)
                .Where(totalLCond)
                .Where(totalGCond)
                .Where(recipientCond)
                .Where(currencyCond)
                .Where(paymentStatusCond)
                .Where(statusCond)
                .OrderByWithDirection(sortFunc, direction)
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
        /// <summary>
        /// Do select query with given filter and sort to receive credit note information for given user.
        /// </summary>
        /// <param name="yourCreditNotes">True if credit note was given by client to user, otherwise false.</param>
        /// <param name="userId">Id of user.</param>
        /// <param name="sort">Contains parameter that object will be sorted by. Must start with D or A to determine ascending order. Then is follow by name of property.</param>
        /// <param name="filters">Object with filter values wrapped in <see cref="CreditNoteFiltersTemplate"/></param>
        /// <returns>List of <see cref="GetCreditNote"/>.</returns>
        public async Task<IEnumerable<GetCreditNote>> GetCreditNotes(bool yourCreditNotes, int userId, string? sort, CreditNoteFiltersTemplate filters)
        {
            var sortFunc = SortFilterUtils.GetCreditNoteSort(sort);
            bool direction;
            if (sort == null)
            {
                direction = true;
            }
            else
            {
                direction = sort.StartsWith('D');
            }
            var dateLCond = CreditNoteFilters.GetDateLowerFilter(filters.DateL);
            var dateGCond = CreditNoteFilters.GetDateGreaterFilter(filters.DateL);
            var qtyLCond = CreditNoteFilters.GetQtyLowerFilter(filters.QtyL);
            var qtyGCond = CreditNoteFilters.GetQtyGreaterFilter(filters.QtyG);
            var totalLCond = CreditNoteFilters.GetPriceLowerFilter(filters.TotalL);
            var totalGCond = CreditNoteFilters.GetPriceGreaterFilter(filters.TotalG);
            var recipientCond = CreditNoteFilters.GetRecipientFilter(filters.Recipient, yourCreditNotes);
            var currencyCond = CreditNoteFilters.GetCurrencyFilter(filters.Currency);
            var paymentStatusCond = CreditNoteFilters.GetPaymentStatusFilter(filters.PaymentStatus);
            var statusCond = CreditNoteFilters.GetStatusFilter(filters.Status);

            return await _handlerContext.CreditNotes
                .Where(e => e.IdUser == userId)
                .Where(e => yourCreditNotes ? e.Invoice.OwnedItems.Any() : e.Invoice.SellingPrices.Any())
                .Where(dateLCond)
                .Where(dateGCond)
                .Where(qtyLCond)
                .Where(qtyGCond)
                .Where(totalLCond)
                .Where(totalGCond)
                .Where(recipientCond)
                .Where(currencyCond)
                .Where(paymentStatusCond)
                .Where(statusCond)
                .OrderByWithDirection(sortFunc, direction)
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
        /// <summary>
        /// Do select query with given search, filter and sort to receive credit note information for given user.
        /// </summary>
        /// <param name="yourCreditNotes">True if proforma is form client to user, false if it's from user to his client.</param>
        /// <param name="search">The phrase searched in credit note information. It will check if phrase exist in organization name or invoice number.</param>
        /// <param name="userId">Id of user.</param>
        /// <param name="sort">Contains parameter that object will be sorted by. Must start with D or A to determine ascending order. Then is follow by name of property.</param>
        /// <param name="filters">Object with filter values wrapped in <see cref="CreditNoteFiltersTemplate"/></param>
        /// <returns>List of <see cref="GetCreditNote"/>.</returns>
        public async Task<IEnumerable<GetCreditNote>> GetCreditNotes(bool yourCreditNotes, string search, int userId, string? sort, CreditNoteFiltersTemplate filters)
        {
            var sortFunc = SortFilterUtils.GetCreditNoteSort(sort);
            bool direction;
            if (sort == null)
            {
                direction = true;
            }
            else
            {
                direction = sort.StartsWith('D');
            }
            var dateLCond = CreditNoteFilters.GetDateLowerFilter(filters.DateL);
            var dateGCond = CreditNoteFilters.GetDateGreaterFilter(filters.DateL);
            var qtyLCond = CreditNoteFilters.GetQtyLowerFilter(filters.QtyL);
            var qtyGCond = CreditNoteFilters.GetQtyGreaterFilter(filters.QtyG);
            var totalLCond = CreditNoteFilters.GetPriceLowerFilter(filters.TotalL);
            var totalGCond = CreditNoteFilters.GetPriceGreaterFilter(filters.TotalG);
            var recipientCond = CreditNoteFilters.GetRecipientFilter(filters.Recipient, yourCreditNotes);
            var currencyCond = CreditNoteFilters.GetCurrencyFilter(filters.Currency);
            var paymentStatusCond = CreditNoteFilters.GetPaymentStatusFilter(filters.PaymentStatus);
            var statusCond = CreditNoteFilters.GetStatusFilter(filters.Status);

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
                .Where(dateLCond)
                .Where(dateGCond)
                .Where(qtyLCond)
                .Where(qtyGCond)
                .Where(totalLCond)
                .Where(totalGCond)
                .Where(recipientCond)
                .Where(currencyCond)
                .Where(paymentStatusCond)
                .Where(statusCond)
                .OrderByWithDirection(sortFunc, direction)
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
        /// <summary>
        /// Check if deduction that comes with credit note can be applied to the user with given item.
        /// </summary>
        /// <param name="userId">Id of user that owns credit note.</param>
        /// <param name="invoiceId">Id of invoice that credit note will be applied.</param>
        /// <param name="itemId">Id of item that quantity will be deduced from.</param>
        /// <param name="qty">Quantity that will be deduced. Must be negative.</param>
        /// <returns>True if can be deduced or false if can't</returns>
        public async Task<bool> CreditDeductionCanBeApplied(int userId, int invoiceId, int itemId, int qty)
        {
            var exist = await _handlerContext.ItemOwners
                .AnyAsync(e => e.IdUser == userId && e.InvoiceId == invoiceId && e.OwnedItemId == itemId);
            if (!exist) return false;
            var currentResult = await _handlerContext.ItemOwners
                .Where(e => e.IdUser == userId && e.InvoiceId == invoiceId && e.OwnedItemId == itemId)
                .Select(e => e.Qty).FirstAsync();
            return (currentResult + qty) >= 0;
        }
        /// <summary>
        /// Checks if credit note with given invoice id and credit note number exist.
        /// </summary>
        /// <param name="creditNoteNumber">Credit note number</param>
        /// <param name="invoiceId">Id of invoice bounded to credit note.</param>
        /// <returns>True if exist, false if don't</returns>
        public async Task<bool> CreditNoteExist(string creditNoteNumber, int invoiceId)
        {
            return await _handlerContext.CreditNotes.AnyAsync(x => x.CreditNoteNumber == creditNoteNumber && x.InvoiceId == invoiceId);
        }
        /// <summary>
        /// Checks if credit note with given id exist.
        /// </summary>
        /// <param name="creditNoteId">Credit note id</param>
        /// <returns>True if exist, false if not.</returns>
        public async Task<bool> CreditNoteExist(int creditNoteId)
        {
            return await _handlerContext.CreditNotes.AnyAsync(x => x.IdCreditNote == creditNoteId);
        }
        /// <summary>
        /// Do select query using given id to receive credit note information that was not given in bulk query.
        /// </summary>
        /// <param name="creditNoteId">Credit note id.</param>
        /// <returns>Credit note information wrapped in <see cref="Models.DTOs.Get.GetRestCreditNote"/>.</returns>
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
        /// <summary>
        /// Do delete query on credit note and credit note items table. Also deduces item qty from item-owner table.
        /// </summary>
        /// <param name="creditNoteId">Credit note id to delete.</param>
        /// <returns>True if action was success, false if failure.</returns>
        public async Task<bool> DeleteCreditNote(int creditNoteId)
        {
            using var trans = await _handlerContext.Database.BeginTransactionAsync();
            try
            {
                var creditItems = await _handlerContext.CreditNoteItems
                    .Where(e => e.CreditNoteId == creditNoteId)
                    .Select(e => new
                    {
                        e.CreditItemId,
                        e.PurchasePrice.InvoiceId,
                        e.PurchasePrice.OwnedItemId,
                        e.CreditNote.IdUser,
                        e.Qty
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
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Delete credit note error.");
                await trans.RollbackAsync();
                return false;
            }
        }
        /// <summary>
        /// Do select query on credit note table to get user id bounded to credit note.
        /// </summary>
        /// <param name="creditNoteId">Credit note id.</param>
        /// <returns>Return id of user.</returns>
        public async Task<int> GetCreditNoteUser(int creditNoteId)
        {
            return await _handlerContext.CreditNotes.Where(e => e.IdCreditNote == creditNoteId).Select(e => e.IdUser).FirstAsync();
        }
        /// <summary>
        /// Do select query on credit note table to get credit note number.
        /// </summary>
        /// <param name="creditNoteId">Credit note id.</param>
        /// <returns>String containing credit note number.</returns>
        public async Task<string> GetCreditNumber(int creditNoteId)
        {
            return await _handlerContext.CreditNotes.Where(e => e.IdCreditNote == creditNoteId).Select(e => e.CreditNoteNumber).FirstAsync();
        }
        /// <summary>
        /// Do select query on credit note table to get credit note file path.
        /// </summary>
        /// <param name="creditNoteId">Credit note id.</param>
        /// <returns>String containing file path.</returns>
        public async Task<string?> GetCreditFilePath(int creditNoteId)
        {
            return await _handlerContext.CreditNotes.Where(e => e.IdCreditNote == creditNoteId).Select(e => e.CreditFilePath).FirstAsync();
        }
        /// <summary>
        /// Do select query to receive credit note information needed to modify it, but not passed in bulk query.
        /// </summary>
        /// <param name="creditNoteId">Credit note id.</param>
        /// <param name="isYourCredit">True if credit note was given by client to user, otherwise false.</param>
        /// <returns>Chosen credit note information wrapped in  <see cref="Models.DTOs.Get.GetRestModifyCredit"/>.</returns>
        public async Task<GetRestModifyCredit> GetRestModifyCredit(int creditNoteId, bool isYourCredit)
        {
            return await _handlerContext.CreditNotes
                .Where(e => e.IdCreditNote == creditNoteId)
                .Select(e => new GetRestModifyCredit
                {
                    CreditNumber = e.CreditNoteNumber,
                    // OrgName should always be user organization name
                    OrgName = isYourCredit ? e.Invoice.BuyerNavigation.OrgName : e.Invoice.SellerNavigation.OrgName,
                    Note = e.Note
                }).FirstAsync();
        }
        /// <summary>
        /// Do select query to receive credit note invoice id.
        /// </summary>
        /// <param name="creditNoteId">Credit note id.</param>
        /// <returns>Invoice id that's bound to credit note.</returns>
        public async Task<int> GetCreditNoteInvoiceId(int creditNoteId)
        {
            return await _handlerContext.CreditNotes
                .Where(e => e.IdCreditNote == creditNoteId)
                .Select(e => e.InvoiceId)
                .FirstAsync();
        }
        /// <summary>
        /// Overwrites chosen credit note properties given in data parameter.
        /// </summary>
        /// <param name="data">Object of <see cref="Models.DTOs.Modify.ModifyCreditNote"/>.</param>
        /// <returns>True if success, false if failure.</returns>
        public async Task<bool> ModifyCreditNote(ModifyCreditNote data)
        {
            using var trans = await _handlerContext.Database.BeginTransactionAsync();
            try
            {
                if (data.CreditNumber != null)
                {
                    await _handlerContext.CreditNotes
                        .Where(e => e.IdCreditNote == data.Id)
                        .ExecuteUpdateAsync(setter =>
                            setter.SetProperty(s => s.CreditNoteNumber, data.CreditNumber)
                        );
                }
                if (data.Note != null)
                {
                    await _handlerContext.CreditNotes
                        .Where(e => e.IdCreditNote == data.Id)
                        .ExecuteUpdateAsync(setter =>
                            setter.SetProperty(s => s.Note, data.Note)
                        );
                }
                if (data.IsPaid != null)
                {
                    await _handlerContext.CreditNotes
                        .Where(e => e.IdCreditNote == data.Id)
                        .ExecuteUpdateAsync(setter =>
                            setter.SetProperty(s => s.IsPaid, data.IsPaid)
                        );
                }
                if (data.InSystem != null)
                {
                    await _handlerContext.CreditNotes
                        .Where(e => e.IdCreditNote == data.Id)
                        .ExecuteUpdateAsync(setter =>
                            setter.SetProperty(s => s.InSystem, data.InSystem)
                        );
                }
                if (data.Date != null)
                {
                    await _handlerContext.CreditNotes
                        .Where(e => e.IdCreditNote == data.Id)
                        .ExecuteUpdateAsync(setter =>
                            setter.SetProperty(s => s.CreditNoteDate, data.Date)
                        );
                }
                if (data.Path != null)
                {
                    await _handlerContext.CreditNotes
                        .Where(e => e.IdCreditNote == data.Id)
                        .ExecuteUpdateAsync(setter =>
                            setter.SetProperty(s => s.CreditFilePath, data.Path)
                        );
                }
                await _handlerContext.SaveChangesAsync();
                await trans.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Modify credit note error.");
                await trans.RollbackAsync();
                return false;
            }

        }
    }
}
