namespace database_communicator.Models.DTOs
{
    public class AddRequest
    {
        public required int CreatorId { get; set; }
        public required int ReceiverId { get; set; }
        public required string ObjectType { get; set;}
        public string? Path { get; set;}
        public required string Note { get; set;}
        public required string Title { get; set;}
    }
}
