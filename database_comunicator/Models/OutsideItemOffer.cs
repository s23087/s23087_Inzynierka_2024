using System;
using System.Collections.Generic;

namespace database_comunicator.Models;

public partial class OutsideItemOffer
{
    public int OfferId { get; set; }

    public int OutsideItemId { get; set; }

    public int OrganizationId { get; set; }

    public int Qty { get; set; }

    public decimal SellingPrice { get; set; }

    public virtual OutsideItem OutsideItem { get; set; } = null!;

    public virtual Offer Offer { get; set; } = null!;
}
