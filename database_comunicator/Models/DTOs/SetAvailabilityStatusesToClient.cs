namespace database_communicator.Models.DTOs
{
    public class SetAvailabilityStatusesToClient
    {
        public required int OrgId { get; set; }
        public required int StatusId { get; set; }
    }
}
