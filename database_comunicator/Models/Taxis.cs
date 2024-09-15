using System;
using System.Collections.Generic;

namespace database_comunicator.Models;

public partial class Taxis
{
    public int TaxesId { get; set; }

    public decimal TaxValue { get; set; }

    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();

    public virtual ICollection<Proforma> Proformas { get; set; } = new List<Proforma>();
}
