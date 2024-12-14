﻿namespace database_communicator.Models.DTOs.Get
{
    public class GetCredtItemForTable
    {
        public int CreditItemId { get; set; }
        public string Partnumber { get; set; } = null!;
        public string ItemName { get; set; } = null!;
        public int Qty { get; set; }
        public decimal Price { get; set; }
    }
}