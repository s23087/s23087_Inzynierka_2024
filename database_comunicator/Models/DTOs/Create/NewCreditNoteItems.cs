namespace database_communicator.Models.DTOs.Create
{
    public class NewCreditNoteItems
    {
        public int UserId { get; set; }
        public int ItemId { get; set; }
        public int InvoiceId { get; set; }
        public int PurchasePriceId { get; set; }
        public int Qty { get; set; }
        public decimal NewPrice { get; set; }
    }
}
