namespace database_communicator.Models.DTOs
{
    public class GetInvoiceItems
    {
        public int PriceId { get; set; }
        public int InvoiceId { get; set; }
        public int ItemId { get; set; }
        public string ItemName { get; set; } = null!;
        public string Partnumber { get; set; } = null!;
        public int Qty { get; set; }
        public decimal Price { get; set; }
    }
}
