namespace database_comunicator.Models
{
    public class SoldItem
    {
        public int PurchaseInvoiceId { get; set; }
        public int SellInvoiceId { get; set; }
        public int OwnedItemId { get; set; }
        public int Qty { get; set; }
        public decimal SellingPrice { get; set; }
        public virtual Invoice SellInvoice { get; set; } = null!;
        public virtual OwnedItem OwnedItemNavigation { get; set; } = null!;

    }
}
