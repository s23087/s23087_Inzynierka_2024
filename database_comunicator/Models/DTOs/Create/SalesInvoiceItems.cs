namespace database_communicator.Models.DTOs.Create
{
    public class SalesInvoiceItems
    {
        public int PriceId { get; set; }
        public int ItemId { get; set; }
        public int Qty { get; set; }
        public int Price { get; set; }
        public int BuyInvoiceId { get; set; }
    }
}
