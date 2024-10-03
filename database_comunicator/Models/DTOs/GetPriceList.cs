namespace database_comunicator.Models.DTOs
{
    public class GetPriceList
    {
        public int PricelistId { get; set; }
        public DateTime Created { get; set; }
        public string Status { get; set; } = null!;
        public string Name { get; set; } = null!;
        public int TotalItems { get; set; }
        public string Path { get; set; } = null!;
        public string Currency { get; set; } = null!;
        public DateTime Modified { get; set; }
    }
}
