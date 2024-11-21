namespace database_communicator.Models.DTOs.Modify
{
    public class ChangePassword
    {
        public required int UserId { get; set; }
        public required string OldPassword { get; set; }
        public required string NewPassword { get; set; }
    }
}
