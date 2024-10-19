using database_communicator.Models;
using System.Globalization;
using System.Linq.Expressions;

namespace database_comunicator.FilterClass
{
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
    public static class ProformaFilters
    {
        public static Expression<Func<Proforma, bool>> GetQtyLowerFilter(int? qtyL, bool isYourProforma)
        {
            return qtyL == null ?
                e => true
                : e => isYourProforma ? e.ProformaFutureItems.Select(d => d.Qty).Sum() <= qtyL : e.ProformaOwnedItems.Select(d => d.Qty).Sum() <= qtyL;
        }
        public static Expression<Func<Proforma, bool>> GetQtyGreaterFilter(int? qtyG, bool isYourProforma)
        {
            return qtyG == null ?
                e => true
                : e => isYourProforma ? e.ProformaFutureItems.Select(d => d.Qty).Sum() >= qtyG : e.ProformaOwnedItems.Select(d => d.Qty).Sum() >= qtyG;
        }
        public static Expression<Func<Proforma, bool>> GetTotalLowerFilter(int? totalL, bool isYourProforma)
        {
            return totalL == null ?
                e => true
                : e => isYourProforma ? e.ProformaFutureItems.Select(d => d.PurchasePrice * d.Qty).Sum() <= totalL : e.ProformaOwnedItems.Select(d => d.SellingPrice * d.Qty).Sum() <= totalL;
        }
        public static Expression<Func<Proforma, bool>> GetTotalGreaterFilter(int? totalG, bool isYourProforma)
        {
            return totalG == null ?
                e => true
                : e => isYourProforma ? e.ProformaFutureItems.Select(d => d.PurchasePrice * d.Qty).Sum() >= totalG : e.ProformaOwnedItems.Select(d => d.SellingPrice * d.Qty).Sum() >= totalG;
        }
        public static Expression<Func<Proforma, bool>> GetDateLowerFilter(string? dateL)
        {
            return dateL == null ?
                e => true
                : e => e.ProformaDate <= DateTime.ParseExact(dateL, "yyyy-MM-dd", CultureInfo.InvariantCulture);
        }
        public static Expression<Func<Proforma, bool>> GetDateGreaterFilter(string? dateG)
        {
            return dateG == null ?
                e => true
                : e => e.ProformaDate >= DateTime.ParseExact(dateG, "yyyy-MM-dd", CultureInfo.InvariantCulture);
        }
        public static Expression<Func<Proforma, bool>> GetRecipientFilter(int? recipient, bool isYourProforma)
        {
            return recipient == null ?
                e => true
                : e => isYourProforma ? e.Seller == recipient : e.Buyer == recipient;
        }
        public static Expression<Func<Proforma, bool>> GetCurrencyFilter(string? currency)
        {
            return currency == null ?
                e => true
                : e => e.CurrencyName == currency;
        }
    }
}
