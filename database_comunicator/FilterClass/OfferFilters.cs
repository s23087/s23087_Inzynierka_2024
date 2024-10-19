using database_communicator.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Globalization;
using System.Linq.Expressions;

namespace database_comunicator.FilterClass
{
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
    public static class OfferFilters
    {
        public static Expression<Func<Offer, bool>> GetStatusFilter(string? status)
        {
            return status == null ?
                e => true
                : e => e.OfferStatus.StatusName == status;
        }
        public static Expression<Func<Offer, bool>> GetCurrencyFilter(string? currency)
        {
            return currency == null ?
                e => true
                : e => e.CurrencyName == currency;
        }
        public static Expression<Func<Offer, bool>> GetTypeFilter(string? type)
        {
            return type == null ?
                e => true
                : e => e.PathToFile.EndsWith(type);
        }
        public static Expression<Func<Offer, bool>> GetTotalLowerFilter(int? totalL)
        {
            return totalL == null ?
                e => true
                : e => e.OfferItems.Count < totalL;
        }
        public static Expression<Func<Offer, bool>> GetTotalGreaterFilter(int? totalG)
        {
            return totalG == null ?
                e => true
                : e => e.OfferItems.Count < totalG;
        }
        public static Expression<Func<Offer, bool>> GetCreatedLowerFilter(string? createdL)
        {
            return createdL == null ?
                e => true
                : e => e.CreationDate <= DateTime.ParseExact(createdL, "yyyy-MM-dd", CultureInfo.InvariantCulture);
        }
        public static Expression<Func<Offer, bool>> GetCreatedGreaterFilter(string? createdG)
        {
            return createdG == null ?
                e => true
                : e => e.CreationDate >= DateTime.ParseExact(createdG, "yyyy-MM-dd", CultureInfo.InvariantCulture);
        }
        public static Expression<Func<Offer, bool>> GetModifiedLowerFilter(string? modifiedL)
        {
            return modifiedL == null ?
                e => true
                : e => e.ModificationDate <= DateTime.ParseExact(modifiedL, "yyyy-MM-dd", CultureInfo.InvariantCulture);
        }
        public static Expression<Func<Offer, bool>> GetModifiedGreaterFilter(string? modifiedG)
        {
            return modifiedG == null ?
                e => true
                : e => e.ModificationDate >= DateTime.ParseExact(modifiedG, "yyyy-MM-dd", CultureInfo.InvariantCulture);
        }
    }
}
