namespace database_comunicator.Models.DTOs
{
    public class AddAvailabilityStatus
    {
        public string StatusName { get; set; } = null!;
        public int DaysForRealization { get; set; }
    }
}
