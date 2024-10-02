namespace database_comunicator.Models.DTOs
{
    public class GetOutsideItem
    {
        public IEnumerable<string>? Users { get; set; }
        public string Partnumber { get; set; } = null!;
        public int ItemId { get; set; }
        public int OrgId { get; set; }
        public string OrgName { get; set; } = null!;
        public decimal Price { get; set; }
        public int Qty { get; set; }
        public string Currency { get; set; } = null!;
    }
}
