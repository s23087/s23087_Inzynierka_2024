namespace database_comunicator.Models.DTOs
{
    public class UpdateItem
    {
        public string ItemName { get; set; } = null!;

        public string ItemDescription { get; set; } = null!;

        public string PartNumber { get; set; } = null!;
    }
}
