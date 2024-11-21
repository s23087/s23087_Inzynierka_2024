namespace database_communicator.Models.DTOs.Get
{
    public class GetRestProforma
    {
        public decimal Taxes { get; set; }
        public decimal CurrencyValue { get; set; }
        public DateTime CurrencyDate { get; set; }
        public string PaymentMethod { get; set; } = null!;
        public bool InSystem { get; set; }
        public string Note { get; set; } = null!;
        public string Path { get; set; } = null!;
        public IEnumerable<GetCredtItemForTable> Items { get; set; } = new List<GetCredtItemForTable>();
    }
}
