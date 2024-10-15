namespace database_communicator.Models.DTOs
{
    public class GetItemInvoiceData
    {
        public string Source { get; set; } = null!;
        public string InvoiceNumber { get; set; } = null!;
        public int Qty { get; set; }
        public decimal PurchasePrice { get; set; }
    }
}
