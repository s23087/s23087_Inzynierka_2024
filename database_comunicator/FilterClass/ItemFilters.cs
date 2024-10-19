using database_communicator.Models;
using database_communicator.Models.DTOs;
using System.Linq.Expressions;

namespace database_comunicator.FilterClass
{
    public class ItemFiltersTemplate
    {
        public string? Status { get; set; } 
        public string? Ean { get; set; }
        public int? QtyL { get; set; }
        public int? QtyG { get; set; }
        public int? PriceL { get; set; }
        public int? PriceG { get; set; }
    }
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
        public static Func<GetManyItems, bool> GetStatusFilter(string? status)
        {
            Func<GetManyItems, bool> result = status switch
            {
                "war" => e => e.StatusName == "In warehouse",
                "deli" => e => e.StatusName == "In delivery",
                "wardeli" => e => e.StatusName == "In warehouse | In delivery",
                "req" => e => e.StatusName == "On request",
                "unavail" => e => e.StatusName == "Unavailable",
                _ => e => true,
            };
            return result;
        }
    }
}
