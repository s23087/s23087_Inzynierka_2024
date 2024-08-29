namespace database_comunicator.Models.DTOs
{
    public class GetOrg
    {
        public int Id { get; set; }
        public string OrgName { get; set; } = null!;

        public int? Nip { get; set; }

        public string Street { get; set; } = null!;

        public string City { get; set; } = null!;

        public string PostalCode { get; set; } = null!;
        public int CountryId { get; set; }

        public string Country { get; set; } = null!;
    }
}
