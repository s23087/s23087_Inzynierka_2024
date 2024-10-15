namespace database_communicator.Models.DTOs
{
    public class SetRequestStatus
    {
        public required string StatusName { get; set; }
        public string? Note { get; set; }
    }
}
