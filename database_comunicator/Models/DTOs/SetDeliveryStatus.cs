﻿namespace database_comunicator.Models.DTOs
{
    public class SetDeliveryStatus
    {
        public required string StatusName { get; set; }
        public required int DeliveryId { get; set; }
    }
}