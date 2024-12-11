namespace database_communicator.Models;

public partial class ItemOwner
{
    public int IdUser { get; set; }

    public int InvoiceId { get; set; }

    public int OwnedItemId { get; set; }

    public int Qty { get; set; }

    public virtual AppUser IdUserNavigation { get; set; } = null!;

    public virtual OwnedItem OwnedItem { get; set; } = null!;
}
