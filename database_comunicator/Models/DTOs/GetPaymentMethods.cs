namespace database_communicator.Models.DTOs
{
    public class GetPaymentMethods
    {
        public int PaymentMethodId { get; set; }
        public string MethodName { get; set; } = null!;
    }
}
