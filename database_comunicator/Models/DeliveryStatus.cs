namespace database_communicator.Models;

public partial class DeliveryStatus
{
    public int DeliveryStatusId { get; set; }

    public string StatusName { get; set; } = null!;

    public virtual ICollection<Delivery> Deliveries { get; set; } = new List<Delivery>();
}
