using System;
using System.Collections.Generic;

namespace database_comunicator.Models;

public partial class Offer
{
    public int OfferId { get; set; }

    public string OfferName { get; set; } = null!;

    public int CreationDate { get; set; }

    public int ModificationDate { get; set; }

    public int? OrganizationsId { get; set; }

    public string PathToFile { get; set; } = null!;

    public int OfferStatusId { get; set; }

    public virtual ICollection<CurrencyValueOffer> CurrencyValueOffers { get; set; } = new List<CurrencyValueOffer>();

    public virtual ICollection<OfferItem> OfferItems { get; set; } = new List<OfferItem>();

    public virtual OfferStatus OfferStatus { get; set; } = null!;

    public virtual Organization? Organizations { get; set; }

    public virtual ICollection<OutsideItemOffer> OutsideItemOffers { get; set; } = new List<OutsideItemOffer>();
}
