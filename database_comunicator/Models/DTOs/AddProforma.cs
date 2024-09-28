namespace database_comunicator.Models.DTOs
{
    public class AddProforma
    {
        public required bool IsYourProforma { get; set; }
        public required string ProformaNumber { get; set; }
        public required int SellerId { get; set; }
        public required int BuyerId { get; set; }
        public required DateTime Date { get; set; }
        public required decimal TransportCost { get; set; }
        public required string Note { get; set; }
        public required bool InSystem { get; set; }
        public string? Path { get; set; }
        public required int TaxId { get; set; }
        public required int PaymentId { get; set; }
        public required DateTime CurrencyDate { get; set; }
        public required string CurrencyName { get; set; }
        public required decimal CurrencyValue { get; set; }
        public required int UserId { get; set; }
        public required IEnumerable<AddProformaItem> ProformaItems { get; set; } = new List<AddProformaItem>();
    }
}
