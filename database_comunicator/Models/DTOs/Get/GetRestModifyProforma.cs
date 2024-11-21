namespace database_communicator.Models.DTOs.Get
{
    public class GetRestModifyProforma
    {
        public int UserId { get; set; }
        public string PaymentMethod { get; set; } = null!;
        public bool InSystem { get; set; }
        public string Note { get; set; } = null!;
    }
}
