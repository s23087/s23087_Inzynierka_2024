﻿using System;
using System.Collections.Generic;

namespace database_comunicator.Models;

public partial class Delivery
{
    public int DeliveryId { get; set; }

    public DateTime EstimatedDeliveryDate { get; set; }

    public DateTime DeliveryDate { get; set; }

    public int DeliveryStatusId { get; set; }

    public int ProformaId { get; set; }

    public int? InvoiceId { get; set; }

    public virtual DeliveryStatus DeliveryStatus { get; set; } = null!;

    public virtual Invoice? Invoice { get; set; }

    public virtual Proforma Proforma { get; set; } = null!;

    public virtual ICollection<Waybill> Waybills { get; set; } = new List<Waybill>();

    public virtual ICollection<Note> Notes { get; set; } = new List<Note>();
}