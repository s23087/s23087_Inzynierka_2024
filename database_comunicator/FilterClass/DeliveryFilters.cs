using database_communicator.Models;
using System.Linq.Expressions;

namespace database_comunicator.FilterClass
{
    public static class DeliveryFilters
    {
        public static Expression<Func<Delivery, bool>> GetEstimatedLowerFilter(string? estimatedL)
        {
            return estimatedL == null ?
                e => true
                : e => e.EstimatedDeliveryDate <= DateTime.Parse(estimatedL);
        }
        public static Expression<Func<Delivery, bool>> GetEstimatedGreaterFilter(string? estimatedG)
        {
            return estimatedG == null ?
                e => true
                : e => e.EstimatedDeliveryDate >= DateTime.Parse(estimatedG);
        }
        public static Expression<Func<Delivery, bool>> GetDeliveredLowerFilter(string? deliveredL)
        {
            return deliveredL == null ?
                e => true
                : e => e.DeliveryDate <= DateTime.Parse(deliveredL);
        }
        public static Expression<Func<Delivery, bool>> GetDeliveredGreaterFilter(string? deliveredG)
        {
            return deliveredG == null ?
                e => true
                : e => e.DeliveryDate >= DateTime.Parse(deliveredG);
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
