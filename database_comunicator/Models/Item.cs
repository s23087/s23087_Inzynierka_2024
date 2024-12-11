namespace database_communicator.Models;

public partial class Item
{
    public int ItemId { get; set; }

    public string ItemName { get; set; } = null!;

    public string ItemDescription { get; set; } = null!;

    public string PartNumber { get; set; } = null!;

    public virtual ICollection<Ean> Eans { get; set; } = new List<Ean>();

    public virtual ICollection<OutsideItem> OutsideItems { get; set; } = new List<OutsideItem>();

    public virtual ICollection<OwnedItem> OwnedItems { get; set; } = new List<OwnedItem>();

    public virtual ICollection<ProformaFutureItem> ProformaFutureItems { get; set; } = new List<ProformaFutureItem>();
    public virtual ICollection<OfferItem> OfferItems { get; set; } = new List<OfferItem>();
}
