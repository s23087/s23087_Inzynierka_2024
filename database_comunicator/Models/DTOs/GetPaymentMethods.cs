namespace database_comunicator.Models.DTOs
{
    public class GetPaymentMethods
    {
        public int PaymentMethodId { get; set; }
        public string MethodName { get; set; } = null!;
    }
}
