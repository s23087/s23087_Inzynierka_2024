namespace database_communicator.Models.DTOs.Create
{
    public class AddUser
    {
        public string Email { get; set; } = null!;

        public string Username { get; set; } = null!;

        public string Surname { get; set; } = null!;

        public string Password { get; set; } = null!;
        public string? RoleName { get; set; } = null!;
    }
}
