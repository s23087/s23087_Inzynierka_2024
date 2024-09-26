namespace database_comunicator.Models.DTOs
{
    public class ModifyRequest
    {
        public required int RequestId { get; set; }
        public int? RecevierId { get; set; }
        public string? ObjectType { get; set; }
        public string? Note { get; set; }
        public string? Path { get; set; }
    }
}
