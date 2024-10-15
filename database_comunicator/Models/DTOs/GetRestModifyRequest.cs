namespace database_communicator.Models.DTOs
{
    public class GetRestModifyRequest
    {
        public int RecevierId { get; set; }
        public string Note { get; set; } = null!;
    }
}
