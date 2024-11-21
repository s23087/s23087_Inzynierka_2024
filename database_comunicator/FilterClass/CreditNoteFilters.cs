using database_communicator.Models;
using System.Globalization;
using System.Linq.Expressions;

namespace database_communicator.FilterClass
{
    /// <summary>
    /// All properties of credit note that filters are implemented in web client.
    /// </summary>
    public class CreditNoteFiltersTemplate
    {
        public string? DateL { get; set; }
        public string? DateG { get; set; }
        public int? QtyL { get; set; }
        public int? QtyG { get; set; }
        public int? TotalL { get; set; }
        public int? TotalG { get; set; }
        public int? Recipient { get; set; }
        public string? Currency { get; set; }
        public bool? PaymentStatus { get; set; }
        public bool? Status { get; set; }
    }
    /// <summary>
    /// Class that represent credit note filters. Contains static methods that return expression that can be passed in LINQ where functions.
    /// </summary>
    public static class CreditNoteFilters
    {
        /// <summary>
        /// If date value is passed return expression that will filter out object where credit note date is greater then given value. Otherwise return always true.
        /// </summary>
        /// <param name="dateL">Date string</param>
        /// <returns>Predicate that allow to filter Credit note table.</returns>
        public static Expression<Func<CreditNote, bool>> GetDateLowerFilter(string? dateL)
        {
            return dateL == null ?
                e => true
                : e => e.CreditNoteDate <= DateTime.ParseExact(dateL, "yyyy-MM-dd", CultureInfo.InvariantCulture);
        }
        /// <summary>
        /// If date value is passed return expression that will filter out object where credit note date is lower then given value.Otherwise return always true.
        /// </summary>
        /// <param name="dateG">Date string</param>
        /// <returns>Predicate that allow to filter Credit note table.</returns>
        public static Expression<Func<CreditNote, bool>> GetDateGreaterFilter(string? dateG)
        {
            return dateG == null ?
                e => true
                : e => e.CreditNoteDate >= DateTime.ParseExact(dateG, "yyyy-MM-dd", CultureInfo.InvariantCulture);
        }
        /// <summary>
        /// If quantity value is passed return expression that will filter out object where credit note item qty is greater then given value. Otherwise return always true.
        /// </summary>
        /// <param name="qtyL">Number of items</param>
        /// <returns>Predicate that allow to filter Credit note table.</returns>
        public static Expression<Func<CreditNote, bool>> GetQtyLowerFilter(int? qtyL)
        {
            return qtyL == null ?
                e => true
                : 
                e => (e.CreditNoteItems.Any(d => d.Qty > 0) 
                ? e.CreditNoteItems.Where(d => d.Qty > 0).Select(d => d.Qty).Sum() 
                : e.CreditNoteItems.Sum(d => d.Qty)) <= qtyL;
        }
        /// <summary>
        /// If quantity value is passed return expression that will filter out object where credit note item qty is lower then given value. Otherwise return always true.
        /// </summary>
        /// <param name="qtyG">Number of items</param>
        /// <returns>Predicate that allow to filter Credit note table.</returns>
        public static Expression<Func<CreditNote, bool>> GetQtyGreaterFilter(int? qtyG)
        {
            return qtyG == null ?
                e => true
                : e => (e.CreditNoteItems.Any(d => d.Qty > 0) 
                ? e.CreditNoteItems.Where(d => d.Qty > 0).Select(d => d.Qty).Sum() 
                : e.CreditNoteItems.Sum(d => d.Qty)) >= qtyG;
        }
        /// <summary>
        /// If total value is passed return expression that will filter out object where credit note total value is greater then given value. Otherwise return always true.
        /// </summary>
        /// <param name="totalL">Total value of credit note</param>
        /// <returns>Predicate that allow to filter Credit note table.</returns>
        public static Expression<Func<CreditNote, bool>> GetPriceLowerFilter(int? totalL)
        {
            return totalL == null ?
                e => true
                : e => e.CreditNoteItems.Sum(d => d.NewPrice * d.Qty) <= totalL;
        }
        /// <summary>
        /// If total value is passed return expression that will filter out object where credit note total value is lower then given value. Otherwise return always true.
        /// </summary>
        /// <param name="totalG">Total value of credit note</param>
        /// <returns>Predicate that allow to filter Credit note table.</returns>
        public static Expression<Func<CreditNote, bool>> GetPriceGreaterFilter(int? totalG)
        {
            return totalG == null ?
                e => true
                : e => e.CreditNoteItems.Sum(d => d.NewPrice * d.Qty) >= totalG;
        }
        /// <summary>
        /// If recipient id is passed return expression that will filter out object where credit note recipient is not equal to given value. Otherwise return always true.
        /// </summary>
        /// <param name="recipient">Id of recipient.</param>
        /// <param name="yourCreditNotes">True if credit note is for user, false if for client.</param>
        /// <returns>Predicate that allow to filter Credit note table.</returns>
        public static Expression<Func<CreditNote, bool>> GetRecipientFilter(int? recipient, bool yourCreditNotes)
        {
            return recipient == null ?
                e => true
                : e => yourCreditNotes ? e.Invoice.Seller == recipient : e.Invoice.Buyer == recipient;
        }
        /// <summary>
        /// If currency shortcut value is passed return expression that will filter out object where credit note currency is not equal to given value. Otherwise return always true.
        /// </summary>
        /// <param name="currency">Shortcut name of currency</param>
        /// <returns>Predicate that allow to filter Credit note table.</returns>
        public static Expression<Func<CreditNote, bool>> GetCurrencyFilter(string? currency)
        {
            return currency == null ?
                e => true
                : e => e.Invoice.CurrencyName == currency;
        }
        /// <summary>
        /// If payment status value is passed return expression that will filter out object where credit note payment status is not equal to given value. Otherwise return always true.
        /// </summary>
        /// <param name="paymentStatus">Boolean value. If true will search for paid credit notes, otherwise will look for unpaid.</param>
        /// <returns>Predicate that allow to filter Credit note table.</returns>
        public static Expression<Func<CreditNote, bool>> GetPaymentStatusFilter(bool? paymentStatus)
        {
            return paymentStatus == null ?
                e => true
                : e => e.IsPaid == paymentStatus;
        }
        /// <summary>
        /// If payment status value is passed return expression that will filter out object where credit note payment status is not equal to given value. Otherwise return always true.
        /// </summary>
        /// <param name="status">Boolean value. If true will search for credit notes that have their InSystem property true. Reverse in false case.</param>
        /// <returns>Predicate that allow to filter Credit note table.</returns>
        public static Expression<Func<CreditNote, bool>> GetStatusFilter(bool? status)
        {
            return status == null ?
                e => true
                : e => e.InSystem == status;
        }
    }
}
