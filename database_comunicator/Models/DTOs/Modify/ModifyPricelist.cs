using database_communicator.Models.DTOs.Create;

namespace database_communicator.Models.DTOs.Modify
{
    public class ModifyPricelist
    {
        public required int UserId { get; set; }
        public required int OfferId { get; set; }
        public string? OfferName { get; set; }
        public int? OfferStatusId { get; set; }
        public int? MaxQty { get; set; }
        public string? CurrencyName { get; set; }
        public string? Path { get; set; }
        public string? Type { get; set; }
        public IEnumerable<AddOfferItem> Items { get; set; } = new List<AddOfferItem>();
    }
}
