namespace database_comunicator.Models.DTOs
{
    public class GetInvoiceItems
    {
        public int PriceId { get; set; }
        public int ItemId { get; set; }
        public int Qty { get; set; }
        public decimal Price { get; set; }
    }
}
