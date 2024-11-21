namespace database_communicator.Models.DTOs.Get
{
    public class GetManyItems
    {
        public IEnumerable<string>? Users { get; set; } = new List<string>();
        public int ItemId { get; set; }
        public string ItemName { get; set; } = null!;
        public string PartNumber { get; set; } = null!;
        public string StatusName { get; set; } = null!;
        public IEnumerable<string>? Eans { get; set; } = new List<string>();
        public int? Qty { get; set; } = 0;
        public decimal? PurchasePrice { get; set; } = 0;
        public IEnumerable<string>? Sources { get; set; } = new List<string>();
    }
}
