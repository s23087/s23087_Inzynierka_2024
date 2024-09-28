using database_comunicator.Data;
using database_comunicator.Models;
using database_comunicator.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace database_comunicator.Services
{
    public interface IProformaServices
    {
        public Task<int> AddProforma(AddProforma data);
        public Task<bool> ProformaExist(string number, int sellerId, int buyerId);
        public Task<IEnumerable<GetProforma>> GetProformas(bool isYourProfrma);
        public Task<IEnumerable<GetProforma>> GetProformas(bool isYourProfrma, string search);
        public Task<IEnumerable<GetProforma>> GetProformas(bool isYourProfrma, int userId);
        public Task<IEnumerable<GetProforma>> GetProformas(bool isYourProfrma, int userId, string search);
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
    }
}
