﻿namespace database_communicator.Models.DTOs.Create
{
    public class AddOffer
    {
        public required int UserId { get; set; }
        public required string OfferName { get; set; }
        public required string Path { get; set; }
        public required int OfferStatusId { get; set; }
        public required int MaxQty { get; set; }
        public required string Currency { get; set; }
        public IEnumerable<AddOfferItem> Items { get; set; } = new List<AddOfferItem>();
    }
}
