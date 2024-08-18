using System;
using System.Collections.Generic;

namespace database_comunicator.Models;

public partial class CurrencyName
{
    public string Curenncy { get; set; } = null!;

    public virtual ICollection<CurrencyValue> CurrencyValues { get; set; } = new List<CurrencyValue>();

    public virtual ICollection<PurchasePrice> PurchasePrices { get; set; } = new List<PurchasePrice>();
}
