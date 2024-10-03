using database_comunicator.Data;
using database_comunicator.Models;
using database_comunicator.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace database_comunicator.Services
{
    public interface IOutsideItemServices
    {

        public Task<IEnumerable<GetOutsideItem>> GetItems();
        public Task<IEnumerable<GetOutsideItem>> GetItems(int userId);
        public Task<IEnumerable<GetOutsideItem>> GetItems(string search);
        public Task<IEnumerable<GetOutsideItem>> GetItems(int userId, string search);
        public Task<bool> ItemExist(int itemId, int orgId);
        public Task DeleteItem(int itemId, int orgId);
        public Task<IEnumerable<string>> AddItems(CreateOutsideItems data);
        public Task<IEnumerable<int>> GetItemOwners(int itemId, int orgId);
    }
    public class OutsideItemServices : IOutsideItemServices
    {
        private readonly HandlerContext _handlerContext;
        public OutsideItemServices(HandlerContext handlerContext)
        {
            _handlerContext = handlerContext;
        }
        public async Task<IEnumerable<GetOutsideItem>> GetItems()
        {
            return await _handlerContext.OutsideItems
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
        public async Task<IEnumerable<GetOutsideItem>> GetItems(int userId)
        {
            return await _handlerContext.OutsideItems
                .Where(e => e.Organization.AppUsers.Any(x => x.IdUser == userId))
                .Select(obj => new GetOutsideItem
                {
                    Partnumber = obj.Item.PartNumber,
                    ItemId = obj.ItemId,
                    OrgId = obj.OrganizationId,
                    OrgName = obj.Organization.OrgName,
                    Price = obj.PurchasePrice,
                    Qty = obj.Qty,
                    Currency = obj.CurrencyName
                }).ToListAsync();
        }
        public async Task<IEnumerable<GetOutsideItem>> GetItems(string search)
        {
            return await _handlerContext.OutsideItems
                .Where(e => e.Item.PartNumber.ToLower().Contains(search.ToLower()) || e.Item.ItemName.ToLower().Contains(search.ToLower()))
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
        public async Task<IEnumerable<GetOutsideItem>> GetItems(int userId, string search)
        {
            return await _handlerContext.OutsideItems
                .Where(e => e.Organization.AppUsers.Any(x => x.IdUser == userId))
                .Where(e => e.Item.PartNumber.ToLower().Contains(search.ToLower()) || e.Item.ItemName.ToLower().Contains(search.ToLower()))
                .Select(item => new GetOutsideItem
                {
                    Partnumber = item.Item.PartNumber,
                    ItemId = item.ItemId,
                    OrgId = item.OrganizationId,
                    OrgName = item.Organization.OrgName,
                    Price = item.PurchasePrice,
                    Qty = item.Qty,
                    Currency = item.CurrencyName
                }).ToListAsync();
        }
        public async Task<bool> ItemExist(int itemId, int orgId)
        {
            return await _handlerContext.OutsideItems.AnyAsync(x => x.ItemId == itemId && x.OrganizationId == orgId);
        }
        public async Task DeleteItem(int itemId, int orgId)
        {
            await _handlerContext.OutsideItems
                .Where(e => e.ItemId == itemId && e.OrganizationId == orgId)
                .ExecuteDeleteAsync();
            await _handlerContext.SaveChangesAsync();
        }
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
                Console.WriteLine(ex.ToString());
                await trans.RollbackAsync();
                return new List<string>();
            }
        }
        public async Task<IEnumerable<int>> GetItemOwners(int itemId, int orgId)
        {
            return await _handlerContext.OutsideItems
                .Where(e => e.ItemId == itemId && e.OrganizationId == orgId)
                .SelectMany(e => e.Organization.AppUsers)
                .Select(e => e.IdUser).ToListAsync();
        }
    }
}
