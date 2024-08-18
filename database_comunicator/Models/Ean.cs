using System;
using System.Collections.Generic;

namespace database_comunicator.Models;

public partial class Ean
{
    public int Ean1 { get; set; }

    public int ItemId { get; set; }

    public virtual Item Item { get; set; } = null!;
}
