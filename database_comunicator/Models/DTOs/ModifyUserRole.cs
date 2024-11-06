namespace database_communicator.Models.DTOs
{
    public class ModifyUserRole
    {
        public required int UserId { get; set; }
        public required int ChosenUserId { get; set; }
        public string RoleName { get; set; } = null!;
    }
}
