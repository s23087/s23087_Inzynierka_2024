namespace database_comunicator.Models.DTOs
{
    public class GetInvoicesList
    {
        public int InvoiceId { get; set;  }
        public string InvoiceNumber { get; set; } = null!;
    }
}
