namespace database_comunicator.Models.DTOs
{
    public class CreateNotification
    {
        public required int UserId { get; set; }
        public required string Info { get; set; }
        public required string ObjectType { get; set; }
        public required string Referance { get; set; }
    }
}
