using database_communicator.Data;
using database_communicator.Models;
using database_communicator.Utils;
using database_communicator.FilterClass;
using database_communicator.Models.DTOs.Create;
using database_communicator.Models.DTOs.Get;
using database_communicator.Models.DTOs.Modify;
using LINQtoCSV;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Xml.Linq;

namespace database_communicator.Services
{
    public interface IOfferServices
    {
        public Task<IEnumerable<GetPriceList>> GetPriceLists(int userId, string? sort, OfferFiltersTemplate filters);
        public Task<IEnumerable<GetPriceList>> GetPriceLists(int userId, string search, string? sort, OfferFiltersTemplate filters);
        public Task<bool> DoesPricelistExist(int offerId);
        public Task<bool> DoesPricelistExist(string offerName, int userId);
        public Task<bool> DeletePricelist(int offerId);
        public Task<int> CreatePricelist(AddOffer data);
        public Task<IEnumerable<GetOfferStatus>> GetPricelistStatuses();
        public Task<IEnumerable<GetItemsForOffer>> GetItemsForPricelist(int userId, string currency);
        public Task<bool> CreateCsvFile(int offerId, int userId, string path, int maxQty);
        public Task<bool> UpdateCsvFile(int offerId);
        public Task<bool> CreateXmlFile(int offerId, int userId, string path, int maxQty);
        public Task<bool> UpdateXmlFile(int offerId);
        public Task<int> GetDeactivatedStatusId();
        public Task<IEnumerable<GetCredtItemForTable>> GetPricelistItems(int offerId, int userId);
        public Task<GetRestModifyOffer> GetRestModifyPricelist(int offerId, int userId);
        public Task<bool> ModifyPricelist(ModifyPricelist data);
        public Task<string> GetPricelistPath(int offerId);
        public Task<int> GetOfferMaxQty(int offerId);
        public Task<IEnumerable<int>> GetAllActiveXmlOfferId();
        public Task<IEnumerable<int>> GetAllActiveCsvOfferId();
    }
    /// <summary>
    /// Class that interact with database and contains functions allowing to work on offers/pricelist.
    /// </summary>
    public class OfferServices : IOfferServices
    {
        private readonly HandlerContext _handlerContext;
        private readonly ILogger<CreditNoteServices> _logger;
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="handlerContext">Database context</param>
        /// <param name="logger">Log interface</param>
        public OfferServices(HandlerContext handlerContext, ILogger<CreditNoteServices> logger)
        {
            _handlerContext = handlerContext;
            _logger = logger;

        }
        /// <summary>
        /// Do select query with given sort and filter to receive pricelists/offers information from database for chosen user.
        /// </summary>
        /// <param name="userId">User id.</param>
        /// <param name="sort">Contains parameter that object will be sorted by. Must start with D or A to determine ascending order. Then is follow by name of property.</param>
        /// <param name="filters">Object with filter values wrapped in <see cref="OfferFiltersTemplate"/></param>
        /// <returns>List of pricelists/offers owned by chosen user.</returns>
        public async Task<IEnumerable<GetPriceList>> GetPriceLists(int userId, string? sort, OfferFiltersTemplate filters)
        {
            var sortFunc = SortFilterUtils.GetPricelistSort(sort);
            bool direction;
            if (sort == null)
            {
                direction = true;
            }
            else
            {
                direction = sort.StartsWith('D');
            }
            var statusCond = OfferFilters.GetStatusFilter(filters.Status);
            var currencyCond = OfferFilters.GetCurrencyFilter(filters.Currency);
            var typeCond = OfferFilters.GetTypeFilter(filters.Type);
            var totalLCond = OfferFilters.GetTotalLowerFilter(filters.TotalL);
            var totalGCond = OfferFilters.GetTotalGreaterFilter(filters.TotalG);
            var createdLCond = OfferFilters.GetCreatedLowerFilter(filters.CreatedL);
            var createdGCond = OfferFilters.GetCreatedGreaterFilter(filters.CreatedG);
            var modifiedLCond = OfferFilters.GetModifiedLowerFilter(filters.ModifiedL);
            var modifiedGCond = OfferFilters.GetModifiedGreaterFilter(filters.ModifiedG);

            return await _handlerContext.Offers
                .Where(e => e.UserId == userId)
                .Where(statusCond)
                .Where(typeCond)
                .Where(currencyCond)
                .Where(totalLCond)
                .Where(totalGCond)
                .Where(createdLCond)
                .Where(createdGCond)
                .Where(modifiedLCond)
                .Where(modifiedGCond)
                .OrderByWithDirection(sortFunc, direction)
                .Select(obj => new GetPriceList
                {
                    PricelistId = obj.OfferId,
                    Created = obj.CreationDate,
                    Status = obj.OfferStatus.StatusName,
                    Name = obj.OfferName,
                    TotalItems = obj.OfferItems.Count,
                    Path = obj.PathToFile,
                    Modified = obj.ModificationDate,
                    Currency = obj.CurrencyName
                }).ToListAsync();
        }
        /// <summary>
        /// Do select query with given search, sort and filter to receive pricelists/offers information from database for chosen user.
        /// </summary>
        /// <param name="userId">User id.</param>
        /// <param name="search">The phrase searched in pricelist information. It will check if phrase exist in pricelist name.</param>
        /// <param name="sort">Contains parameter that object will be sorted by. Must start with D or A to determine ascending order. Then is follow by name of property.</param>
        /// <param name="filters">Object with filter values wrapped in <see cref="OfferFiltersTemplate"/></param>
        /// <returns>List of pricelists/offers owned by chosen user.</returns>
        public async Task<IEnumerable<GetPriceList>> GetPriceLists(int userId, string search, string? sort, OfferFiltersTemplate filters)
        {
            var sortFunc = SortFilterUtils.GetPricelistSort(sort);
            bool direction;
            if (sort == null)
            {
                direction = true;
            }
            else
            {
                direction = sort.StartsWith('D');
            }

            var statusCond = OfferFilters.GetStatusFilter(filters.Status);
            var currencyCond = OfferFilters.GetCurrencyFilter(filters.Currency);
            var typeCond = OfferFilters.GetTypeFilter(filters.Type);
            var totalLCond = OfferFilters.GetTotalLowerFilter(filters.TotalL);
            var totalGCond = OfferFilters.GetTotalGreaterFilter(filters.TotalG);
            var createdLCond = OfferFilters.GetCreatedLowerFilter(filters.CreatedL);
            var createdGCond = OfferFilters.GetCreatedGreaterFilter(filters.CreatedG);
            var modifiedLCond = OfferFilters.GetModifiedLowerFilter(filters.ModifiedL);
            var modifiedGCond = OfferFilters.GetModifiedGreaterFilter(filters.ModifiedG);

            return await _handlerContext.Offers
                .Where(e => e.UserId == userId)
                .Where(e => e.OfferName.ToLower().Contains(search.ToLower()))
                .Where(statusCond)
                .Where(typeCond)
                .Where(currencyCond)
                .Where(totalLCond)
                .Where(totalGCond)
                .Where(createdLCond)
                .Where(createdGCond)
                .Where(modifiedLCond)
                .Where(modifiedGCond)
                .OrderByWithDirection(sortFunc, direction)
                .Select(ent => new GetPriceList
                {
                    PricelistId = ent.OfferId,
                    Created = ent.CreationDate,
                    Status = ent.OfferStatus.StatusName,
                    Name = ent.OfferName,
                    TotalItems = ent.OfferItems.Count,
                    Path = ent.PathToFile,
                    Modified = ent.ModificationDate,
                    Currency = ent.CurrencyName
                }).ToListAsync();
        }
        /// <summary>
        /// Checks if pricelist/offer id exists.
        /// </summary>
        /// <param name="offerId">Offer/Pricelist id</param>
        /// <returns>True if exist, false if not.</returns>
        public async Task<bool> DoesPricelistExist(int offerId)
        {
            return await _handlerContext.Offers.AnyAsync(x => x.OfferId == offerId);
        }
        /// <summary>
        /// Checks if pricelist/offer with given name and user exists.
        /// </summary>
        /// <param name="offerName">Offer name</param>
        /// <param name="userId">Id of user that owns offer/pricelist</param>
        /// <returns>True if exist, false if not.</returns>
        public async Task<bool> DoesPricelistExist(string offerName, int userId)
        {
            return await _handlerContext.Offers.AnyAsync(x => x.OfferName == offerName && x.UserId == userId);
        }
        /// <summary>
        /// Using transactions delete pricelist/offer from database.
        /// </summary>
        /// <param name="offerId">Id of offer/pricelist to delete</param>
        /// <returns>True if success or false if failure</returns>
        public async Task<bool> DeletePricelist(int offerId)
        {
            using var trans = await _handlerContext.Database.BeginTransactionAsync();
            try
            {
                await _handlerContext.OfferItems
                    .Where(e => e.OfferId == offerId)
                    .ExecuteDeleteAsync();
                await _handlerContext.Offers
                    .Where(e => e.OfferId == offerId)
                    .ExecuteDeleteAsync();
                await _handlerContext.SaveChangesAsync();
                await trans.CommitAsync();
                return true;
            } catch (Exception ex)
            {
                _logger.LogError(ex, "Delete pricelist error.");
                await trans.RollbackAsync();
                return false;
            }
        }
        /// <summary>
        /// Using transactions add new offer/pricelist to database.
        /// </summary>
        /// <param name="data">New pricelist/offer data</param>
        /// <returns>True if success or false if failure</returns>
        public async Task<int> CreatePricelist(AddOffer data)
        {
            using var trans = await _handlerContext.Database.BeginTransactionAsync();
            try
            {
                var newOffer = new Offer
                {
                    OfferName = data.OfferName,
                    CreationDate = DateTime.Now,
                    ModificationDate = DateTime.Now,
                    PathToFile = data.Path,
                    OfferStatusId = data.OfferStatusId,
                    MaxQty = data.MaxQty,
                    CurrencyName = data.Currency,
                    UserId = data.UserId,
                };
                await _handlerContext.Offers.AddAsync(newOffer);
                await _handlerContext.SaveChangesAsync();
                var newItems = data.Items.Select(e => new OfferItem
                {
                    OfferId = newOffer.OfferId,
                    ItemId = e.ItemId,
                    SellingPrice = e.Price
                }).ToList();
                await _handlerContext.OfferItems.AddRangeAsync(newItems);
                await _handlerContext.SaveChangesAsync();
                await trans.CommitAsync();
                return newOffer.OfferId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Create pricelist error.");
                await trans.RollbackAsync();
                return 0;
            }
        }
        /// <summary>
        /// Do select query to receive list of pricelist statuses from database.
        /// </summary>
        /// <returns>List of statuses containing id and status name.</returns>
        public async Task<IEnumerable<GetOfferStatus>> GetPricelistStatuses()
        {
            return await _handlerContext.OfferStatuses
                .Select(e => new GetOfferStatus
                {
                    StatusId = e.OfferStatusId,
                    StatusName = e.StatusName
                }).ToListAsync();
        }
        /// <summary>
        /// Do select query to receive list of items available to add to pricelist/offer.
        /// </summary>
        /// <param name="userId">Id of user that is owner of pricelist/offer</param>
        /// <param name="currency">Shortcut name of currency that price of item will be shown</param>
        /// <returns>List of items in given currency that's available to add to pricelist/offer.</returns>
        public async Task<IEnumerable<GetItemsForOffer>> GetItemsForPricelist(int userId, string currency)
        {
            if (currency == "PLN")
            {
                return await _handlerContext.Items
                .Where(e => e.OwnedItems.SelectMany(d => d.ItemOwners).Any(x => x.IdUser == userId && x.Qty > 0))
                .Select(e => new GetItemsForOffer
                {
                    ItemId = e.ItemId,
                    Partnumber = e.PartNumber,
                    Qty = e.OwnedItems.SelectMany(d => d.ItemOwners).Where(d => d.IdUser == userId).Sum(x => x.Qty),
                    PurchasePrice = e.OwnedItems.SelectMany(d => d.PurchasePrices)
                        .Where(d =>
                            d.Qty
                            - d.SellingPrices.Select(x => x.Qty).Sum()
                            + d.CreditNoteItems.Select(x => x.Qty).Sum()
                            > 0
                        ).Average(x => x.Price)
                }).ToListAsync();
            }
            return await _handlerContext.Items
                .Where(e => e.OwnedItems.SelectMany(d => d.ItemOwners).Any(x => x.IdUser == userId && x.Qty > 0))
                .Select(e => new GetItemsForOffer
                {
                    ItemId = e.ItemId,
                    Partnumber = e.PartNumber,
                    Qty = e.OwnedItems.SelectMany(d => d.ItemOwners).Where(d => d.IdUser == userId).Sum(x => x.Qty),
                    PurchasePrice = e.OwnedItems.SelectMany(d => d.PurchasePrices)
                        .Where(d =>
                            d.Qty
                            - d.SellingPrices.Select(x => x.Qty).Sum()
                            + d.CreditNoteItems.Select(x => x.Qty).Sum()
                            > 0
                        )
                        .SelectMany(d => d.CalculatedPrices)
                        .Where(d => d.CurrencyName == currency)
                        .Average(x => x.Price)
                }).ToListAsync();
        }
        /// <summary>
        /// Do select query on chosen pricelist/offer items and put them into csv file.
        /// </summary>
        /// <param name="offerId">Offer/pricelist id</param>
        /// <param name="userId">Id of user that owns this offer/pricelist</param>
        /// <param name="path">Path where csv file will be created</param>
        /// <param name="maxQty">Max shown quantity in pricelist/offer</param>
        /// <returns>True if file was created, false if not.</returns>
        public async Task<bool> CreateCsvFile(int offerId, int userId, string path, int maxQty)
        {
            var items = await _handlerContext.Items
                .Where(e => e.OfferItems.Any(x => x.OfferId == offerId))
                .Select(e => new
                {
                    e.PartNumber,
                    e.ItemName,
                    e.ItemDescription,
                    Eans = e.Eans.Select(d => d.EanValue).ToArray(),
                    Qty = e.OwnedItems.SelectMany(d => d.ItemOwners).Where(d => d.IdUser == userId).Sum(x => x.Qty),
                    Price = e.OfferItems.Where(d => d.OfferId == offerId && d.ItemId == e.ItemId).Select(d => d.SellingPrice).First()
                }).ToListAsync();
            var csvItems = items.Select(e => new CreateCsvPricelist
            {
                Partnumber = e.PartNumber,
                ItemName = e.ItemName,
                ItemDescription = e.ItemDescription,
                Eans = String.Join(", ", e.Eans),
                Qty = e.Qty > maxQty ? $"> {maxQty}" : $"{e.Qty}",
                Price = e.Price
            }).ToList();
            CsvFileDescription csvFileDescription = new()
            {
                SeparatorChar = ',',
                FirstLineHasColumnNames = true,
                FileCultureName = "en-US",
            };
            CsvContext cc = new();
            string newPath = $"../web/handler_b2b/{path}";
            try
            {
                cc.Write(csvItems, newPath, csvFileDescription);
                return true;
            } catch (Exception ex)
            {
                _logger.LogError(ex, "Create pricelist csv file error.");
                return false;
            }
        }
        /// <summary>
        /// Update csv file for given offer/pricelist id.
        /// </summary>
        /// <param name="offerId">Offer/pricelist id</param>
        /// <returns>True if success or false if failure</returns>
        public async Task<bool> UpdateCsvFile(int offerId)
        {
            var restOfferData = await _handlerContext.Offers.Where(e => e.OfferId == offerId).Select(e => new { e.UserId, e.PathToFile, e.MaxQty }).FirstAsync();
            return await CreateCsvFile(offerId, restOfferData.UserId, restOfferData.PathToFile, restOfferData.MaxQty);
        }
        /// <summary>
        /// Do select query on chosen pricelist/offer items and put them into xml file.
        /// </summary>
        /// <param name="offerId">Offer/pricelist id</param>
        /// <param name="userId">Id of user that owns this offer/pricelist</param>
        /// <param name="path">Path where csv file will be created</param>
        /// <param name="maxQty">Max shown quantity in pricelist/offer</param>
        /// <returns>True if file was created, false if not.</returns>
        public async Task<bool> CreateXmlFile(int offerId, int userId, string path, int maxQty)
        {
            var items = await _handlerContext.Items
                .Where(e => e.OfferItems.Any(x => x.OfferId == offerId))
                .Select(e => new
                {
                    e.PartNumber,
                    e.ItemName,
                    e.ItemDescription,
                    Eans = e.Eans.Select(d => d.EanValue).ToArray(),
                    Qty = e.OwnedItems.SelectMany(d => d.ItemOwners).Where(d => d.IdUser == userId).Sum(x => x.Qty),
                    Price = e.OfferItems.Where(d => d.OfferId == offerId && d.ItemId == e.ItemId).Select(d => d.SellingPrice).First()
                }).ToListAsync();
            var xmlItems = items.Select(e => 
                    new XElement("Product",
                        new XElement("Partnumber", e.PartNumber),
                        new XElement("ItemName", e.ItemName),
                        new XElement("ItemDescription", e.ItemDescription),
                        new XElement("Eans", String.Join(", ", e.Eans)),
                        new XElement("Qty", e.Qty > maxQty ? $"> {maxQty}" : $"{e.Qty}"),
                        new XElement("Price", e.Price)
                    )
                ).ToList();
            var xmlFile = new XElement("Products", xmlItems);
            string newPath = $"../web/handler_b2b/{path}";
            try
            {
                xmlFile.Save( newPath );
                return true;
            } catch (Exception ex)
            {
                _logger.LogError(ex, "Create pricelist xml file error.");
                return false;
            }
        }
        /// <summary>
        /// Update csv file for given offer/pricelist id.
        /// </summary>
        /// <param name="offerId">Offer/pricelist id</param>
        /// <returns>True if success or false if failure</returns>
        public async Task<bool> UpdateXmlFile(int offerId)
        {
            var restOfferData = await _handlerContext.Offers.Where(d => d.OfferId == offerId).Select(d => new { d.UserId, d.PathToFile, d.MaxQty }).FirstAsync();
            return await CreateXmlFile(offerId, restOfferData.UserId, restOfferData.PathToFile, restOfferData.MaxQty);
        }
        /// <summary>
        /// Do select query to receive status id for Deactivated value.
        /// </summary>
        /// <returns>Id of deactivated status</returns>
        public async Task<int> GetDeactivatedStatusId()
        {
            return await _handlerContext.OfferStatuses.Where(e => e.StatusName == "Deactivated").Select(e => e.OfferStatusId).FirstAsync();
        }
        /// <summary>
        /// Do select query to receive list of items that chosen pricelist contain.
        /// </summary>
        /// <param name="offerId">Offer/pricelist id</param>
        /// <param name="userId">Id of user that owns pricelist</param>
        /// <returns>List of items that belongs ot the chosen pricelist.</returns>
        public async Task<IEnumerable<GetCredtItemForTable>> GetPricelistItems(int offerId, int userId)
        {
            return await _handlerContext.OfferItems
                .Where(e => e.OfferId == offerId)
                .Select(e => new GetCredtItemForTable
                {
                    CreditItemId = e.ItemId,
                    Partnumber = e.Item.PartNumber,
                    ItemName = e.Item.ItemName,
                    Qty = e.Item.OwnedItems.SelectMany(d => d.ItemOwners).Where(d => d.IdUser == userId).Sum(x => x.Qty),
                    Price = e.SellingPrice
                }).ToListAsync();
        }
        /// <summary>
        /// Do select query using given ids to receive pricelist/offer information that was not given in bulk query and is needed to modify it.
        /// </summary>
        /// <param name="offerId">Offer/pricelist id</param>
        /// <param name="userId">Id of user that owns pricelist/offer</param>
        /// <returns>Object containing information about pricelist/offer</returns>
        public async Task<GetRestModifyOffer> GetRestModifyPricelist(int offerId, int userId)
        {
            return await _handlerContext.Offers
                .Where(e => e.OfferId == offerId)
                .Select(e => new GetRestModifyOffer
                {
                    MaxQty = e.MaxQty,
                    Items = e.OfferItems.Select(d => new GetOfferItemForModify
                    {
                        Id = d.ItemId,
                        Partnumber = d.Item.PartNumber,
                        Qty = d.Item.OwnedItems.SelectMany(d => d.ItemOwners).Where(d => d.IdUser == userId).Sum(x => x.Qty),
                        Price = d.SellingPrice,
                        PurchasePrice = e.CurrencyName == "PLN" ?
                        d.Item.OwnedItems.SelectMany(x => x.PurchasePrices)
                            .Where(x =>
                                x.Qty
                                - x.SellingPrices.Select(y => y.Qty).Sum()
                                + x.CreditNoteItems.Select(y => y.Qty).Sum()
                                > 0
                            )
                            .Average(x => x.Price)
                        :
                        d.Item.OwnedItems.SelectMany(x => x.PurchasePrices)
                            .Where(x =>
                                x.Qty
                                - x.SellingPrices.Select(y => y.Qty).Sum()
                                + x.CreditNoteItems.Select(y => y.Qty).Sum()
                                > 0
                            )
                            .SelectMany(x => x.CalculatedPrices)
                            .Where(x => x.CurrencyName == e.CurrencyName)
                            .Average(x => x.Price)
                    }).ToList()
                }).FirstAsync();
        }
        /// <summary>
        /// Using transactions overwrites given properties with new values.
        /// </summary>
        /// <param name="data">New pricelist/offer properties values</param>
        /// <returns>True if success or false if failure.</returns>
        public async Task<bool> ModifyPricelist(ModifyPricelist data)
        {
            using var trans = await _handlerContext.Database.BeginTransactionAsync();
            try
            {
                await _handlerContext.OfferItems
                    .Where(e => e.OfferId == data.OfferId && !data.Items.Select(e => e.ItemId).Contains(e.ItemId))
                    .ExecuteDeleteAsync();
                foreach (var item in data.Items)
                {
                    var exist = await _handlerContext.OfferItems.AnyAsync(x => x.OfferId == data.OfferId && x.ItemId == item.ItemId);
                    if (exist)
                    {
                        await _handlerContext.OfferItems.Where(x => x.OfferId == data.OfferId && x.ItemId == item.ItemId)
                            .ExecuteUpdateAsync(setter =>
                                setter.SetProperty(s => s.SellingPrice, item.Price)
                            );
                    } else
                    {
                        await _handlerContext.OfferItems.AddAsync(new OfferItem
                        {
                            OfferId = data.OfferId,
                            ItemId = item.ItemId,
                            SellingPrice = item.Price
                        });
                    }
                }
                if (data.OfferName != null)
                {
                    await _handlerContext.Offers.Where(e => e.OfferId == data.OfferId)
                        .ExecuteUpdateAsync(setter => 
                            setter.SetProperty(s => s.OfferName, data.OfferName)
                        );
                }
                if (data.OfferStatusId != null)
                {
                    await _handlerContext.Offers.Where(e => e.OfferId == data.OfferId)
                        .ExecuteUpdateAsync(setter =>
                            setter.SetProperty(s => s.OfferStatusId, data.OfferStatusId)
                        );
                }
                if (data.MaxQty != null)
                {
                    await _handlerContext.Offers.Where(e => e.OfferId == data.OfferId)
                        .ExecuteUpdateAsync(setter =>
                            setter.SetProperty(s => s.MaxQty, data.MaxQty)
                        );
                }
                if (data.CurrencyName != null)
                {
                    await _handlerContext.Offers.Where(e => e.OfferId == data.OfferId)
                        .ExecuteUpdateAsync(setter =>
                            setter.SetProperty(s => s.CurrencyName, data.CurrencyName)
                        );
                }
                if (data.Path != null)
                {
                    await _handlerContext.Offers.Where(e => e.OfferId == data.OfferId)
                        .ExecuteUpdateAsync(setter =>
                            setter.SetProperty(s => s.PathToFile, data.Path)
                        );
                }
                await _handlerContext.Offers.Where(e => e.OfferId == data.OfferId)
                        .ExecuteUpdateAsync(setter =>
                            setter.SetProperty(s => s.ModificationDate, DateTime.Now)
                        );
                await _handlerContext.SaveChangesAsync();
                await trans.CommitAsync();
                return true;
            } catch (Exception ex)
            {
                _logger.LogError(ex, "Modify pricelist error.");
                await trans.RollbackAsync();
                return false;
            }
        }
        /// <summary>
        /// Do select query to receive pricelist file path from database.
        /// </summary>
        /// <param name="offerId">Offer/Pricelist id</param>
        /// <returns>String containing file path.</returns>
        public async Task<string> GetPricelistPath(int offerId)
        {
            return await _handlerContext.Offers.Where(e => e.OfferId == offerId).Select(e => e.PathToFile).FirstAsync();
        }
        /// <summary>
        /// Do select query to receive given offer/pricelist max showed quantity.
        /// </summary>
        /// <param name="offerId">Pricelist/offer id</param>
        /// <returns>Number that indicates max quantity shown in pricelist/offer.</returns>
        public async Task<int> GetOfferMaxQty(int offerId)
        {
            return await _handlerContext.Offers.Where(e => e.OfferId == offerId).Select(e => e.MaxQty).FirstAsync();
        }
        /// <summary>
        /// Do select query to receive list of current active pricelist/offer with created xml file.
        /// </summary>
        /// <returns>List of pricelist/offer ids that have created xml filer</returns>
        public async Task<IEnumerable<int>> GetAllActiveXmlOfferId()
        {
            return await _handlerContext.Offers.Where(e => !e.PathToFile.EndsWith("csv") && e.OfferStatus.StatusName == "Active").Select(e => e.OfferId).ToListAsync();
        }
        /// <summary>
        /// Do select query to receive list of current active pricelist/offer with created csv file.
        /// </summary>
        /// <returns>List of pricelist/offer ids that have created csv filer</returns>
        public async Task<IEnumerable<int>> GetAllActiveCsvOfferId()
        {
            return await _handlerContext.Offers.Where(e => e.PathToFile.EndsWith("csv") && e.OfferStatus.StatusName == "Active").Select(e => e.OfferId).ToListAsync();
        }
    }
}
