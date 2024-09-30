namespace database_comunicator.Models.DTOs
{
    public class GetDelivery
    {
        public string User { get; set; } = null!;
        public int DeliveryId { get; set; }
        public string Status { get; set; } = null!;
        public IEnumerable<string> Waybill { get; set; } = new List<string>();
        public string DeliveryCompany { get; set; } = null!;
        public DateTime Estimated { get; set; }
        public string Proforma { get; set; } = null!;
        public string ClientName { get; set; } = null!;
        public DateTime? Delivered { get; set; }
    }
}
