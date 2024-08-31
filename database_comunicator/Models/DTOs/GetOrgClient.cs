namespace database_comunicator.Models.DTOs
{
    public class GetOrgClient
    {
        public int ClientId { get; set; }
        public IEnumerable<string> Users { get; set; } = new List<string>();
        public string ClientName { get; set; } = null!;
        public string Street { get; set; } = null!;
        public string City { get; set; } = null!;
        public string Postal { get; set; } = null!;
        public int? Nip { get; set; }
        public string Country { get; set; } = null!;
    }
}
