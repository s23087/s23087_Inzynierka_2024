namespace database_comunicator.Models.DTOs
{
    public class ModifyCreditNote
    {
        public required bool IsYourCredit { get; set; }
        public required int Id { get; set; }
        public string? CreditNumber { get; set; }
        public DateTime? Date { get; set; }
        public bool? InSystem { get; set; }
        public bool? IsPaid { get; set; }
        public string? Path { get; set; }
        public string? Note { get; set; }
    }
}
