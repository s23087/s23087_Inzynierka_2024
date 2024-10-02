namespace database_comunicator.Models.DTOs
{
    public class AddOutsideItem
    {
        public required string Partnumber { get; set; }
        public required string ItemName { get; set; }
        public string ItemDesc { get; set; } = null!;
        public IEnumerable<string> Eans { get; set; } = new List<string>();
        public required int Qty { get; set; }
        public required decimal Price { get; set; }
    }
}
