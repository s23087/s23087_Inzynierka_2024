namespace database_communicator.Models.DTOs.Create
{
    public class AddOfferItem
    {
        public required int ItemId { get; set; }
        public required decimal Price { get; set; }
    }
}
