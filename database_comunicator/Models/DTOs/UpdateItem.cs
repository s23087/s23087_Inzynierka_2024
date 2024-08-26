namespace database_comunicator.Models.DTOs
{
    public class UpdateItem
    {
        public int UserId {  get; set; }
        public int Id { get; set; }
        public string? ItemName { get; set; } = null!;

        public string? ItemDescription { get; set; } = null!;

        public string? PartNumber { get; set; } = null!;
        public IEnumerable<int> Eans { get; set; } = new List<int>();
    }
}
