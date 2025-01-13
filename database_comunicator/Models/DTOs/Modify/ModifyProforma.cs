using database_communicator.Models.DTOs.Create;

namespace database_communicator.Models.DTOs.Modify
{
    public class ModifyProforma
    {
        public required bool IsYourProforma { get; set; }
        public required int ProformaId { get; set; }
        public int? UserId { get; set; }
        public string? ProformaNumber { get; set; }
        public int? ClientId { get; set; }
        public decimal? Transport { get; set; }
        public int? PaymentMethodId { get; set; }
        public bool? InSystem { get; set; }
        public string? Path { get; set; }
        public string? Note { get; set; }
        public IEnumerable<AddProformaItem>? ProformaItems { get; set; } = new List<AddProformaItem>();
    }
}
