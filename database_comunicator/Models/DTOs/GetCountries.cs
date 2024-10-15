namespace database_communicator.Models.DTOs
{
    public class GetCountries
    {
        public int Id { get; set; }
        public string CountryName { get; set; } = null!;
    }
}
