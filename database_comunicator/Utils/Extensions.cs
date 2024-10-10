using System.Linq.Expressions;

namespace database_comunicator.Utils
{
    public static class Extensions
    {
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
        public static IOrderedEnumerable<TSource> OrderByWithDirection<TSource, TKey>
            (
                this IQueryable<TSource> source,
                Func<TSource, TKey> orderBy,
                bool descending
            )
        {
            return descending ? source.OrderByDescending(orderBy)
                : source.OrderBy(orderBy);
        }
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
