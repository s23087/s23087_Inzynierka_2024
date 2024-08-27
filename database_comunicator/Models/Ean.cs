using System;
using System.Collections.Generic;

namespace database_comunicator.Models;

public partial class Ean
{
    public string EanValue { get; set; } = null!;

    public int ItemId { get; set; }

    public virtual Item Item { get; set; } = null!;
}
