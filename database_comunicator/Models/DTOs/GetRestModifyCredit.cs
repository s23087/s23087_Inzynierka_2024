﻿namespace database_communicator.Models.DTOs
{
    public class GetRestModifyCredit
    {
        public string CreditNumber { get; set; } = null!;
        public string OrgName { get; set; } = null!;
        public string Note { get; set; } = null!;
    }
}
