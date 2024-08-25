namespace database_comunicator.Models.DTOs
{
    public class GetOutsideItemData
    {
        public string Source { get; set; } = null!;
        public int DaysForRealization { get; set; }
        public int Qty { get; set; }
        public decimal PurchasePrice { get; set; }
    }
}
