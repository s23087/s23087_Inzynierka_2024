namespace database_comunicator.Models.DTOs
{
    public class GetRestItemInfo
    {
        public string OrganizationName { get; set; } = null!;
        public string? InvoiceNumber { get; set; } = null!;
        public int Qty { get; set; }
        public int? DaysForRealization { get; set; }
        public decimal Price { get; set; }
        public string Curenncy { get; set; } = null!;
    }
}
