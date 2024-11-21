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
    public interface IProformaServices
    {
        public Task<int> AddProforma(AddProforma data);
        public Task<bool> ProformaExist(string number, int sellerId, int buyerId);
        public Task<bool> ProformaExist(int proformaId);
        public Task<IEnumerable<GetProforma>> GetProformas(bool isYourProforma, string? sort, ProformaFiltersTemplate filters);
        public Task<IEnumerable<GetProforma>> GetProformas(bool isYourProforma, string search, string? sort, ProformaFiltersTemplate filters);
        public Task<IEnumerable<GetProforma>> GetProformas(bool isYourProforma, int userId, string? sort, ProformaFiltersTemplate filters);
        public Task<IEnumerable<GetProforma>> GetProformas(bool isYourProforma, int userId, string search, string? sort, ProformaFiltersTemplate filters);
        public Task<GetRestProforma> GetRestProforma(bool isYourProforma, int proformaId);
        public Task<bool> DeleteProforma(bool isYourProforma, int proformaId);
        public Task<bool> DoesDeliveryExist(int proformaId);
        public Task<int> GetProformaUserId(int proformaId);
        public Task<string?> GetProformaNumber(int proformaId);
        public Task<string?> GetProformaPath(int proformaId);
        public Task<GetRestModifyProforma> GetRestModifyProforma(int proformaId);
        public Task<bool> ModifyProforma(ModifyProforma data);
    }
    /// <summary>
    /// Class that interact with database and contains functions allowing to work on proformas.
    /// </summary>
    public class ProformaServices : IProformaServices
    {
        private readonly HandlerContext _handlerContext;
        private readonly ILogger<ProformaServices> _logger;
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="handlerContext">Database context</param>
        /// <param name="logger">Log interface</param>
        public ProformaServices(HandlerContext handlerContext, ILogger<ProformaServices> logger)
        {
            _handlerContext = handlerContext;
            _logger = logger;

        }
        /// <summary>
        /// Using transactions add new proforma to database.
        /// </summary>
        /// <param name="data">New proforma data</param>
        /// <returns>True if success or false if failure.</returns>
        public async Task<int> AddProforma(AddProforma data)
        {
            using var trans = await _handlerContext.Database.BeginTransactionAsync();
            try
            {
                var plnData = new DateTime(2024, 9, 3, 0, 0, 0, DateTimeKind.Utc);
                CurrencyValue currVal;
                if (data.CurrencyName == "PLN")
                {
                    currVal = new CurrencyValue
                    {
                        CurrencyName = data.CurrencyName,
                        UpdateDate = plnData,
                        CurrencyValue1 = 1
                    };
                }
                else
                {
                    var checkCurrency = await _handlerContext.CurrencyValues.Where(e => e.CurrencyName == data.CurrencyName && e.UpdateDate.Equals(data.CurrencyDate)).AnyAsync();
                    currVal = new CurrencyValue
                    {
                        CurrencyName = data.CurrencyName,
                        UpdateDate = data.CurrencyDate,
                        CurrencyValue1 = data.CurrencyValue
                    };
                    if (checkCurrency)
                    {
                        _handlerContext.CurrencyValues.Update(currVal);
                    }
                    else
                    {
                        _handlerContext.CurrencyValues.Add(currVal);
                    }
                }

                await _handlerContext.SaveChangesAsync();
                var newProforma = new Proforma
                {
                    ProformaNumber = data.ProformaNumber,
                    Seller = data.SellerId,
                    Buyer = data.BuyerId,
                    ProformaDate = data.Date,
                    TransportCost = data.TransportCost,
                    Note = data.Note,
                    InSystem = data.InSystem,
                    ProformaFilePath = data.Path,
                    Taxes = data.TaxId,
                    PaymentMethodId = data.PaymentId,
                    CurrencyValueDate = currVal.UpdateDate,
                    CurrencyName = data.CurrencyName,
                    UserId = data.UserId,
                };
                await _handlerContext.Proformas.AddAsync(newProforma);
                await _handlerContext.SaveChangesAsync();
                if (data.IsYourProforma)
                {
                    var futureItems = data.ProformaItems
                        .Select(e => new ProformaFutureItem
                        {
                            ProformaId = newProforma.ProformaId,
                            ItemId = e.ItemId,
                            Qty = e.Qty,
                            PurchasePrice = e.Price
                        }).ToList();
                    await _handlerContext.ProformaFutureItems.AddRangeAsync(futureItems);
                }
                else
                {
                    var ownedItems = data.ProformaItems
                        .Select(e => new ProformaOwnedItem
                        {
                            ProformaId = newProforma.ProformaId,
                            PurchasePriceId = e.ItemId,
                            Qty = e.Qty,
                            SellingPrice = e.Price
                        }).ToList();
                    await _handlerContext.ProformaOwnedItems.AddRangeAsync(ownedItems);
                }
                await _handlerContext.SaveChangesAsync();
                await trans.CommitAsync();
                return newProforma.ProformaId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Create proforma error.");
                await trans.RollbackAsync();
                return 0;
            }
        }
        /// <summary>
        /// Checks if proforma with given parameters exists.
        /// </summary>
        /// <param name="number">Proforma number</param>
        /// <param name="sellerId">Id of seller organization</param>
        /// <param name="buyerId">Id of buyer organization</param>
        /// <returns>True if exist or false if not.</returns>
        public async Task<bool> ProformaExist(string number, int sellerId, int buyerId)
        {
            return await _handlerContext.Proformas
                .Where(e => e.ProformaNumber == number && e.Seller == sellerId && e.Buyer == buyerId)
                .AnyAsync();
        }
        /// <summary>
        /// Checks if proforma with given id exists.
        /// </summary>
        /// <param name="proformaId">Proforma id</param>
        /// <returns>True if exist or false if not.</returns>
        public async Task<bool> ProformaExist(int proformaId)
        {
            return await _handlerContext.Proformas
                .Where(e => e.ProformaId == proformaId)
                .AnyAsync();
        }
        /// <summary>
        /// Do select query to receive sorted and filtered proformas information.
        /// </summary>
        /// <param name="isYourProforma">True if proforma is for future purchase invoice, otherwise false.</param>
        /// <param name="sort">Contains parameter that object will be sorted by. Must start with D or A to determine ascending order. Then is follow by name of property.</param>
        /// <param name="filters">Object with filter values wrapped in <see cref="ProformaFiltersTemplate"/>.</param>
        /// <returns>List of object containing proforma information.</returns>
        public async Task<IEnumerable<GetProforma>> GetProformas(bool isYourProforma, string? sort, ProformaFiltersTemplate filters)
        {
            var sortFunc = SortFilterUtils.GetProformaSort(sort, isYourProforma);
            bool direction;
            if (sort == null)
            {
                direction = true;
            }
            else
            {
                direction = sort.StartsWith('D');
            }
            var qtyLCond = ProformaFilters.GetQtyLowerFilter(filters.QtyL, isYourProforma);
            var qtyGCond = ProformaFilters.GetQtyGreaterFilter(filters.QtyG, isYourProforma);
            var totalLCond = ProformaFilters.GetTotalLowerFilter(filters.TotalL, isYourProforma);
            var totalGCond = ProformaFilters.GetTotalGreaterFilter(filters.TotalG, isYourProforma);
            var dateLCond = ProformaFilters.GetDateLowerFilter(filters.DateL);
            var dateGCond = ProformaFilters.GetDateGreaterFilter(filters.DateG);
            var recipientCond = ProformaFilters.GetRecipientFilter(filters.Recipient, isYourProforma);
            var currencyCond = ProformaFilters.GetCurrencyFilter(filters.Currency);

            return await _handlerContext.Proformas
                .Where(e => isYourProforma ? e.ProformaFutureItems.Any() : e.ProformaOwnedItems.Any())
                .Where(qtyLCond)
                .Where(qtyGCond)
                .Where(totalLCond)
                .Where(totalGCond)
                .Where(dateLCond)
                .Where(dateGCond)
                .Where(recipientCond)
                .Where(currencyCond)
                .OrderByWithDirection(sortFunc, direction)
                .Select(e => new GetProforma
                {
                    User = e.User.Username + " " + e.User.Surname,
                    ProformaId = e.ProformaId,
                    ProformaNumber = e.ProformaNumber,
                    Date = e.ProformaDate,
                    Transport = e.TransportCost,
                    Qty = isYourProforma ? e.ProformaFutureItems.Select(d => d.Qty).Sum() : e.ProformaOwnedItems.Select(d => d.Qty).Sum(),
                    Total = isYourProforma ? e.ProformaFutureItems.Select(d => d.PurchasePrice * d.Qty).Sum() : e.ProformaOwnedItems.Select(d => d.SellingPrice * d.Qty).Sum(),
                    CurrencyName = e.CurrencyName,
                    ClientName = isYourProforma ? e.SellerNavigation.OrgName : e.BuyerNavigation.OrgName
                }).ToListAsync();
        }
        /// <summary>
        /// Do select query to receive searched, sorted and filtered proformas information.
        /// </summary>
        /// <param name="isYourProforma">True if proforma is for future purchase invoice, otherwise false.</param>
        /// <param name="search">The phrase searched in proforma information. It will check if phrase exist in proforma number.</param>
        /// <param name="sort">Contains parameter that object will be sorted by. Must start with D or A to determine ascending order. Then is follow by name of property.</param>
        /// <param name="filters">Object with filter values wrapped in <see cref="ProformaFiltersTemplate"/>.</param>
        /// <returns>List of object containing proforma information.</returns>
        public async Task<IEnumerable<GetProforma>> GetProformas(bool isYourProforma, string search, string? sort, ProformaFiltersTemplate filters)
        {
            var sortFunc = SortFilterUtils.GetProformaSort(sort, isYourProforma);
            bool direction;
            if (sort == null)
            {
                direction = true;
            }
            else
            {
                direction = sort.StartsWith('D');
            }
            var qtyLCond = ProformaFilters.GetQtyLowerFilter(filters.QtyL, isYourProforma);
            var qtyGCond = ProformaFilters.GetQtyGreaterFilter(filters.QtyG, isYourProforma);
            var totalLCond = ProformaFilters.GetTotalLowerFilter(filters.TotalL, isYourProforma);
            var totalGCond = ProformaFilters.GetTotalGreaterFilter(filters.TotalG, isYourProforma);
            var dateLCond = ProformaFilters.GetDateLowerFilter(filters.DateL);
            var dateGCond = ProformaFilters.GetDateGreaterFilter(filters.DateG);
            var recipientCond = ProformaFilters.GetRecipientFilter(filters.Recipient, isYourProforma);
            var currencyCond = ProformaFilters.GetCurrencyFilter(filters.Currency);

            return await _handlerContext.Proformas
                .Where(e => isYourProforma ? e.ProformaFutureItems.Any() : e.ProformaOwnedItems.Any())
                .Where(e => e.ProformaNumber.ToLower().Contains(search.ToLower()))
                .Where(qtyLCond)
                .Where(qtyGCond)
                .Where(totalLCond)
                .Where(totalGCond)
                .Where(dateLCond)
                .Where(dateGCond)
                .Where(recipientCond)
                .Where(currencyCond)
                .OrderByWithDirection(sortFunc, direction)
                .Select(obj => new GetProforma
                {
                    User = obj.User.Username + " " + obj.User.Surname,
                    ProformaId = obj.ProformaId,
                    ProformaNumber = obj.ProformaNumber,
                    Date = obj.ProformaDate,
                    Transport = obj.TransportCost,
                    Qty = isYourProforma ? obj.ProformaFutureItems.Select(e => e.Qty).Sum() : obj.ProformaOwnedItems.Select(e => e.Qty).Sum(),
                    Total = isYourProforma ? obj.ProformaFutureItems.Select(e => e.PurchasePrice * e.Qty).Sum() : obj.ProformaOwnedItems.Select(e => e.SellingPrice * e.Qty).Sum(),
                    CurrencyName = obj.CurrencyName,
                    ClientName = isYourProforma ? obj.SellerNavigation.OrgName : obj.BuyerNavigation.OrgName
                }).ToListAsync();
        }
        /// <summary>
        /// Do select query to receive searched, sorted and filtered user proformas information.
        /// </summary>
        /// <param name="isYourProforma">True if proforma is for future purchase invoice, otherwise false.</param>
        /// <param name="userId">User id.</param>
        /// <param name="sort">Contains parameter that object will be sorted by. Must start with D or A to determine ascending order. Then is follow by name of property.</param>
        /// <param name="filters">Object with filter values wrapped in <see cref="ProformaFiltersTemplate"/>.</param>
        /// <returns>List of object containing proforma information.</returns>
        public async Task<IEnumerable<GetProforma>> GetProformas(bool isYourProforma, int userId, string? sort, ProformaFiltersTemplate filters)
        {
            var sortFunc = SortFilterUtils.GetProformaSort(sort, isYourProforma);
            bool direction;
            if (sort == null)
            {
                direction = true;
            }
            else
            {
                direction = sort.StartsWith('D');
            }
            var qtyLCond = ProformaFilters.GetQtyLowerFilter(filters.QtyL, isYourProforma);
            var qtyGCond = ProformaFilters.GetQtyGreaterFilter(filters.QtyG, isYourProforma);
            var totalLCond = ProformaFilters.GetTotalLowerFilter(filters.TotalL, isYourProforma);
            var totalGCond = ProformaFilters.GetTotalGreaterFilter(filters.TotalG, isYourProforma);
            var dateLCond = ProformaFilters.GetDateLowerFilter(filters.DateL);
            var dateGCond = ProformaFilters.GetDateGreaterFilter(filters.DateG);
            var recipientCond = ProformaFilters.GetRecipientFilter(filters.Recipient, isYourProforma);
            var currencyCond = ProformaFilters.GetCurrencyFilter(filters.Currency);

            return await _handlerContext.Proformas
                .Where(e => isYourProforma ? e.ProformaFutureItems.Any() : e.ProformaOwnedItems.Any())
                .Where(p => p.UserId == userId)
                .Where(qtyLCond)
                .Where(qtyGCond)
                .Where(totalLCond)
                .Where(totalGCond)
                .Where(dateLCond)
                .Where(dateGCond)
                .Where(recipientCond)
                .Where(currencyCond)
                .OrderByWithDirection(sortFunc, direction)
                .Select(ent => new GetProforma
                {
                    ProformaId = ent.ProformaId,
                    ProformaNumber = ent.ProformaNumber,
                    Date = ent.ProformaDate,
                    Transport = ent.TransportCost,
                    Qty = isYourProforma ? ent.ProformaFutureItems.Select(e => e.Qty).Sum() : ent.ProformaOwnedItems.Select(e => e.Qty).Sum(),
                    Total = isYourProforma ? ent.ProformaFutureItems.Select(e => e.PurchasePrice * e.Qty).Sum() : ent.ProformaOwnedItems.Select(e => e.SellingPrice * e.Qty).Sum(),
                    CurrencyName = ent.CurrencyName,
                    ClientName = isYourProforma ? ent.SellerNavigation.OrgName : ent.BuyerNavigation.OrgName
                }).ToListAsync();
        }
        /// <summary>
        /// Do select query to receive searched, sorted and filtered user proformas information.
        /// </summary>
        /// <param name="isYourProforma">True if proforma is for future purchase invoice, otherwise false.</param>
        /// <param name="userId">User id.</param>
        /// <param name="search">The phrase searched in proforma information. It will check if phrase exist in proforma number.</param>
        /// <param name="sort">Contains parameter that object will be sorted by. Must start with D or A to determine ascending order. Then is follow by name of property.</param>
        /// <param name="filters">Object with filter values wrapped in <see cref="ProformaFiltersTemplate"/>.</param>
        /// <returns>List of object containing proforma information.</returns>
        public async Task<IEnumerable<GetProforma>> GetProformas(bool isYourProforma, int userId, string search, string? sort, ProformaFiltersTemplate filters)
        {
            var sortFunc = SortFilterUtils.GetProformaSort(sort, isYourProforma);
            bool direction;
            if (sort == null)
            {
                direction = true;
            }
            else
            {
                direction = sort.StartsWith('D');
            }
            var qtyLCond = ProformaFilters.GetQtyLowerFilter(filters.QtyL, isYourProforma);
            var qtyGCond = ProformaFilters.GetQtyGreaterFilter(filters.QtyG, isYourProforma);
            var totalLCond = ProformaFilters.GetTotalLowerFilter(filters.TotalL, isYourProforma);
            var totalGCond = ProformaFilters.GetTotalGreaterFilter(filters.TotalG, isYourProforma);
            var dateLCond = ProformaFilters.GetDateLowerFilter(filters.DateL);
            var dateGCond = ProformaFilters.GetDateGreaterFilter(filters.DateG);
            var recipientCond = ProformaFilters.GetRecipientFilter(filters.Recipient, isYourProforma);
            var currencyCond = ProformaFilters.GetCurrencyFilter(filters.Currency);

            return await _handlerContext.Proformas
                .Where(e => isYourProforma ? e.ProformaFutureItems.Any() : e.ProformaOwnedItems.Any())
                .Where(p => p.UserId == userId)
                .Where(e => e.ProformaNumber.ToLower().Contains(search.ToLower()))
                .Where(qtyLCond)
                .Where(qtyGCond)
                .Where(totalLCond)
                .Where(totalGCond)
                .Where(dateLCond)
                .Where(dateGCond)
                .Where(recipientCond)
                .Where(currencyCond)
                .OrderByWithDirection(sortFunc, direction)
                .Select(prof => new GetProforma
                {
                    ProformaId = prof.ProformaId,
                    ProformaNumber = prof.ProformaNumber,
                    Date = prof.ProformaDate,
                    Transport = prof.TransportCost,
                    Qty = isYourProforma ? prof.ProformaFutureItems.Select(e => e.Qty).Sum() : prof.ProformaOwnedItems.Select(e => e.Qty).Sum(),
                    Total = isYourProforma ? prof.ProformaFutureItems.Select(e => e.PurchasePrice * e.Qty).Sum() : prof.ProformaOwnedItems.Select(e => e.SellingPrice * e.Qty).Sum(),
                    CurrencyName = prof.CurrencyName,
                    ClientName = isYourProforma ? prof.SellerNavigation.OrgName : prof.BuyerNavigation.OrgName
                }).ToListAsync();
        }
        /// <summary>
        /// Do select query using given id to receive proforma information that was not given in bulk query.
        /// </summary>
        /// <param name="isYourProforma">True if proforma is for future purchase invoice, otherwise false.</param>
        /// <param name="proformaId">Proforma id</param>
        /// <returns>Object containing proforma information.</returns>
        public async Task<GetRestProforma> GetRestProforma(bool isYourProforma, int proformaId)
        {
            return await _handlerContext.Proformas
                .Where(e => e.ProformaId == proformaId)
                .Select(e => new GetRestProforma
                {
                    Taxes = e.TaxesNavigation.TaxValue,
                    CurrencyValue = e.CurrencyName == "PLN" ? 1 : e.Currency.CurrencyValue1,
                    CurrencyDate = e.CurrencyValueDate,
                    PaymentMethod = e.PaymentMethod.MethodName,
                    InSystem = e.InSystem,
                    Note = e.Note,
                    Path = e.ProformaFilePath ?? "",
                    Items = isYourProforma ?
                        e.ProformaFutureItems.Select(d => new GetCredtItemForTable
                        {
                            CreditItemId = d.ProformaFutureItemId,
                            Partnumber = d.Item.PartNumber,
                            ItemName = d.Item.ItemName,
                            Qty = d.Qty,
                            Price = d.PurchasePrice
                        }).ToList()
                        :
                        e.ProformaOwnedItems.Select(d => new GetCredtItemForTable
                        {
                            CreditItemId = d.ProformaOwnedItemId,
                            Partnumber = d.Item.OwnedItem.OriginalItem.PartNumber,
                            ItemName = d.Item.OwnedItem.OriginalItem.ItemName,
                            Qty = d.Qty,
                            Price = d.SellingPrice
                        }).ToList()
                })
                .FirstAsync();
        }
        /// <summary>
        /// Using transactions delete proforma from database.
        /// </summary>
        /// <param name="isYourProforma">True if proforma is for future purchase invoice, otherwise false.</param>
        /// <param name="proformaId">Proforma id.</param>
        /// <returns>True fi success or false if not.</returns>
        public async Task<bool> DeleteProforma(bool isYourProforma, int proformaId)
        {
            using var trans = await _handlerContext.Database.BeginTransactionAsync();
            try
            {
                if (isYourProforma)
                {
                    await _handlerContext.ProformaFutureItems.Where(e => e.ProformaId == proformaId).ExecuteDeleteAsync();
                }
                else
                {
                    await _handlerContext.ProformaOwnedItems.Where(e => e.ProformaId == proformaId).ExecuteDeleteAsync();
                }
                await _handlerContext.Proformas.Where(e => e.ProformaId == proformaId).ExecuteDeleteAsync();
                await _handlerContext.SaveChangesAsync();
                await trans.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Delete proforma error.");
                await trans.RollbackAsync();
                return false;
            }
        }
        /// <summary>
        /// Checks if delivery with given proforma id exist in database.
        /// </summary>
        /// <param name="proformaId">Proforma id</param>
        /// <returns>True if exist or false if not.</returns>
        public async Task<bool> DoesDeliveryExist(int proformaId)
        {
            return await _handlerContext.Proformas.AnyAsync(e => e.ProformaId == proformaId && e.Deliveries.Any());
        }
        /// <summary>
        /// Do select query to receive id of user that owns that proforma.
        /// </summary>
        /// <param name="proformaId">Proforma id</param>
        /// <returns>Id of user or 0 if do not exist.</returns>
        public async Task<int> GetProformaUserId(int proformaId)
        {
            return await _handlerContext.Proformas
                .Where(e => e.ProformaId == proformaId)
                .Select(e => e.UserId).FirstOrDefaultAsync();
        }
        /// <summary>
        /// Do select query to receive given proforma number.
        /// </summary>
        /// <param name="proformaId">Proforma id</param>
        /// <returns>Proforma number or null if do not exist.</returns>
        public async Task<string?> GetProformaNumber(int proformaId)
        {
            return await _handlerContext.Proformas
                .Where(e => e.ProformaId == proformaId)
                .Select(e => e.ProformaNumber)
                .FirstOrDefaultAsync();
        }
        /// <summary>
        /// Do select query to receive given proforma file path.
        /// </summary>
        /// <param name="proformaId">Proforma id</param>
        /// <returns>Chosen proforma file path or null if do not exist.</returns>
        public async Task<string?> GetProformaPath(int proformaId)
        {
            return await _handlerContext.Proformas
                .Where(e => e.ProformaId == proformaId)
                .Select(e => e.ProformaFilePath)
                .FirstOrDefaultAsync();
        }
        /// <summary>
        /// Do select query using given id to receive proforma information that was not given in bulk query.
        /// </summary>
        /// <param name="proformaId">Proforma id</param>
        /// <returns>Object containing proforma information.</returns>
        public async Task<GetRestModifyProforma> GetRestModifyProforma(int proformaId)
        {
            return await _handlerContext.Proformas
                .Where(e => e.ProformaId == proformaId)
                .Select(e => new GetRestModifyProforma
                {
                    UserId = e.UserId,
                    PaymentMethod = e.PaymentMethod.MethodName,
                    InSystem = e.InSystem,
                    Note = e.Note
                }).FirstAsync();
        }
        /// <summary>
        /// Using transactions overwrite old proforma properties with given new values.
        /// </summary>
        /// <param name="data">New proforma properties values.</param>
        /// <returns>True if success or false if failure.</returns>
        public async Task<bool> ModifyProforma(ModifyProforma data)
        {
            using var trans = await _handlerContext.Database.BeginTransactionAsync();
            try
            {
                if (data.UserId != null)
                {
                    await _handlerContext.Proformas
                        .Where(e => e.ProformaId == data.ProformaId)
                        .ExecuteUpdateAsync(setter =>
                            setter.SetProperty(s => s.UserId, data.UserId)
                        );
                }
                if (data.ProformaNumber != null)
                {
                    await _handlerContext.Proformas
                        .Where(e => e.ProformaId == data.ProformaId)
                        .ExecuteUpdateAsync(setter =>
                            setter.SetProperty(s => s.ProformaNumber, data.ProformaNumber)
                        );
                }
                if (data.ClientId != null)
                {
                    await _handlerContext.Proformas
                        .Where(e => e.ProformaId == data.ProformaId)
                        .ExecuteUpdateAsync(setter =>
                            setter.SetProperty(s => data.IsYourProforma ? s.Seller : s.Buyer, data.ClientId)
                        );
                }
                if (data.Transport != null)
                {
                    await _handlerContext.Proformas
                        .Where(e => e.ProformaId == data.ProformaId)
                        .ExecuteUpdateAsync(setter =>
                            setter.SetProperty(s => s.TransportCost, data.Transport)
                        );
                }
                if (data.PaymentMethodId != null)
                {
                    await _handlerContext.Proformas
                        .Where(e => e.ProformaId == data.ProformaId)
                        .ExecuteUpdateAsync(setter =>
                            setter.SetProperty(s => s.PaymentMethodId, data.PaymentMethodId)
                        );
                }
                if (data.InSystem != null)
                {
                    await _handlerContext.Proformas
                        .Where(e => e.ProformaId == data.ProformaId)
                        .ExecuteUpdateAsync(setter =>
                            setter.SetProperty(s => s.InSystem, data.InSystem)
                        );
                }
                if (data.Path != null)
                {
                    await _handlerContext.Proformas
                        .Where(e => e.ProformaId == data.ProformaId)
                        .ExecuteUpdateAsync(setter =>
                            setter.SetProperty(s => s.ProformaFilePath, data.Path)
                        );
                }
                if (data.Note != null)
                {
                    await _handlerContext.Proformas
                        .Where(e => e.ProformaId == data.ProformaId)
                        .ExecuteUpdateAsync(setter =>
                            setter.SetProperty(s => s.Note, data.Note)
                        );
                }
                await _handlerContext.SaveChangesAsync();
                await trans.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Modify proforma error.");
                await trans.RollbackAsync();
                return false;
            }
        }
    }
}
