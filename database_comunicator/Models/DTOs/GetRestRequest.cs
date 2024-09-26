namespace database_comunicator.Models.DTOs
{
    public class GetRestRequest
    {
        public string? Path { get; set; }
        public string Note { get; set; } = null!;
    }
}
