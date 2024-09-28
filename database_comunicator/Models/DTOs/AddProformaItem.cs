namespace database_comunicator.Models.DTOs
{
    public class AddProformaItem
    {
        public required int ItemId { get; set; }
        public required int Qty { get; set; }
        public required decimal Price { get; set; }
    }
}
