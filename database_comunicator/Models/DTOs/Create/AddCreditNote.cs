namespace database_communicator.Models.DTOs.Create
{
    public class AddCreditNote
    {
        public required string CreditNoteNumber { get; set; } = null!;
        public required DateTime CreditNoteDate { get; set; }
        public required bool InSystem { get; set; }
        public required bool IsPaid { get; set; }
        public string Note { get; set; } = null!;
        public required int InvoiceId { get; set; }
        public required bool IsYourCreditNote { get; set; }
        public string? FilePath { get; set; }
        public IEnumerable<NewCreditNoteItems> CreditNoteItems { get; set; } = new List<NewCreditNoteItems>();
    }
}
