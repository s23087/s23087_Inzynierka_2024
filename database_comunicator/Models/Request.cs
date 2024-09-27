using System;
using System.Collections.Generic;

namespace database_comunicator.Models;

public partial class Request
{
    public int RequestId { get; set; }
    public int IdUserCreator { get; set; }
    public int IdUserReciver { get; set; }

    public int RequestStatusId { get; set; }
    public int ObjectTypeId { get; set; }
    public string? FilePath { get; set; }
    public string Note { get; set; } = null!;
    public string Title { get; set; } = null!;
    public DateTime CreationDate { get; set; }

    public virtual AppUser UserReciver { get; set; } = null!;
    public virtual AppUser UserCreator { get; set; } = null!;
    public virtual ObjectType ObjectType { get; set; } = null!;

    public virtual RequestStatus RequestStatus { get; set; } = null!;

    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();

    public virtual ICollection<Note> Notes { get; set; } = new List<Note>();

    public virtual ICollection<Proforma> Proformas { get; set; } = new List<Proforma>();
}
