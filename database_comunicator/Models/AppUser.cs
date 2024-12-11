namespace database_communicator.Models;

public partial class AppUser
{
    public int IdUser { get; set; }

    public string Email { get; set; } = null!;

    public string Username { get; set; } = null!;

    public string Surname { get; set; } = null!;

    public int? SoloUserId { get; set; }

    public int? OrgUserId { get; set; }

    public string PassHash { get; set; } = null!;

    public string PassSalt { get; set; } = null!;

    public virtual ICollection<ActionLog> ActionLogs { get; set; } = new List<ActionLog>();

    public virtual ICollection<ItemOwner> ItemOwners { get; set; } = new List<ItemOwner>();

    public virtual ICollection<Note> Notes { get; set; } = new List<Note>();

    public virtual OrgUser? OrgUser { get; set; }

    public virtual SoloUser? SoloUser { get; set; }

    public virtual ICollection<UserNotification> UserNotifications { get; set; } = new List<UserNotification>();

    public virtual ICollection<Organization> Clients { get; set; } = new List<Organization>();
    public virtual ICollection<SellingPrice> SellingPrices { get; set; } = new List<SellingPrice>();
    public virtual ICollection<CreditNote> CreditNotes { get; set; } = new List<CreditNote>();
    public virtual ICollection<Request> CreatedRequest { get; set; } = new List<Request>();
    public virtual ICollection<Request> Requests { get; set; } = new List<Request>();
    public virtual ICollection<Proforma> Proformas { get; set; } = new List<Proforma>();
    public virtual ICollection<Offer> Offers { get; set; } = new List<Offer>();
}
