namespace database_comunicator.Models.DTOs
{
    public class CreateNotification
    {
        public int UserId { get; set; }
        public string Info { get; set; } = null!;
        public string ObjectType { get; set; } = null!;
        public string Referance { get; set; } = null!;
    }
}
