namespace database_communicator.Models.DTOs.Get
{
    public class GetRestModifyOffer
    {
        public int MaxQty { get; set; }
        public IEnumerable<GetOfferItemForModify> Items { get; set; } = new List<GetOfferItemForModify>();
    }
}
