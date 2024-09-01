namespace database_comunicator.Models.DTOs
{
    public class GetPaymentStatuses
    {
        public int PaymentStatusId { get; set; }
        public string StatusName { get; set; } = null!;
    }
}
