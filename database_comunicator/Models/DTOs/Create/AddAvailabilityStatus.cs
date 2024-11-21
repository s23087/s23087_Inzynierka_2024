namespace database_communicator.Models.DTOs.Create
{
    public class AddAvailabilityStatus
    {
        public required string StatusName { get; set; } = null!;
        public required int DaysForRealization { get; set; }
    }
}
