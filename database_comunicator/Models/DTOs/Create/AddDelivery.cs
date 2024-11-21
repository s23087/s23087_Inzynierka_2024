namespace database_communicator.Models.DTOs.Create
{
    public class AddDelivery
    {
        public required int UserId { get; set; }
        public required bool IsDeliveryToUser { get; set; }
        public required DateTime EstimatedDeliveryDate { get; set; }
        public required int ProformaId { get; set; }
        public required int CompanyId { get; set; }
        public required IEnumerable<string> Waybills { get; set; } = new List<string>();
        public string? Note { get; set; }
    }
}
