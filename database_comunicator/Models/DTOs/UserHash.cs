namespace database_comunicator.Models.DTOs
{
    public class UserHash
    {
        public string PassHash { get; set; } = null!;

        public string PassSalt { get; set; } = null!;
    }
}
