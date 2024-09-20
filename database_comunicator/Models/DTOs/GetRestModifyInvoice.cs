namespace database_comunicator.Models.DTOs
{
    public class GetRestModifyInvoice
    {
        public decimal Transport { get; set; }
        public string PaymentMethod { get; set; } = null!;
        public string Note { get; set; } = null!;
    }
}
