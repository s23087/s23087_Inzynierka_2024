namespace database_communicator.Models.DTOs.Create
{
    public class AddOrganization
    {
        public required string OrgName { get; set; } = null!;

        public int? Nip { get; set; }

        public required string Street { get; set; } = null!;

        public required string City { get; set; } = null!;

        public required string PostalCode { get; set; } = null!;

        public int? CreditLimit { get; set; }

        public required int CountryId { get; set; }
    }
}
