namespace database_communicator.Models.DTOs
{
    public class UserHash
    {
        public int Id { get; set; }
        public string PassHash { get; set; } = null!;

        public string PassSalt { get; set; } = null!;
    }
}
