namespace database_comunicator.Models.DTOs
{
    public class GetRestInvoice
    {
        public decimal Tax { get; set; }
        public decimal CurrencyValue { get; set; }
        public string CurrencyName { get; set; } = null!;
        public DateTime CurrencyDate { get; set; }
        public decimal TransportCost { get; set; }
        public string PaymentType { get; set; } = null!;
        public string Note { get; set; } = null!;
        public string? Path { get; set; } = null!;
        public IEnumerable<string> CreditNotes { get; set; } = new List<string>();
        public IEnumerable<GetInvoiceItemsForTable> Items { get; set; } = new List<GetInvoiceItemsForTable>();
    }
}
