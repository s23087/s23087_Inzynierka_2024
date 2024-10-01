namespace database_comunicator.Models.DTOs
{
    public class GetItemForDeliveryTable
    {
        public string ItemName { get; set; } = null!;
        public string Partnumber { get; set; } = null!;
        public int Qty { get; set; }
    }
}
