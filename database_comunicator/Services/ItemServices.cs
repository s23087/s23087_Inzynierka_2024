using database_comunicator.Data;
using database_comunicator.Models;
using database_comunicator.Models.DTOs;
using database_comunicator.Utils;
using Microsoft.EntityFrameworkCore;

namespace database_comunicator.Services
{
    public interface IItemServices
    {
        public Task<Item> AddItem(AddItem newItem);
        public Task<bool> UpdateItem(UpdateItem postItem);
        public Task<bool> RemoveItem(int id);
        public Task<bool> ItemExist(int id);
        public Task<bool> ItemExist(string partNumber);
        public Task<bool> EanExist(IEnumerable<int> eans);
        public Task<IEnumerable<GetManyItems>> GetItems(string currency);
        public Task<IEnumerable<GetManyItems>> GetItems(string currency, string search);
        public Task<IEnumerable<GetManyItems>> GetItems(string currency, int userId);
        public Task<IEnumerable<GetManyItems>> GetItems(string currency, int userId, string search);
        public Task<IEnumerable<GetOneItem>> GetItem(int id);
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

        public Task<IEnumerable<GetOneItem>> GetItem(int id)
        {
            throw new NotImplementedException();
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

        public Task<bool> RemoveItem(int id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateItem(UpdateItem postItem)
        {
            throw new NotImplementedException();
        }
    }
}
