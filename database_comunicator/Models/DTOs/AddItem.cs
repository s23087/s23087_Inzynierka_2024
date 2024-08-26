namespace database_comunicator.Models.DTOs
{
    public class AddItem
    {
        public int UserId { get; set; }
        public string ItemName { get; set; } = null!;

        public string ItemDescription { get; set; } = null!;

        public string PartNumber { get; set; } = null!;
        public ICollection<int> Eans { get; set; } = new List<int>();
    }
}
