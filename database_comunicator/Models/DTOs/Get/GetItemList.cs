﻿namespace database_communicator.Models.DTOs.Get
{
    public class GetItemList
    {
        public int Id { get; set; }
        public string Partnumber { get; set; } = null!;
        public string Name { get; set; } = null!;
    }
}