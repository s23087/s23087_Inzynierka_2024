﻿namespace database_communicator.Models.DTOs.Get
{
    public class RestOrgs
    {
        public int OrgId { get; set; }
        public string OrgName { get; set; } = null!;
    }
    public class GetOrgsForInvocie
    {
        public int UserOrgId { get; set; }
        public string OrgName { get; set; } = null!;
        public IEnumerable<RestOrgs> RestOrgs { get; set; } = new List<RestOrgs>();

    }
}
