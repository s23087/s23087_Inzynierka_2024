﻿namespace database_communicator.Models.DTOs
{
    public class GetLogs
    {
        public string LogType { get; set; } = null!;
        public string LogDescription { get; set; } = null!;
        public DateTime LogDate { get; set; }
        public string Username { get; set; } = null!;
        public string Surname { get; set; } = null!;
    }
}
