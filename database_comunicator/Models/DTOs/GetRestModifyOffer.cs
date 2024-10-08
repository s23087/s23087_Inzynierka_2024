namespace database_comunicator.Models.DTOs
{
    public class GetRestModifyOffer
    {
        public int MaxQty { get; set; }
        public IEnumerable<GetOfferItemForModify> Items { get; set; } = new List<GetOfferItemForModify>();
    }
}
