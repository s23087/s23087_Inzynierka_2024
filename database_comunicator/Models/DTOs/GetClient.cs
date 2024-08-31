namespace database_comunicator.Models.DTOs
{
    public class GetClient
    {
        public int ClientId { get; set; }
        public string ClientName { get; set; } = null!;
        public string Street { get; set; } = null!;
        public string City { get; set; } = null!;
        public string Postal { get; set; } = null!;
        public int? Nip { get; set; }
        public string Country { get; set; } = null!;
    }
}
