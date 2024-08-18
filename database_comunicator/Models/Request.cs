using System;
using System.Collections.Generic;

namespace database_comunicator.Models;

public partial class Request
{
    public int RequestId { get; set; }

    public int IdUser { get; set; }

    public int RequestStatusId { get; set; }

    public virtual OrgUser IdUserNavigation { get; set; } = null!;

    public virtual ICollection<RequestCreditNote> RequestCreditNotes { get; set; } = new List<RequestCreditNote>();

    public virtual RequestStatus RequestStatus { get; set; } = null!;

    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();

    public virtual ICollection<Note> Notes { get; set; } = new List<Note>();

    public virtual ICollection<Proforma> Proformas { get; set; } = new List<Proforma>();
}
