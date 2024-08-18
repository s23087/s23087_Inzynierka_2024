using System;
using System.Collections.Generic;

namespace database_comunicator.Models;

public partial class AvailabilityStatus
{
    public int AvailabilityStatusId { get; set; }

    public string StatusName { get; set; } = null!;

    public int DaysForRealization { get; set; }

    public virtual ICollection<Organization> Organizations { get; set; } = new List<Organization>();
}
