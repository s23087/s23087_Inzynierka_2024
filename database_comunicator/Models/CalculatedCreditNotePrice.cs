using System;
using System.Collections.Generic;

namespace database_comunicator.Models;

public partial class CalculatedCreditNotePrice
{
    public string CurrencyName { get; set; } = null!;

    public DateTime UpdateDate { get; set; }

    public int CreditItemId { get; set; }

    public decimal Price { get; set; }

    public virtual CreditNoteItem CreditNoteItem { get; set; } = null!;

    public virtual CurrencyValue CurrencyValue { get; set; } = null!;
}
