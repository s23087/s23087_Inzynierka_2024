using database_communicator.Models;
using System.Linq.Expressions;

namespace database_communicator.FilterClass
{
    /// <summary>
    /// All properties of outside items that filters are implemented in web client.
    /// </summary>
    public class OutsideItemFiltersTemplate
    {
        public int? QtyL { get; set; }
        public int? QtyG { get; set; }
        public int? PriceL { get; set; }
        public int? PriceG { get; set; }
        public int? Source { get; set; }
        public string? Currency { get; set; }
    }
    /// <summary>
    /// Class that represent outside items filters. Contains static methods that return expression that can be passed in LINQ where functions.
    /// </summary>
    public static class OutsideItemFilters
    {
        /// <summary>
        /// If quantity value is passed return expression that will filter out object where outside item qty is greater then given value. Otherwise return always true.
        /// </summary>
        /// <param name="qtyL">Item quantity</param>
        /// <returns>Predicate that allow to filter Outside item table.</returns>
        public static Expression<Func<OutsideItem, bool>> GetQtyLowerFilter(int? qtyL)
        {
            return qtyL == null ?
                e => true
                : e => e.Qty <= qtyL;
        }
        /// <summary>
        /// If quantity value is passed return expression that will filter out object where outside item qty is lower then given value. Otherwise return always true.
        /// </summary>
        /// <param name="qtyG">Item quantity</param>
        /// <returns>Predicate that allow to filter Outside item table.</returns>
        public static Expression<Func<OutsideItem, bool>> GetQtyGreaterFilter(int? qtyG)
        {
            return qtyG == null ?
                e => true
                : e => e.Qty >= qtyG;
        }
        /// <summary>
        /// If price value is passed return expression that will filter out object where outside item price is greater then given value. Otherwise return always true.
        /// </summary>
        /// <param name="priceL">Item price</param>
        /// <returns>Predicate that allow to filter Outside item table.</returns>
        public static Expression<Func<OutsideItem, bool>> GetPriceLowerFilter(int? priceL)
        {
            return priceL == null ?
                e => true
                : e => e.PurchasePrice <= priceL;
        }
        /// <summary>
        /// If price value is passed return expression that will filter out object where outside item price is lower then given value. Otherwise return always true.
        /// </summary>
        /// <param name="priceG">Item price</param>
        /// <returns>Predicate that allow to filter Outside item table.</returns>
        public static Expression<Func<OutsideItem, bool>> GetPriceGreaterFilter(int? priceG)
        {
            return priceG == null ?
                e => true
                : e => e.PurchasePrice >= priceG;
        }
        /// <summary>
        /// If organization id is passed return expression that will filter out object where outside item source is not equal to given value. Otherwise return always true.
        /// </summary>
        /// <param name="source">Organization id that is source of item.</param>
        /// <returns>Predicate that allow to filter Outside item table.</returns>
        public static Expression<Func<OutsideItem, bool>> GetSourceFilter(int? source)
        {
            return source == null ?
                e => true
                : e => e.OrganizationId == source;
        }
        /// <summary>
        /// If currency shortcut value is passed return expression that will filter out object where outside item currency is not equal to given value. Otherwise return always true.
        /// </summary>
        /// <param name="currency">Shortcut name of currency</param>
        /// <returns>Predicate that allow to filter Outside item table.</returns>
        public static Expression<Func<OutsideItem, bool>> GetCurrencyFilter(string? currency)
        {
            return currency == null ?
                e => true
                : e => e.CurrencyName == currency;
        }
    }
}
