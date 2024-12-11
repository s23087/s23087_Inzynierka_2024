using System.Globalization;
using System.Linq.Expressions;
using database_communicator.Models;

namespace database_communicator.FilterClass
{
    /// <summary>
    /// All properties of proforma that filters are implemented in web client.
    /// </summary>
    public class ProformaFiltersTemplate
    {
        public int? QtyL { get; set; }
        public int? QtyG { get; set; }
        public int? TotalL { get; set; }
        public int? TotalG { get; set; }
        public string? DateL { get; set; }
        public string? DateG { get; set; }
        public int? Recipient { get; set; }
        public string? Currency { get; set; }
    }
    /// <summary>
    /// Class that represent proforma filters. Contains static methods that return expression that can be passed in LINQ where functions.
    /// </summary>
    public static class ProformaFilters
    {
        /// <summary>
        /// If quantity value is passed return expression that will filter out object where proforma total item qty is greater then given value. Otherwise return always true.
        /// </summary>
        /// <param name="qtyL">Number of proforma items</param>
        /// <param name="isYourProforma">True if proforma is from client and false if proforma is for users client</param>
        /// <returns>Predicate that allow to filter Proforma table.</returns>
        public static Expression<Func<Proforma, bool>> GetQtyLowerFilter(int? qtyL, bool isYourProforma)
        {
            return qtyL == null ?
                e => true
                : e => isYourProforma ? e.ProformaFutureItems.Select(d => d.Qty).Sum() <= qtyL : e.ProformaOwnedItems.Select(d => d.Qty).Sum() <= qtyL;
        }
        /// <summary>
        /// If quantity value is passed return expression that will filter out object where proforma total item qty is lower then given value. Otherwise return always true.
        /// </summary>
        /// <param name="qtyG">Number of proforma items</param>
        /// <param name="isYourProforma">True if proforma is from client and false if proforma is for users client</param>
        /// <returns>Predicate that allow to filter Proforma table.</returns>
        public static Expression<Func<Proforma, bool>> GetQtyGreaterFilter(int? qtyG, bool isYourProforma)
        {
            return qtyG == null ?
                e => true
                : e => isYourProforma ? e.ProformaFutureItems.Select(d => d.Qty).Sum() >= qtyG : e.ProformaOwnedItems.Select(d => d.Qty).Sum() >= qtyG;
        }
        /// <summary>
        /// If quantity value is passed return expression that will filter out object where proforma total value is greater then given value. Otherwise return always true.
        /// </summary>
        /// <param name="totalL">Proforma total value</param>
        /// <param name="isYourProforma">True if proforma is from client and false if proforma is for users client</param>
        /// <returns>Predicate that allow to filter Proforma table.</returns>
        public static Expression<Func<Proforma, bool>> GetTotalLowerFilter(int? totalL, bool isYourProforma)
        {
            return totalL == null ?
                e => true
                : e => isYourProforma ? e.ProformaFutureItems.Select(d => d.PurchasePrice * d.Qty).Sum() <= totalL : e.ProformaOwnedItems.Select(d => d.SellingPrice * d.Qty).Sum() <= totalL;
        }
        /// <summary>
        /// If total value is passed return expression that will filter out object where proforma total value is lower then given value. Otherwise return always true.
        /// </summary>
        /// <param name="totalG">Proforma total value</param>
        /// <param name="isYourProforma">True if proforma is from client and false if proforma is for users client</param>
        /// <returns>Predicate that allow to filter Proforma table.</returns>
        public static Expression<Func<Proforma, bool>> GetTotalGreaterFilter(int? totalG, bool isYourProforma)
        {
            return totalG == null ?
                e => true
                : e => isYourProforma ? e.ProformaFutureItems.Select(d => d.PurchasePrice * d.Qty).Sum() >= totalG : e.ProformaOwnedItems.Select(d => d.SellingPrice * d.Qty).Sum() >= totalG;
        }
        /// <summary>
        /// If date value is passed return expression that will filter out object where proforma date is greater then given value. Otherwise return always true.
        /// </summary>
        /// <param name="dateL">Date string</param>
        /// <returns>Predicate that allow to filter Proforma table.</returns>
        public static Expression<Func<Proforma, bool>> GetDateLowerFilter(string? dateL)
        {
            return dateL == null ?
                e => true
                : e => e.ProformaDate <= DateTime.ParseExact(dateL, "yyyy-MM-dd", CultureInfo.InvariantCulture);
        }
        /// <summary>
        /// If date value is passed return expression that will filter out object where proforma date is lower then given value.Otherwise return always true.
        /// </summary>
        /// <param name="dateG">Date string</param>
        /// <returns>Predicate that allow to filter Proforma table.</returns>
        public static Expression<Func<Proforma, bool>> GetDateGreaterFilter(string? dateG)
        {
            return dateG == null ?
                e => true
                : e => e.ProformaDate >= DateTime.ParseExact(dateG, "yyyy-MM-dd", CultureInfo.InvariantCulture);
        }
        /// <summary>
        /// If recipient id is passed return expression that will filter out object where proforma recipient is not equal to given value. Otherwise return always true.
        /// </summary>
        /// <param name="recipient">Recipient id.</param>
        /// <param name="isYourProforma">True if proforma is from client and false if proforma is for users client</param>
        /// <returns>Predicate that allow to filter Proforma table.</returns>
        public static Expression<Func<Proforma, bool>> GetRecipientFilter(int? recipient, bool isYourProforma)
        {
            return recipient == null ?
                e => true
                : e => isYourProforma ? e.Seller == recipient : e.Buyer == recipient;
        }
        /// <summary>
        /// If currency shortcut value is passed return expression that will filter out object where proforma currency is not equal to given value. Otherwise return always true.
        /// </summary>
        /// <param name="currency">Shortcut name of currency</param>
        /// <returns>Predicate that allow to filter Proforma table.</returns>
        public static Expression<Func<Proforma, bool>> GetCurrencyFilter(string? currency)
        {
            return currency == null ?
                e => true
                : e => e.CurrencyName == currency;
        }
    }
}
