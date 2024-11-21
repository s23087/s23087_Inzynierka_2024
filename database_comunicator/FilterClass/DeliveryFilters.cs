using database_communicator.Models;
using System.Globalization;
using System.Linq.Expressions;

namespace database_communicator.FilterClass
{
    /// <summary>
    /// All properties of delivery that filters are implemented in web client.
    /// </summary>
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
        /// <summary>
        /// If estimated date value is passed return expression that will filter out object where delivery estimated date is greater then given value. Otherwise return always true.
        /// </summary>
        /// <param name="estimatedL">Estimated date string</param>
        /// <returns>Predicate that allow to filter Delivery table.</returns>
        public static Expression<Func<Delivery, bool>> GetEstimatedLowerFilter(string? estimatedL)
        {
            return estimatedL == null ?
                e => true
                : e => e.EstimatedDeliveryDate <= DateTime.ParseExact(estimatedL, DeliveryFilters.dateFormat, CultureInfo.InvariantCulture);
        }
        /// <summary>
        /// If estimated date value is passed return expression that will filter out object where delivery estimated date is lower then given value. Otherwise return always true.
        /// </summary>
        /// <param name="estimatedG">Estimated date string</param>
        /// <returns>Predicate that allow to filter Delivery table.</returns>
        public static Expression<Func<Delivery, bool>> GetEstimatedGreaterFilter(string? estimatedG)
        {
            return estimatedG == null ?
                e => true
                : e => e.EstimatedDeliveryDate >= DateTime.ParseExact(estimatedG, DeliveryFilters.dateFormat, CultureInfo.InvariantCulture);
        }
        /// <summary>
        /// If delivered date value is passed return expression that will filter out object where delivery delivered date is greater then given value. Otherwise return always true.
        /// </summary>
        /// <param name="deliveredL">Delivered date string</param>
        /// <returns>Predicate that allow to filter Delivery table.</returns>
        public static Expression<Func<Delivery, bool>> GetDeliveredLowerFilter(string? deliveredL)
        {
            return deliveredL == null ?
                e => true
                : e => e.DeliveryDate <= DateTime.ParseExact(deliveredL, DeliveryFilters.dateFormat, CultureInfo.InvariantCulture);
        }
        /// <summary>
        /// If delivered date value is passed return expression that will filter out object where delivery delivered date is lower then given value. Otherwise return always true.
        /// </summary>
        /// <param name="deliveredG">Delivered date string</param>
        /// <returns>Predicate that allow to filter Delivery table.</returns>
        public static Expression<Func<Delivery, bool>> GetDeliveredGreaterFilter(string? deliveredG)
        {
            return deliveredG == null ?
                e => true
                : e => e.DeliveryDate >= DateTime.ParseExact(deliveredG, DeliveryFilters.dateFormat, CultureInfo.InvariantCulture);
        }
        /// <summary>
        /// If recipient id is passed return expression that will filter out object where delivery recipient is not equal to given value. Otherwise return always true.
        /// </summary>
        /// <param name="recipient">Id of recipient.</param>
        /// <param name="IsDeliveryToUser">True if delivery is coming to user, false if to client.</param>
        /// <returns>Predicate that allow to filter Delivery table.</returns>
        public static Expression<Func<Delivery, bool>> GetRecipientFilter(int? recipient, bool IsDeliveryToUser)
        {
            return recipient == null ?
                e => true
                : e => IsDeliveryToUser ? e.Proforma.Seller == recipient : e.Proforma.Buyer == recipient;
        }
        /// <summary>
        /// If status id value is passed return expression that will filter out object where delivery status is not equal to given value. Otherwise return always true.
        /// </summary>
        /// <param name="status">Id of delivery status.</param>
        /// <returns>Predicate that allow to filter Delivery table.</returns>
        public static Expression<Func<Delivery, bool>> GetStatusFilter(int? status)
        {
            return status == null ?
                e => true
                : e => e.DeliveryStatusId == status;
        }
        /// <summary>
        /// If delivery company id value is passed return expression that will filter out object where delivery company is not equal to given value. Otherwise return always true.
        /// </summary>
        /// <param name="company">Id of delivery company.</param>
        /// <returns>Predicate that allow to filter Delivery table.</returns>
        public static Expression<Func<Delivery, bool>> GetCompanyFilter(int? company)
        {
            return company == null ?
                e => true
                : e => e.DeliveryCompanyId == company;
        }

        /// <summary>
        /// If waybill value is passed return expression that will filter out object where delivery waybills do not contain given value. Otherwise return always true.
        /// </summary>
        /// <param name="waybill">Waybill value.</param>
        /// <returns>Predicate that allow to filter Delivery table.</returns>
        public static Expression<Func<Delivery, bool>> GetWaybillFilter(string? waybill)
        {
            return waybill == null ?
                e => true
                : e => e.Waybills.Any(x => x.WaybillValue == waybill);
        }
    }
}
