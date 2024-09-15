using System;
using System.Collections.Generic;

namespace database_comunicator.Models;

public partial class CreditNote
{
    public int IdCreditNote { get; set; }

    public string CreditNoteNumber { get; set; } = null!;

    public DateTime CreditNoteDate { get; set; }

    public bool InSystem { get; set; }

    public string Note { get; set; } = null!;

    public int InvoiceId { get; set; }

    public virtual CreditNoteItem? CreditNoteItem { get; set; }

    public virtual Invoice Invoice { get; set; } = null!;

    public virtual RequestCreditNote? RequestCreditNote { get; set; }
}
