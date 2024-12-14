﻿namespace database_communicator.Models.DTOs.Get
{
    public class GetOrgUsersWithRoles
    {
        public int UserId { get; set; }
        public string Username { get; set; } = null!;
        public string Surname { get; set; } = null!;
        public string RoleName { get; set; } = null!;
    }
}