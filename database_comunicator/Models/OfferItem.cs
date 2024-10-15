using System;
using System.Collections.Generic;

namespace database_communicator.Models;

public partial class OfferItem
{
    public int OfferId { get; set; }

    public int ItemId { get; set; }

    public decimal SellingPrice { get; set; }

    public virtual Item Item { get; set; } = null!;

    public virtual Offer Offer { get; set; } = null!;
}
