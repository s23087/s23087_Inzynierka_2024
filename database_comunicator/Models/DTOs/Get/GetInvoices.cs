namespace database_communicator.Models.DTOs.Get
{
    public class GetInvoices
    {
        public IEnumerable<string>? Users { get; set; } = new List<string>();
        public int InvoiceId { get; set; }
        public string InvoiceNumber { get; set; } = null!;

        public string ClientName { get; set; } = null!;

        public DateTime InvoiceDate { get; set; }

        public DateTime DueDate { get; set; }
        public string PaymentStatus { get; set; } = null!;
        public bool InSystem { get; set; }
        public int Qty { get; set; }
        public decimal Price { get; set; }
    }
}
