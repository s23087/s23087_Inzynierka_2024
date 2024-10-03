namespace database_comunicator.Models.DTOs
{
    public class GetRestInfoOutsideItems
    {
        public IEnumerable<KeyValuePair<int, string>> Users { get; set; } = new List<KeyValuePair<int, string>>();
        public string OrganizationName { get; set; } = null!;
        public int Qty { get; set; }
        public decimal Price { get; set; }
        public string Currency { get; set; } = null!;
    }
}
