using database_communicator.Models;
using System.Globalization;
using System.Linq.Expressions;

namespace database_comunicator.FilterClass
{
    public static class RequestFilters
    {
        public static Expression<Func<Request, bool>> GetDateLowerFilter(string? dateL)
        {
            return dateL == null ?
                e => true
                : e => e.CreationDate <= DateTime.ParseExact(dateL, "yyyy-MM-dd", CultureInfo.InvariantCulture);
        }
        public static Expression<Func<Request, bool>> GetDateGreaterFilter(string? dateG)
        {
            return dateG == null ?
                e => true
                : e => e.CreationDate >= DateTime.ParseExact(dateG, "yyyy-MM-dd", CultureInfo.InvariantCulture);
        }
        public static Expression<Func<Request, bool>> GetTypeFilter(string? type)
        {
            return type == null ?
                e => true
                : e => e.ObjectType.ObjectTypeName == type;
        }
        public static Expression<Func<Request, bool>> GetStatusFilter(int? status)
        {
            return status == null ?
                e => true
                : e => e.RequestStatusId == status;
        }
    }
}
