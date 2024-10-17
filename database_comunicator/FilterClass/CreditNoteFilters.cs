using database_communicator.Models;
using System.Linq.Expressions;

namespace database_comunicator.FilterClass
{
    public static class CreditNoteFilters
    {
        public static Expression<Func<CreditNote, bool>> GetDateLowerFilter(string? dateL)
        {
            return dateL == null ?
                e => true
                : e => e.CreditNoteDate <= DateTime.Parse(dateL);
        }
    }
}
