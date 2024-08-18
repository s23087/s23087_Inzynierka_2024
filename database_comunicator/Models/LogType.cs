using System;
using System.Collections.Generic;

namespace database_comunicator.Models;

public partial class LogType
{
    public int LogTypeId { get; set; }

    public string LogTypeName { get; set; } = null!;

    public virtual ICollection<ActionLog> ActionLogs { get; set; } = new List<ActionLog>();
}
