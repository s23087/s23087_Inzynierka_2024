namespace database_communicator.Models.DTOs
{
    public class GetOfferItemForModify
    {
        public int Id { get; set; }
        public string Partnumber { get; set; } = null!;
        public int Qty { get; set; }
        public decimal PurchasePrice { get; set; }
        public decimal Price { get; set; }
    }
}
