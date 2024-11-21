using database_communicator.Models;
using database_communicator.Models.DTOs.Get;
using System.Linq.Expressions;

namespace database_communicator.FilterClass
{
    /// <summary>
    /// All properties of item that filters are implemented in web client.
    /// </summary>
    public class ItemFiltersTemplate
    {
        public string? Status { get; set; } 
        public string? Ean { get; set; }
        public int? QtyL { get; set; }
        public int? QtyG { get; set; }
        public int? PriceL { get; set; }
        public int? PriceG { get; set; }
    }
    /// <summary>
    /// Class that represent item filters. Contains static methods that return expression that can be passed in LINQ where functions.
    /// </summary>
    public static class ItemFilters
    {
        /// <summary>
        /// If quantity value is passed return expression that will filter out object where item qty is greater then given value. Otherwise return always true.
        /// </summary>
        /// <param name="qtyL">Item quantity</param>
        /// <returns>Predicate that allow to filter item quantity.</returns>
        public static Expression<Func<GetManyItems, bool>> GetQtyLowerFilter(int? qtyL)
        {
            return qtyL == null ?
                e => true
                : e => e.Qty <= qtyL;
        }
        /// <summary>
        /// If quantity value is passed return expression that will filter out object where item qty is lower then given value. Otherwise return always true.
        /// </summary>
        /// <param name="qtyG">Item quantity</param>
        /// <returns>Predicate that allow to filter item quantity.</returns>
        public static Expression<Func<GetManyItems, bool>> GetQtyGreaterFilter(int? qtyG)
        {
            return qtyG == null ?
                e => true
                : e => e.Qty >= qtyG;
        }
        /// <summary>
        /// If price value is passed return expression that will filter out object where item price is greater then given value. Otherwise return always true.
        /// </summary>
        /// <param name="priceL">Item price</param>
        /// <returns>Predicate that allow to filter item price.</returns>
        public static Expression<Func<GetManyItems, bool>> GetPriceLowerFilter(int? priceL)
        {
            return priceL == null ?
                e => true
                : e => e.PurchasePrice <= priceL;
        }
        /// <summary>
        /// If price value is passed return expression that will filter out object where item price is lower then given value. Otherwise return always true.
        /// </summary>
        /// <param name="priceG">Item price</param>
        /// <returns>Predicate that allow to filter item price.</returns>
        public static Expression<Func<GetManyItems, bool>> GetPriceGreaterFilter(int? priceG)
        {
            return priceG == null ?
                e => true
                : e => e.PurchasePrice >= priceG;
        }
        /// <summary>
        /// If ean value is passed return expression that will filter out object where item eans contains given value. Otherwise return always true.
        /// </summary>
        /// <param name="ean">Ean value</param>
        /// <returns>Predicate that allow to filter items.</returns>
        public static Expression<Func<Item, bool>> GetEanFilter(string? ean)
        {
            return ean == null ?
                e => true
                : e => e.Eans.Any(x => x.EanValue == ean);
        }
        /// <summary>
        /// If status shortcut value is passed return expression that will filter out object where item status is not equal to given value. Otherwise return always true.
        /// </summary>
        /// <param name="status">Shortcut status name (war - In warehouse, deli - In delivery, wardeli - In warehouse / In delivery, req - On request, unavail  - Unavailable)</param>
        /// <returns>Predicate that allow to filter items.</returns>
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
