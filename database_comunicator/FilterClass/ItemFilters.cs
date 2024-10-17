using database_communicator.Models;
using database_communicator.Models.DTOs;
using System.Linq.Expressions;

namespace database_comunicator.FilterClass
{
    public static class ItemFilters
    {
        public static Expression<Func<GetManyItems, bool>> GetQtyLowerFilter(int? qtyL)
        {
            return qtyL == null ?
                e => true
                : e => e.Qty <= qtyL;
        }
        public static Expression<Func<GetManyItems, bool>> GetQtyGreaterFilter(int? qtyG)
        {
            return qtyG == null ?
                e => true
                : e => e.Qty >= qtyG;
        }
        public static Expression<Func<GetManyItems, bool>> GetPriceLowerFilter(int? priceL)
        {
            return priceL == null ?
                e => true
                : e => e.PurchasePrice <= priceL;
        }
        public static Expression<Func<GetManyItems, bool>> GetPriceGreaterFilter(int? priceG)
        {
            return priceG == null ?
                e => true
                : e => e.PurchasePrice >= priceG;
        }
        public static Expression<Func<Item, bool>> GetEanFilter(string? ean)
        {
            return ean == null ?
                e => true
                : e => e.Eans.Any(x => x.EanValue == ean);
        }
    }
}
