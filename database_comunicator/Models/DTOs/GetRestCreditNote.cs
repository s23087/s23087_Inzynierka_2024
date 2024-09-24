namespace database_comunicator.Models.DTOs
{
    public class GetRestCreditNote
    {
        public string Note { get; set; } = null!;
        public IEnumerable<GetCredtItemForTable> CreditItems { get; set; } = new List<GetCredtItemForTable>();
    }
}
