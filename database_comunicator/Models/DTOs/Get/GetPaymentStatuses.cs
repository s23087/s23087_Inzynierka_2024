namespace database_communicator.Models.DTOs.Get
{
    public class GetPaymentStatuses
    {
        public int PaymentStatusId { get; set; }
        public string StatusName { get; set; } = null!;
    }
}
