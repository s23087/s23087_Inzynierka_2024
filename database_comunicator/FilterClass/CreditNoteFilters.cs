using database_communicator.Models;
using System.Globalization;
using System.Linq.Expressions;

namespace database_comunicator.FilterClass
{
    public static class CreditNoteFilters
    {
        public static Expression<Func<CreditNote, bool>> GetDateLowerFilter(string? dateL)
        {
            return dateL == null ?
                e => true
                : e => e.CreditNoteDate <= DateTime.ParseExact(dateL, "yyyy-MM-dd", CultureInfo.InvariantCulture);
        }
        public static Expression<Func<CreditNote, bool>> GetDateGreaterFilter(string? dateG)
        {
            return dateG == null ?
                e => true
                : e => e.CreditNoteDate >= DateTime.ParseExact(dateG, "yyyy-MM-dd", CultureInfo.InvariantCulture);
        }
        public static Expression<Func<CreditNote, bool>> GetQtyLowerFilter(int? qtyL)
        {
            return qtyL == null ?
                e => true
                : 
                e => (e.CreditNoteItems.Any(d => d.Qty > 0) 
                ? e.CreditNoteItems.Where(d => d.Qty > 0).Select(d => d.Qty).Sum() 
                : e.CreditNoteItems.Sum(d => d.Qty)) <= qtyL;
        }
        public static Expression<Func<CreditNote, bool>> GetQtyGreaterFilter(int? qtyG)
        {
            return qtyG == null ?
                e => true
                : e => (e.CreditNoteItems.Any(d => d.Qty > 0) 
                ? e.CreditNoteItems.Where(d => d.Qty > 0).Select(d => d.Qty).Sum() 
                : e.CreditNoteItems.Sum(d => d.Qty)) >= qtyG;
        }
        public static Expression<Func<CreditNote, bool>> GetPriceLowerFilter(int? totalL)
        {
            return totalL == null ?
                e => true
                : e => e.CreditNoteItems.Sum(d => d.NewPrice * d.Qty) <= totalL;
        }
        public static Expression<Func<CreditNote, bool>> GetPriceGreaterFilter(int? totalG)
        {
            return totalG == null ?
                e => true
                : e => e.CreditNoteItems.Sum(d => d.NewPrice * d.Qty) >= totalG;
        }
        public static Expression<Func<CreditNote, bool>> GetRecipientFilter(int? recipient, bool yourCreditNotes)
        {
            return recipient == null ?
                e => true
                : e => yourCreditNotes ? e.Invoice.Seller == recipient : e.Invoice.Buyer == recipient;
        }
        public static Expression<Func<CreditNote, bool>> GetCurrencyFilter(string? currency)
        {
            return currency == null ?
                e => true
                : e => e.Invoice.CurrencyName == currency;
        }
        public static Expression<Func<CreditNote, bool>> GetPaymentStatusFilter(bool? paymentStatus)
        {
            return paymentStatus == null ?
                e => true
                : e => e.IsPaid == paymentStatus;
        }
        public static Expression<Func<CreditNote, bool>> GetStatusFilter(bool? status)
        {
            return status == null ?
                e => true
                : e => e.InSystem == status;
        }
    }
}
