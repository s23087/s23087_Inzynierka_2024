﻿namespace database_communicator.Models;

public partial class OfferStatus
{
    public int OfferStatusId { get; set; }

    public string StatusName { get; set; } = null!;

    public virtual ICollection<Offer> Offers { get; set; } = new List<Offer>();
}
