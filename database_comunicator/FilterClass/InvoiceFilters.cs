using System.Globalization;
using System.Linq.Expressions;
using database_communicator.Models;

namespace database_communicator.FilterClass
{
    /// <summary>
    /// All properties of invoice that filters are implemented in web client.
    /// </summary>
    public class InvoiceFiltersTemplate
    {
        public string? DateL { get; set; }
        public string? DateG { get; set; }
        public string? DueL { get; set; }
        public string? DueG { get; set; }
        public int? QtyL { get; set; }
        public int? QtyG { get; set; }
        public int? TotalL { get; set; }
        public int? TotalG { get; set; }
        public int? Recipient { get; set; }
        public string? Currency { get; set; }
        public int? PaymentStatus { get; set; }
        public bool? Status { get; set; }
    }
    /// <summary>
    /// Class that represent invoice filters. Contains static methods that return expression that can be passed in LINQ where functions.
    /// </summary>
    public static class InvoiceFilters
    {
        private const string dateFormat = "yyyy-MM-dd";
        /// <summary>
        /// If date value is passed return expression that will filter out object where invoice date is greater then given value. Otherwise return always true.
        /// </summary>
        /// <param name="dateL">Date string</param>
        /// <returns>Predicate that allow to filter Invoice table.</returns>
        public static Expression<Func<Invoice, bool>> GetDateLowerFilter(string? dateL)
        {
            return dateL == null ?
                e => true
                : e => e.InvoiceDate <= DateTime.ParseExact(dateL, InvoiceFilters.dateFormat, CultureInfo.InvariantCulture);
        }
        /// <summary>
        /// If date value is passed return expression that will filter out object where invoice date is lower then given value. Otherwise return always true.
        /// </summary>
        /// <param name="dateL">Date string</param>
        /// <returns>Predicate that allow to filter Invoice table.</returns>
        public static Expression<Func<Invoice, bool>> GetDateGreaterFilter(string? dateG)
        {
            return dateG == null ?
                e => true
                : e => e.InvoiceDate >= DateTime.ParseExact(dateG, InvoiceFilters.dateFormat, CultureInfo.InvariantCulture);
        }
        /// <summary>
        /// If due date value is passed return expression that will filter out object where invoice due date is greater then given value. Otherwise return always true.
        /// </summary>
        /// <param name="dueL">Due date string</param>
        /// <returns>Predicate that allow to filter Invoice table.</returns>
        public static Expression<Func<Invoice, bool>> GetDueLowerFilter(string? dueL)
        {
            return dueL == null ?
                e => true
                : e => e.DueDate <= DateTime.ParseExact(dueL, InvoiceFilters.dateFormat, CultureInfo.InvariantCulture);
        }
        /// <summary>
        /// If due date value is passed return expression that will filter out object where invoice due date is lower then given value. Otherwise return always true.
        /// </summary>
        /// <param name="dueG">Due date string</param>
        /// <returns>Predicate that allow to filter Invoice table.</returns>
        public static Expression<Func<Invoice, bool>> GetDueGreaterFilter(string? dueG)
        {
            return dueG == null ?
                e => true
                : e => e.DueDate >= DateTime.ParseExact(dueG, InvoiceFilters.dateFormat, CultureInfo.InvariantCulture);
        }
        /// <summary>
        /// If quantity value is passed return expression that will filter out object where invoice total number of items is greater then given value. Otherwise return always true.
        /// </summary>
        /// <param name="qtyL">Invoice items number</param>
        /// <param name="isPurchaseInvoice">True if it's purchase invoice, otherwise false.</param>
        /// <returns>Predicate that allow to filter Invoice table.</returns>
        public static Expression<Func<Invoice, bool>> GetQtyLowerFilter(int? qtyL, bool isPurchaseInvoice)
        {
            if (isPurchaseInvoice)
            {
                return qtyL == null ?
                e => true
                : e => e.OwnedItems.SelectMany(d => d.PurchasePrices).Select(d => d.Qty).Sum() <= qtyL;
            }
            return qtyL == null ?
                e => true
                : e => e.SellingPrices.Select(d => d.Qty).Sum() <= qtyL;
        }
        /// <summary>
        /// If quantity value is passed return expression that will filter out object where invoice total number of items is lower then given value. Otherwise return always true.
        /// </summary>
        /// <param name="qtyG">Invoice items number</param>
        /// <param name="isPurchaseInvoice">True if it's purchase invoice, otherwise false.</param>
        /// <returns>Predicate that allow to filter Invoice table.</returns>
        public static Expression<Func<Invoice, bool>> GetQtyGreaterFilter(int? qtyG, bool isPurchaseInvoice)
        {
            if (isPurchaseInvoice)
            {
                return qtyG == null ?
                e => true
                : e => e.OwnedItems.SelectMany(d => d.PurchasePrices).Select(d => d.Qty).Sum() >= qtyG;
            }
            return qtyG == null ?
                e => true
                : e => e.SellingPrices.Select(d => d.Qty).Sum() >= qtyG;
        }
        /// <summary>
        /// If total value is passed return expression that will filter out object where invoice total value is greater then given value. Otherwise return always true.
        /// </summary>
        /// <param name="totalL">Total invoice value</param>
        /// <param name="isPurchaseInvoice">True if it's purchase invoice, otherwise false.</param>
        /// <returns>Predicate that allow to filter Invoice table.</returns>
        public static Expression<Func<Invoice, bool>> GetTotalLowerFilter(int? totalL, bool isPurchaseInvoice)
        {
            if (isPurchaseInvoice)
            {
                return totalL == null ?
                e => true
                : e => e.OwnedItems.SelectMany(d => d.PurchasePrices).Select(d => d.Qty * d.Price).Sum() <= totalL;
            }
            return totalL == null ?
                e => true
                : e => e.SellingPrices.Select(d => d.Qty * d.Price).Sum() <= totalL;
        }
        /// <summary>
        /// If total value is passed return expression that will filter out object where invoice total value is lower then given value. Otherwise return always true.
        /// </summary>
        /// <param name="totalG">Total invoice value</param>
        /// <param name="isPurchaseInvoice">True if it's purchase invoice, otherwise false.</param>
        /// <returns>Predicate that allow to filter Invoice table.</returns>
        public static Expression<Func<Invoice, bool>> GetTotalGreaterFilter(int? totalG, bool isPurchaseInvoice)
        {
            if (isPurchaseInvoice)
            {
                return totalG == null ?
                e => true
                : e => e.OwnedItems.SelectMany(d => d.PurchasePrices).Select(d => d.Qty * d.Price).Sum() >= totalG;
            }
            return totalG == null ?
                e => true
                : e => e.SellingPrices.Select(d => d.Qty * d.Price).Sum() >= totalG;
        }
        /// <summary>
        /// If recipient id is passed return expression that will filter out object where invoice recipient is not equal to given value. Otherwise return always true.
        /// </summary>
        /// <param name="recipient">Id of recipient.</param>
        /// <param name="isPurchaseInvoice">True if it's purchase invoice, otherwise false.</param>
        /// <returns>Predicate that allow to filter Invoice table.</returns>
        public static Expression<Func<Invoice, bool>> GetRecipientFilter(int? recipient, bool isPurchaseInvoice)
        {
            return recipient == null ?
                e => true
                : e => isPurchaseInvoice ? e.Seller == recipient : e.Buyer == recipient;
        }
        /// <summary>
        /// If currency shortcut value is passed return expression that will filter out object where invoice currency is not equal to given value. Otherwise return always true.
        /// </summary>
        /// <param name="currency">Shortcut name of currency</param>
        /// <returns>Predicate that allow to filter Invoice table.</returns>
        public static Expression<Func<Invoice, bool>> GetCurrencyFilter(string? currency)
        {
            return currency == null ?
                e => true
                : e => e.CurrencyName == currency;
        }
        /// <summary>
        /// If payment status id value is passed return expression that will filter out object where invoice payment status is not equal to given value. Otherwise return always true.
        /// </summary>
        /// <param name="paymentStatus">Payment status id</param>
        /// <returns>Predicate that allow to filter Invoice table.</returns>
        public static Expression<Func<Invoice, bool>> GetPaymentStatusFilter(int? paymentStatus)
        {
            return paymentStatus == null ?
                e => true
                : e => e.PaymentsStatusId == paymentStatus;
        }
        /// <summary>
        /// If payment status value is passed return expression that will filter out object where invoice payment status is not equal to given value. Otherwise return always true.
        /// </summary>
        /// <param name="status">Boolean value. If true will search for invoices that have their InSystem property true. Reverse in false case.</param>
        /// <returns>Predicate that allow to filter Invoice table.</returns>
        public static Expression<Func<Invoice, bool>> GetStatusFilter(bool? status)
        {
            return status == null ?
                e => true
                : e => e.InSystem == status;
        }
    }
}
