using database_comunicator.Data;
using database_comunicator.Models;
using database_comunicator.Models.DTOs;
using database_comunicator.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Linq;

namespace database_comunicator.Services
{
    public interface IItemServices
    {
        public Task<Item?> AddItem(AddItem newItem);
        public Task UpdateItem(UpdateItem postItem);
        public Task<bool> RemoveItem(int id);
        public Task<bool> ItemExist(int id);
        public Task<bool> ItemExist(string partNumber);
        public Task<bool> EanExist(IEnumerable<string> eans);
        public Task<IEnumerable<GetManyItems>> GetItems(string currency);
        public Task<IEnumerable<GetManyItems>> GetItems(string currency, string search);
        public Task<IEnumerable<GetManyItems>> GetItems(string currency, int userId);
        public Task<IEnumerable<GetManyItems>> GetItems(string currency, int userId, string search);
        public Task<GetRestInfo> GetRestOfItem(int id, int userId, string currency);
        public Task<GetRestInfo> GetRestOfItemOrg(int id, string currency);
        public Task<IEnumerable<GetBinding>> GetModifyRestOfItem(int id, string currency);
        public Task<string> GetDescription(int id);
        public Task<IEnumerable<GetItemList>> GetItemList();
        public Task<IEnumerable<GetSalesItemList>> GetSalesItemList(int userId, string currency);
        public Task<IEnumerable<GetUsers>> GetItemOwners(int itemId);
    }
    public class ItemServices : IItemServices
    {
        private readonly HandlerContext _handlerContext;
        public ItemServices(HandlerContext handlerContext)
        {
            _handlerContext = handlerContext;
        }

        public async Task<Item?> AddItem(AddItem newItem)
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
                return item;
            } catch (Exception)
            {
                await trans.RollbackAsync();
                return null;
            }
        }

        public async Task<bool> EanExist(IEnumerable<string> eans)
        {
            return await _handlerContext.Eans.Where(e => eans.Contains(e.EanValue)).AnyAsync();
        }

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
                .Where(e => e.ItemId == id && e.Organization.AppUsers.Where(d => d.IdUser == userId).Any())
                .Include(e => e.Organization)
                    .ThenInclude(e => e.AppUsers)
                .Select(obj => new GetRestInfoOutsideItems
                {
                    Users = obj.Organization.AppUsers.Select(e => new KeyValuePair<int, string>(e.IdUser, e.Username)).ToList(),
                    OrganizationName = obj.Organization.OrgName,
                    Qty = obj.Qty,
                    Price = obj.PurchasePrice,
                    Curenncy = obj.CurrencyName
                }).ToListAsync();

            return new GetRestInfo
            {
                OutsideItemInfos = outsideItems,
                OwnedItemInfos = result
            };

        }
        public async Task<GetRestInfo> GetRestOfItemOrg(int id, string currency)
        {
            List<GetRestInfoOwnedItems> result;
            if (currency == "PLN")
            {
                result = await _handlerContext.ItemOwners
                    .Where(e => e.OwnedItemId == id)
                    .Include(e => e.IdUserNavigation)
                    .Include (e => e.OwnedItem)
                        .ThenInclude(e => e.Invoice)
                            .ThenInclude(e => e.SellerNavigation)
                    .Include(e => e.OwnedItem)
                        .ThenInclude(e => e.PurchasePrices)
                    .Where(e => e.Qty > 0)
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
            } else
            {
                result = await _handlerContext.ItemOwners
                    .Where(e => e.OwnedItemId == id)
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
                .Include(e => e.Organization)
                    .ThenInclude(e => e.AppUsers)
                .Select(obj => new GetRestInfoOutsideItems
                {
                    Users = obj.Organization.AppUsers.Select(e => new KeyValuePair<int, string>(e.IdUser, e.Username)).ToList(),
                    OrganizationName = obj.Organization.OrgName,
                    Qty = obj.Qty,
                    Price = obj.PurchasePrice,
                    Curenncy = obj.CurrencyName
                }).ToListAsync();

            return new GetRestInfo
            {
                OutsideItemInfos = outsideItems,
                OwnedItemInfos = result
            };
        }
        public async Task<IEnumerable<GetManyItems>> GetItems(string currency)
        {
            List<GetManyItems> items;

            if (currency == "PLN")
            {
                items = await _handlerContext.Items
                .Include(ent => ent.Eans)
                .Include(ent => ent.OwnedItems)
                    .ThenInclude(c => c.Invoice)
                        .ThenInclude(h => h.Deliveries)
                .Include(ent => ent.OwnedItems)
                    .ThenInclude(k => k.Invoice)
                        .ThenInclude(h => h.SellerNavigation)
                .Include(ent => ent.OwnedItems)
                    .ThenInclude(e => e.PurchasePrices)
                        .ThenInclude(e => e.CalculatedPrices)
                .Include(ent => ent.OwnedItems)
                    .ThenInclude(g => g.ItemOwners)
                        .ThenInclude(k => k.IdUserNavigation)
                .Include(ent => ent.OutsideItems)
                .Select(instance => new GetManyItems
                {
                    Users = instance.OwnedItems.SelectMany(e => e.ItemOwners).Select(e => e.IdUserNavigation)
                        .GroupBy(e => new { e.Username, e.Surname })
                        .Select(e => e.Key.Username + " " + e.Key.Surname).ToList(),
                    ItemId = instance.ItemId,
                    ItemName = instance.ItemName,
                    PartNumber = instance.PartNumber,
                    StatusName = WarehouseUtils.getItemStatus(
                            instance.OwnedItems
                                .SelectMany(e => e.PurchasePrices)
                                .Select(e => e.Qty).Sum()
                                - instance.OwnedItems.SelectMany(e => e.PurchasePrices).SelectMany(e => e.SellingPrices).Select(d => d.Qty).Sum()
                                + instance.OwnedItems.SelectMany(e => e.PurchasePrices).SelectMany(e => e.CreditNoteItems)
                                    .Where(d => d.IncludeQty == true).Select(d => d.Qty).Sum(),
                            instance.OutsideItems.Select(e => e.Qty).Sum(),
                            instance.OwnedItems.SelectMany(e => e.Invoice.Deliveries).Any()
                        ),
                    Eans = instance.Eans.Select(e => e.EanValue),
                    Qty = instance.OwnedItems
                    .SelectMany(e => e.PurchasePrices)
                    .Select(e => e.Qty).Sum()
                    - instance.OwnedItems.SelectMany(e => e.PurchasePrices).SelectMany(e => e.SellingPrices).Select(d => d.Qty).Sum()
                    + instance.OwnedItems.SelectMany(e => e.PurchasePrices).SelectMany(e => e.CreditNoteItems).Where(d => d.IncludeQty == true).Select(d => d.Qty).Sum(),
                    PurchasePrice = instance.OwnedItems
                        .SelectMany(e => e.PurchasePrices)
                        .Where(e => 
                        e.Qty 
                        - e.SellingPrices.Select(d => d.Qty).Sum()
                        + e.CreditNoteItems.Where(d  => d.IncludeQty == true).Select(d => d.Qty).Sum()
                        > 0
                        )
                        .Select(e => e.Price)
                        .Union(
                            instance.OwnedItems
                            .SelectMany(e => e.PurchasePrices)
                            .SelectMany(e => e.CreditNoteItems)
                            .Where(e => e.Qty != e.PurchasePrice.Qty && e.IncludeQty == true)
                            .Select(e => e.NewPrice)
                        )
                        .Average(),
                    Sources = instance.OwnedItems.Select(e => e.Invoice).Select(e => e.SellerNavigation).GroupBy(e => e.OrgName).Select(e => e.Key).ToList()
                })
                .OrderByDescending(e => e.Qty)
                .ToListAsync();
            } else
            {
                items = await _handlerContext.Items
                .Include(ent => ent.Eans)
                .Include(ent => ent.OwnedItems)
                    .ThenInclude(c => c.Invoice)
                        .ThenInclude(h => h.Deliveries)
                .Include(ent => ent.OwnedItems)
                    .ThenInclude(k => k.Invoice)
                        .ThenInclude(h => h.SellerNavigation)
                .Include(ent => ent.OwnedItems)
                    .ThenInclude(e => e.PurchasePrices)
                        .ThenInclude(e => e.CalculatedPrices)
                .Include(ent => ent.OwnedItems)
                    .ThenInclude(g => g.ItemOwners)
                        .ThenInclude(k => k.IdUserNavigation)
                .Include(ent => ent.OutsideItems)
                .Select(instance => new GetManyItems
                {
                    Users = instance.OwnedItems
                        .SelectMany(e => e.ItemOwners)
                        .Select(e => e.IdUserNavigation)
                        .GroupBy(e => new { e.Username, e.Surname })
                        .Select(e => e.Key.Username + " " + e.Key.Surname).ToList(),
                    ItemId = instance.ItemId,
                    ItemName = instance.ItemName,
                    PartNumber = instance.PartNumber,
                    StatusName = WarehouseUtils.getItemStatus(
                            instance.OwnedItems
                                .SelectMany(e => e.PurchasePrices)
                                .Select(e => e.Qty).Sum()
                                - instance.OwnedItems.SelectMany(e => e.PurchasePrices).SelectMany(e => e.SellingPrices).Select(d => d.Qty).Sum()
                                + instance.OwnedItems.SelectMany(e => e.PurchasePrices).SelectMany(e => e.CreditNoteItems)
                                    .Where(d => d.IncludeQty == true).Select(d => d.Qty).Sum(),
                            instance.OutsideItems.Select(e => e.Qty).Sum(),
                            instance.OwnedItems.SelectMany(e => e.Invoice.Deliveries).Any()
                        ),
                    Eans = instance.Eans.Select(e => e.EanValue),
                    Qty = instance.OwnedItems
                        .SelectMany(e => e.PurchasePrices)
                        .Select(e => e.Qty).Sum()
                        - instance.OwnedItems.SelectMany(e => e.PurchasePrices).SelectMany(e => e.SellingPrices).Select(d => d.Qty).Sum()
                        + instance.OwnedItems.SelectMany(e => e.PurchasePrices).SelectMany(e => e.CreditNoteItems).Where(d => d.IncludeQty == true).Select(d => d.Qty).Sum(),
                    PurchasePrice = instance.OwnedItems
                        .SelectMany(e => e.PurchasePrices)
                        .Where(e =>
                        e.Qty
                        - e.SellingPrices.Select(d => d.Qty).Sum()
                        + e.CreditNoteItems.Where(d => d.IncludeQty).Select(d => d.Qty).Sum()
                        > 0
                        ).SelectMany(e => e.CalculatedPrices)
                        .Where(e => e.CurrencyName == currency)
                        .Select(e => e.Price)
                        .Union(
                            instance.OwnedItems
                            .SelectMany(e => e.PurchasePrices)
                            .SelectMany(e => e.CreditNoteItems)
                            .Where(e => e.Qty != e.PurchasePrice.Qty && e.IncludeQty == true)
                            .SelectMany(e => e.CalculatedCreditNotePrices)
                            .Where(e => e.CurrencyName == currency)
                            .Select(e => e.Price)
                        )
                        .Average(),
                    Sources = instance.OwnedItems.Select(e => e.Invoice).Select(e => e.SellerNavigation).GroupBy(e => e.OrgName).Select(e => e.Key).ToList()
                })
                .OrderByDescending(e => e.Qty)
                .ToListAsync();
            }
                
            return items;
        }
        public async Task<IEnumerable<GetManyItems>> GetItems(string currency, string search)
        {
            List<GetManyItems> items;
            if (currency == "PLN")
            {
                items = await _handlerContext.Items
                    .Where(j => EF.Functions.FreeText(j.PartNumber, search) || EF.Functions.FreeText(j.ItemName, search))
                    .Include(entit => entit.Eans)
                    .Include(entit => entit.OwnedItems)
                        .ThenInclude(c => c.Invoice)
                            .ThenInclude(h => h.Deliveries)
                    .Include(entit => entit.OwnedItems)
                        .ThenInclude(k => k.Invoice)
                            .ThenInclude(h => h.SellerNavigation)
                    .Include(entit => entit.OwnedItems)
                        .ThenInclude(e => e.PurchasePrices)
                            .ThenInclude(e => e.CalculatedPrices)
                    .Include(entit => entit.OwnedItems)
                        .ThenInclude(g => g.ItemOwners)
                            .ThenInclude(k => k.IdUserNavigation)
                    .Include(entit => entit.OutsideItems)
                    .Select(instc => new GetManyItems
                    {
                        Users = instc.OwnedItems.SelectMany(e => e.ItemOwners).Select(e => e.IdUserNavigation)
                        .GroupBy(e => new {e.Username, e.Surname})
                        .Select(e => e.Key.Username + " " + e.Key.Surname).ToList(),
                        ItemId = instc.ItemId,
                        ItemName = instc.ItemName,
                        PartNumber = instc.PartNumber,
                        StatusName = WarehouseUtils.getItemStatus(
                                instc.OwnedItems
                                    .SelectMany(e => e.PurchasePrices)
                                    .Select(e =>
                                        e.Qty
                                        - e.SellingPrices.Select(d => d.Qty).Sum()
                                        + e.CreditNoteItems.Where(d => d.IncludeQty == true).Select(d => d.Qty).Sum()
                                    )
                                    .Sum(),
                                instc.OutsideItems.Select(e => e.Qty).Sum(),
                                instc.OwnedItems.SelectMany(e => e.Invoice.Deliveries).Any()
                            ),
                        Eans = instc.Eans.Select(e => e.EanValue),
                        Qty = instc.OwnedItems
                            .SelectMany(e => e.PurchasePrices)
                            .Select(e =>
                                e.Qty
                                - e.SellingPrices.Select(d => d.Qty).Sum()
                                + e.CreditNoteItems.Where(d => d.IncludeQty == true).Select(d => d.Qty).Sum()
                            )
                            .Sum(),
                        PurchasePrice = instc.OwnedItems
                            .SelectMany(e => e.PurchasePrices)
                            .Where(e =>
                            e.Qty
                            - e.SellingPrices.Select(d => d.Qty).Sum()
                            + e.CreditNoteItems.Where(d => d.IncludeQty == true).Select(d => d.Qty).Sum()
                            > 0
                            ).SelectMany(e => e.CalculatedPrices)
                            .Where(e => e.CurrencyName == currency)
                            .Select(e => e.Price)
                            .Union(
                                instc.OwnedItems
                                .SelectMany(e => e.PurchasePrices)
                                .SelectMany(e => e.CreditNoteItems)
                                .Where(e => e.Qty != e.PurchasePrice.Qty && e.IncludeQty == true)
                                .SelectMany(e => e.CalculatedCreditNotePrices)
                                .Where(e => e.CurrencyName == currency)
                                .Select(e => e.Price)
                            )
                            .Average(),
                        Sources = instc.OwnedItems.Select(e => e.Invoice).Select(e => e.SellerNavigation).GroupBy(e => e.OrgName).Select(e => e.Key).ToList()
                    })
                    .OrderByDescending(e => e.Qty)
                    .ToListAsync();
            } else
            {
                items = await _handlerContext.Items
                    .Where(j => EF.Functions.FreeText(j.PartNumber, search) || EF.Functions.FreeText(j.ItemName, search))
                    .Include(entit => entit.Eans)
                    .Include(entit => entit.OwnedItems)
                        .ThenInclude(c => c.Invoice)
                            .ThenInclude(h => h.Deliveries)
                    .Include(entit => entit.OwnedItems)
                        .ThenInclude(k => k.Invoice)
                            .ThenInclude(h => h.SellerNavigation)
                    .Include(entit => entit.OwnedItems)
                        .ThenInclude(e => e.PurchasePrices)
                            .ThenInclude(e => e.CalculatedPrices)
                    .Include(entit => entit.OwnedItems)
                        .ThenInclude(g => g.ItemOwners)
                            .ThenInclude(k => k.IdUserNavigation)
                    .Include(entit => entit.OutsideItems)
                    .Select(instc => new GetManyItems
                    {
                        Users = instc.OwnedItems.SelectMany(e => e.ItemOwners).Select(e => e.IdUserNavigation)
                        .GroupBy(e => new { e.Username, e.Surname })
                        .Select(e => e.Key.Username + " " + e.Key.Surname).ToList(),
                        ItemId = instc.ItemId,
                        ItemName = instc.ItemName,
                        PartNumber = instc.PartNumber,
                        StatusName = WarehouseUtils.getItemStatus(
                                instc.OwnedItems
                                    .SelectMany(e => e.PurchasePrices)
                                    .Select(e =>
                                        e.Qty
                                        - e.SellingPrices.Select(d => d.Qty).Sum()
                                        + e.CreditNoteItems.Where(d => d.IncludeQty == true).Select(d => d.Qty).Sum()
                                    )
                                    .Sum(),
                                instc.OutsideItems.Select(e => e.Qty).Sum(),
                                instc.OwnedItems.SelectMany(e => e.Invoice.Deliveries).Any()
                            ),
                        Eans = instc.Eans.Select(e => e.EanValue),
                        Qty = instc.OwnedItems
                            .SelectMany(e => e.PurchasePrices)
                            .Select(e =>
                                e.Qty
                                - e.SellingPrices.Select(d => d.Qty).Sum()
                                + e.CreditNoteItems.Where(d => d.IncludeQty == true).Select(d => d.Qty).Sum()
                            )
                            .Sum(),
                        PurchasePrice = instc.OwnedItems
                            .SelectMany(e => e.PurchasePrices)
                            .Where(e =>
                            e.Qty
                            - e.SellingPrices.Select(d => d.Qty).Sum()
                            + e.CreditNoteItems.Select(d => d.Qty).Sum()
                            + e.CreditNoteItems.Where(d => d.Qty > 0 && e.SellingPrices.Select(f => f.SellInvoiceId).Contains(d.CreditNote.InvoiceId)).Select(d => d.Qty).Sum()
                            > 0
                            ).SelectMany(e => e.CalculatedPrices)
                            .Where(e => e.CurrencyName == currency)
                            .Select(e => e.Price)
                            .Union(
                                instc.OwnedItems
                                .SelectMany(e => e.PurchasePrices)
                                .SelectMany(e => e.CreditNoteItems)
                                .Where(e => e.Qty != e.PurchasePrice.Qty && e.IncludeQty == true)
                                .SelectMany(e => e.CalculatedCreditNotePrices)
                                .Where(e => e.CurrencyName == currency)
                                .Select(e => e.Price)
                            )
                            .Average(),
                        Sources = instc.OwnedItems.Select(e => e.Invoice).Select(e => e.SellerNavigation).GroupBy(e => e.OrgName).Select(e => e.Key).ToList()
                    })
                    .OrderByDescending(e => e.Qty)
                    .ToListAsync();
            }
            return items;
        }
        public async Task<IEnumerable<GetManyItems>> GetItems(string currency, int userId)
        {
            List<GetManyItems> items;
            if (currency == "PLN")
            {
                items = await _handlerContext.Items
                    .Include(tmp => tmp.Eans)
                    .Include(tmp => tmp.OwnedItems)
                        .ThenInclude(c => c.Invoice)
                            .ThenInclude(h => h.Deliveries)
                    .Include(tmp => tmp.OwnedItems)
                        .ThenInclude(k => k.Invoice)
                            .ThenInclude(h => h.SellerNavigation)
                    .Include(tmp => tmp.OwnedItems)
                        .ThenInclude(e => e.PurchasePrices)
                            .ThenInclude(e => e.CalculatedPrices)
                    .Include(tmp => tmp.OwnedItems)
                        .ThenInclude(g => g.ItemOwners)
                    .Include(tmp => tmp.OutsideItems)
                    .Select(inst => new GetManyItems
                    {
                        ItemId = inst.ItemId,
                        ItemName = inst.ItemName,
                        PartNumber = inst.PartNumber,
                        StatusName = WarehouseUtils.getItemStatus(
                                inst.OwnedItems.SelectMany(e => e.ItemOwners).Where(e => e.IdUser == userId).Select(e => e.Qty).Sum(),
                                inst.OutsideItems.Select(e => e.Qty).Sum(),
                                inst.OwnedItems.Where(e => e.ItemOwners.Select(e => e.IdUser).Contains(userId)).SelectMany(e => e.Invoice.Deliveries).Any()
                            ),
                        Eans = inst.Eans.Select(e => e.EanValue),
                        Qty = inst.OwnedItems
                            .SelectMany(e => e.ItemOwners)
                            .Where(e => e.IdUser == userId)
                            .Select(e => e.Qty).Sum(),
                        PurchasePrice = inst.OwnedItems
                            .SelectMany(e => e.PurchasePrices)
                            .Where(e =>
                            e.Qty
                            - e.SellingPrices.Select(d => d.Qty).Sum()
                            + e.CreditNoteItems.Select(d => d.Qty).Sum()
                            + e.CreditNoteItems.Where(d => d.Qty > 0 && e.SellingPrices.Select(f => f.SellInvoiceId).Contains(d.CreditNote.InvoiceId)).Select(d => d.Qty).Sum()
                            > 0
                            )
                            .Select(e => e.Price)
                            .Union(
                                inst.OwnedItems
                                .SelectMany(e => e.PurchasePrices)
                                .SelectMany(e => e.CreditNoteItems)
                                .Where(e => e.Qty != e.PurchasePrice.Qty && e.IncludeQty == true)
                                .Select(e => e.NewPrice)
                            )
                            .Average(),
                        Sources = inst.OwnedItems.Select(e => e.Invoice).Select(e => e.SellerNavigation).GroupBy(e => e.OrgName).Select(e => e.Key).ToList()
                    })
                    .OrderByDescending(e => e.Qty)
                    .ToListAsync();
            } else
            {
                items = await _handlerContext.Items
                    .Include(tmp => tmp.Eans)
                    .Include(tmp => tmp.OwnedItems)
                        .ThenInclude(c => c.Invoice)
                            .ThenInclude(h => h.Deliveries)
                    .Include(tmp => tmp.OwnedItems)
                        .ThenInclude(k => k.Invoice)
                            .ThenInclude(h => h.SellerNavigation)
                    .Include(tmp => tmp.OwnedItems)
                        .ThenInclude(e => e.PurchasePrices)
                            .ThenInclude(e => e.CalculatedPrices)
                    .Include(tmp => tmp.OutsideItems)
                    .Select(inst => new GetManyItems
                    {
                        ItemId = inst.ItemId,
                        ItemName = inst.ItemName,
                        PartNumber = inst.PartNumber,
                        StatusName = WarehouseUtils.getItemStatus(
                                inst.OwnedItems.SelectMany(e => e.ItemOwners).Where(e => e.IdUser == userId).Select(e => e.Qty).Sum(),
                                inst.OutsideItems.Select(e => e.Qty).Sum(),
                                inst.OwnedItems.Where(e => e.ItemOwners.Select(e => e.IdUser).Contains(userId)).SelectMany(e => e.Invoice.Deliveries).Any()
                            ),
                        Eans = inst.Eans.Select(e => e.EanValue),
                        Qty = inst.OwnedItems
                            .SelectMany(e => e.ItemOwners)
                            .Where(e => e.IdUser == userId)
                            .Select(e => e.Qty).Sum(),
                        PurchasePrice = inst.OwnedItems
                            .SelectMany(e => e.PurchasePrices)
                            .Where(e =>
                            e.Qty
                            - e.SellingPrices.Select(d => d.Qty).Sum()
                            + e.CreditNoteItems.Select(d => d.Qty).Sum()
                            + e.CreditNoteItems.Where(d => d.Qty > 0 && e.SellingPrices.Select(f => f.SellInvoiceId).Contains(d.CreditNote.InvoiceId)).Select(d => d.Qty).Sum()
                            > 0
                            ).SelectMany(e => e.CalculatedPrices)
                            .Where(e => e.CurrencyName == currency)
                            .Select(e => e.Price)
                            .Union(
                                inst.OwnedItems
                                .SelectMany(e => e.PurchasePrices)
                                .SelectMany(e => e.CreditNoteItems)
                                .Where(e => e.Qty != e.PurchasePrice.Qty && e.IncludeQty == true)
                                .SelectMany(e => e.CalculatedCreditNotePrices)
                                .Where(e => e.CurrencyName == currency)
                                .Select(e => e.Price)
                            )
                            .Average(),
                        Sources = inst.OwnedItems.Select(e => e.Invoice).Select(e => e.SellerNavigation).GroupBy(e => e.OrgName).Select(e => e.Key).ToList()
                    })
                    .OrderByDescending(e => e.Qty)
                    .ToListAsync();
            }
            return items;
        }
        public async Task<IEnumerable<GetManyItems>> GetItems(string currency, int userId, string search)
        {
            List<GetManyItems> items;
            if (currency == "PLN")
            {
                items = await _handlerContext.Items
                    .Where(j => EF.Functions.FreeText(j.PartNumber, search) || EF.Functions.FreeText(j.ItemName, search))
                    .Include(tab => tab.Eans)
                    .Include(tab => tab.OwnedItems)
                        .ThenInclude(c => c.Invoice)
                            .ThenInclude(h => h.Deliveries)
                    .Include(tab => tab.OwnedItems)
                        .ThenInclude(k => k.Invoice)
                            .ThenInclude(h => h.SellerNavigation)
                    .Include(tab => tab.OwnedItems)
                        .ThenInclude(e => e.PurchasePrices)
                            .ThenInclude(e => e.CalculatedPrices)
                    .Include(tab => tab.OwnedItems)
                        .ThenInclude(g => g.ItemOwners.Where(own => own.IdUser == userId))
                    .Include(tab => tab.OutsideItems)
                    .Select(obj => new GetManyItems
                    {
                        ItemId = obj.ItemId,
                        ItemName = obj.ItemName,
                        PartNumber = obj.PartNumber,
                        StatusName = WarehouseUtils.getItemStatus(
                                obj.OwnedItems.SelectMany(e => e.ItemOwners).Where(e => e.IdUser == userId).Select(e => e.Qty).Sum(),
                                obj.OutsideItems.Select(e => e.Qty).Sum(),
                                obj.OwnedItems.SelectMany(e => e.Invoice.Deliveries).Any()
                            ),
                        Eans = obj.Eans.Select(e => e.EanValue),
                        Qty = obj.OwnedItems
                            .SelectMany(e => e.ItemOwners)
                            .Where(e => e.IdUser == userId)
                            .Select(e => e.Qty).Sum(),
                        PurchasePrice = obj.OwnedItems
                            .SelectMany(e => e.PurchasePrices)
                            .Where(e =>
                            e.Qty
                            - e.SellingPrices.Select(d => d.Qty).Sum()
                            + e.CreditNoteItems.Select(d => d.Qty).Sum()
                            + e.CreditNoteItems.Where(d => d.Qty > 0 && e.SellingPrices.Select(f => f.SellInvoiceId).Contains(d.CreditNote.InvoiceId)).Select(d => d.Qty).Sum()
                            > 0
                            ).SelectMany(e => e.CalculatedPrices)
                            .Where(e => e.CurrencyName == currency)
                            .Select(e => e.Price)
                            .Union(
                                obj.OwnedItems
                                .SelectMany(e => e.PurchasePrices)
                                .SelectMany(e => e.CreditNoteItems)
                                .Where(e => e.Qty != e.PurchasePrice.Qty && e.IncludeQty == true)
                                .SelectMany(e => e.CalculatedCreditNotePrices)
                                .Where(e => e.CurrencyName == currency)
                                .Select(e => e.Price)
                            )
                            .Average(),
                        Sources = obj.OwnedItems.Select(e => e.Invoice).Select(e => e.SellerNavigation).GroupBy(e => e.OrgName).Select(e => e.Key).ToList()
                    })
                    .OrderByDescending(e => e.Qty)
                    .ToListAsync();
            } else
            {
                items = await _handlerContext.Items
                    .Where(j => EF.Functions.FreeText(j.PartNumber, search) || EF.Functions.FreeText(j.ItemName, search))
                    .Include(tab => tab.Eans)
                    .Include(tab => tab.OwnedItems)
                        .ThenInclude(c => c.Invoice)
                            .ThenInclude(h => h.Deliveries)
                    .Include(tab => tab.OwnedItems)
                        .ThenInclude(k => k.Invoice)
                            .ThenInclude(h => h.SellerNavigation)
                    .Include(tab => tab.OwnedItems)
                        .ThenInclude(e => e.PurchasePrices)
                            .ThenInclude(e => e.CalculatedPrices)
                    .Include(tab => tab.OwnedItems)
                        .ThenInclude(g => g.ItemOwners.Where(own => own.IdUser == userId))
                    .Include(tab => tab.OutsideItems)
                    .Select(obj => new GetManyItems
                    {
                        ItemId = obj.ItemId,
                        ItemName = obj.ItemName,
                        PartNumber = obj.PartNumber,
                        StatusName = WarehouseUtils.getItemStatus(
                                obj.OwnedItems.SelectMany(e => e.ItemOwners).Where(e => e.IdUser == userId).Select(e => e.Qty).Sum(),
                                obj.OutsideItems.Select(e => e.Qty).Sum(),
                                obj.OwnedItems.Where(e => e.ItemOwners.Select(e => e.IdUser).Contains(userId)).SelectMany(e => e.Invoice.Deliveries).Any()
                            ),
                        Eans = obj.Eans.Select(e => e.EanValue),
                        Qty = obj.OwnedItems
                            .SelectMany(e => e.ItemOwners)
                            .Where(e => e.IdUser == userId)
                            .Select(e => e.Qty).Sum(),
                        PurchasePrice = obj.OwnedItems
                            .SelectMany(e => e.PurchasePrices)
                            .Where(e =>
                            e.Qty
                            - e.SellingPrices.Select(d => d.Qty).Sum()
                            + e.CreditNoteItems.Select(d => d.Qty).Sum()
                            + e.CreditNoteItems.Where(d => d.Qty > 0 && e.SellingPrices.Select(f => f.SellInvoiceId).Contains(d.CreditNote.InvoiceId)).Select(d => d.Qty).Sum()
                            > 0
                            ).SelectMany(e => e.CalculatedPrices)
                            .Where(e => e.CurrencyName == currency)
                            .Select(e => e.Price)
                            .Union(
                                obj.OwnedItems
                                .SelectMany(e => e.PurchasePrices)
                                .SelectMany(e => e.CreditNoteItems)
                                .Where(e => e.Qty != e.PurchasePrice.Qty && e.IncludeQty == true)
                                .SelectMany(e => e.CalculatedCreditNotePrices)
                                .Where(e => e.CurrencyName == currency)
                                .Select(e => e.Price)
                            )
                            .Average(),
                        Sources = obj.OwnedItems.Select(e => e.Invoice).Select(e => e.SellerNavigation).GroupBy(e => e.OrgName).Select(e => e.Key).ToList()
                    })
                    .OrderByDescending(e => e.Qty)
                    .ToListAsync();
            }

            return items;
        }

        public async Task<bool> ItemExist(int id)
        {
            return await _handlerContext.Items.Where(e => e.ItemId == id).AnyAsync();
        }

        public async Task<bool> ItemExist(string partNumber)
        {
            return await _handlerContext.Items.Where(e => e.PartNumber == partNumber).AnyAsync();
        }

        public async Task<bool> RemoveItem(int id)
        {
            var ownedExist = await _handlerContext.OwnedItems.Where(e => e.OwnedItemId == id).AnyAsync();

            if (ownedExist) return false;

            var eansToRemove = await _handlerContext.Eans.Where(e => e.ItemId == id).ToArrayAsync();

            _handlerContext.Eans.RemoveRange(eansToRemove);

            _handlerContext.Remove<Item>( new Item
            {
                ItemId = id
            });

            await _handlerContext.SaveChangesAsync();

            return true;
        }

        public async Task UpdateItem(UpdateItem postItem)
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

            var upadtedItem = new Item
            {
                ItemId = postItem.Id,
                ItemName = postItem.ItemName,
                ItemDescription = postItem.ItemDescription,
                PartNumber = postItem.PartNumber
            };

            _handlerContext.Update<Item>( upadtedItem );

            if (postItem.ItemName == null)
            {
                _handlerContext.Entry(upadtedItem).Property("ItemName").IsModified = false;
            }

            if (postItem.ItemDescription == null)
            {
                _handlerContext.Entry(upadtedItem).Property("ItemDescription").IsModified = false;
            }

            if (postItem.PartNumber == null)
            {
                _handlerContext.Entry(upadtedItem).Property("PartNumber").IsModified = false;
            }

            await _handlerContext.SaveChangesAsync();

        }
        public async Task<IEnumerable<GetBinding>> GetModifyRestOfItem(int id, string currency)
        {
            List<GetBinding> binding;
            List<GetBinding> noUserItem;
            if (currency == "PLN")
            {
                binding = await _handlerContext.ItemOwners
                .Where(e => e.OwnedItemId == id)
                .Include(e => e.IdUserNavigation)
                .Include(e => e.OwnedItem)
                    .ThenInclude(e => e.Invoice)
                .Include(e => e.OwnedItem)
                    .ThenInclude(e => e.PurchasePrices)
                .Select(res => new GetBinding
                {
                    UserId = res.IdUser,
                    Username = res.IdUserNavigation.Username + " " + res.IdUserNavigation.Surname,
                    Qty = res.Qty,
                    Price = res.OwnedItem.PurchasePrices.Select(e => e.Price).Average(),
                    Currency = currency,
                    InvoiceNumber = res.OwnedItem.Invoice.InvoiceNumber

                }).ToListAsync();

                noUserItem = await _handlerContext.OwnedItems
                .Where(e => e.OwnedItemId == id)
                .Include(e => e.Invoice)
                .Include(e => e.PurchasePrices)
                .Include(e => e.ItemOwners)
                .Select(inst => new GetBinding
                {
                    UserId = null,
                    Username = null,
                    Qty = inst.PurchasePrices.Select(e => e.Qty).Sum() 
                        - inst.PurchasePrices.SelectMany(e => e.SellingPrices).Select(e => e.Qty).Sum()
                        + inst.PurchasePrices.SelectMany(e => e.CreditNoteItems).Where(e => e.Qty < 0).Select(e => e.Qty).Sum()
                        + inst.PurchasePrices.SelectMany(e => e.CreditNoteItems)
                            .Where(e => e.Qty > 0 && inst.PurchasePrices
                                .SelectMany(f => f.SellingPrices).Select(f => f.SellInvoiceId).Contains(e.CreditNote.InvoiceId)).Select(e => e.Qty).Sum()
                        - inst.ItemOwners.Select(e => e.Qty).Sum(),
                    Price = inst.PurchasePrices.Select(e => e.Price).Average(),
                    Currency = currency,
                    InvoiceNumber = inst.Invoice.InvoiceNumber
                })
                .Where(e => e.Qty > 0)
                .ToListAsync();
            } else
            {
                binding = await _handlerContext.ItemOwners
                    .Where(e => e.OwnedItemId == id)
                    .Include(e => e.IdUserNavigation)
                    .Include(e => e.OwnedItem)
                        .ThenInclude(e => e.Invoice)
                    .Include(e => e.OwnedItem)
                        .ThenInclude(e => e.PurchasePrices)
                    .Select(res => new GetBinding
                    {
                        UserId = res.IdUser,
                        Username = res.IdUserNavigation.Username + " " + res.IdUserNavigation.Surname,
                        Qty = res.Qty,
                        Price = res.OwnedItem.PurchasePrices.SelectMany(e => e.CalculatedPrices).Where(e => e.CurrencyName == currency).Select(e => e.Price).Average(),
                        Currency = currency,
                        InvoiceNumber = res.OwnedItem.Invoice.InvoiceNumber

                    }).ToListAsync();

                noUserItem = await _handlerContext.OwnedItems
                    .Where(e => e.OwnedItemId == id)
                    .Include(e => e.Invoice)
                    .Include(e => e.PurchasePrices)
                    .Include(e => e.ItemOwners)
                    .Select(inst => new GetBinding
                    {
                        UserId = null,
                        Username = null,
                        Qty = inst.PurchasePrices.Select(e => e.Qty).Sum()
                        - inst.PurchasePrices.SelectMany(e => e.SellingPrices).Select(e => e.Qty).Sum()
                        + inst.PurchasePrices.SelectMany(e => e.CreditNoteItems).Where(e => e.Qty < 0).Select(e => e.Qty).Sum()
                        + inst.PurchasePrices.SelectMany(e => e.CreditNoteItems)
                            .Where(e => e.Qty > 0 && inst.PurchasePrices
                                .SelectMany(f => f.SellingPrices).Select(f => f.SellInvoiceId).Contains(e.CreditNote.InvoiceId)).Select(e => e.Qty).Sum()
                        - inst.ItemOwners.Select(e => e.Qty).Sum(),
                        Price = inst.PurchasePrices.SelectMany(e => e.CalculatedPrices).Where(e => e.CurrencyName == currency).Select(e => e.Price).Average(),
                        Currency = currency,
                        InvoiceNumber = inst.Invoice.InvoiceNumber
                    })
                    .Where(e => e.Qty > 0)
                    .ToListAsync();
            }

            binding.AddRange(noUserItem);
            return binding;
        }
        public async Task<string> GetDescription(int id)
        {
            var result = await _handlerContext.Items.Where(e => e.ItemId == id).Select(e => e.ItemDescription).ToListAsync();
            return result[0];
        }
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
        public async Task<IEnumerable<GetSalesItemList>> GetSalesItemList(int userId, string currency)
        {
            if (currency == "PLN")
            {
                return await _handlerContext.PurchasePrices
                        .Include(e => e.OwnedItem)
                            .ThenInclude(e => e.Invoice)
                        .Include(e => e.OwnedItem)
                            .ThenInclude(e => e.ItemOwners)
                        .Where(e => e.OwnedItem.ItemOwners.Where(d => d.IdUser == userId).Any())
                        .Join(
                            _handlerContext.Items,
                            price => price.OwnedItemId,
                            item => item.ItemId,
                            (price, item) => new GetSalesItemList
                            {
                                ItemId = price.OwnedItemId,
                                PriceId = price.PurchasePriceId,
                                InvoiceId = price.InvoiceId,
                                InvoiceNumber = price.OwnedItem.Invoice.InvoiceNumber,
                                Partnumber = item.PartNumber,
                                Name = item.ItemName,
                                Qty = price.Qty - price.SellingPrices.Select(d => d.Qty).Sum() + 
                                price.CreditNoteItems.Where(d => d.IncludeQty == true).Select(d => d.Qty).Sum(),
                                Price = price.Price
                            }
                        ).ToListAsync();
            }
            return await _handlerContext.PurchasePrices
                        .Include(e => e.OwnedItem)
                            .ThenInclude(e => e.Invoice)
                        .Include(e => e.OwnedItem)
                            .ThenInclude(e => e.ItemOwners)
                        .Include(e => e.CalculatedPrices)
                        .Where(e => e.OwnedItem.ItemOwners.Where(d => d.IdUser == userId).Any())
                        .Join(
                            _handlerContext.Items,
                            price => price.OwnedItemId,
                            item => item.ItemId,
                            (price, item) => new GetSalesItemList
                            {
                                ItemId = price.OwnedItemId,
                                PriceId = price.PurchasePriceId,
                                InvoiceId = price.InvoiceId,
                                InvoiceNumber = price.OwnedItem.Invoice.InvoiceNumber,
                                Partnumber = item.PartNumber,
                                Name = item.ItemName,
                                Qty = price.Qty - price.SellingPrices.Select(d => d.Qty).Sum() +
                                price.CreditNoteItems.Where(d => d.IncludeQty == true).Select(d => d.Qty).Sum(),
                                Price = price.CalculatedPrices.Where(d => d.CurrencyName == currency).Select(d => d.Price).First()
                            }
                        ).ToListAsync();
        }
        public async Task<IEnumerable<GetUsers>> GetItemOwners(int itemId)
        {
            return await _handlerContext.AppUsers
                .Where(e => 
                    e.ItemOwners.Where(d => d.OwnedItemId == itemId && d.Qty > 0).Any()
                    ||
                    e.Clients.SelectMany(d => d.OutsideItems).Where(d => d.ItemId == itemId).Any()
                )
                .Select(e => new GetUsers
                {
                    IdUser = e.IdUser,
                    Surname = e.Surname,
                    Username = e.Username
                })
                .ToListAsync();
        }
    }
}
