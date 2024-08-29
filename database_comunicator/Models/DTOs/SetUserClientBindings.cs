﻿namespace database_comunicator.Models.DTOs
{
    public class SetUserClientBindings
    {
        public required int OrgId { get; set; }
        public IEnumerable<int> UsersId { get; set; } = new List<int>();
    }
}
