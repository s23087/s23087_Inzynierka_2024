namespace database_comunicator.Models.DTOs
{
    public class GetInvoiceItemsForTable
    {
        public int PriceId { get; set; }
        public string Partnumber { get; set; } = null!;
        public IEnumerable<String> Users { get; set; } = new List<string>();
        public string ItemName { get; set; } = null!;
        public int Qty { get; set; }
        public decimal Price { get; set; }
    }
}
