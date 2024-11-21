using System.Linq.Expressions;

namespace database_communicator.Utils
{
    /// <summary>
    /// This class holds Extension function for LINQ list objects. These function allow to order objects with chosen direction.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Allow to order by on IOrderedEnumerable object with given direction.
        /// </summary>
        /// <typeparam name="TSource">Wildcard for Object type that IOrderedEnumerable holds</typeparam>
        /// <typeparam name="TKey">Wildcard for function that will direct after which object property order by will be made</typeparam>
        /// <param name="source">IOrderedEnumerable object</param>
        /// <param name="orderBy">Function that will direct after which object property order by will be made</param>
        /// <param name="descending">True if you want descending direction or false if ascending</param>
        /// <returns>Ordered source</returns>
        public static IOrderedEnumerable<TSource> OrderByWithDirection<TSource, TKey>
            (
                this IEnumerable<TSource> source,
                Func<TSource, TKey> orderBy,
                bool descending
            )
        {
            return descending ? source.OrderByDescending(orderBy) 
                : source.OrderBy(orderBy);
        }
        /// <summary>
        /// Allow to order by on IOrderedQueryable object with given direction.
        /// </summary>
        /// <typeparam name="TSource">Wildcard for Object type that IOrderedQueryable holds</typeparam>
        /// <typeparam name="TKey">Wildcard for function that will direct after which object property order by will be made</typeparam>
        /// <param name="source">IOrderedQueryable object</param>
        /// <param name="orderBy">Function that will direct after which object property order by will be made</param>
        /// <param name="descending">True if you want descending direction or false if ascending</param>
        /// <returns>Ordered source</returns>
        public static IOrderedQueryable<TSource> OrderByWithDirection<TSource, TKey>
            (
                this IQueryable<TSource> source,
                Expression<Func<TSource, TKey>> orderBy,
                bool descending
            )
        {
            return descending ? source.OrderByDescending(orderBy)
                : source.OrderBy(orderBy);
        }
    }
}
