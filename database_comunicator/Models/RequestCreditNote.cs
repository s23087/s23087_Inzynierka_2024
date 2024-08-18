using System;
using System.Collections.Generic;

namespace database_comunicator.Models;

public partial class RequestCreditNote
{
    public int CreditNoteId { get; set; }

    public int RequestId { get; set; }

    public virtual CreditNote CreditNote { get; set; } = null!;

    public virtual Request Request { get; set; } = null!;
}
