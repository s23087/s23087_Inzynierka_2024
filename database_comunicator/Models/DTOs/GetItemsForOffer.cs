namespace database_communicator.Models.DTOs
{
    public class GetItemsForOffer
    {
        public int ItemId { get; set; }
        public string Partnumber { get; set; } = null!;
        public int Qty { get; set; }
        public decimal PurchasePrice { get; set; }
    }
}
