using System;
using System.Collections.Generic;

namespace database_comunicator.Models;

public partial class CurrencyName
{
    public string Curenncy { get; set; } = null!;

    public virtual ICollection<CurrencyValue> CurrencyValues { get; set; } = new List<CurrencyValue>();

    public virtual ICollection<OutsideItem> OutsideItems { get; set; } = new List<OutsideItem>();
    public virtual ICollection<Offer> Offers { get; set; } = new List<Offer>();
}
