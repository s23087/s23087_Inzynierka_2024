using LINQtoCSV;

namespace database_communicator.Models.DTOs
{
    public class CreateCsvPricelist
    {
        [CsvColumn(FieldIndex = 1, Name = "Partnumber")]
        public string Partnumber { get; set; } = null!;

        [CsvColumn(FieldIndex = 2, Name = "Item_name")]
        public string ItemName { get; set; } = null!;

        [CsvColumn(FieldIndex = 3, Name = "Item_desc")]
        public string ItemDescription { get; set; } = null!;

        [CsvColumn(FieldIndex = 4, Name = "Eans")]
        public string Eans { get; set; } = null!;

        [CsvColumn(FieldIndex = 5, Name = "Qty")]
        public string Qty { get; set; } = null!;

        [CsvColumn(FieldIndex = 6, Name = "Price")]
        public decimal Price { get; set; }
    }
}
