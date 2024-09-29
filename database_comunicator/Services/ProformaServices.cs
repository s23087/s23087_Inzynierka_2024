using database_comunicator.Data;
using database_comunicator.Models;
using database_comunicator.Models.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace database_comunicator.Services
{
    public interface IProformaServices
    {
        public Task<int> AddProforma(AddProforma data);
        public Task<bool> ProformaExist(string number, int sellerId, int buyerId);
        public Task<bool> ProformaExist(int proformaId);
        public Task<IEnumerable<GetProforma>> GetProformas(bool isYourProforma);
        public Task<IEnumerable<GetProforma>> GetProformas(bool isYourProforma, string search);
        public Task<IEnumerable<GetProforma>> GetProformas(bool isYourProforma, int userId);
        public Task<IEnumerable<GetProforma>> GetProformas(bool isYourProforma, int userId, string search);
        public Task<GetRestProforma> GetRestProforma(bool isYourProforma, int proformaId);
        public Task<bool> DeleteProforma(bool isYourProforma, int proformaId);
        public Task<bool> DoesDeliveryExist(int proformaId);
        public Task<int> GetProformaUserId(int proformaId);
        public Task<string> GetProformaNumber(int proformaId);
        public Task<string?> GetProformaPath(int proformaId);
        public Task<GetRestModifyProforma> GetRestModifyProforma(int proformaId);
        public Task<bool> ModifyProforma(ModifyProforma data);
    }
    public class ProformaServices : IProformaServices
    {
        private readonly HandlerContext _handlerContext;
        public ProformaServices(HandlerContext handlerContext)
        {
            _handlerContext = handlerContext;
        }
        public async Task<int> AddProforma(AddProforma data)
        {
            using var trans = await _handlerContext.Database.BeginTransactionAsync();
            try
            {
                var plnData = new DateTime(2024, 9, 3, 0, 0, 0, DateTimeKind.Utc);
                var checkCurrency = await _handlerContext.CurrencyValues.Where(e => e.CurrencyName == data.CurrencyName && e.UpdateDate.Equals(data.CurrencyDate)).AnyAsync();
                var currVal = new CurrencyValue
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
                    CurrencyValueDate = data.CurrencyDate,
                    CurrencyName = data.CurrencyName,
                    InvoiceId = null,
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
                } else
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
            } catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                await trans.RollbackAsync();
                return 0;
            }
        }
        public async Task<bool> ProformaExist(string number, int sellerId, int buyerId)
        {
            return await _handlerContext.Proformas
                .Where(e => e.ProformaNumber == number && e.Seller == sellerId && e.Buyer == buyerId)
                .AnyAsync();
        }
        public async Task<bool> ProformaExist(int proformaId)
        {
            return await _handlerContext.Proformas
                .Where(e => e.ProformaId == proformaId)
                .AnyAsync();
        }
        public async Task<IEnumerable<GetProforma>> GetProformas(bool isYourProfrma)
        {
            return await _handlerContext.Proformas
                .Where(e => isYourProfrma ? e.ProformaFutureItems.Any() : e.ProformaOwnedItems.Any())
                .Select(e => new GetProforma
                {
                    User = e.User.Username + " " + e.User.Surname,
                    ProformaId = e.ProformaId,
                    ProformaNumber = e.ProformaNumber,
                    Date = e.ProformaDate,
                    Transport = e.TransportCost,
                    Qty = isYourProfrma ? e.ProformaFutureItems.Select(d => d.Qty).Sum() : e.ProformaOwnedItems.Select(d => d.Qty).Sum(),
                    Total = isYourProfrma ? e.ProformaFutureItems.Select(d => d.PurchasePrice * d.Qty).Sum() : e.ProformaOwnedItems.Select(d => d.SellingPrice * d.Qty).Sum(),
                    CurrencyName = e.CurrencyName,
                    ClientName = isYourProfrma ? e.SellerNavigation.OrgName : e.BuyerNavigation.OrgName
                }).ToListAsync();
        }
        public async Task<IEnumerable<GetProforma>> GetProformas(bool isYourProfrma, string search)
        {
            return await _handlerContext.Proformas
                .Where(e => isYourProfrma ? e.ProformaFutureItems.Any() : e.ProformaOwnedItems.Any())
                .Where(e => e.ProformaNumber.ToLower().Contains(search.ToLower()))
                .Select(obj => new GetProforma
                {
                    User = obj.User.Username + " " + obj.User.Surname,
                    ProformaId = obj.ProformaId,
                    ProformaNumber = obj.ProformaNumber,
                    Date = obj.ProformaDate,
                    Transport = obj.TransportCost,
                    Qty = isYourProfrma ? obj.ProformaFutureItems.Select(e => e.Qty).Sum() : obj.ProformaOwnedItems.Select(e => e.Qty).Sum(),
                    Total = isYourProfrma ? obj.ProformaFutureItems.Select(e => e.PurchasePrice * e.Qty).Sum() : obj.ProformaOwnedItems.Select(e => e.SellingPrice * e.Qty).Sum(),
                    CurrencyName = obj.CurrencyName,
                    ClientName = isYourProfrma ? obj.SellerNavigation.OrgName : obj.BuyerNavigation.OrgName
                }).ToListAsync();
        }
        public async Task<IEnumerable<GetProforma>> GetProformas(bool isYourProfrma, int userId)
        {
            return await _handlerContext.Proformas
                .Where(e => isYourProfrma ? e.ProformaFutureItems.Any() : e.ProformaOwnedItems.Any())
                .Where(p => p.UserId == userId)
                .Select(ent => new GetProforma
                {
                    ProformaId = ent.ProformaId,
                    ProformaNumber = ent.ProformaNumber,
                    Date = ent.ProformaDate,
                    Transport = ent.TransportCost,
                    Qty = isYourProfrma ? ent.ProformaFutureItems.Select(e => e.Qty).Sum() : ent.ProformaOwnedItems.Select(e => e.Qty).Sum(),
                    Total = isYourProfrma ? ent.ProformaFutureItems.Select(e => e.PurchasePrice * e.Qty).Sum() : ent.ProformaOwnedItems.Select(e => e.SellingPrice * e.Qty).Sum(),
                    CurrencyName = ent.CurrencyName,
                    ClientName = isYourProfrma ? ent.SellerNavigation.OrgName : ent.BuyerNavigation.OrgName
                }).ToListAsync();
        }
        public async Task<IEnumerable<GetProforma>> GetProformas(bool isYourProfrma, int userId, string search)
        {
            return await _handlerContext.Proformas
                .Where(e => isYourProfrma ? e.ProformaFutureItems.Any() : e.ProformaOwnedItems.Any())
                .Where(p => p.UserId == userId)
                .Where(e => e.ProformaNumber.ToLower().Contains(search.ToLower()))
                .Select(prof => new GetProforma
                {
                    ProformaId = prof.ProformaId,
                    ProformaNumber = prof.ProformaNumber,
                    Date = prof.ProformaDate,
                    Transport = prof.TransportCost,
                    Qty = isYourProfrma ? prof.ProformaFutureItems.Select(e => e.Qty).Sum() : prof.ProformaOwnedItems.Select(e => e.Qty).Sum(),
                    Total = isYourProfrma ? prof.ProformaFutureItems.Select(e => e.PurchasePrice * e.Qty).Sum() : prof.ProformaOwnedItems.Select(e => e.SellingPrice * e.Qty).Sum(),
                    CurrencyName = prof.CurrencyName,
                    ClientName = isYourProfrma ? prof.SellerNavigation.OrgName : prof.BuyerNavigation.OrgName
                }).ToListAsync();
        }
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
        public async Task<bool> DeleteProforma(bool isYourProforma, int proformaId)
        {
            using var trans = await _handlerContext.Database.BeginTransactionAsync();
            try
            {
                if (isYourProforma)
                {
                    await _handlerContext.ProformaFutureItems.Where(e => e.ProformaId == proformaId).ExecuteDeleteAsync();
                } else
                {
                    await _handlerContext.ProformaOwnedItems.Where(e => e.ProformaId == proformaId).ExecuteDeleteAsync();
                }
                await _handlerContext.Proformas.Where(e => e.ProformaId == proformaId).ExecuteDeleteAsync();
                await _handlerContext.SaveChangesAsync();
                await trans.CommitAsync();
                return true;
            } catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                await trans.RollbackAsync();
                return false;
            }
        }
        public async Task<bool> DoesDeliveryExist(int proformaId)
        {
            return await _handlerContext.Proformas.AnyAsync(e => e.ProformaId == proformaId && e.Deliveries.Any());
        }
        public async Task<int> GetProformaUserId(int proformaId)
        {
            return await _handlerContext.Proformas
                .Where(e => e.ProformaId == proformaId)
                .Select(e => e.UserId).FirstAsync();
        }
        public async Task<string> GetProformaNumber(int proformaId)
        {
            return await _handlerContext.Proformas
                .Where(e => e.ProformaId == proformaId)
                .Select(e => e.ProformaNumber)
                .FirstAsync();
        }
        public async Task<string?> GetProformaPath(int proformaId)
        {
            return await _handlerContext.Proformas
                .Where(e => e.ProformaId == proformaId)
                .Select(e => e.ProformaFilePath)
                .FirstAsync();
        }
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
            } catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                await trans.RollbackAsync();
                return false;
            }
        }
    }
}
