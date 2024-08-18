using System;
using System.Collections.Generic;

namespace database_comunicator.Models;

public partial class PurchasePrice
{
    public int PurchasePriceId { get; set; }

    public int OwnedItemId { get; set; }

    public int InvoiceId { get; set; }

    public int PriceDate { get; set; }

    public string Curenncy { get; set; } = null!;

    public decimal PurchasePrice1 { get; set; }

    public virtual CurrencyName CurenncyNavigation { get; set; } = null!;

    public virtual OwnedItem OwnedItem { get; set; } = null!;
}
