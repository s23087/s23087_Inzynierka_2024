using System;
using System.Collections.Generic;

namespace database_communicator.Models;

public partial class PaymentMethod
{
    public int PaymentMethodId { get; set; }

    public string MethodName { get; set; } = null!;

    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();

    public virtual ICollection<Proforma> Proformas { get; set; } = new List<Proforma>();
}
