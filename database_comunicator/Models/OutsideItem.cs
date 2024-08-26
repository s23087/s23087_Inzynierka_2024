using System;
using System.Collections.Generic;

namespace database_comunicator.Models;

public partial class OutsideItem
{
    public int ItemId { get; set; }

    public int OrganizationId { get; set; }

    public decimal PurchasePrice { get; set; }
    public string Curenncy { get; set; } = null!;

    public int Qty { get; set; }
    public virtual CurrencyName CurrencyName { get; set; } = null!;

    public virtual Item Item { get; set; } = null!;

    public virtual Organization Organization { get; set; } = null!;

    public virtual ICollection<OutsideItemOffer> OutsideItemOffers { get; set; } = new List<OutsideItemOffer>();

    public virtual ICollection<AppUser> Users { get; set; } = new List<AppUser>();
}
