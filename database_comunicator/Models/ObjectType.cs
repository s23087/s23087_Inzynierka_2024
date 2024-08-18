using System;
using System.Collections.Generic;

namespace database_comunicator.Models;

public partial class ObjectType
{
    public int ObjectTypeId { get; set; }

    public string ObjectTypeName { get; set; } = null!;

    public virtual ICollection<UserNotification> UserNotifications { get; set; } = new List<UserNotification>();
}
