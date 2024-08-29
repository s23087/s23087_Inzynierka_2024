namespace database_comunicator.Models.DTOs
{
    public class GetClientRestInfo
    {
        public int? CreditLimit { get; set; }
        public string? Availability { get; set; } = null!;
        public int? DaysForRealization { get; set; }
    }
}
