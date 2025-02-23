﻿namespace database_communicator.Models;

public partial class Taxes
{
    public int TaxesId { get; set; }

    public decimal TaxValue { get; set; }

    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();

    public virtual ICollection<Proforma> Proformas { get; set; } = new List<Proforma>();
}
