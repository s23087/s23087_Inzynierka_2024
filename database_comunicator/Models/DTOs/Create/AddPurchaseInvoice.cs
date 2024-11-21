using database_communicator.Models.DTOs.Get;

namespace database_communicator.Models.DTOs.Create
{
    public class AddPurchaseInvoice
    {
        public required int UserId { get; set; }
        public required string InvoiceNumber { get; set; } = null!;
        public required int Seller { get; set; }
        public required int Buyer { get; set; }
        public required DateTime InvoiceDate { get; set; }
        public required DateTime DueDate { get; set; }
        public required string Note { get; set; }
        public required bool InSystem { get; set; }
        public required decimal TransportCost { get; set; }
        public string? InvoiceFilePath { get; set; }
        public required int Taxes { get; set; }
        public required DateTime CurrencyValueDate { get; set; }
        public required string CurrencyName { get; set; } = null!;
        public required int PaymentMethodId { get; set; }
        public required int PaymentsStatusId { get; set; }
        public required decimal EuroValue { get; set; }
        public required decimal UsdValue { get; set; }
        public IEnumerable<InvoiceItems> InvoiceItems { get; set; } = new List<InvoiceItems>();

    }
}
