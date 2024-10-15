namespace database_communicator.Models.DTOs
{
    public class AddNote
    {
        public required string Note {  get; set; }
        public required int UserId { get; set; }
        public required int DeliveryId { get; set; }
    }
}
