using System;
using System.Collections.Generic;

namespace database_communicator.Models;

public partial class SellingPrice
{
    public int SellingPriceId { get; set; }

    public int SellInvoiceId { get; set; }

    public int PurchasePriceId { get; set; }
    public int IdUser { get; set; }

    public int Qty { get; set; }

    public decimal Price { get; set; }
    public virtual AppUser User { get; set; } = null!;

    public virtual PurchasePrice PurchasePrice { get; set; } = null!;

    public virtual Invoice SellInvoice { get; set; } = null!;
}
