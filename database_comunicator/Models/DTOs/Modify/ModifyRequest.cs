namespace database_communicator.Models.DTOs.Modify
{
    public class ModifyRequest
    {
        public required int RequestId { get; set; }
        public int? ReceiverId { get; set; }
        public string? ObjectType { get; set; }
        public string? Note { get; set; }
        public string? Path { get; set; }
        public string? Title { get; set; }
    }
}
