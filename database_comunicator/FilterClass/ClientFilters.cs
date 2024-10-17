using database_communicator.Models;
using System.Linq.Expressions;

namespace database_comunicator.FilterClass
{
    public static class ClientFilters
    {
        public static Expression<Func<Organization, bool>> GetCountryFilter(int? country)
        {
            return country == null ?
                e => true
                : e => e.CountryId == country;
        }
    }
}
