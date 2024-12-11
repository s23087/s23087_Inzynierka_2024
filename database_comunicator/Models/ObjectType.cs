namespace database_communicator.Models;

public partial class ObjectType
{
    public int ObjectTypeId { get; set; }

    public string ObjectTypeName { get; set; } = null!;

    public virtual ICollection<UserNotification> UserNotifications { get; set; } = new List<UserNotification>();
    public virtual ICollection<Request> Requests { get; set; } = new List<Request>();
}
