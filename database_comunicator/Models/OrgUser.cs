using System;
using System.Collections.Generic;

namespace database_comunicator.Models;

public partial class OrgUser
{
    public int OrgUserId { get; set; }

    public int RoleId { get; set; }

    public int OrganizationsId { get; set; }

    public virtual ICollection<AppUser> AppUsers { get; set; } = new List<AppUser>();

    public virtual Organization Organizations { get; set; } = null!;

    public virtual UserRole Role { get; set; } = null!;
}
