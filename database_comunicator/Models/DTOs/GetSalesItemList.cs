namespace database_communicator.Models.DTOs
{
    public class GetSalesItemList
    {
        public int ItemId { get; set; }
        public int PriceId { get; set; }
        public int InvoiceId { get; set; }
        public string InvoiceNumber { get; set; } = null!;
        public string Partnumber { get; set; } = null!;
        public string Name { get; set; } = null!;
        public int Qty { get; set; }
        public decimal Price { get; set; }
    }
}
