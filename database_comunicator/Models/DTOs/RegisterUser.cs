namespace database_comunicator.Models.DTOs
{
    public class RegisterUser
    {
        public string Email { get; set; } = null!;

        public string Username { get; set; } = null!;

        public string Surname { get; set; } = null!;
        public string OrgName { get; set; } = null!;

        public int? Nip { get; set; }

        public string Street { get; set; } = null!;

        public string City { get; set; } = null!;

        public string PostalCode { get; set; } = null!;
        public string Country { get; set; } = null!;
        public string Password { get; set; } = null!;
        public bool IsOrg { get; set; }
    }
}
