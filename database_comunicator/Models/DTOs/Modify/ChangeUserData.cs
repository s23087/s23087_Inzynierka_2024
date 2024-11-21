namespace database_communicator.Models.DTOs.Modify
{
    public class ChangeUserData
    {
        public required int UserId { get; set; }
        public string? Email { get; set; } = null!;
        public string? Username { get; set; } = null!;
        public string? Surname { get; set; } = null!;
    }
}
