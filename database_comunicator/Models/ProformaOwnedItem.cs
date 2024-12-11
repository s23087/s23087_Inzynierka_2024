namespace database_communicator.Models;

public partial class ProformaOwnedItem
{
    public int ProformaOwnedItemId { get; set; }

    public int ProformaId { get; set; }

    public int PurchasePriceId { get; set; }

    public int Qty { get; set; }

    public decimal SellingPrice { get; set; }

    public virtual PurchasePrice Item { get; set; } = null!;

    public virtual Proforma Proforma { get; set; } = null!;
}
