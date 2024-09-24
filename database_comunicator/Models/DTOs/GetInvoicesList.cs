namespace database_comunicator.Models.DTOs
{
    public class GetInvoicesList
    {
        public int InvoiceId { get; set;  }
        public string InvoiceNumber { get; set; } = null!;
        public string ClientName { get; set; } = null!;
        public string OrgName { get; set; } = null!;
    }
}
