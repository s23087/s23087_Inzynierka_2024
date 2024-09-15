using System;
using System.Collections.Generic;
using database_comunicator.Models;

namespace database_comunicator.Models;

public partial class CreditNoteItem
{
    public int CreditItemId { get; set; }
    public int CreditNoteId { get; set; }

    public int PurchasePriceId { get; set; }

    public int Qty { get; set; }

    public decimal NewPrice { get; set; }
    public bool IncludeQty { get; set; }

    public virtual ICollection<CalculatedCreditNotePrice> CalculatedCreditNotePrices { get; set; } = new List<CalculatedCreditNotePrice>();

    public virtual CreditNote CreditNote { get; set; } = null!;

    public virtual PurchasePrice PurchasePrice { get; set; } = null!;
}
