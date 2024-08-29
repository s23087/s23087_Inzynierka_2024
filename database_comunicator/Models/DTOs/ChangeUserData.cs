namespace database_comunicator.Models.DTOs
{
    public class ChangeUserData
    {
        public required int UserId { get; set; }
        public string? Email { get; set; } = null!;
        public string? Username { get; set;} = null!;
        public string? Surname { get; set; } = null!;
    }
}
