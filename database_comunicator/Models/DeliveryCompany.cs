﻿namespace database_communicator.Models;

public partial class DeliveryCompany
{
    public int DeliveryCompanyId { get; set; }

    public string DeliveryCompanyName { get; set; } = null!;

    public virtual ICollection<Delivery> Deliveries { get; set; } = new List<Delivery>();
}
