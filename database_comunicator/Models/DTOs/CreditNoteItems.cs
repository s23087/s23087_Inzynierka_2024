namespace database_comunicator.Models.DTOs
{
    public class CreditNoteItems
    {
        public int UserId { get; set; }
        public int ItemId { get; set; }
        public int InvoiceId { get; set; }
        public int PurchasePriceId { get; set; }
        public int Qty { get; set; }
        public decimal NewPrice { get; set; }
        public bool IncludeQty { get; set; }
    }
}
