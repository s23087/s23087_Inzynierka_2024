namespace database_communicator.Models.DTOs.Get
{
    public class GetNotifications
    {
        public int NotificationId { get; set; }

        public string Info { get; set; } = null!;

        public string ObjectType { get; set; } = null!;
        public string? Reference { get; set; } = null!;
        public bool IsRead { get; set; }
    }
}
