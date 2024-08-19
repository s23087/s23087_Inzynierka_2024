﻿using System;
using System.Collections.Generic;

namespace database_comunicator.Models;

public partial class ItemOwner
{
    public int IdUser { get; set; }

    public int InvoiceId { get; set; }

    public int Qty { get; set; }

    public int OwnedItemId { get; set; }

    public virtual AppUser IdUserNavigation { get; set; } = null!;

    public virtual ICollection<OfferItem> OfferItems { get; set; } = new List<OfferItem>();

    public virtual OwnedItem OwnedItem { get; set; } = null!;
}