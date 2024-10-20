using database_communicator.Models;
using System.Globalization;
using System.Linq.Expressions;

namespace database_comunicator.FilterClass
{
    public class DeliveryFiltersTemplate
    {
        public string? EstimatedL { get; set; } 
        public string? EstimatedG { get; set; }
        public string? DeliveredL { get; set; }
        public string? DeliveredG { get; set; }
        public int? Recipient { get; set; }
        public int? Status { get; set; } 
        public int? Company { get; set; } 
        public string? Waybill { get; set; }
    }
    public static class DeliveryFilters
    {
        private const string dateFormat = "yyyy-MM-dd";

        public static Expression<Func<Delivery, bool>> GetEstimatedLowerFilter(string? estimatedL)
        {
            return estimatedL == null ?
                e => true
                : e => e.EstimatedDeliveryDate <= DateTime.ParseExact(estimatedL, DeliveryFilters.dateFormat, CultureInfo.InvariantCulture);
        }
        public static Expression<Func<Delivery, bool>> GetEstimatedGreaterFilter(string? estimatedG)
        {
            return estimatedG == null ?
                e => true
                : e => e.EstimatedDeliveryDate >= DateTime.ParseExact(estimatedG, DeliveryFilters.dateFormat, CultureInfo.InvariantCulture);
        }
        public static Expression<Func<Delivery, bool>> GetDeliveredLowerFilter(string? deliveredL)
        {
            return deliveredL == null ?
                e => true
                : e => e.DeliveryDate <= DateTime.ParseExact(deliveredL, DeliveryFilters.dateFormat, CultureInfo.InvariantCulture);
        }
        public static Expression<Func<Delivery, bool>> GetDeliveredGreaterFilter(string? deliveredG)
        {
            return deliveredG == null ?
                e => true
                : e => e.DeliveryDate >= DateTime.ParseExact(deliveredG, DeliveryFilters.dateFormat, CultureInfo.InvariantCulture);
        }

        public static Expression<Func<Delivery, bool>> GetRecipientFilter(int? recipient, bool IsDeliveryToUser)
        {
            return recipient == null ?
                e => true
                : e => IsDeliveryToUser ? e.Proforma.Seller == recipient : e.Proforma.Buyer == recipient;
        }
        public static Expression<Func<Delivery, bool>> GetStatusFilter(int? status)
        {
            return status == null ?
                e => true
                : e => e.DeliveryStatusId == status;
        }
        public static Expression<Func<Delivery, bool>> GetCompanyFilter(int? company)
        {
            return company == null ?
                e => true
                : e => e.DeliveryCompanyId == company;
        }
        public static Expression<Func<Delivery, bool>> GetWaybillFilter(string? waybill)
        {
            return waybill == null ?
                e => true
                : e => e.Waybills.Any(x => x.WaybillValue == waybill);
        }
    }
}
