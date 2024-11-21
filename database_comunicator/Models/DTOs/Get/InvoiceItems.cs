namespace database_communicator.Models.DTOs.Get
{
    public class InvoiceItems
    {
        public int ItemId { get; set; }
        public int Qty { get; set; }
        public decimal Price { get; set; }
    }
}
