using System.Globalization;
using System.Linq.Expressions;
using database_communicator.Models;

namespace database_communicator.FilterClass
{
    /// <summary>
    /// Class that represent request filters. Contains static methods that return expression that can be passed in LINQ where functions.
    /// </summary>
    public static class RequestFilters
    {
        /// <summary>
        /// If date value is passed return expression that will filter out object where request creation date is greater then given value. Otherwise return always true.
        /// </summary>
        /// <param name="dateL">Request creation date string</param>
        /// <returns>Predicate that allow to filter Request table.</returns>
        public static Expression<Func<Request, bool>> GetDateLowerFilter(string? dateL)
        {
            return dateL == null ?
                e => true
                : e => e.CreationDate <= DateTime.ParseExact(dateL, "yyyy-MM-dd", CultureInfo.InvariantCulture);
        }
        /// <summary>
        /// If date value is passed return expression that will filter out object where request creation date is lower then given value. Otherwise return always true.
        /// </summary>
        /// <param name="dateG">Request creation date string</param>
        /// <returns>Predicate that allow to filter Request table.</returns>
        public static Expression<Func<Request, bool>> GetDateGreaterFilter(string? dateG)
        {
            return dateG == null ?
                e => true
                : e => e.CreationDate >= DateTime.ParseExact(dateG, "yyyy-MM-dd", CultureInfo.InvariantCulture);
        }
        /// <summary>
        /// If object type name is passed return expression that will filter out object where request object type is not equal to given value. Otherwise return always true.
        /// </summary>
        /// <param name="type">Name of object type.</param>
        /// <returns>Predicate that allow to filter Request table.</returns>
        public static Expression<Func<Request, bool>> GetTypeFilter(string? type)
        {
            return type == null ?
                e => true
                : e => e.ObjectType.ObjectTypeName == type;
        }
        /// <summary>
        /// If request status id is passed return expression that will filter out object where request status is not equal to given value. Otherwise return always true.
        /// </summary>
        /// <param name="status">Id of request status id.</param>
        /// <returns>Predicate that allow to filter Request table.</returns>
        public static Expression<Func<Request, bool>> GetStatusFilter(int? status)
        {
            return status == null ?
                e => true
                : e => e.RequestStatusId == status;
        }
    }
}
