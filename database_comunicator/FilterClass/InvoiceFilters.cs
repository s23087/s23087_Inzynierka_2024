using database_communicator.Models;
using System.Globalization;
using System.Linq.Expressions;

namespace database_comunicator.FilterClass
{
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
    public static class InvoiceFilters
    {
        private const string dateFormat = "yyyy-MM-dd";

        public static Expression<Func<Invoice, bool>> GetDateLowerFilter(string? dateL)
        {
            return dateL == null ?
                e => true
                : e => e.InvoiceDate <= DateTime.ParseExact(dateL, InvoiceFilters.dateFormat, CultureInfo.InvariantCulture);
        }
        public static Expression<Func<Invoice, bool>> GetDateGreaterFilter(string? dateG)
        {
            return dateG == null ?
                e => true
                : e => e.InvoiceDate >= DateTime.ParseExact(dateG, InvoiceFilters.dateFormat, CultureInfo.InvariantCulture);
        }
        public static Expression<Func<Invoice, bool>> GetDueLowerFilter(string? dueL)
        {
            return dueL == null ?
                e => true
                : e => e.DueDate <= DateTime.ParseExact(dueL, InvoiceFilters.dateFormat, CultureInfo.InvariantCulture);
        }
        public static Expression<Func<Invoice, bool>> GetDueGreaterFilter(string? dueG)
        {
            return dueG == null ?
                e => true
                : e => e.DueDate >= DateTime.ParseExact(dueG, InvoiceFilters.dateFormat, CultureInfo.InvariantCulture);
        }
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
        public static Expression<Func<Invoice, bool>> GetRecipientFilter(int? recipient, bool isPurchaseInvoice)
        {
            return recipient == null ?
                e => true
                : e => isPurchaseInvoice ? e.Seller == recipient : e.Buyer == recipient;
        }
        public static Expression<Func<Invoice, bool>> GetCurrencyFilter(string? currency)
        {
            return currency == null ?
                e => true
                : e => e.CurrencyName == currency;
        }
        public static Expression<Func<Invoice, bool>> GetPaymentStatusFilter(int? paymentStatus)
        {
            return paymentStatus == null ?
                e => true
                : e => e.PaymentsStatusId == paymentStatus;
        }
        public static Expression<Func<Invoice, bool>> GetStatusFilter(bool? status)
        {
            return status == null ?
                e => true
                : e => e.InSystem == status;
        }
    }
}
