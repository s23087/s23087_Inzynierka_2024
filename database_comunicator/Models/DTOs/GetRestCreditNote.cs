namespace database_communicator.Models.DTOs
{
    public class GetRestCreditNote
    {
        public string CreditNoteNumber { get; set; } = null!;
        public string CurrencyName { get; set; } = null!;
        public string Note { get; set; } = null!;
        public string Path { get; set; } = null!;
        public IEnumerable<GetCredtItemForTable> CreditItems { get; set; } = new List<GetCredtItemForTable>();
    }
}
