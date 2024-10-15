using System;
using System.Collections.Generic;

namespace database_communicator.Models;

public partial class CreditNote
{
    public int IdCreditNote { get; set; }

    public string CreditNoteNumber { get; set; } = null!;

    public DateTime CreditNoteDate { get; set; }

    public bool InSystem { get; set; }

    public string Note { get; set; } = null!;

    public int InvoiceId { get; set; }
    public bool IsPaid { get; set; }
    public string? CreditFilePath { get; set; }
    public int IdUser { get; set; }
    public virtual AppUser User { get; set; } = null!;

    public virtual IEnumerable<CreditNoteItem> CreditNoteItems { get; set; } = new List<CreditNoteItem>();

    public virtual Invoice Invoice { get; set; } = null!;
}
