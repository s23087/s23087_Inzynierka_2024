using System;
using System.Collections.Generic;

namespace database_communicator.Models;

public partial class Waybill
{
    public int WaybillId { get; set; }

    public string WaybillValue { get; set; } = null!;

    public int DeliveriesId { get; set; }

    public virtual Delivery Deliveries { get; set; } = null!;
}
