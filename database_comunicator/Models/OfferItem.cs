using System;
using System.Collections.Generic;

namespace database_comunicator.Models;

public partial class OfferItem
{
    public int OfferId { get; set; }

    public int IdUser { get; set; }

    public int InvoiceId { get; set; }

    public int Qty { get; set; }

    public decimal SellingPrice { get; set; }

    public virtual ItemOwner I { get; set; } = null!;

    public virtual Offer Offer { get; set; } = null!;
}
