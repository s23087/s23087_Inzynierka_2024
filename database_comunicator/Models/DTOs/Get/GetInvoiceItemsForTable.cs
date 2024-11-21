namespace database_communicator.Models.DTOs.Get
{
    public class GetInvoiceItemsForTable
    {
        public int PriceId { get; set; }
        public string Partnumber { get; set; } = null!;
        public IEnumerable<string> Users { get; set; } = new List<string>();
        public string ItemName { get; set; } = null!;
        public int Qty { get; set; }
        public decimal Price { get; set; }
    }
}
