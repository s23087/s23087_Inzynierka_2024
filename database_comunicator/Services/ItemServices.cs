﻿using database_comunicator.Data;
using database_comunicator.Models;
using database_comunicator.Models.DTOs;
using database_comunicator.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

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
        public Task<GetRestInfo> GetRestOfItem(int id, string currency);
        public Task<IEnumerable<GetRestInfoOrg>> GetRestOfItemOrg(int id, string currency);
        public Task<IEnumerable<GetBinding>> GetModifyRestOfItem(int id, string currency);
        public Task<string> GetDescription(int id);
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

        public async Task<GetRestInfo> GetRestOfItem(int id, string currency)
        {
            var outsideItems = await _handlerContext.OutsideItems
                .Where(e => e.ItemId == id)
                .Include(e => e.Organization)
                .ThenInclude(e => e.AvailabilityStatus)
                .Select(e => new GetRestItemInfo
            {
                OrganizationName = e.Organization.OrgName,
                Qty = e.Qty,
                DaysForRealization = e.Organization.AvailabilityStatus == null ? 0 : e.Organization.AvailabilityStatus.DaysForRealization,
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
                OutsideItemInfos = outsideItems,
                OwnedItemInfos = ownedItems
            };

        }
        public async Task<IEnumerable<GetRestInfoOrg>> GetRestOfItemOrg(int id, string currency)
        {
            var result = await _handlerContext.AppUsers
                .Include(e => e.ItemOwners)
                 .ThenInclude(e => e.OwnedItem)
                    .ThenInclude(e => e.Invoice)
                        .ThenInclude(e => e.SellerNavigation)
                .Include(e => e.ItemOwners)
                 .ThenInclude(e => e.OwnedItem)
                    .ThenInclude(e => e.PurchasePrices)
                .Include(e => e.Clients)
                    .ThenInclude(e => e.OutsideItems)
                .Include(e => e.Clients)
                    .ThenInclude(e => e.AvailabilityStatus)
                .Select(instance => new GetRestInfoOrg
                {
                    UserId = instance.IdUser,
                    Username = instance.Username,
                    OutsideItemInfos = instance.Clients.Select(e => new GetRestItemInfo
                    {
                        OrganizationName = e.OrgName,
                        Qty = e.OutsideItems.Where(d => d.ItemId == id).Select(f => f.Qty).First(),
                        DaysForRealization = e.AvailabilityStatus == null ? 0 : e.AvailabilityStatus.DaysForRealization,
                        Price = e.OutsideItems.Where(d => d.ItemId == id).Select(f => f.PurchasePrice).First(),
                        Curenncy = e.OutsideItems.Where(d => d.ItemId == id).Select(f => f.Curenncy).First()
                    }).ToList(),
                    OwnedItemInfos = instance.ItemOwners.Select(e => new GetRestItemInfo
                    {
                        OrganizationName = e.OwnedItem.Invoice.SellerNavigation.OrgName,
                        InvoiceNumber = e.OwnedItem.Invoice.InvoiceNumber,
                        Qty = e.Qty,
                        Price = e.OwnedItem.PurchasePrices.Where(e => e.Curenncy.Equals(currency)).Select(e => e.PurchasePrice1).ToList()[0],
                        Curenncy = currency
                    }).ToList()
                }).ToListAsync();

            return result;
        }
        public async Task<IEnumerable<GetManyItems>> GetItems(string currency)
        {
            List<GetManyItems> items = await _handlerContext.Items
                .Include(ent => ent.Eans)
                .Include(ent => ent.OwnedItems)
                    .ThenInclude(c => c.Invoice)
                        .ThenInclude(h => h.Deliveries)
                .Include(ent => ent.OwnedItems)
                    .ThenInclude(k => k.Invoice)
                        .ThenInclude(h => h.SellerNavigation)
                .Include(ent => ent.OwnedItems)
                    .ThenInclude(e => e.PurchasePrices.Where(pur => pur.Curenncy == currency))
                .Include(ent => ent.OwnedItems)
                    .ThenInclude(g => g.ItemOwners)
                        .ThenInclude(k => k.IdUserNavigation)
                .Include(ent => ent.OutsideItems)
                .Select(instance => new GetManyItems
                {
                    Users = instance.OwnedItems.Select(e => e.ItemOwners.Select(e => e.IdUserNavigation.Username).First()),
                    ItemId = instance.ItemId,
                    ItemName = instance.ItemName,
                    PartNumber = instance.PartNumber,
                    StatusName = WarehouseUtils.getItemStatus(
                            instance.OwnedItems.Select(e => e.Qty).Sum(),
                            instance.OutsideItems.Select(e => e.Qty).Sum(),
                            instance.OwnedItems.Select(e => e.Invoice.Deliveries).Any()
                        ),
                    Eans = instance.Eans.Select(e => e.EanValue),
                    Qty = instance.OwnedItems.Select(e => e.Qty).Sum() + instance.OutsideItems.Select(e => e.Qty).Sum(),
                    PurchasePrice = instance.OwnedItems.Select(e => e.PurchasePrices.Select(pur => pur.PurchasePrice1).Average()).First(),
                    Sources = instance.OwnedItems.Select(e => e.Invoice).Select(e => e.SellerNavigation.OrgName)
                })
                .ToListAsync();
            return items;
        }
        public async Task<IEnumerable<GetManyItems>> GetItems(string currency, string search)
        {
            List<GetManyItems> items = await _handlerContext.Items
                .Where(j => EF.Functions.FreeText(j.PartNumber, search) || EF.Functions.FreeText(j.ItemName, search))
                .Include(entit => entit.Eans)
                .Include(entit => entit.OwnedItems)
                    .ThenInclude(c => c.Invoice)
                        .ThenInclude(h => h.Deliveries)
                .Include(entit => entit.OwnedItems)
                    .ThenInclude(k => k.Invoice)
                        .ThenInclude(h => h.SellerNavigation)
                .Include(entit => entit.OwnedItems)
                    .ThenInclude(e => e.PurchasePrices.Where(pur => pur.Curenncy == currency))
                .Include(entit => entit.OwnedItems)
                    .ThenInclude(g => g.ItemOwners)
                        .ThenInclude(k => k.IdUserNavigation)
                .Include(entit => entit.OutsideItems)
                .Select(instc => new GetManyItems
                {
                    Users = instc.OwnedItems.Select(e => e.ItemOwners.Select(e => e.IdUserNavigation.Username).First()),
                    ItemId = instc.ItemId,
                    ItemName = instc.ItemName,
                    PartNumber = instc.PartNumber,
                    StatusName = WarehouseUtils.getItemStatus(
                            instc.OwnedItems.Select(e => e.Qty).Sum(),
                            instc.OutsideItems.Select(e => e.Qty).Sum(),
                            instc.OwnedItems.Select(e => e.Invoice.Deliveries).Any()
                        ),
                    Eans = instc.Eans.Select(e => e.EanValue),
                    Qty = instc.OwnedItems.Select(e => e.Qty).Sum() + instc.OutsideItems.Select(e => e.Qty).Sum(),
                    PurchasePrice = instc.OwnedItems.Select(e => e.PurchasePrices.Select(pur => pur.PurchasePrice1).Average()).First(),
                    Sources = instc.OwnedItems.Select(e => e.Invoice).Select(e => e.SellerNavigation.OrgName)
                })
                .ToListAsync();
            return items;
        }
        public async Task<IEnumerable<GetManyItems>> GetItems(string currency, int userId)
        {
            List<GetManyItems> items = await _handlerContext.Items
                .Include(tmp => tmp.Eans)
                .Include(tmp => tmp.OwnedItems)
                    .ThenInclude(c => c.Invoice)
                        .ThenInclude(h => h.Deliveries)
                .Include(tmp => tmp.OwnedItems)
                    .ThenInclude(k => k.Invoice)
                        .ThenInclude(h => h.SellerNavigation)
                .Include(tmp => tmp.OwnedItems)
                    .ThenInclude(e => e.PurchasePrices.Where(pur => pur.Curenncy == currency))
                .Include(tmp => tmp.OwnedItems)
                    .ThenInclude(g => g.ItemOwners.Where(own => own.IdUser == userId))
                .Include(tmp => tmp.OutsideItems)
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
                    Eans = inst.Eans.Select(e => e.EanValue),
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
                .Include(tab => tab.Eans)
                .Include(tab => tab.OwnedItems)
                    .ThenInclude(c => c.Invoice)
                        .ThenInclude(h => h.Deliveries)
                .Include(tab => tab.OwnedItems)
                    .ThenInclude(k => k.Invoice)
                        .ThenInclude(h => h.SellerNavigation)
                .Include(tab => tab.OwnedItems)
                    .ThenInclude(e => e.PurchasePrices.Where(pur => pur.Curenncy == currency))
                .Include(tab => tab.OwnedItems)
                    .ThenInclude(g => g.ItemOwners.Where(own => own.IdUser == userId))
                .Include(tab => tab.OutsideItems)
                .Select(obj => new GetManyItems
                {
                    ItemId = obj.ItemId,
                    ItemName = obj.ItemName,
                    PartNumber = obj.PartNumber,
                    StatusName = WarehouseUtils.getItemStatus(
                            obj.OwnedItems.Select(e => e.Qty).Sum(),
                            obj.OutsideItems.Select(e => e.Qty).Sum(),
                            obj.OwnedItems.Select(e => e.Invoice.Deliveries).Any()
                        ),
                    Eans = obj.Eans.Select(e => e.EanValue),
                    Qty = obj.OwnedItems.Select(e => e.Qty).Sum() + obj.OutsideItems.Select(e => e.Qty).Sum(),
                    PurchasePrice = obj.OwnedItems.Select(e => e.PurchasePrices.Select(pur => pur.PurchasePrice1).Average()).First(),
                    Sources = obj.OwnedItems.Select(e => e.Invoice).Select(e => e.SellerNavigation.OrgName)
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
            var binding = await _handlerContext.ItemOwners
                .Where(e => e.OwnedItemId == id)
                .Include(e => e.IdUserNavigation)
                .Include(e => e.OwnedItem)
                    .ThenInclude(e => e.Invoice)
                .Include(e => e.OwnedItem)
                    .ThenInclude(e => e.PurchasePrices)
                .Select(res => new GetBinding
                {
                    UserId = res.IdUser,
                    Username = res.IdUserNavigation.Username,
                    Qty = res.Qty,
                    Price = res.OwnedItem.PurchasePrices.Where(e => e.Curenncy.Equals(currency)).Select(e => e.PurchasePrice1).ToList()[0],
                    InvoiceNumber = res.OwnedItem.Invoice.InvoiceNumber

                }).ToListAsync();
            var noUserItem = await _handlerContext.OwnedItems
                .Where(e => e.OwnedItemId == id)
                .Include(e => e.Invoice)
                .Include(e => e.PurchasePrices)
                .Include(e => e.ItemOwners)
                .Select(inst => new GetBinding
                {
                    UserId = null,
                    Username = null,
                    Qty = inst.Qty - inst.ItemOwners.Select(e => e.Qty).Sum(),
                    Price = inst.PurchasePrices.Where(e => e.Curenncy.Equals(currency)).Select(e => e.PurchasePrice1).ToList()[0],
                    InvoiceNumber = inst.Invoice.InvoiceNumber
                }).ToListAsync();
            binding.AddRange(noUserItem);
            return binding;
        }
        public async Task<string> GetDescription(int id)
        {
            var result = await _handlerContext.Items.Where(e => e.ItemId == id).Select(e => e.ItemDescription).ToListAsync();
            return result[0];
        }
    }
}
