using System;
using System.Collections.Generic;

namespace database_comunicator.Models;

public partial class ProformaFutureItem
{
    public int ProformaFutureItemId { get; set; }

    public int ProformaId { get; set; }

    public int ItemId { get; set; }

    public int Qty { get; set; }

    public decimal PurchasePrice { get; set; }

    public virtual Item Item { get; set; } = null!;

    public virtual Proforma Proforma { get; set; } = null!;
}
