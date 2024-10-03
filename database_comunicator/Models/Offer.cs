using System;
using System.Collections.Generic;

namespace database_comunicator.Models;

public partial class Offer
{
    public int OfferId { get; set; }

    public string OfferName { get; set; } = null!;

    public DateTime CreationDate { get; set; }

    public DateTime ModificationDate { get; set; }

    public string PathToFile { get; set; } = null!;

    public int OfferStatusId { get; set; }
    public int MaxQty { get; set; }
    public string CurrencyName { get; set; } = null!;
    public int UserId { get; set; }
    public virtual AppUser User { get; set; } = null!;
    public virtual CurrencyName CurrencyNameNavigation { get; set; } = null!;

    public virtual ICollection<OfferItem> OfferItems { get; set; } = new List<OfferItem>();

    public virtual OfferStatus OfferStatus { get; set; } = null!;
}
