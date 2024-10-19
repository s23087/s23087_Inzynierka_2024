using database_communicator.Models;
using System.Linq.Expressions;

namespace database_comunicator.FilterClass
{
    public class OutsideItemFiltersTemplate
    {
        public int? QtyL { get; set; }
        public int? QtyG { get; set; }
        public int? PriceL { get; set; }
        public int? PriceG { get; set; }
        public int? Source { get; set; }
        public string? Currency { get; set; }
    }
    public static class OutsideItemFilters
    {
        public static Expression<Func<OutsideItem, bool>> GetQtyLowerFilter(int? qtyL)
        {
            return qtyL == null ?
                e => true
                : e => e.Qty <= qtyL;
        }
        public static Expression<Func<OutsideItem, bool>> GetQtyGreaterFilter(int? qtyG)
        {
            return qtyG == null ?
                e => true
                : e => e.Qty >= qtyG;
        }
        public static Expression<Func<OutsideItem, bool>> GetPriceLowerFilter(int? priceL)
        {
            return priceL == null ?
                e => true
                : e => e.PurchasePrice <= priceL;
        }
        public static Expression<Func<OutsideItem, bool>> GetPriceGreaterFilter(int? priceG)
        {
            return priceG == null ?
                e => true
                : e => e.PurchasePrice >= priceG;
        }
        public static Expression<Func<OutsideItem, bool>> GetSourceFilter(int? source)
        {
            return source == null ?
                e => true
                : e => e.OrganizationId == source;
        }
        public static Expression<Func<OutsideItem, bool>> GetCurrencyFilter(string? currency)
        {
            return currency == null ?
                e => true
                : e => e.CurrencyName == currency;
        }
    }
}
