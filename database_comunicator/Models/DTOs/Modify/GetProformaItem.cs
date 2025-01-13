namespace database_communicator.Models.DTOs.Modify
{
    public class GetProformaItem
    {
        // Item id
        public int Id { get; set; }
        public int? PriceId { get; set; }
        public int Qty { get; set; }
        public decimal Price { get; set; }
        public decimal? PurchasePrice { get; set; }
        public string Name { get; set; } = null!;
        public string Partnumber { get; set; } = null!;
        public string? InvoiceNumber { get; set; }
    }
}
