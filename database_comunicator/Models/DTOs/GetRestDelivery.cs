namespace database_communicator.Models.DTOs
{
    public class GetRestDelivery
    {
        public IEnumerable<GetNote> Notes { get; set; } = new List<GetNote>();
        public IEnumerable<GetItemForDeliveryTable> Items { get; set; } = new List<GetItemForDeliveryTable>();
    }
}
