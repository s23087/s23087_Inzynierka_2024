namespace database_communicator.Models.DTOs
{
    public class ModifyUserRole
    {
        public required int UserId { get; set; }
        public required int ChoosenUserId { get; set; }
        public string RoleName { get; set; } = null!;
    }
}
