﻿using System;
using System.Collections.Generic;

namespace database_comunicator.Models;

public partial class Item
{
    public int ItemId { get; set; }

    public string ItemName { get; set; } = null!;

    public string ItemDescription { get; set; } = null!;

    public string PartNumber { get; set; } = null!;

    public virtual ICollection<Ean> Eans { get; set; } = new List<Ean>();

    public virtual ICollection<OutsideItem> OutsideItems { get; set; } = new List<OutsideItem>();

    public virtual ICollection<OwnedItem> OwnedItems { get; set; } = new List<OwnedItem>();
}
