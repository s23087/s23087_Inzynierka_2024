using database_comunicator.Data;
using database_comunicator.Models;
using database_comunicator.Models.DTOs;
using database_comunicator.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace database_comunicator.Services
{
    public interface IItemServices
    {
        public Task<Item> AddItem(AddItem newItem);
        public Task UpdateItem(UpdateItem postItem);
        public Task<bool> RemoveItem(int id);
        public Task<bool> ItemExist(int id);
        public Task<bool> ItemExist(string partNumber);
        public Task<bool> EanExist(IEnumerable<int> eans);
        public Task<IEnumerable<GetManyItems>> GetItems(string currency);
        public Task<IEnumerable<GetManyItems>> GetItems(string currency, string search);
        public Task<IEnumerable<GetManyItems>> GetItems(string currency, int userId);
        public Task<IEnumerable<GetManyItems>> GetItems(string currency, int userId, string search);
        public Task<GetRestInfo> GetRestOfItem(int id, string currency);
    }
    public class ItemServices : IItemServices
    {
        private readonly HandlerContext _handlerContext;
        public ItemServices(HandlerContext handlerContext)
        {
            _handlerContext = handlerContext;
        }

        public async Task<Item> AddItem(AddItem newItem)
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
                Ean1 = ean,
                ItemId = item.ItemId
            }));
            await _handlerContext.SaveChangesAsync();

            return item;
        }

        public async Task<bool> EanExist(IEnumerable<int> eans)
        {
            return await _handlerContext.Eans.Where(e => eans.Contains(e.Ean1)).AnyAsync();
        }

        public async Task<GetRestInfo> GetRestOfItem(int id, string currency)
        {
            var desc = await _handlerContext.Items.Where(e => e.ItemId == id).Select(e => e.ItemDescription).ToListAsync();
            var outsideItems = await _handlerContext.OutsideItems
                .Where(e => e.ItemId == id)
                .Include(e => e.Organization)
                .ThenInclude(e => e.AvailabilityStatuses)
                .Select(e => new GetRestItemInfo
            {
                OrganizationName = e.Organization.OrgName,
                Qty = e.Qty,
                DaysForRealization = e.Organization.AvailabilityStatuses.Select(e => e.DaysForRealization).ToList()[0],
                Price = e.PurchasePrice,
                Curenncy = e.CurrencyName.Curenncy

            }).ToListAsync();
            var ownedItems = await _handlerContext.OwnedItems.Where(e => e.OwnedItemId == id)
                .Include(e => e.Invoice)
                .ThenInclude(e => e.SellerNavigation)
                .Include(e => e.PurchasePrices.Where(a => a.InvoiceId == e.InvoiceId && a.OwnedItemId == e.OwnedItemId))
                .Select(e => new GetRestItemInfo
                {
                    OrganizationName = e.Invoice.SellerNavigation.OrgName,
                    InvoiceNumber = e.Invoice.InvoiceNumber,
                    Qty = e.Qty,
                    Price = e.PurchasePrices.Where(e => e.Curenncy.Equals(currency)).Select(e => e.PurchasePrice1).ToList()[0],
                    Curenncy = currency
                }).ToListAsync();

            return new GetRestInfo
            {
                ItemDescription = desc[0],
                OutsideItemInfos = outsideItems,
                OwnedItemInfos = ownedItems
            };

        }
        public async Task<IEnumerable<GetManyItems>> GetItems(string currency)
        {
            List<GetManyItems> items = await _handlerContext.Items
                .Include(a => a.Eans)
                .Include(b => b.OwnedItems)
                    .ThenInclude(c => c.Invoice)
                        .ThenInclude(h => h.Deliveries)
                .Include(j => j.OwnedItems)
                    .ThenInclude(k => k.Invoice)
                        .ThenInclude(h => h.SellerNavigation)
                .Include(d => d.OwnedItems)
                    .ThenInclude(e => e.PurchasePrices.Where(pur => pur.Curenncy == currency))
                .Include(f => f.OwnedItems)
                    .ThenInclude(g => g.ItemOwners)
                        .ThenInclude(k => k.IdUserNavigation)
                .Include(i => i.OutsideItems)
                .Select(inst => new GetManyItems
                {
                    Users = inst.OwnedItems.Select(e => e.ItemOwners.Select(e => e.IdUserNavigation.Username).First()),
                    ItemId = inst.ItemId,
                    ItemName = inst.ItemName,
                    PartNumber = inst.PartNumber,
                    StatusName = WarehouseUtils.getItemStatus(
                            inst.OwnedItems.Select(e => e.Qty).Sum(),
                            inst.OutsideItems.Select(e => e.Qty).Sum(),
                            inst.OwnedItems.Select(e => e.Invoice.Deliveries).Any()
                        ),
                    Eans = inst.Eans.Select(e => e.Ean1),
                    Qty = inst.OwnedItems.Select(e => e.Qty).Sum() + inst.OutsideItems.Select(e => e.Qty).Sum(),
                    PurchasePrice = inst.OwnedItems.Select(e => e.PurchasePrices.Select(pur => pur.PurchasePrice1).Average()).First(),
                    Sources = inst.OwnedItems.Select(e => e.Invoice).Select(e => e.SellerNavigation.OrgName)
                })
                .ToListAsync();
            return items;
        }
        public async Task<IEnumerable<GetManyItems>> GetItems(string currency, string search)
        {
            List<GetManyItems> items = await _handlerContext.Items
                .Where(j => EF.Functions.FreeText(j.PartNumber, search) || EF.Functions.FreeText(j.ItemName, search))
                .Include(a => a.Eans)
                .Include(b => b.OwnedItems)
                    .ThenInclude(c => c.Invoice)
                        .ThenInclude(h => h.Deliveries)
                .Include(j => j.OwnedItems)
                    .ThenInclude(k => k.Invoice)
                        .ThenInclude(h => h.SellerNavigation)
                .Include(d => d.OwnedItems)
                    .ThenInclude(e => e.PurchasePrices.Where(pur => pur.Curenncy == currency))
                .Include(f => f.OwnedItems)
                    .ThenInclude(g => g.ItemOwners)
                        .ThenInclude(k => k.IdUserNavigation)
                .Include(i => i.OutsideItems)
                .Select(inst => new GetManyItems
                {
                    Users = inst.OwnedItems.Select(e => e.ItemOwners.Select(e => e.IdUserNavigation.Username).First()),
                    ItemId = inst.ItemId,
                    ItemName = inst.ItemName,
                    PartNumber = inst.PartNumber,
                    StatusName = WarehouseUtils.getItemStatus(
                            inst.OwnedItems.Select(e => e.Qty).Sum(),
                            inst.OutsideItems.Select(e => e.Qty).Sum(),
                            inst.OwnedItems.Select(e => e.Invoice.Deliveries).Any()
                        ),
                    Eans = inst.Eans.Select(e => e.Ean1),
                    Qty = inst.OwnedItems.Select(e => e.Qty).Sum() + inst.OutsideItems.Select(e => e.Qty).Sum(),
                    PurchasePrice = inst.OwnedItems.Select(e => e.PurchasePrices.Select(pur => pur.PurchasePrice1).Average()).First(),
                    Sources = inst.OwnedItems.Select(e => e.Invoice).Select(e => e.SellerNavigation.OrgName)
                })
                .ToListAsync();
            return items;
        }
        public async Task<IEnumerable<GetManyItems>> GetItems(string currency, int userId)
        {
            List<GetManyItems> items = await _handlerContext.Items
                .Include(a => a.Eans)
                .Include(b => b.OwnedItems)
                    .ThenInclude(c => c.Invoice)
                        .ThenInclude(h => h.Deliveries)
                .Include(j => j.OwnedItems)
                    .ThenInclude(k => k.Invoice)
                        .ThenInclude(h => h.SellerNavigation)
                .Include(d => d.OwnedItems)
                    .ThenInclude(e => e.PurchasePrices.Where(pur => pur.Curenncy == currency))
                .Include(f => f.OwnedItems)
                    .ThenInclude(g => g.ItemOwners.Where(own => own.IdUser == userId))
                .Include(i => i.OutsideItems)
                .Select(inst => new GetManyItems
                {
                    ItemId = inst.ItemId,
                    ItemName = inst.ItemName,
                    PartNumber = inst.PartNumber,
                    StatusName = WarehouseUtils.getItemStatus(
                            inst.OwnedItems.Select(e => e.Qty).Sum(),
                            inst.OutsideItems.Select(e => e.Qty).Sum(),
                            inst.OwnedItems.Select(e => e.Invoice.Deliveries).Any()
                        ),
                    Eans = inst.Eans.Select(e => e.Ean1),
                    Qty = inst.OwnedItems.Select(e => e.Qty).Sum() + inst.OutsideItems.Select(e => e.Qty).Sum(),
                    PurchasePrice = inst.OwnedItems.Select(e => e.PurchasePrices.Select(pur => pur.PurchasePrice1).Average()).First(),
                    Sources = inst.OwnedItems.Select(e => e.Invoice).Select(e => e.SellerNavigation.OrgName)
                })
                .ToListAsync();
            return items;
        }
        public async Task<IEnumerable<GetManyItems>> GetItems(string currency, int userId, string search)
        {
            List<GetManyItems> items = await _handlerContext.Items
                .Where(j => EF.Functions.FreeText(j.PartNumber, search) || EF.Functions.FreeText(j.ItemName, search))
                .Include(a => a.Eans)
                .Include(b => b.OwnedItems)
                    .ThenInclude(c => c.Invoice)
                        .ThenInclude(h => h.Deliveries)
                .Include(j => j.OwnedItems)
                    .ThenInclude(k => k.Invoice)
                        .ThenInclude(h => h.SellerNavigation)
                .Include(d => d.OwnedItems)
                    .ThenInclude(e => e.PurchasePrices.Where(pur => pur.Curenncy == currency))
                .Include(f => f.OwnedItems)
                    .ThenInclude(g => g.ItemOwners.Where(own => own.IdUser == userId))
                .Include(i => i.OutsideItems)
                .Select(inst => new GetManyItems
                {
                    ItemId = inst.ItemId,
                    ItemName = inst.ItemName,
                    PartNumber = inst.PartNumber,
                    StatusName = WarehouseUtils.getItemStatus(
                            inst.OwnedItems.Select(e => e.Qty).Sum(),
                            inst.OutsideItems.Select(e => e.Qty).Sum(),
                            inst.OwnedItems.Select(e => e.Invoice.Deliveries).Any()
                        ),
                    Eans = inst.Eans.Select(e => e.Ean1),
                    Qty = inst.OwnedItems.Select(e => e.Qty).Sum() + inst.OutsideItems.Select(e => e.Qty).Sum(),
                    PurchasePrice = inst.OwnedItems.Select(e => e.PurchasePrices.Select(pur => pur.PurchasePrice1).Average()).First(),
                    Sources = inst.OwnedItems.Select(e => e.Invoice).Select(e => e.SellerNavigation.OrgName)
                })
                .ToListAsync();
            return items;
        }

        public async Task<bool> ItemExist(int id)
        {
            return await _handlerContext.Items.Where(e => e.ItemId == id).AnyAsync();
        }

        public async Task<bool> ItemExist(string partNumber)
        {
            return await _handlerContext.Items.Where(e => e.PartNumber.Equals(partNumber)).AnyAsync();
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
                var toDeleteEans = await _handlerContext.Eans.Where(e => e.ItemId == postItem.Id && !postItem.Eans.Contains(e.Ean1)).ToArrayAsync();
                _handlerContext.Eans.RemoveRange(toDeleteEans);
                var restEans = await _handlerContext.Eans.Where(e => e.ItemId == postItem.Id && postItem.Eans.Contains(e.Ean1)).Select(e => e.Ean1).ToArrayAsync();
                var eanToAdd = postItem.Eans.Where(e => !restEans.Contains(e)).ToArray();
                if (!eanToAdd.IsNullOrEmpty())
                {
                    _handlerContext.Eans.AddRange(eanToAdd.Select(e => new Ean
                    {
                        Ean1 = e,
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
    }
}
