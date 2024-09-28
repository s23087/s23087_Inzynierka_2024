namespace database_comunicator.Models.DTOs
{
    public class GetProforma
    {
        public string? User { get; set; }
        public int ProformaId { get; set; }
        public string ProformaNumber { get; set; } = null!;
        public DateTime Date { get; set; }
        public decimal Transport { get; set; }
        public int Qty { get; set; }
        public decimal Total { get; set; }
        public string CurrencyName { get; set; } = null!;
        public string ClientName { get; set; } = null!;
    }
}
