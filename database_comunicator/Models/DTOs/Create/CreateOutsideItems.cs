namespace database_communicator.Models.DTOs.Create
{
    public class CreateOutsideItems
    {
        public required int OrgId { get; set; }
        public required string Currency { get; set; }
        public IEnumerable<AddOutsideItem> Items { get; set; } = new List<AddOutsideItem>();
    }
}
