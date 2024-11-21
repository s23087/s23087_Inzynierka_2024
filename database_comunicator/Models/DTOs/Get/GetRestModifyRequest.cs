namespace database_communicator.Models.DTOs.Get
{
    public class GetRestModifyRequest
    {
        public int ReceiverId { get; set; }
        public string Note { get; set; } = null!;
    }
}
