namespace database_comunicator.Models.DTOs
{
    public class GetRequest
    {
        public int Id { get; set; }
        public string Username { get; set; } = null!;
        public string Status { get; set; } = null!;
        public string ObjectType { get; set; } = null!;
        public DateTime CreationDate { get; set; }
        public string Title { get; set; } = null!;
    }
}
