namespace database_communicator.Models.DTOs.Create
{
    public class CreateNotification
    {
        public required int UserId { get; set; }
        public required string Info { get; set; }
        public required string ObjectType { get; set; }
        public required string Referance { get; set; }
    }
}
