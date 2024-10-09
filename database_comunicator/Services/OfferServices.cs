using database_comunicator.Data;
using database_comunicator.Models;
using database_comunicator.Models.DTOs;
using LINQtoCSV;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Xml.Linq;

namespace database_comunicator.Services
{
    public interface IOfferServices
    {
        public Task<IEnumerable<GetPriceList>> GetPriceLists(int userId);
        public Task<IEnumerable<GetPriceList>> GetPriceLists(int userId, string serach);
        public Task<bool> DoesPricelistExist(int offerId);
        public Task<bool> DoesPricelistExist(string offerName, int userId);
        public Task<bool> DeletePricelist(int offerId);
        public Task<int> CreateOffer(AddOffer data);
        public Task<IEnumerable<GetOfferStatus>> GetOfferStatuses();
        public Task<IEnumerable<GetItemsForOffer>> GetItemsForOffers(int userId, string currency);
        public Task<bool> CreateCsvFile(int offerId, int userId, string path, int maxQty);
        public Task<bool> CreateCsvFile(int offerId);
        public Task<bool> CreateXmlFile(int offerId, int userId, string path, int maxQty);
        public Task<bool> CreateXmlFile(int offerId);
        public Task<int> GetDeactivatedStatusId();
        public Task<IEnumerable<GetCredtItemForTable>> GetOfferItems(int offerId, int userId);
        public Task<GetRestModifyOffer> GetRestModifyOffer(int offerId, int userId);
        public Task<bool> ModifyOffer(ModifyPricelist data);
        public Task<string> GetOfferPath(int offerId);
        public Task<int> GetOfferMaxQty(int offerId);
        public Task<IEnumerable<int>> GetAllActiveXmlOfferId();
        public Task<IEnumerable<int>> GetAllActiveCsvOfferId();
    }
    public class OfferServices : IOfferServices
    {
        private readonly HandlerContext _handlerContext;
        public OfferServices(HandlerContext handlerContext)
        {
            _handlerContext = handlerContext;
        }
        public async Task<IEnumerable<GetPriceList>> GetPriceLists(int userId)
        {
            return await _handlerContext.Offers
                .Where(e => e.UserId == userId)
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
        public async Task<IEnumerable<GetPriceList>> GetPriceLists(int userId, string serach)
        {
            return await _handlerContext.Offers
                .Where(e => e.UserId == userId)
                .Where(e => e.OfferName.ToLower().Contains(serach.ToLower()))
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
        public async Task<bool> DoesPricelistExist(int offerId)
        {
            return await _handlerContext.Offers.AnyAsync(x => x.OfferId == offerId);
        }
        public async Task<bool> DoesPricelistExist(string offerName, int userId)
        {
            return await _handlerContext.Offers.AnyAsync(x => x.OfferName == offerName && x.UserId == userId);
        }
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
                Console.WriteLine(ex.ToString());
                await trans.RollbackAsync();
                return false;
            }
        }
        public async Task<int> CreateOffer(AddOffer data)
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
                Console.WriteLine(ex.ToString());
                await trans.RollbackAsync();
                return 0;
            }
        }
        public async Task<IEnumerable<GetOfferStatus>> GetOfferStatuses()
        {
            return await _handlerContext.OfferStatuses
                .Select(e => new GetOfferStatus
                {
                    StatusId = e.OfferId,
                    StatusName = e.StatusName
                }).ToListAsync();
        }
        public async Task<IEnumerable<GetItemsForOffer>> GetItemsForOffers(int userId, string currency)
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
                Console.WriteLine(ex.ToString());
                return false;
            }
        }
        public async Task<bool> CreateCsvFile(int offerId)
        {
            var restOfferData = await _handlerContext.Offers.Where(e => e.OfferId == offerId).Select(e => new { e.UserId, e.PathToFile, e.MaxQty }).FirstAsync();
            return await CreateCsvFile(offerId, restOfferData.UserId, restOfferData.PathToFile, restOfferData.MaxQty);
        }
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
                Console.WriteLine(ex.ToString());
                return false;
            }
        }
        public async Task<bool> CreateXmlFile(int offerId)
        {
            var restOfferData = await _handlerContext.Offers.Where(d => d.OfferId == offerId).Select(d => new { d.UserId, d.PathToFile, d.MaxQty }).FirstAsync();
            return await CreateXmlFile(offerId, restOfferData.UserId, restOfferData.PathToFile, restOfferData.MaxQty);
        }
        public async Task<int> GetDeactivatedStatusId()
        {
            return await _handlerContext.OfferStatuses.Where(e => e.StatusName == "Deactivated").Select(e => e.OfferId).FirstAsync();
        }
        public async Task<IEnumerable<GetCredtItemForTable>> GetOfferItems(int offerId, int userId)
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
        public async Task<GetRestModifyOffer> GetRestModifyOffer(int offerId, int userId)
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
        public async Task<bool> ModifyOffer(ModifyPricelist data)
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
                Console.WriteLine(ex.Message);
                await trans.RollbackAsync();
                return false;
            }
        }
        public async Task<string> GetOfferPath(int offerId)
        {
            return await _handlerContext.Offers.Where(e => e.OfferId == offerId).Select(e => e.PathToFile).FirstAsync();
        }
        public async Task<int> GetOfferMaxQty(int offerId)
        {
            return await _handlerContext.Offers.Where(e => e.OfferId == offerId).Select(e => e.MaxQty).FirstAsync();
        }
        public async Task<IEnumerable<int>> GetAllActiveXmlOfferId()
        {
            return await _handlerContext.Offers.Where(e => !e.PathToFile.EndsWith("csv") && e.OfferStatus.StatusName == "Active").Select(e => e.OfferId).ToListAsync();
        }
        public async Task<IEnumerable<int>> GetAllActiveCsvOfferId()
        {
            return await _handlerContext.Offers.Where(e => e.PathToFile.EndsWith("csv") && e.OfferStatus.StatusName == "Active").Select(e => e.OfferId).ToListAsync();
        }
    }
}
