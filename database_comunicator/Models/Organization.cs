﻿using System;
using System.Collections.Generic;

namespace database_comunicator.Models;

public partial class Organization
{
    public int OrganizationId { get; set; }

    public string OrgName { get; set; } = null!;

    public int? Nip { get; set; }

    public string Street { get; set; } = null!;

    public string City { get; set; } = null!;

    public string PostalCode { get; set; } = null!;

    public int? CreditLimit { get; set; }

    public int CountryId { get; set; }

    public virtual Country Country { get; set; } = null!;

    public virtual ICollection<Invoice> InvoiceBuyerNavigations { get; set; } = new List<Invoice>();

    public virtual ICollection<Invoice> InvoiceSellerNavigations { get; set; } = new List<Invoice>();

    public virtual ICollection<Offer> Offers { get; set; } = new List<Offer>();

    public virtual ICollection<OrgUser> OrgUsers { get; set; } = new List<OrgUser>();

    public virtual ICollection<OutsideItem> OutsideItems { get; set; } = new List<OutsideItem>();

    public virtual ICollection<Proforma> ProformaBuyerNavigations { get; set; } = new List<Proforma>();

    public virtual ICollection<Proforma> ProformaSellerNavigations { get; set; } = new List<Proforma>();

    public virtual ICollection<SoloUser> SoloUsers { get; set; } = new List<SoloUser>();

    public virtual ICollection<AvailabilityStatus> AvailabilityStatuses { get; set; } = new List<AvailabilityStatus>();
}