﻿namespace database_comunicator.Models.DTOs
{
    public class AddCreditNote
    {
        public required string CreditNotenumber { get; set; } = null!;
        public required DateTime CreditNoteDate { get; set; }
        public bool InSystem { get; set; }
        public string Note { get; set; } = null!;
        public int InvoiceId { get; set; }
        public IEnumerable<CreditNoteItems> CreditNoteItems { get; set; } = new List<CreditNoteItems>();
    }
}
