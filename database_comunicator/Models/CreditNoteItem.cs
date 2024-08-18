using System;
using System.Collections.Generic;

namespace database_comunicator.Models;

public partial class CreditNoteItem
{
    public int CreditNoteId { get; set; }

    public int OwnedItemId { get; set; }

    public int InvoiceId { get; set; }

    public int Qty { get; set; }

    public decimal Amount { get; set; }

    public virtual CreditNote CreditNote { get; set; } = null!;

    public virtual OwnedItem OwnedItem { get; set; } = null!;
}
