namespace database_comunicator.Models.DTOs
{
    public class GetOneItem
    {
        public int ItemId { get; set; }
        public string ItemName { get; set; } = null!;
        public string ItemDescription { get; set; } = null!;
        public string PartNumber { get; set; } = null!;
        public string StatusName { get; set; } = null!;
        public ICollection<int> Eans { get; set; } = new List<int>();
        public ICollection<GetItemInvoiceData> Items { get; set; } = new List<GetItemInvoiceData>();
        public ICollection<GetOutsideItemData> OutsideItems { get; set; } = new List<GetOutsideItemData>();
        public int QtySum { get; set; }
        public decimal AvgPrice { get; set; }
    }
}
