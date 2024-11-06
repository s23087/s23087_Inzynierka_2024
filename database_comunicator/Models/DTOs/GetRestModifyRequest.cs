namespace database_communicator.Models.DTOs
{
    public class GetRestModifyRequest
    {
        public int ReceiverId { get; set; }
        public string Note { get; set; } = null!;
    }
}
