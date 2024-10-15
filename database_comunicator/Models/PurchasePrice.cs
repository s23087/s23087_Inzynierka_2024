using System;
using System.Collections.Generic;

namespace database_communicator.Models;

public partial class PurchasePrice
{
    public int PurchasePriceId { get; set; }

    public int Qty { get; set; }

    public decimal Price { get; set; }

    public int OwnedItemId { get; set; }

    public int InvoiceId { get; set; }

    public virtual ICollection<CalculatedPrice> CalculatedPrices { get; set; } = new List<CalculatedPrice>();

    public virtual ICollection<CreditNoteItem> CreditNoteItems { get; set; } = new List<CreditNoteItem>();

    public virtual OwnedItem OwnedItem { get; set; } = null!;
    public virtual ICollection<ProformaOwnedItem> ProformaOwnedItems { get; set; } = new List<ProformaOwnedItem>();

    public virtual ICollection<SellingPrice> SellingPrices { get; set; } = new List<SellingPrice>();
}
