﻿namespace database_communicator.Models.DTOs.Get
{
    public class GetBinding
    {
        public int? UserId { get; set; }
        public string? Username { get; set; } = null!;
        public int Qty { get; set; }
        public decimal Price { get; set; }
        public string Currency { get; set; } = null!;
        public string InvoiceNumber { get; set; } = null!;
        public int InvoiceId { get; set; }
    }
}
