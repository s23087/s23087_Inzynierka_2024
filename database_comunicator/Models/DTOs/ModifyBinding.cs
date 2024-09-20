namespace database_comunicator.Models.DTOs
{
    public class ModifyBinding
    {
        public int UserId { get; set; }

        public int InvoiceId { get; set; }

        public int ItemId { get; set; }

        public int Qty { get; set; }
    }
}
