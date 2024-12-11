using System.Linq.Expressions;
using database_communicator.Models;

namespace database_communicator.FilterClass
{
    /// <summary>
    /// Class that represent client filters. Contains static methods that return expression that can be passed in LINQ where functions.
    /// </summary>
    public static class ClientFilters
    {
        /// <summary>
        /// If country id is passed return expression that will filter out object where country id are not equal to given value. Otherwise return always true.
        /// </summary>
        /// <param name="country">Id of country</param>
        /// <returns>Predicate that allow to filter Client table.</returns>
        public static Expression<Func<Organization, bool>> GetCountryFilter(int? country)
        {
            return country == null ?
                e => true
                : e => e.CountryId == country;
        }
    }
}
