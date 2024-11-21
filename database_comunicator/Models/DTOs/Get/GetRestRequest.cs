namespace database_communicator.Models.DTOs.Get
{
    public class GetRestRequest
    {
        public string? Path { get; set; }
        public string Note { get; set; } = null!;
    }
}
