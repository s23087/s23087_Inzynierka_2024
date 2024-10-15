using System;
using System.Collections.Generic;

namespace database_communicator.Models;

public partial class SoloUser
{
    public int SoloUserId { get; set; }

    public int OrganizationsId { get; set; }

    public virtual ICollection<AppUser> AppUsers { get; set; } = new List<AppUser>();

    public virtual Organization Organizations { get; set; } = null!;
}
