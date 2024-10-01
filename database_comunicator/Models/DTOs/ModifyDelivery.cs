namespace database_comunicator.Models.DTOs
{
    public class ModifyDelivery
    {
        public required int DeliveryId { get; set; }
        public DateTime? Estimated { get; set; }
        public int? CompanyId { get; set; }
        public IEnumerable<string>? Waybills { get; set; }
    }
}
