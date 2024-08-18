using System;
using System.Collections.Generic;

namespace database_comunicator.Models;

public partial class ProformaOwnedItem
{
    public int ProformaId { get; set; }

    public int OwnedItemId { get; set; }

    public int InvoiceId { get; set; }

    public int Qty { get; set; }

    public decimal SellingPrice { get; set; }

    public virtual OwnedItem OwnedItem { get; set; } = null!;

    public virtual Proforma Proforma { get; set; } = null!;
}
