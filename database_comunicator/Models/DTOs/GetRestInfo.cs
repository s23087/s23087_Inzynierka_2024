﻿namespace database_comunicator.Models.DTOs
{
    public class GetRestInfo
    {
        public IEnumerable<GetRestItemInfo> OutsideItemInfos { get; set; } = new List<GetRestItemInfo>();
        public IEnumerable<GetRestItemInfo> OwnedItemInfos { get; set; } = new List<GetRestItemInfo>();
    }
}
