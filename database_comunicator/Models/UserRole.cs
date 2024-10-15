using System;
using System.Collections.Generic;

namespace database_communicator.Models;

public partial class UserRole
{
    public int RoleId { get; set; }

    public string RoleName { get; set; } = null!;

    public virtual ICollection<OrgUser> OrgUsers { get; set; } = new List<OrgUser>();
}
