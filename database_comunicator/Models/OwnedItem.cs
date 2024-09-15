using System;
using System.Collections.Generic;

namespace database_comunicator.Models;

public partial class OwnedItem
{
    public int InvoiceId { get; set; }

    public int OwnedItemId { get; set; }

    public virtual Invoice Invoice { get; set; } = null!;

    public virtual ICollection<ItemOwner> ItemOwners { get; set; } = new List<ItemOwner>();

    public virtual Item OwnedItemNavigation { get; set; } = null!;

    public virtual ICollection<ProformaOwnedItem> ProformaOwnedItems { get; set; } = new List<ProformaOwnedItem>();

    public virtual ICollection<PurchasePrice> PurchasePrices { get; set; } = new List<PurchasePrice>();
}
