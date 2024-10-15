namespace database_communicator.Models.DTOs
{
    public class GetCreditNote
    {
        public string? User { get; set; }
        public int CreditNoteId { get; set; }
        public string InvoiceNumber { get; set; } = null!;
        public DateTime Date { get; set; }
        public int Qty { get; set; }
        public decimal Total { get; set; }
        public string ClientName { get; set; } = null!;
        public bool InSystem { get; set; }
        public bool IsPaid { get; set; }
    }
}
