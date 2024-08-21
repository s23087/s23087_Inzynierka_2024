namespace database_comunicator.Models.DTOs
{
    public class AddUser
    {
        public string Email { get; set; } = null!;

        public string Username { get; set; } = null!;

        public string Surname { get; set; } = null!;

        public string Password { get; set; } = null!;

        public int OrganizationsId { get; set; }
    }
}
