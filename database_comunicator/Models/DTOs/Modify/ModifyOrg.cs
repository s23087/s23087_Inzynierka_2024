namespace database_communicator.Models.DTOs.Modify
{
    public class ModifyOrg
    {
        public required int OrgId { get; set; }
        public string? OrgName { get; set; } = null!;

        public int? Nip { get; set; }

        public string? Street { get; set; } = null!;

        public string? City { get; set; } = null!;

        public string? PostalCode { get; set; } = null!;

        public int? CreditLimit { get; set; }

        public required int CountryId { get; set; }
    }
}
