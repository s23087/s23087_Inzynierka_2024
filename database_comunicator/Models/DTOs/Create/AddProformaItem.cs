namespace database_communicator.Models.DTOs.Create
{
    public class AddProformaItem
    {
        public required int ItemId { get; set; }
        public required int Qty { get; set; }
        public required decimal Price { get; set; }
    }
}
