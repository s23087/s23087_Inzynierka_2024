namespace database_comunicator.Models.DTOs
{
    public class AddOrganization
    {
        public string OrgName { get; set; } = null!;

        public int? Nip { get; set; }

        public string Street { get; set; } = null!;

        public string City { get; set; } = null!;

        public string PostalCode { get; set; } = null!;

        public int? CreditLimit { get; set; }

        public int CountryId { get; set; }
    }
}
