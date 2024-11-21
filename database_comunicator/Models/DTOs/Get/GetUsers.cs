namespace database_communicator.Models.DTOs.Get
{
    public class GetUsers
    {
        public int IdUser { get; set; }

        public string Username { get; set; } = null!;

        public string Surname { get; set; } = null!;
    }
}
