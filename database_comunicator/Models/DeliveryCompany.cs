using System;
using System.Collections.Generic;

namespace database_comunicator.Models;

public partial class DeliveryCompany
{
    public int DeliveryCompanyId { get; set; }

    public string DeliveryCompanyName { get; set; } = null!;

    public virtual ICollection<Waybill> Waybills { get; set; } = new List<Waybill>();
}
