using System;
using System.Collections.Generic;

namespace database_communicator.Models;

public partial class OutsideItem
{
    public int ItemId { get; set; }

    public int OrganizationId { get; set; }

    public decimal PurchasePrice { get; set; }

    public int Qty { get; set; }

    public string CurrencyName { get; set; } = null!;

    public virtual CurrencyName CurrencyNameNavigation { get; set; } = null!;

    public virtual Item Item { get; set; } = null!;

    public virtual Organization Organization { get; set; } = null!;
}
