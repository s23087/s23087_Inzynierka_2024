using database_comunicator.Data;
using database_comunicator.Models;
using database_comunicator.Models.DTOs;
using Microsoft.EntityFrameworkCore;

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
    }
}
