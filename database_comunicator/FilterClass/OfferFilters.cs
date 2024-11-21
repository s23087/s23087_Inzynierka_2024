using database_communicator.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Globalization;
using System.Linq.Expressions;

namespace database_communicator.FilterClass
{
    /// <summary>
    /// All properties of pricelist/offer that filters are implemented in web client.
    /// </summary>
    public class OfferFiltersTemplate
    {
        public string? Status { get; set; } 
        public string? Currency { get; set; }
        public string? Type { get; set; }
        public int? TotalL { get; set; }
        public int? TotalG { get; set; }
        public string? CreatedL { get; set; }
        public string? CreatedG { get; set; }
        public string? ModifiedL { get; set; }
        public string? ModifiedG { get; set; }
    }
    /// <summary>
    /// Class that represent offer/pricelist filters. Contains static methods that return expression that can be passed in LINQ where functions.
    /// </summary>
    public static class OfferFilters
    {
        private const string dateFormat = "yyyy-MM-dd";
        /// <summary>
        /// If status value is passed return expression that will filter out object where pricelist/offer status is not equal to given value. Otherwise return always true.
        /// </summary>
        /// <param name="status">Name of pricelist status</param>
        /// <returns>Predicate that allow to filter Offer table.</returns>
        public static Expression<Func<Offer, bool>> GetStatusFilter(string? status)
        {
            return status == null ?
                e => true
                : e => e.OfferStatus.StatusName == status;
        }
        /// <summary>
        /// If shortcut currency name is passed return expression that will filter out object where pricelist/offer currency is not equal to given value. Otherwise return always true.
        /// </summary>
        /// <param name="currency">Shortcut name of currency</param>
        /// <returns>Predicate that allow to filter Offer table.</returns>
        public static Expression<Func<Offer, bool>> GetCurrencyFilter(string? currency)
        {
            return currency == null ?
                e => true
                : e => e.CurrencyName == currency;
        }
        /// <summary>
        /// If name of file type is passed return expression that will filter out object where pricelist/offer file path do not ends with given value. Otherwise return always true.
        /// </summary>
        /// <param name="type">File type name</param>
        /// <returns>Predicate that allow to filter Offer table.</returns>
        public static Expression<Func<Offer, bool>> GetTypeFilter(string? type)
        {
            return type == null ?
                e => true
                : e => e.PathToFile.EndsWith(type);
        }
        /// <summary>
        /// If total value is passed return expression that will filter out object where offer/pricelist total items added is greater then given value. Otherwise return always true.
        /// </summary>
        /// <param name="totalL">Total items added</param>
        /// <returns>Predicate that allow to filter Offer table.</returns>
        public static Expression<Func<Offer, bool>> GetTotalLowerFilter(int? totalL)
        {
            return totalL == null ?
                e => true
                : e => e.OfferItems.Count < totalL;
        }
        /// <summary>
        /// If total value is passed return expression that will filter out object where offer/pricelist total items added is lower then given value. Otherwise return always true.
        /// </summary>
        /// <param name="totalG">Total items added</param>
        /// <returns>Predicate that allow to filter Offer table.</returns>
        public static Expression<Func<Offer, bool>> GetTotalGreaterFilter(int? totalG)
        {
            return totalG == null ?
                e => true
                : e => e.OfferItems.Count < totalG;
        }
        /// <summary>
        /// If created date value is passed return expression that will filter out object where pricelist/offer created date is greater then given value. Otherwise return always true.
        /// </summary>
        /// <param name="createdL">Created date string</param>
        /// <returns>Predicate that allow to filter Offer table.</returns>
        public static Expression<Func<Offer, bool>> GetCreatedLowerFilter(string? createdL)
        {
            return createdL == null ?
                e => true
                : e => e.CreationDate <= DateTime.ParseExact(createdL, OfferFilters.dateFormat, CultureInfo.InvariantCulture);
        }
        /// <summary>
        /// If created date value is passed return expression that will filter out object where pricelist/offer created date is lower then given value. Otherwise return always true.
        /// </summary>
        /// <param name="createdG">Created date string</param>
        /// <returns>Predicate that allow to filter Offer table.</returns>
        public static Expression<Func<Offer, bool>> GetCreatedGreaterFilter(string? createdG)
        {
            return createdG == null ?
                e => true
                : e => e.CreationDate >= DateTime.ParseExact(createdG, OfferFilters.dateFormat, CultureInfo.InvariantCulture);
        }
        /// <summary>
        /// If modified date value is passed return expression that will filter out object where pricelist/offer modified date is greater then given value. Otherwise return always true.
        /// </summary>
        /// <param name="modifiedL">Modified date string</param>
        /// <returns>Predicate that allow to filter Offer table.</returns>
        public static Expression<Func<Offer, bool>> GetModifiedLowerFilter(string? modifiedL)
        {
            return modifiedL == null ?
                e => true
                : e => e.ModificationDate <= DateTime.ParseExact(modifiedL, OfferFilters.dateFormat, CultureInfo.InvariantCulture);
        }
        /// <summary>
        /// If modified date value is passed return expression that will filter out object where pricelist/offer modified date is lower then given value. Otherwise return always true.
        /// </summary>
        /// <param name="modifiedG">Modified date string</param>
        /// <returns>Predicate that allow to filter Offer table.</returns>
        public static Expression<Func<Offer, bool>> GetModifiedGreaterFilter(string? modifiedG)
        {
            return modifiedG == null ?
                e => true
                : e => e.ModificationDate >= DateTime.ParseExact(modifiedG, OfferFilters.dateFormat, CultureInfo.InvariantCulture);
        }
    }
}
