﻿namespace database_communicator.Models.DTOs
{
    public class GetClientRestInfo
    {
        public int? CreditLimit { get; set; }
        public string? Availability { get; set; } = null!;
        public int? DaysForRealization { get; set; }
    }
}
