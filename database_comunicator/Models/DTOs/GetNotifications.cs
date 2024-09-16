﻿namespace database_comunicator.Models.DTOs
{
    public class GetNotifications
    {
        public int NotificationId { get; set; }

        public string Info { get; set; } = null!;

        public string ObjectType { get; set; }
        public string Referance { get; set; } = null!;
        public bool IsRead { get; set; }
    }
}