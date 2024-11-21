using database_communicator.Data;
using database_communicator.Models;
using database_communicator.Utils;
using database_communicator.FilterClass;
using database_communicator.Models.DTOs.Create;
using database_communicator.Models.DTOs.Get;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace database_communicator.Services
{
    public interface IOutsideItemServices
    {

        public Task<IEnumerable<GetOutsideItem>> GetItems(string? sort, OutsideItemFiltersTemplate filters);
        public Task<IEnumerable<GetOutsideItem>> GetItems(string search, string? sort, OutsideItemFiltersTemplate filters);
        public Task<bool> ItemExist(int itemId, int orgId);
        public Task<bool> DeleteItem(int itemId, int orgId);
        public Task<IEnumerable<string>> AddItems(CreateOutsideItems data);
        public Task<IEnumerable<int>> GetItemOwners(int itemId, int orgId);
    }
    /// <summary>
    /// Class that interact with database and contains functions allowing to work on outside items. Outside item is item that user client sells or used to sell.
    /// </summary>
    public class OutsideItemServices : IOutsideItemServices
    {
        private readonly HandlerContext _handlerContext;
        private readonly ILogger<CreditNoteServices> _logger;
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="handlerContext">Database context</param>
        /// <param name="logger">Log interface</param>
        public OutsideItemServices(HandlerContext handlerContext, ILogger<CreditNoteServices> logger)
        {
            _handlerContext = handlerContext;
            _logger = logger;

        }
        /// <summary>
        /// Do select query to receive sorted and filtered items information.
        /// </summary>
        /// <param name="sort">Contains parameter that object will be sorted by. Must start with D or A to determine ascending order. Then is follow by name of property.</param>
        /// <param name="filters">Object with filter values wrapped in <see cref="OutsideItemFiltersTemplate"/></param>
        /// <returns>List of object containing outside items information.</returns>
        public async Task<IEnumerable<GetOutsideItem>> GetItems(string? sort, OutsideItemFiltersTemplate filters)
        {
            var sortFunc = SortFilterUtils.GetOutsideItemSort(sort);
            bool direction;
            if (sort == null)
            {
                direction = true;
            }
            else
            {
                direction = sort.StartsWith('D');
            }
            var qtyLCond = OutsideItemFilters.GetQtyLowerFilter(filters.QtyL);
            var qtyGCond = OutsideItemFilters.GetQtyGreaterFilter(filters.QtyG);
            var priceLCond = OutsideItemFilters.GetPriceLowerFilter(filters.PriceL);
            var priceGCond = OutsideItemFilters.GetPriceGreaterFilter(filters.PriceG);
            var sourceCond = OutsideItemFilters.GetSourceFilter(filters.Source);
            var currencyCond = OutsideItemFilters.GetCurrencyFilter(filters.Currency);

            return await _handlerContext.OutsideItems
                .Where(qtyLCond)
                .Where(qtyGCond)
                .Where(priceLCond)
                .Where(priceGCond)
                .Where(sourceCond)
                .Where(currencyCond)
                .OrderByWithDirection(sortFunc, direction)
                .Select(e => new GetOutsideItem
                {
                    Users = e.Organization.AppUsers.Select(d => d.Username + " " + d.Surname).ToList(),
                    Partnumber = e.Item.PartNumber,
                    ItemId = e.ItemId,
                    OrgId = e.OrganizationId,
                    OrgName = e.Organization.OrgName,
                    Price = e.PurchasePrice,
                    Qty = e.Qty,
                    Currency = e.CurrencyName
                }).ToListAsync();
        }
        /// <summary>
        /// Do select query to receive search, sorted and filtered items information.
        /// </summary>
        /// <param name="search">The phrase searched in items information. It will check if phrase exist in item name or partnumber.</param>
        /// <param name="sort">Contains parameter that object will be sorted by. Must start with D or A to determine ascending order. Then is follow by name of property.</param>
        /// <param name="filters">Object with filter values wrapped in <see cref="OutsideItemFiltersTemplate"/></param>
        /// <returns>List of object containing outside items information.</returns>
        public async Task<IEnumerable<GetOutsideItem>> GetItems(string search, string? sort, OutsideItemFiltersTemplate filters)
        {
            var sortFunc = SortFilterUtils.GetOutsideItemSort(sort);
            bool direction;
            if (sort == null)
            {
                direction = true;
            }
            else
            {
                direction = sort.StartsWith('D');
            }
            var qtyLCond = OutsideItemFilters.GetQtyLowerFilter(filters.QtyL);
            var qtyGCond = OutsideItemFilters.GetQtyGreaterFilter(filters.QtyG);
            var priceLCond = OutsideItemFilters.GetPriceLowerFilter(filters.PriceL);
            var priceGCond = OutsideItemFilters.GetPriceGreaterFilter(filters.PriceG);
            var sourceCond = OutsideItemFilters.GetSourceFilter(filters.Source);
            var currencyCond = OutsideItemFilters.GetCurrencyFilter(filters.Currency);

            return await _handlerContext.OutsideItems
                .Where(e => e.Item.PartNumber.ToLower().Contains(search.ToLower()) || e.Item.ItemName.ToLower().Contains(search.ToLower()))
                .Where(qtyLCond)
                .Where(qtyGCond)
                .Where(priceLCond)
                .Where(priceGCond)
                .Where(sourceCond)
                .Where(currencyCond)
                .OrderByWithDirection(sortFunc, direction)
                .Select(ent => new GetOutsideItem
                {
                    Users = ent.Organization.AppUsers.Select(d => d.Username + " " + d.Surname).ToList(),
                    Partnumber = ent.Item.PartNumber,
                    ItemId = ent.ItemId,
                    OrgId = ent.OrganizationId,
                    OrgName = ent.Organization.OrgName,
                    Price = ent.PurchasePrice,
                    Qty = ent.Qty,
                    Currency = ent.CurrencyName
                }).ToListAsync();
        }
        /// <summary>
        /// Checks if outside item with given ids exists in database.
        /// </summary>
        /// <param name="itemId">Item id</param>
        /// <param name="orgId">Organization id</param>
        /// <returns></returns>
        public async Task<bool> ItemExist(int itemId, int orgId)
        {
            return await _handlerContext.OutsideItems.AnyAsync(x => x.ItemId == itemId && x.OrganizationId == orgId);
        }
        /// <summary>
        /// Using transactions delete outside item from database.
        /// </summary>
        /// <param name="itemId">Item id.</param>
        /// <param name="orgId">Organization id.</param>
        /// <returns>True if success or flase if failure.</returns>
        public async Task<bool> DeleteItem(int itemId, int orgId)
        {
            using var trans = await _handlerContext.Database.BeginTransactionAsync();
            try
            {
                await _handlerContext.OutsideItems
                                .Where(e => e.ItemId == itemId && e.OrganizationId == orgId)
                                .ExecuteDeleteAsync();
                await _handlerContext.SaveChangesAsync();
                await trans.CommitAsync();
                return true;
            } catch (Exception ex)
            {
                _logger.LogError(ex, "Delete outside item error.");
                await trans.RollbackAsync();
                return false;
            }

        }
        /// <summary>
        /// Using transactions add new outside item to database.
        /// </summary>
        /// <param name="data">New outside item data</param>
        /// <returns>True if success or false if failure.</returns>
        public async Task<IEnumerable<string>> AddItems(CreateOutsideItems data)
        {
            using var trans = await _handlerContext.Database.BeginTransactionAsync();
            try
            {
                var errorItems = new List<string>();
                foreach (var item in data.Items)
                {
                    var eanExist = await _handlerContext.Eans.AnyAsync(x => item.Eans.Contains(x.EanValue) && x.Item.PartNumber != item.Partnumber);
                    if (eanExist)
                    {
                        errorItems.Add(item.Partnumber);
                        continue;
                    }
                    var exist = await _handlerContext.Items.AnyAsync(x => x.PartNumber == item.Partnumber);
                    if (!exist)
                    {
                        var newItem = new Item
                        {
                            ItemName = item.ItemName,
                            ItemDescription = item.ItemDesc,
                            PartNumber = item.Partnumber,
                        };
                        await _handlerContext.Items.AddAsync(newItem);
                        await _handlerContext.SaveChangesAsync();
                        var newEans = item.Eans.Select(e => new Ean
                        {
                            EanValue = e,
                            ItemId = newItem.ItemId
                        });
                        await _handlerContext.Eans.AddRangeAsync(newEans);
                        await _handlerContext.SaveChangesAsync();
                        await _handlerContext.OutsideItems.AddAsync(new OutsideItem
                        {
                            ItemId = newItem.ItemId,
                            OrganizationId = data.OrgId,
                            PurchasePrice = item.Price,
                            Qty = item.Qty,
                            CurrencyName = data.Currency,
                        });
                    } else
                    {
                        var itemId = await _handlerContext.Items.Where(e => e.PartNumber == item.Partnumber).Select(e => e.ItemId).FirstAsync();
                        var outsideItemExist = await _handlerContext.OutsideItems.AnyAsync(x => x.ItemId == itemId && x.OrganizationId == data.OrgId);
                        if (outsideItemExist)
                        {
                            await _handlerContext.OutsideItems
                                .Where(x => x.ItemId == itemId && x.OrganizationId == data.OrgId)
                                .ExecuteUpdateAsync(setter =>
                                    setter.SetProperty(s => s.Qty, item.Qty)
                                    .SetProperty(s => s.PurchasePrice, item.Price)
                                    .SetProperty(s => s.CurrencyName, data.Currency)
                                );
                        } else
                        {
                            await _handlerContext.OutsideItems.AddAsync(new OutsideItem
                            {
                                ItemId = itemId,
                                OrganizationId = data.OrgId,
                                PurchasePrice = item.Price,
                                Qty = item.Qty,
                                CurrencyName = data.Currency,
                            });
                        }
                    }
                }
                await _handlerContext.SaveChangesAsync();
                await trans.CommitAsync();
                return errorItems;
            } catch (Exception ex)
            {
                _logger.LogError(ex, "Create outside item error.");
                await trans.RollbackAsync();
                return new List<string>();
            }
        }
        /// <summary>
        /// Do select query to receive list of owner for given outside item.
        /// </summary>
        /// <param name="itemId">Item id</param>
        /// <param name="orgId">Organization id</param>
        /// <returns>List of outside item owners.</returns>
        public async Task<IEnumerable<int>> GetItemOwners(int itemId, int orgId)
        {
            return await _handlerContext.OutsideItems
                .Where(e => e.ItemId == itemId && e.OrganizationId == orgId)
                .SelectMany(e => e.Organization.AppUsers)
                .Select(e => e.IdUser).ToListAsync();
        }
    }
}
