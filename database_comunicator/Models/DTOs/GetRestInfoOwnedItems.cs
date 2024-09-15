namespace database_comunicator.Models.DTOs
{
    public class GetRestInfoOwnedItems
    {
        public int UserId { get; set; }
        public string Username { get; set; } = null!;
        public string OrganizationName { get; set; } = null!;
        public string? InvoiceNumber { get; set; } = null!;
        public int Qty { get; set; }
        public decimal Price { get; set; }
        public string Currency { get; set; } = null!;
    }
}
