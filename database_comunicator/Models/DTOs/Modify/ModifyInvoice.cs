namespace database_communicator.Models.DTOs.Modify
{
    public class ModifyInvoice
    {
        public required bool IsYourInvoice { get; set; }
        public required int InvoiceId { get; set; }
        public string? InvoiceNumber { get; set; }
        public int? ClientId { get; set; }
        public decimal? TransportCost { get; set; }
        public int? PaymentMethod { get; set; }
        public int? PaymentStatus { get; set; }
        public bool? InSystem { get; set; }
        public string? Path { get; set; }
        public string? Note { get; set; }
    }
}
