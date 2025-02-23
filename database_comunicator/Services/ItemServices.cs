﻿using database_communicator.Data;
using database_communicator.FilterClass;
using database_communicator.Models;
using database_communicator.Models.DTOs.Create;
using database_communicator.Models.DTOs.Get;
using database_communicator.Models.DTOs.Modify;
using database_communicator.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace database_communicator.Services
{
    public interface IItemServices
    {
        public Task<int?> AddItem(AddItem newItem);
        public Task<bool> UpdateItem(UpdateItem postItem);
        public Task<bool> RemoveItem(int id);
        public Task<bool> ItemExist(int id);
        public Task<bool> ItemExist(string partNumber);
        public Task<bool> EanExist(IEnumerable<string> eans);
        public Task<IEnumerable<GetManyItems>> GetItems(string currency, string? orderBy, ItemFiltersTemplate filters);
        public Task<IEnumerable<GetManyItems>> GetItems(string currency, string search, string? orderBy, ItemFiltersTemplate filters);
        public Task<IEnumerable<GetManyItems>> GetItems(string currency, int userId, string? orderBy, ItemFiltersTemplate filters);
        public Task<IEnumerable<GetManyItems>> GetItems(string currency, int userId, string search, string? orderBy, ItemFiltersTemplate filters);
        public Task<GetRestInfo> GetRestOfItem(int id, int userId, string currency);
        public Task<GetRestInfo> GetRestOfItemOrg(int id, string currency);
        public Task<IEnumerable<GetBinding>> GetModifyRestOfItem(int id, string currency);
        public Task<string> GetDescription(int id);
        public Task<IEnumerable<GetItemList>> GetItemList();
        public Task<IEnumerable<GetSalesItemList>> GetSalesItemList(int userId, string currency);
        public Task<IEnumerable<GetUsers>> GetItemOwners(int itemId);
        public Task<bool> ChangeBindings(IEnumerable<ModifyBinding> data);
        public Task<bool> ItemHaveRelations(int itemId);
    }
    /// <summary>
    /// Class that interact with database and contains functions allowing to work on credit notes.
    /// </summary>
    public class ItemServices : IItemServices
    {
        private readonly HandlerContext _handlerContext;
        private readonly ILogger<ItemServices> _logger;
        public ItemServices(HandlerContext handlerContext, ILogger<ItemServices> logger)
        {
            _handlerContext = handlerContext;
            _logger = logger;
        }
        /// <summary>
        /// Using transactions add new item to database.
        /// </summary>
        /// <param name="newItem">New item values wrapped in <see cref="Models.DTOs.Create.AddItem"/>.</param>
        /// <returns>True if success or false if failure.</returns>
        public async Task<int?> AddItem(AddItem newItem)
        {
            using var trans = await _handlerContext.Database.BeginTransactionAsync();
            try
            {
                var item = new Item
                {
                    ItemName = newItem.ItemName,
                    ItemDescription = newItem.ItemDescription,
                    PartNumber = newItem.PartNumber
                };
                await _handlerContext.Items.AddAsync(item);
                await _handlerContext.SaveChangesAsync();

                await _handlerContext.Eans.AddRangeAsync(newItem.Eans.Select(ean => new Ean
                {
                    EanValue = ean,
                    ItemId = item.ItemId
                }));
                await _handlerContext.SaveChangesAsync();

                await trans.CommitAsync();
                return item.ItemId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Add item error.");
                await trans.RollbackAsync();
                return null;
            }
        }
        /// <summary>
        /// Checks if any of given eans exist in database.
        /// </summary>
        /// <param name="eans">Array of eans</param>
        /// <returns>True if at least one ean exist in database, otherwise false.</returns>
        public async Task<bool> EanExist(IEnumerable<string> eans)
        {
            return await _handlerContext.Eans.AnyAsync(e => eans.Contains(e.EanValue));
        }
        /// <summary>
        /// Do select query to receive item information for chosen user that was not passed in bulk query.
        /// </summary>
        /// <param name="id">Item id.</param>
        /// <param name="userId">Id of item owner.</param>
        /// <param name="currency">Shortcut name of currency.</param>
        /// <returns>Object that contains information about invoice where that item is used and information about user clients stock of this item.</returns>
        public async Task<GetRestInfo> GetRestOfItem(int id, int userId, string currency)
        {
            List<GetRestInfoOwnedItems> result;
            if (currency == "PLN")
            {
                result = await _handlerContext.ItemOwners
                    .Where(e => e.OwnedItemId == id && e.IdUser == userId)
                    .Include(e => e.IdUserNavigation)
                    .Include(e => e.OwnedItem)
                        .ThenInclude(e => e.Invoice)
                            .ThenInclude(e => e.SellerNavigation)
                    .Include(e => e.OwnedItem)
                        .ThenInclude(e => e.PurchasePrices)
                    .Where(e => e.Qty > 0)
                    .Select(instance => new GetRestInfoOwnedItems
                    {
                        UserId = instance.IdUserNavigation.IdUser,
                        Username = instance.IdUserNavigation.Username,
                        OrganizationName = instance.OwnedItem.Invoice.SellerNavigation.OrgName,
                        InvoiceNumber = instance.OwnedItem.Invoice.InvoiceNumber,
                        Qty = instance.Qty,
                        Price = instance.OwnedItem.PurchasePrices.Select(e => e.Price).Average(),
                        Currency = "PLN"
                    }).ToListAsync();
            }
            else
            {
                result = await _handlerContext.ItemOwners
                    .Where(e => e.OwnedItemId == id && e.IdUser == userId)
                    .Include(e => e.IdUserNavigation)
                    .Include(e => e.OwnedItem)
                        .ThenInclude(e => e.Invoice)
                            .ThenInclude(e => e.SellerNavigation)
                    .Include(e => e.OwnedItem)
                        .ThenInclude(e => e.PurchasePrices)
                            .ThenInclude(e => e.CalculatedPrices)
                    .Where(e => e.Qty > 0)
                    .Select(instance => new GetRestInfoOwnedItems
                    {
                        UserId = instance.IdUserNavigation.IdUser,
                        Username = instance.IdUserNavigation.Username,
                        OrganizationName = instance.OwnedItem.Invoice.SellerNavigation.OrgName,
                        InvoiceNumber = instance.OwnedItem.Invoice.InvoiceNumber,
                        Qty = instance.Qty,
                        Price = instance.OwnedItem.PurchasePrices
                            .SelectMany(e => e.CalculatedPrices)
                            .Where(e => e.CurrencyName == currency)
                            .Select(e => e.Price).Average(),
                        Currency = currency
                    }).ToListAsync();
            }
            var outsideItems = await _handlerContext.OutsideItems
                .Where(e => e.ItemId == id && e.Organization.AppUsers.Any(d => d.IdUser == userId))
                .Include(d => d.Organization)
                    .ThenInclude(d => d.AppUsers)
                .Select(outItem => new GetRestInfoOutsideItems
                {
                    Users = outItem.Organization.AppUsers.Select(e => new KeyValuePair<int, string>(e.IdUser, e.Username)).ToList(),
                    OrganizationName = outItem.Organization.OrgName,
                    Qty = outItem.Qty,
                    Price = outItem.PurchasePrice,
                    Currency = outItem.CurrencyName
                }).ToListAsync();

            return new GetRestInfo
            {
                OutsideItemInfos = outsideItems,
                OwnedItemInfos = result
            };

        }
        /// <summary>
        /// Do select query to receive item information user that was not passed in bulk query.
        /// </summary>
        /// <param name="id">Item id.</param>
        /// <param name="currency">Shortcut name of currency.</param>
        /// <returns>Object that contains information about invoice where that item is used and information about user clients stock of this item.</returns>
        public async Task<GetRestInfo> GetRestOfItemOrg(int id, string currency)
        {
            List<GetRestInfoOwnedItems> result;
            if (currency == "PLN")
            {
                result = await _handlerContext.ItemOwners
                    .Where(e => e.OwnedItemId == id && e.Qty > 0)
                    .Select(instance => new GetRestInfoOwnedItems
                    {
                        UserId = instance.IdUserNavigation.IdUser,
                        Username = instance.IdUserNavigation.Username + " " + instance.IdUserNavigation.Surname,
                        OrganizationName = instance.OwnedItem.Invoice.SellerNavigation.OrgName,
                        InvoiceNumber = instance.OwnedItem.Invoice.InvoiceNumber,
                        Qty = instance.Qty,
                        Price = instance.OwnedItem.PurchasePrices.Select(e => e.Price).Average(),
                        Currency = "PLN"
                    }).ToListAsync();
            }
            else
            {
                result = await _handlerContext.ItemOwners
                    .Where(e => e.OwnedItemId == id && e.Qty > 0)
                    .Select(instance => new GetRestInfoOwnedItems
                    {
                        UserId = instance.IdUserNavigation.IdUser,
                        Username = instance.IdUserNavigation.Username + " " + instance.IdUserNavigation.Surname,
                        OrganizationName = instance.OwnedItem.Invoice.SellerNavigation.OrgName,
                        InvoiceNumber = instance.OwnedItem.Invoice.InvoiceNumber,
                        Qty = instance.Qty,
                        Price = instance.OwnedItem.PurchasePrices
                            .SelectMany(e => e.CalculatedPrices)
                            .Where(e => e.CurrencyName == currency)
                            .Select(e => e.Price).Average(),
                        Currency = currency
                    }).ToListAsync();
            }
            var outsideItems = await _handlerContext.OutsideItems
                .Where(e => e.ItemId == id)
                .Select(obj => new GetRestInfoOutsideItems
                {
                    Users = obj.Organization.AppUsers.Select(e => new KeyValuePair<int, string>(e.IdUser, e.Username)).ToList(),
                    OrganizationName = obj.Organization.OrgName,
                    Qty = obj.Qty,
                    Price = obj.PurchasePrice,
                    Currency = obj.CurrencyName
                }).ToListAsync();

            return new GetRestInfo
            {
                OutsideItemInfos = outsideItems,
                OwnedItemInfos = result
            };
        }
        /// <summary>
        /// Do select query to receive sorted and filtered items information with given currency from database.
        /// </summary>
        /// <param name="currency">Shortcut name of currency.</param>
        /// <param name="orderBy">Contains parameter that object will be sorted by. Must start with D or A to determine ascending order. Then is follow by name of property.</param>
        /// <param name="filters">Object with filter values wrapped in <see cref="ItemFiltersTemplate"/></param>
        /// <returns>List of <see cref="GetManyItems"/>.</returns>
        public async Task<IEnumerable<GetManyItems>> GetItems(string currency, string? orderBy, ItemFiltersTemplate filters)
        {
            List<GetManyItems> items;
            var statusCond = ItemFilters.GetStatusFilter(filters.Status);
            var eanCond = ItemFilters.GetEanFilter(filters.Ean);
            var qtyLCondition = ItemFilters.GetQtyLowerFilter(filters.QtyL);
            var qtyGCondition = ItemFilters.GetQtyGreaterFilter(filters.QtyG);
            var priceLCondition = ItemFilters.GetPriceLowerFilter(filters.PriceL);
            var priceGCondition = ItemFilters.GetPriceGreaterFilter(filters.PriceG);

            bool direction;
            if (orderBy == null)
            {
                direction = true;
            }
            else
            {
                direction = orderBy.StartsWith('D');
            }
            var orderByFunc = SortFilterUtils.GetItemSort(orderBy);
            if (currency == "PLN")
            {
                items = await _handlerContext.Items
                .Where(eanCond)
                .Select(instance => new GetManyItems
                {
                    Users = instance.OwnedItems.SelectMany(e => e.ItemOwners).Select(e => e.IdUserNavigation)
                        .GroupBy(e => new { e.IdUser, e.Username, e.Surname })
                        .Select(e => e.Key.Username + " " + e.Key.Surname).ToList(),
                    ItemId = instance.ItemId,
                    ItemName = instance.ItemName,
                    PartNumber = instance.PartNumber,
                    StatusName = WarehouseUtils.GetItemStatus(
                            instance.OwnedItems
                                .SelectMany(e => e.PurchasePrices)
                                .Select(e => e.Qty).Sum()
                                - instance.OwnedItems.SelectMany(e => e.PurchasePrices).SelectMany(e => e.SellingPrices).Select(d => d.Qty).Sum()
                                + instance.OwnedItems.SelectMany(e => e.PurchasePrices).SelectMany(e => e.CreditNoteItems)
                                    .Select(d => d.Qty).Sum(),
                            instance.OutsideItems.Select(e => e.Qty).Sum(),
                            instance.ProformaFutureItems.SelectMany(e => e.Proforma.Deliveries).Any(e => e.DeliveryStatus.StatusName == "In transport")
                        ),
                    Eans = instance.Eans.Select(e => e.EanValue),
                    Qty = instance.OwnedItems
                    .SelectMany(e => e.PurchasePrices)
                    .Select(e => e.Qty).Sum()
                    - instance.OwnedItems.SelectMany(e => e.PurchasePrices).SelectMany(e => e.SellingPrices).Select(d => d.Qty).Sum()
                    + instance.OwnedItems.SelectMany(e => e.PurchasePrices).SelectMany(e => e.CreditNoteItems).Select(d => d.Qty).Sum(),
                    PurchasePrice = instance.OwnedItems
                        .SelectMany(e => e.PurchasePrices)
                        .Where(e =>
                        e.Qty
                        - e.SellingPrices.Select(d => d.Qty).Sum()
                        + e.CreditNoteItems.Select(d => d.Qty).Sum()
                        > 0
                        )
                        .Select(e => e.Price)
                        .Union(
                            instance.OwnedItems
                                .SelectMany(e => e.PurchasePrices)
                                .SelectMany(e => e.CreditNoteItems)
                                .Where(e => e.CreditNote.Invoice.SellingPrices.Count == 0 && e.Qty > 0)
                                .Select(e => e.NewPrice)
                        )
                        .Average(),
                    Sources = instance.OwnedItems.Select(e => e.Invoice).Select(e => e.SellerNavigation).GroupBy(e => e.OrgName).Select(e => e.Key).ToList()
                })
                .Where(qtyLCondition)
                .Where(qtyGCondition)
                .Where(priceGCondition)
                .Where(priceLCondition)
                .ToListAsync();
                return items.Where(statusCond).OrderByWithDirection(orderByFunc, direction);
            }
            else
            {
                items = await _handlerContext.Items
                .Where(eanCond)
                .Select(insta => new GetManyItems
                {
                    Users = insta.OwnedItems
                        .SelectMany(e => e.ItemOwners)
                        .Select(e => e.IdUserNavigation)
                        .GroupBy(e => new { e.IdUser, e.Username, e.Surname })
                        .Select(e => e.Key.Username + " " + e.Key.Surname).ToList(),
                    ItemId = insta.ItemId,
                    ItemName = insta.ItemName,
                    PartNumber = insta.PartNumber,
                    StatusName = WarehouseUtils.GetItemStatus(
                            insta.OwnedItems
                                .SelectMany(e => e.PurchasePrices)
                                .Select(e => e.Qty).Sum()
                                - insta.OwnedItems.SelectMany(e => e.PurchasePrices).SelectMany(e => e.SellingPrices).Select(d => d.Qty).Sum()
                                + insta.OwnedItems.SelectMany(e => e.PurchasePrices).SelectMany(e => e.CreditNoteItems)
                                    .Select(d => d.Qty).Sum(),
                            insta.OutsideItems.Select(e => e.Qty).Sum(),
                            insta.ProformaFutureItems.SelectMany(e => e.Proforma.Deliveries).Any(e => e.DeliveryStatus.StatusName == "In transport")
                        ),
                    Eans = insta.Eans.Select(e => e.EanValue),
                    Qty = insta.OwnedItems
                        .SelectMany(e => e.PurchasePrices)
                        .Select(e => e.Qty).Sum()
                        - insta.OwnedItems.SelectMany(e => e.PurchasePrices).SelectMany(e => e.SellingPrices).Select(d => d.Qty).Sum()
                        + insta.OwnedItems.SelectMany(e => e.PurchasePrices).SelectMany(e => e.CreditNoteItems).Select(d => d.Qty).Sum(),
                    PurchasePrice = insta.OwnedItems
                        .SelectMany(e => e.PurchasePrices)
                        .Where(e =>
                        e.Qty
                        - e.SellingPrices.Select(d => d.Qty).Sum()
                        + e.CreditNoteItems.Select(d => d.Qty).Sum()
                        > 0
                        ).SelectMany(e => e.CalculatedPrices)
                        .Where(e => e.CurrencyName == currency)
                        .Select(e => e.Price)
                        .Union(
                            insta.OwnedItems
                            .SelectMany(e => e.PurchasePrices)
                            .SelectMany(e => e.CreditNoteItems)
                            .Where(e => e.CreditNote.Invoice.SellingPrices.Count == 0 && e.Qty > 0)
                            .SelectMany(e => e.CalculatedCreditNotePrices)
                            .Where(e => e.CurrencyName == currency)
                            .Select(e => e.Price)
                        )
                        .Average(),
                    Sources = insta.OwnedItems.Select(e => e.Invoice).Select(e => e.SellerNavigation).GroupBy(e => e.OrgName).Select(e => e.Key).ToList()
                })
                .Where(qtyLCondition)
                .Where(qtyGCondition)
                .Where(priceGCondition)
                .Where(priceLCondition)
                .ToListAsync();
                return items.Where(statusCond).OrderByWithDirection(orderByFunc, direction);
            }
        }
        /// <summary>
        /// Do select query to receive searched, sorted and filtered items information with given currency from database.
        /// </summary>
        /// <param name="currency">Shortcut name of currency.</param>
        /// <param name="search">The phrase searched in items information. It will check if phrase exist in partnumber or item name.</param>
        /// <param name="orderBy">Contains parameter that object will be sorted by. Must start with D or A to determine ascending order. Then is follow by name of property.</param>
        /// <param name="filters">Object with filter values wrapped in <see cref="ItemFiltersTemplate"/></param>
        /// <returns>List of <see cref="GetManyItems"/>.</returns>
        public async Task<IEnumerable<GetManyItems>> GetItems(string currency, string search, string? orderBy, ItemFiltersTemplate filters)
        {
            List<GetManyItems> items;
            var statusCond = ItemFilters.GetStatusFilter(filters.Status);
            var eanCond = ItemFilters.GetEanFilter(filters.Ean);
            var qtyLCondition = ItemFilters.GetQtyLowerFilter(filters.QtyL);
            var qtyGCondition = ItemFilters.GetQtyGreaterFilter(filters.QtyG);
            var priceLCondition = ItemFilters.GetPriceLowerFilter(filters.PriceL);
            var priceGCondition = ItemFilters.GetPriceGreaterFilter(filters.PriceG);

            bool direction;
            if (orderBy == null)
            {
                direction = true;
            }
            else
            {
                direction = orderBy.StartsWith('D');
            }
            var orderByFunc = SortFilterUtils.GetItemSort(orderBy);
            if (currency == "PLN")
            {
                items = await _handlerContext.Items
                    .Where(eanCond)
                    .Where(j => EF.Functions.FreeText(j.PartNumber, search) || EF.Functions.FreeText(j.ItemName, search))
                    .Select(instc => new GetManyItems
                    {
                        Users = instc.OwnedItems
                        .SelectMany(e => e.ItemOwners)
                        .Select(e => e.IdUserNavigation)
                        .GroupBy(e => new { e.IdUser, e.Username, e.Surname })
                        .Select(e => e.Key.Username + " " + e.Key.Surname).ToList(),
                        ItemId = instc.ItemId,
                        ItemName = instc.ItemName,
                        PartNumber = instc.PartNumber,
                        StatusName = WarehouseUtils.GetItemStatus(
                                instc.OwnedItems
                                    .SelectMany(e => e.PurchasePrices)
                                    .Select(e =>
                                        e.Qty
                                    )
                                    .Sum()
                                    -
                                    instc.OwnedItems
                                    .SelectMany(e => e.PurchasePrices)
                                    .SelectMany(e =>
                                        e.SellingPrices
                                    ).Select(e => e.Qty)
                                    .Sum()
                                     +
                                    instc.OwnedItems
                                    .SelectMany(e => e.PurchasePrices)
                                    .SelectMany(e =>
                                        e.CreditNoteItems
                                    ).Select(e => e.Qty)
                                    .Sum(),
                                instc.OutsideItems.Select(e => e.Qty).Sum(),
                                instc.ProformaFutureItems.SelectMany(e => e.Proforma.Deliveries).Any(e => e.DeliveryStatus.StatusName == "In transport")
                            ),
                        Eans = instc.Eans.Select(e => e.EanValue),
                        Qty = instc.OwnedItems
                            .SelectMany(e => e.PurchasePrices)
                            .Select(e =>
                                e.Qty
                            )
                            .Sum()
                            -
                            instc.OwnedItems
                            .SelectMany(e => e.PurchasePrices)
                            .SelectMany(e =>
                                e.SellingPrices
                            ).Select(e => e.Qty)
                            .Sum()
                             +
                            instc.OwnedItems
                            .SelectMany(e => e.PurchasePrices)
                            .SelectMany(e =>
                                e.CreditNoteItems
                            ).Select(e => e.Qty)
                            .Sum(),
                        PurchasePrice = instc.OwnedItems
                            .SelectMany(e => e.PurchasePrices)
                            .Where(e =>
                            e.Qty
                            - e.SellingPrices.Select(d => d.Qty).Sum()
                            - e.CreditNoteItems.Select(d => Math.Abs(d.Qty)).Sum()
                            + e.CreditNoteItems.Where(d => d.Qty > 0 && e.SellingPrices.Select(f => f.SellInvoiceId).Contains(d.CreditNote.InvoiceId)).Select(d => d.Qty).Sum()
                            > 0
                            ).SelectMany(e => e.CalculatedPrices)
                            .Where(e => e.CurrencyName == currency)
                            .Select(e => e.Price)
                            .Union(
                                instc.OwnedItems
                                .SelectMany(e => e.PurchasePrices)
                                .SelectMany(e => e.CreditNoteItems)
                                .Where(e => e.CreditNote.Invoice.SellingPrices.Count == 0 && e.Qty > 0)
                                .SelectMany(e => e.CalculatedCreditNotePrices)
                                .Where(e => e.CurrencyName == currency)
                                .Select(e => e.Price)
                            )
                            .Average(),
                        Sources = instc.OwnedItems.Select(e => e.Invoice).Select(e => e.SellerNavigation).GroupBy(e => e.OrgName).Select(e => e.Key).ToList()
                    })
                    .Where(qtyLCondition)
                    .Where(qtyGCondition)
                    .Where(priceGCondition)
                    .Where(priceLCondition)
                    .ToListAsync();
                return items.Where(statusCond).OrderByWithDirection(orderByFunc, direction);
            }
            else
            {
                items = await _handlerContext.Items
                    .Where(eanCond)
                    .Where(j => EF.Functions.FreeText(j.PartNumber, search) || EF.Functions.FreeText(j.ItemName, search))
                    .Select(instac => new GetManyItems
                    {
                        Users = instac.OwnedItems
                        .SelectMany(e => e.ItemOwners)
                        .Select(e => e.IdUserNavigation)
                        .GroupBy(e => new { e.IdUser, e.Username, e.Surname })
                        .Select(e => e.Key.Username + " " + e.Key.Surname).ToList(),
                        ItemId = instac.ItemId,
                        ItemName = instac.ItemName,
                        PartNumber = instac.PartNumber,
                        StatusName = WarehouseUtils.GetItemStatus(
                                instac.OwnedItems
                                    .SelectMany(e => e.PurchasePrices)
                                    .Select(e =>
                                        e.Qty
                                    )
                                    .Sum()
                                    -
                                    instac.OwnedItems
                                    .SelectMany(e => e.PurchasePrices)
                                    .SelectMany(e =>
                                        e.SellingPrices
                                    ).Select(e => e.Qty)
                                    .Sum()
                                     +
                                    instac.OwnedItems
                                    .SelectMany(e => e.PurchasePrices)
                                    .SelectMany(e =>
                                        e.CreditNoteItems
                                    ).Select(e => e.Qty)
                                    .Sum(),
                                instac.OutsideItems.Select(e => e.Qty).Sum(),
                                instac.ProformaFutureItems.SelectMany(e => e.Proforma.Deliveries).Any(e => e.DeliveryStatus.StatusName == "In transport")
                            ),
                        Eans = instac.Eans.Select(e => e.EanValue),
                        Qty = instac.OwnedItems
                            .SelectMany(e => e.PurchasePrices)
                            .Select(e =>
                                e.Qty
                            )
                            .Sum()
                            -
                            instac.OwnedItems
                            .SelectMany(e => e.PurchasePrices)
                            .SelectMany(e =>
                                e.SellingPrices
                            ).Select(e => e.Qty)
                            .Sum()
                             +
                            instac.OwnedItems
                            .SelectMany(e => e.PurchasePrices)
                            .SelectMany(e =>
                                e.CreditNoteItems
                            ).Select(e => e.Qty)
                            .Sum(),
                        PurchasePrice = instac.OwnedItems
                            .SelectMany(e => e.PurchasePrices)
                            .Where(pp =>
                            pp.Qty
                            - pp.SellingPrices.Select(d => d.Qty).Sum()
                            - pp.CreditNoteItems.Select(d => Math.Abs(d.Qty)).Sum()
                            + pp.CreditNoteItems.Where(d => d.Qty > 0 && pp.SellingPrices.Select(f => f.SellInvoiceId).Contains(d.CreditNote.InvoiceId)).Select(d => d.Qty).Sum()
                            > 0
                            ).SelectMany(e => e.CalculatedPrices)
                            .Where(e => e.CurrencyName == currency)
                            .Select(e => e.Price)
                            .Union(
                                instac.OwnedItems
                                .SelectMany(e => e.PurchasePrices)
                                .SelectMany(e => e.CreditNoteItems)
                                .Where(e => e.CreditNote.Invoice.SellingPrices.Count == 0 && e.Qty > 0)
                                .SelectMany(e => e.CalculatedCreditNotePrices)
                                .Where(e => e.CurrencyName == currency)
                                .Select(e => e.Price)
                            )
                            .Average(),
                        Sources = instac.OwnedItems.Select(e => e.Invoice).Select(e => e.SellerNavigation).GroupBy(e => e.OrgName).Select(e => e.Key).ToList()
                    })
                    .Where(qtyLCondition)
                    .Where(qtyGCondition)
                    .Where(priceGCondition)
                    .Where(priceLCondition)
                    .ToListAsync();
                return items.Where(statusCond).OrderByWithDirection(orderByFunc, direction);
            }
        }
        /// <summary>
        /// Do select query to receive sorted and filtered items information with given currency from database for chosen user.
        /// </summary>
        /// <param name="currency">Shortcut name of currency.</param>
        /// <param name="userId">User id.</param>
        /// <param name="orderBy">Contains parameter that object will be sorted by. Must start with D or A to determine ascending order. Then is follow by name of property.</param>
        /// <param name="filters">Object with filter values wrapped in <see cref="ItemFiltersTemplate"/></param>
        /// <returns>List of <see cref="GetManyItems"/>.</returns>
        public async Task<IEnumerable<GetManyItems>> GetItems(string currency, int userId, string? orderBy, ItemFiltersTemplate filters)
        {
            List<GetManyItems> items;
            var statusCond = ItemFilters.GetStatusFilter(filters.Status);
            var eanCond = ItemFilters.GetEanFilter(filters.Ean);
            var qtyLCondition = ItemFilters.GetQtyLowerFilter(filters.QtyL);
            var qtyGCondition = ItemFilters.GetQtyGreaterFilter(filters.QtyG);
            var priceLCondition = ItemFilters.GetPriceLowerFilter(filters.PriceL);
            var priceGCondition = ItemFilters.GetPriceGreaterFilter(filters.PriceG);

            bool direction;
            if (orderBy == null)
            {
                direction = true;
            }
            else
            {
                direction = orderBy.StartsWith('D');
            }
            var orderByFunc = SortFilterUtils.GetItemSort(orderBy);
            if (currency == "PLN")
            {
                items = await _handlerContext.Items
                    .Where(eanCond)
                    .Select(inst => new GetManyItems
                    {
                        ItemId = inst.ItemId,
                        ItemName = inst.ItemName,
                        PartNumber = inst.PartNumber,
                        StatusName = WarehouseUtils.GetItemStatus(
                                inst.OwnedItems.SelectMany(e => e.ItemOwners).Where(e => e.IdUser == userId).Select(e => e.Qty).Sum(),
                                inst.OutsideItems.Select(e => e.Qty).Sum(),
                                inst.ProformaFutureItems.SelectMany(e => e.Proforma.Deliveries).Any(e => e.DeliveryStatus.StatusName == "In transport" && e.Proforma.UserId == userId)
                            ),
                        Eans = inst.Eans.Select(e => e.EanValue),
                        Qty = inst.OwnedItems
                            .SelectMany(e => e.ItemOwners)
                            .Where(e => e.IdUser == userId)
                            .Select(e => e.Qty).Sum(),
                        PurchasePrice = inst.OwnedItems
                            .SelectMany(e => e.PurchasePrices)
                            .Where(pur =>
                            pur.OwnedItem.ItemOwners.Where(x => x.OwnedItemId == pur.OwnedItemId && x.IdUser == userId).Sum(x => x.Qty)
                            > 0
                            )
                            .Select(e => e.Price)
                            .Union(
                                inst.OwnedItems
                                .SelectMany(e => e.PurchasePrices)
                                .SelectMany(e => e.CreditNoteItems)
                                .Where(e => e.CreditNote.Invoice.SellingPrices.Count == 0 && e.Qty > 0)
                                .Select(e => e.NewPrice)
                            )
                            .Average(),
                        Sources = inst.OwnedItems.Select(e => e.Invoice).Select(e => e.SellerNavigation).GroupBy(e => e.OrgName).Select(e => e.Key).ToList()
                    })
                    .Where(qtyLCondition)
                    .Where(qtyGCondition)
                    .Where(priceGCondition)
                    .Where(priceLCondition)
                    .ToListAsync();
                return items.Where(statusCond).OrderByWithDirection(orderByFunc, direction);
            }
            else
            {
                items = await _handlerContext.Items
                    .Where(eanCond)
                    .Select(instace => new GetManyItems
                    {
                        ItemId = instace.ItemId,
                        ItemName = instace.ItemName,
                        PartNumber = instace.PartNumber,
                        StatusName = WarehouseUtils.GetItemStatus(
                                instace.OwnedItems.SelectMany(e => e.ItemOwners).Where(e => e.IdUser == userId).Select(e => e.Qty).Sum(),
                                instace.OutsideItems.Select(e => e.Qty).Sum(),
                                instace.ProformaFutureItems.SelectMany(e => e.Proforma.Deliveries).Any(e => e.DeliveryStatus.StatusName == "In transport" && e.Proforma.UserId == userId)
                            ),
                        Eans = instace.Eans.Select(e => e.EanValue),
                        Qty = instace.OwnedItems
                            .SelectMany(e => e.ItemOwners)
                            .Where(e => e.IdUser == userId)
                            .Select(e => e.Qty).Sum(),
                        PurchasePrice = instace.OwnedItems
                            .SelectMany(e => e.PurchasePrices)
                            .Where(e =>
                            e.OwnedItem.ItemOwners.Where(x => x.OwnedItemId == e.OwnedItemId && x.IdUser == userId).Sum(x => x.Qty)
                            > 0
                            ).SelectMany(e => e.CalculatedPrices)
                            .Where(e => e.CurrencyName == currency)
                            .Select(e => e.Price)
                            .Union(
                                instace.OwnedItems
                                .SelectMany(e => e.PurchasePrices)
                                .SelectMany(e => e.CreditNoteItems)
                                .Where(e => e.CreditNote.Invoice.SellingPrices.Count == 0 && e.Qty > 0)
                                .SelectMany(e => e.CalculatedCreditNotePrices)
                                .Where(e => e.CurrencyName == currency)
                                .Select(e => e.Price)
                            )
                            .Average(),
                        Sources = instace.OwnedItems.Select(e => e.Invoice).Select(e => e.SellerNavigation).GroupBy(e => e.OrgName).Select(e => e.Key).ToList()
                    })
                    .Where(qtyLCondition)
                    .Where(qtyGCondition)
                    .Where(priceGCondition)
                    .Where(priceLCondition)
                    .ToListAsync();
                return items.Where(statusCond).OrderByWithDirection(orderByFunc, direction);
            }
        }
        /// <summary>
        /// Do select query to receive searched, sorted and filtered items information with given currency from database for chosen user.
        /// </summary>
        /// <param name="currency">Shortcut name of currency.</param>
        /// <param name="userId">User id.</param>
        /// <param name="search">The phrase searched in items information. It will check if phrase exist in partnumber or item name.</param>
        /// <param name="orderBy">Contains parameter that object will be sorted by. Must start with D or A to determine ascending order. Then is follow by name of property.</param>
        /// <param name="filters">Object with filter values wrapped in <see cref="ItemFiltersTemplate"/></param>
        /// <returns>List of <see cref="GetManyItems"/>.</returns>
        public async Task<IEnumerable<GetManyItems>> GetItems(string currency, int userId, string search, string? orderBy, ItemFiltersTemplate filters)
        {
            List<GetManyItems> items;
            var statusCond = ItemFilters.GetStatusFilter(filters.Status);
            var eanCond = ItemFilters.GetEanFilter(filters.Ean);
            var qtyLCondition = ItemFilters.GetQtyLowerFilter(filters.QtyL);
            var qtyGCondition = ItemFilters.GetQtyGreaterFilter(filters.QtyG);
            var priceLCondition = ItemFilters.GetPriceLowerFilter(filters.PriceL);
            var priceGCondition = ItemFilters.GetPriceGreaterFilter(filters.PriceG);

            bool direction;
            if (orderBy == null)
            {
                direction = true;
            }
            else
            {
                direction = orderBy.StartsWith('D');
            }
            var orderByFunc = SortFilterUtils.GetItemSort(orderBy);
            if (currency == "PLN")
            {
                items = await _handlerContext.Items
                    .Where(eanCond)
                    .Where(j => EF.Functions.FreeText(j.PartNumber, search) || EF.Functions.FreeText(j.ItemName, search))
                    .Where(e => e.OwnedItems.SelectMany(d => d.ItemOwners).Any(x => x.IdUser == userId))
                    .Select(obj => new GetManyItems
                    {
                        ItemId = obj.ItemId,
                        ItemName = obj.ItemName,
                        PartNumber = obj.PartNumber,
                        StatusName = WarehouseUtils.GetItemStatus(
                                obj.OwnedItems.SelectMany(e => e.ItemOwners).Where(e => e.IdUser == userId).Select(e => e.Qty).Sum(),
                                obj.OutsideItems.Select(e => e.Qty).Sum(),
                                obj.ProformaFutureItems.SelectMany(e => e.Proforma.Deliveries).Any(e => e.DeliveryStatus.StatusName == "In transport" && e.Proforma.UserId == userId)
                            ),
                        Eans = obj.Eans.Select(e => e.EanValue),
                        Qty = obj.OwnedItems
                            .SelectMany(e => e.ItemOwners)
                            .Where(e => e.IdUser == userId)
                            .Select(e => e.Qty).Sum(),
                        PurchasePrice = obj.OwnedItems
                            .SelectMany(e => e.PurchasePrices)
                            .Where(e =>
                            e.OwnedItem.ItemOwners.Where(x => x.OwnedItemId == e.OwnedItemId && x.IdUser == userId).Sum(x => x.Qty)
                            > 0
                            ).SelectMany(e => e.CalculatedPrices)
                            .Where(e => e.CurrencyName == currency)
                            .Select(e => e.Price)
                            .Union(
                                obj.OwnedItems
                                .SelectMany(e => e.PurchasePrices)
                                .SelectMany(e => e.CreditNoteItems)
                                .Where(e => e.CreditNote.Invoice.SellingPrices.Count == 0 && e.Qty > 0)
                                .SelectMany(e => e.CalculatedCreditNotePrices)
                                .Where(e => e.CurrencyName == currency)
                                .Select(e => e.Price)
                            )
                            .Average(),
                        Sources = obj.OwnedItems.Select(e => e.Invoice).Select(e => e.SellerNavigation).GroupBy(e => e.OrgName).Select(e => e.Key).ToList()
                    })
                    .Where(qtyLCondition)
                    .Where(qtyGCondition)
                    .Where(priceGCondition)
                    .Where(priceLCondition)
                    .ToListAsync();
                return items.Where(statusCond).OrderByWithDirection(orderByFunc, direction);
            }
            else
            {
                items = await _handlerContext.Items
                    .Where(eanCond)
                    .Where(j => EF.Functions.FreeText(j.PartNumber, search) || EF.Functions.FreeText(j.ItemName, search))
                    .Where(e => e.OwnedItems.SelectMany(d => d.ItemOwners).Any(x => x.IdUser == userId))
                    .Select(objs => new GetManyItems
                    {
                        ItemId = objs.ItemId,
                        ItemName = objs.ItemName,
                        PartNumber = objs.PartNumber,
                        StatusName = WarehouseUtils.GetItemStatus(
                                objs.OwnedItems.SelectMany(e => e.ItemOwners).Where(e => e.IdUser == userId).Select(e => e.Qty).Sum(),
                                objs.OutsideItems.Select(e => e.Qty).Sum(),
                                objs.ProformaFutureItems.SelectMany(e => e.Proforma.Deliveries).Any(e => e.DeliveryStatus.StatusName == "In transport" && e.Proforma.UserId == userId)
                            ),
                        Eans = objs.Eans.Select(e => e.EanValue),
                        Qty = objs.OwnedItems
                            .SelectMany(e => e.ItemOwners)
                            .Where(e => e.IdUser == userId)
                            .Select(e => e.Qty).Sum(),
                        PurchasePrice = objs.OwnedItems
                            .SelectMany(e => e.PurchasePrices)
                            .Where(pri =>
                            pri.OwnedItem.ItemOwners.Where(x => x.OwnedItemId == pri.OwnedItemId && x.IdUser == userId).Sum(x => x.Qty)
                            > 0
                            ).SelectMany(e => e.CalculatedPrices)
                            .Where(e => e.CurrencyName == currency)
                            .Select(e => e.Price)
                            .Union(
                                objs.OwnedItems
                                .SelectMany(e => e.PurchasePrices)
                                .SelectMany(e => e.CreditNoteItems)
                                .Where(e => e.CreditNote.Invoice.SellingPrices.Count == 0 && e.Qty > 0)
                                .SelectMany(e => e.CalculatedCreditNotePrices)
                                .Where(e => e.CurrencyName == currency)
                                .Select(e => e.Price)
                            )
                            .Average(),
                        Sources = objs.OwnedItems.Select(e => e.Invoice).Select(e => e.SellerNavigation).GroupBy(e => e.OrgName).Select(e => e.Key).ToList()
                    })
                    .Where(qtyLCondition)
                    .Where(qtyGCondition)
                    .Where(priceGCondition)
                    .Where(priceLCondition)
                    .ToListAsync();
                return items.Where(statusCond).OrderByWithDirection(orderByFunc, direction);
            }
        }
        /// <summary>
        /// Checks if item with given id exists.
        /// </summary>
        /// <param name="id">Item id.</param>
        /// <returns>True if exists or false if not.</returns>
        public async Task<bool> ItemExist(int id)
        {
            return await _handlerContext.Items.Where(e => e.ItemId == id).AnyAsync();
        }
        /// <summary>
        /// Checks if item with given part number exists.
        /// </summary>
        /// <param name="partNumber">Item part number.</param>
        /// <returns>True if exists or false if not.</returns>
        public async Task<bool> ItemExist(string partNumber)
        {
            return await _handlerContext.Items.Where(e => e.PartNumber == partNumber).AnyAsync();
        }
        /// <summary>
        /// Using transactions delete item with given id from database.
        /// </summary>
        /// <param name="id">Item id.</param>
        /// <returns>True if success or false if failure.</returns>
        public async Task<bool> RemoveItem(int id)
        {
            using var trans = await _handlerContext.Database.BeginTransactionAsync();
            try
            {
                var ownedExist = await _handlerContext.OwnedItems.Where(e => e.OwnedItemId == id).AnyAsync();
                if (ownedExist) return false;
                var eansToRemove = await _handlerContext.Eans.Where(e => e.ItemId == id).ToArrayAsync();

                _handlerContext.Eans.RemoveRange(eansToRemove);
                _handlerContext.Remove<Item>(new Item
                {
                    ItemId = id
                });

                await _handlerContext.SaveChangesAsync();
                await trans.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Delete item error.");
                await trans.RollbackAsync();
                return false;
            }
        }
        /// <summary>
        /// Using transactions overwrites old item properties to given new ones.
        /// </summary>
        /// <param name="postItem">New item properties values</param>
        /// <returns>True if success or false if failure.</returns>
        public async Task<bool> UpdateItem(UpdateItem postItem)
        {
            using var trans = await _handlerContext.Database.BeginTransactionAsync();
            try
            {
                if (!postItem.Eans.IsNullOrEmpty())
                {
                    var toDeleteEans = await _handlerContext.Eans.Where(e => e.ItemId == postItem.Id && !postItem.Eans.Contains(e.EanValue)).ToArrayAsync();
                    _handlerContext.Eans.RemoveRange(toDeleteEans);
                    var restEans = await _handlerContext.Eans.Where(e => e.ItemId == postItem.Id && postItem.Eans.Contains(e.EanValue)).Select(e => e.EanValue).ToArrayAsync();
                    var eanToAdd = postItem.Eans.Where(e => !restEans.Contains(e)).ToArray();
                    if (!eanToAdd.IsNullOrEmpty())
                    {
                        _handlerContext.Eans.AddRange(eanToAdd.Select(e => new Ean
                        {
                            EanValue = e,
                            ItemId = postItem.Id
                        }));
                    }
                }

                if (postItem.ItemName != null)
                {
                    await _handlerContext.Items.Where(e => e.ItemId == postItem.Id).ExecuteUpdateAsync(setter => setter.SetProperty(s => s.ItemName, postItem.ItemName));
                }

                if (postItem.ItemDescription != null)
                {
                    await _handlerContext.Items.Where(e => e.ItemId == postItem.Id).ExecuteUpdateAsync(setter => setter.SetProperty(s => s.ItemDescription, postItem.ItemDescription));
                }

                if (postItem.PartNumber != null)
                {
                    await _handlerContext.Items.Where(e => e.ItemId == postItem.Id).ExecuteUpdateAsync(setter => setter.SetProperty(s => s.PartNumber, postItem.PartNumber));
                }

                await _handlerContext.SaveChangesAsync();
                await trans.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Modify item error.");
                await trans.RollbackAsync();
                return false;
            }

        }
        /// <summary>
        /// Do select query to get necessary item information needed for item modification.
        /// </summary>
        /// <param name="id">Item id.</param>
        /// <param name="currency">Shortcut name of currency that item will be displayed.</param>
        /// <returns>List of item owners which information which invoice and how much they own.</returns>
        public async Task<IEnumerable<GetBinding>> GetModifyRestOfItem(int id, string currency)
        {
            List<GetBinding> binding;
            if (currency == "PLN")
            {
                binding = await _handlerContext.ItemOwners
                .Where(e => e.OwnedItemId == id)
                .Select(res => new GetBinding
                {
                    UserId = res.IdUser,
                    Username = res.IdUserNavigation.Username + " " + res.IdUserNavigation.Surname,
                    Qty = res.Qty,
                    Price = res.OwnedItem.PurchasePrices.Select(e => e.Price).Average(),
                    Currency = currency,
                    InvoiceNumber = res.OwnedItem.Invoice.InvoiceNumber,
                    InvoiceId = res.OwnedItem.InvoiceId
                }).ToListAsync();

            }
            else
            {
                binding = await _handlerContext.ItemOwners
                    .Where(e => e.OwnedItemId == id)
                    .Select(res => new GetBinding
                    {
                        UserId = res.IdUser,
                        Username = res.IdUserNavigation.Username + " " + res.IdUserNavigation.Surname,
                        Qty = res.Qty,
                        Price = res.OwnedItem.PurchasePrices.SelectMany(e => e.CalculatedPrices).Where(e => e.CurrencyName == currency).Select(e => e.Price).Average(),
                        Currency = currency,
                        InvoiceNumber = res.OwnedItem.Invoice.InvoiceNumber,
                        InvoiceId = res.OwnedItem.InvoiceId
                    }).ToListAsync();

            }
            return binding;
        }
        /// <summary>
        /// Do select query to receive item description from database.
        /// </summary>
        /// <param name="id">Item id.</param>
        /// <returns>String that contains item description.</returns>
        public async Task<string> GetDescription(int id)
        {
            return await _handlerContext.Items.Where(e => e.ItemId == id).Select(e => e.ItemDescription).FirstAsync();
        }
        /// <summary>
        /// Do select query to receive list of existing items.
        /// </summary>
        /// <returns>List of <see cref="Models.DTOs.Get.GetItemList"/>.</returns>
        public async Task<IEnumerable<GetItemList>> GetItemList()
        {
            return await _handlerContext.Items
                .Select(e => new GetItemList
                {
                    Id = e.ItemId,
                    Partnumber = e.PartNumber,
                    Name = e.ItemName
                }).ToListAsync();
        }
        /// <summary>
        /// Do select query to receive list of existing items available to sell for chosen user. 
        /// </summary>
        /// <param name="userId">User id.</param>
        /// <param name="currency">Shortcut name of currency.</param>
        /// <returns>List of items with purchase price in given currency</returns>
        public async Task<IEnumerable<GetSalesItemList>> GetSalesItemList(int userId, string currency)
        {
            if (currency == "PLN")
            {
                return await _handlerContext.PurchasePrices
                        .Where(e => e.OwnedItem.ItemOwners.Any(d => d.IdUser == userId))
                        .Select(e => new GetSalesItemList
                        {
                            ItemId = e.OwnedItemId,
                            PriceId = e.PurchasePriceId,
                            InvoiceId = e.InvoiceId,
                            InvoiceNumber = e.OwnedItem.Invoice.InvoiceNumber,
                            Partnumber = e.OwnedItem.OriginalItem.PartNumber,
                            Name = e.OwnedItem.OriginalItem.ItemName,
                            Qty = e.Qty
                                - e.SellingPrices.Select(d => d.Qty).Sum()
                                + e.CreditNoteItems.Select(d => d.Qty).Sum()
                                - e.OwnedItem.ItemOwners.Where(d => d.InvoiceId == e.InvoiceId && d.OwnedItemId == e.OwnedItemId && d.IdUser != userId).Select(d => d.Qty).Sum(),
                            Price = e.Price
                        }).Union(
                            _handlerContext.CreditNoteItems
                                .Where(d => d.CreditNote.CreditNoteItems.Where(x => x.PurchasePriceId == d.PurchasePriceId).Select(x => x.Qty).Sum() != 0)
                                .Where(d => d.PurchasePrice.OwnedItem.ItemOwners.Any(x => x.IdUser == userId))
                                .Where(d => d.Qty > 0 && !d.CreditNote.Invoice.SellingPrices.Any())
                                .Select(d => new GetSalesItemList
                                {
                                    ItemId = d.PurchasePrice.OwnedItemId,
                                    PriceId = d.PurchasePrice.PurchasePriceId,
                                    InvoiceId = d.CreditNote.InvoiceId,
                                    InvoiceNumber = d.CreditNote.Invoice.InvoiceNumber,
                                    Partnumber = d.PurchasePrice.OwnedItem.OriginalItem.PartNumber,
                                    Name = d.PurchasePrice.OwnedItem.OriginalItem.ItemName,
                                    Qty = d.Qty,
                                    Price = d.NewPrice
                                })
                        )
                        .ToListAsync();
            }
            return await _handlerContext.PurchasePrices
                        .Where(e => e.OwnedItem.ItemOwners.Any(d => d.IdUser == userId))
                        .Select(e => new GetSalesItemList
                        {
                            ItemId = e.OwnedItemId,
                            PriceId = e.PurchasePriceId,
                            InvoiceId = e.InvoiceId,
                            InvoiceNumber = e.OwnedItem.Invoice.InvoiceNumber,
                            Partnumber = e.OwnedItem.OriginalItem.PartNumber,
                            Name = e.OwnedItem.OriginalItem.ItemName,
                            Qty = e.Qty
                                - e.SellingPrices.Select(d => d.Qty).Sum()
                                + e.CreditNoteItems.Select(d => d.Qty).Sum()
                                - e.OwnedItem.ItemOwners.Where(d => d.InvoiceId == e.InvoiceId && d.OwnedItemId == e.OwnedItemId && d.IdUser != userId).Select(d => d.Qty).Sum(),
                            Price = e.CalculatedPrices.Where(d => d.CurrencyName == currency).Select(d => d.Price).First()
                        }).Union(
                            _handlerContext.CreditNoteItems
                                .Where(d => d.CreditNote.CreditNoteItems.Where(x => x.PurchasePriceId == d.PurchasePriceId).Select(x => x.Qty).Sum() != 0)
                                .Where(d => d.PurchasePrice.OwnedItem.ItemOwners.Any(x => x.IdUser == userId))
                                .Where(d => d.Qty > 0 && !d.CreditNote.Invoice.SellingPrices.Any())
                                .Select(d => new GetSalesItemList
                                {
                                    ItemId = d.PurchasePrice.OwnedItemId,
                                    PriceId = d.PurchasePrice.PurchasePriceId,
                                    InvoiceId = d.CreditNote.InvoiceId,
                                    InvoiceNumber = d.CreditNote.Invoice.InvoiceNumber,
                                    Partnumber = d.PurchasePrice.OwnedItem.OriginalItem.PartNumber,
                                    Name = d.PurchasePrice.OwnedItem.OriginalItem.ItemName,
                                    Qty = d.Qty,
                                    Price = d.CalculatedCreditNotePrices.Where(x => x.CurrencyName == currency).Select(x => x.Price).First()
                                })
                        )
                        .ToListAsync();
        }
        /// <summary>
        /// Do select query to receive users that holds at least 1 qty of chosen item.
        /// </summary>
        /// <param name="itemId">Item id.</param>
        /// <returns>List of object containing user id, surname and name.</returns>
        public async Task<IEnumerable<GetUsers>> GetItemOwners(int itemId)
        {
            return await _handlerContext.AppUsers
                .Where(e =>
                    e.ItemOwners.Any(d => d.OwnedItemId == itemId && d.Qty > 0)
                    ||
                    e.Clients.SelectMany(d => d.OutsideItems).Any(d => d.ItemId == itemId)
                )
                .Select(e => new GetUsers
                {
                    IdUser = e.IdUser,
                    Surname = e.Surname,
                    Username = e.Username
                })
                .ToListAsync();
        }
        /// <summary>
        /// Using transactions change item bindings (Qty of item possessed by user etc.).
        /// </summary>
        /// <param name="data">New bindings data.</param>
        /// <returns>True if success or false if failure.</returns>
        public async Task<bool> ChangeBindings(IEnumerable<ModifyBinding> data)
        {
            using var trans = await _handlerContext.Database.BeginTransactionAsync();
            try
            {
                var groupedData = data.GroupBy(e => new { e.InvoiceId, e.ItemId, e.UserId }).Select(e => new
                {
                    e.Key.ItemId,
                    e.Key.InvoiceId,
                    e.Key.UserId,
                    toAdd = e.Sum(x => x.Qty)
                }).ToList();

                foreach (var group in groupedData)
                {
                    if (group.toAdd == 0)
                    {
                        continue;
                    }
                    var exist = await _handlerContext.ItemOwners.AnyAsync(x => x.InvoiceId == group.InvoiceId && x.OwnedItemId == group.ItemId && x.IdUser == group.UserId);
                    if (exist)
                    {
                        await _handlerContext.ItemOwners
                            .Where(x => x.InvoiceId == group.InvoiceId && x.OwnedItemId == group.ItemId && x.IdUser == group.UserId)
                            .ExecuteUpdateAsync(setters =>
                                setters.SetProperty(s => s.Qty, s => s.Qty + group.toAdd)
                            );
                    }
                    else
                    {
                        await _handlerContext.AddAsync<ItemOwner>(new ItemOwner
                        {
                            IdUser = group.UserId,
                            InvoiceId = group.InvoiceId,
                            OwnedItemId = group.ItemId,
                            Qty = group.toAdd
                        });
                    }
                }
                await _handlerContext.SaveChangesAsync();
                await trans.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Change item bindings error.");
                await trans.RollbackAsync();
                return false;
            }

        }
        /// <summary>
        /// Checks if item have existing relations that would for example withhold it deletion.
        /// </summary>
        /// <param name="itemId">Item id.</param>
        /// <returns>True if have, false if not.</returns>
        public async Task<bool> ItemHaveRelations(int itemId)
        {
            return await _handlerContext.Items.AnyAsync(x => x.ItemId == itemId && (x.OwnedItems.Any() || x.OfferItems.Any() || x.ProformaFutureItems.Any() || x.OutsideItems.Any()));
        }
    }
}
