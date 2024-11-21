namespace database_communicator.Models.DTOs.Get
{
    public class GetInvoicesList
    {
        public int InvoiceId { get; set; }
        public string InvoiceNumber { get; set; } = null!;
        public string ClientName { get; set; } = null!;
        public string OrgName { get; set; } = null!;
    }
}
