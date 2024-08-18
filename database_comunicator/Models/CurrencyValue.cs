using System;
using System.Collections.Generic;

namespace database_comunicator.Models;

public partial class CurrencyValue
{
    public string CurrencyName { get; set; } = null!;

    public DateTime UpdateDate { get; set; }

    public decimal CurrencyValue1 { get; set; }

    public virtual CurrencyName CurrencyNameNavigation { get; set; } = null!;

    public virtual ICollection<CurrencyValueOffer> CurrencyValueOffers { get; set; } = new List<CurrencyValueOffer>();

    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();

    public virtual ICollection<Proforma> Proformas { get; set; } = new List<Proforma>();
}
