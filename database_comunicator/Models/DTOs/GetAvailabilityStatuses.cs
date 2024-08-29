namespace database_comunicator.Models.DTOs
{
    public class GetAvailabilityStatuses
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int Days { get; set; }
    }
}
