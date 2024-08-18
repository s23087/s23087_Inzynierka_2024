using System;
using System.Collections.Generic;

namespace database_comunicator.Models;

public partial class CurrencyValueOffer
{
    public int OfferId { get; set; }

    public string CurrancyName { get; set; } = null!;

    public DateTime CurencyDate { get; set; }

    public virtual CurrencyValue Cur { get; set; } = null!;

    public virtual Offer Offer { get; set; } = null!;
}
