namespace database_communicator.Models.DTOs
{
    public class GetClientBindings
    {
        public int IdUser { get; set; }
        public string Username { get; set; } = null!;
        public string Surname { get; set; } = null!;
    }
}
