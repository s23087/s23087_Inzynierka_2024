namespace database_communicator.Models.DTOs.Create
{
    public class AddItem
    {
        public required int UserId { get; set; }
        public string ItemName { get; set; } = null!;

        public string ItemDescription { get; set; } = null!;

        public string PartNumber { get; set; } = null!;
        public ICollection<string> Eans { get; set; } = new List<string>();
    }
}
