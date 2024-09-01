using System;
using System.Collections.Generic;

namespace database_comunicator.Models;

public partial class UserNotification
{
    public int NotificationId { get; set; }

    public int UsersId { get; set; }

    public string Info { get; set; } = null!;

    public int ObjectTypeId { get; set; }
    public string Referance { get; set; } = null!;
    public bool IsRead { get; set; }

    public virtual ObjectType ObjectType { get; set; } = null!;

    public virtual AppUser Users { get; set; } = null!;
}
