namespace database_comunicator.Models.DTOs
{
    public class AddSalesInvoice
    {
        public required int UserId { get; set; }
        public required string InvoiceNumber { get; set; } = null!;
        public required int Seller { get; set; }
        public required int Buyer { get; set; }
        public required DateTime InvoiceDate { get; set; }
        public required DateTime DueDate { get; set; }
        public string Note { get; set; } = null!;
        public bool InSystem { get; set; }
        public decimal TransportCost { get; set; }
        public string? InvoiceFilePath { get; set; }
        public int Taxes { get; set; }
        public DateTime CurrencyValueDate { get; set; }
        public string CurrencyName { get; set; } = null!;
        public decimal CurrencyValue { get; set; }
        public int PaymentMethodId { get; set; }
        public int PaymentsStatusId { get; set; }
        public IEnumerable<SalesInvoiceItems> InvoiceItems { get; set; } = new List<SalesInvoiceItems>();
    }
}
