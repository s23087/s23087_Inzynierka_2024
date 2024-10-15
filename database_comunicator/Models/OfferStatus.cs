using System;
using System.Collections.Generic;

namespace database_communicator.Models;

public partial class OfferStatus
{
    public int OfferId { get; set; }

    public string StatusName { get; set; } = null!;

    public virtual ICollection<Offer> Offers { get; set; } = new List<Offer>();
}
