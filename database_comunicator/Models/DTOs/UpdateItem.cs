namespace database_communicator.Models.DTOs
{
    public class UpdateItem
    {
        public required int UserId {  get; set; }
        public required int Id { get; set; }
        public string? ItemName { get; set; } = null!;

        public string? ItemDescription { get; set; } = null!;

        public string? PartNumber { get; set; } = null!;
        public IEnumerable<string> Eans { get; set; } = new List<string>();
    }
}
