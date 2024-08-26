namespace database_comunicator.Models.DTOs
{
    public class GetRestInfo
    {
        public string ItemDescription { get; set; } = null!;
        public IEnumerable<GetRestItemInfo> OutsideItemInfos { get; set; } = new List<GetRestItemInfo>();
        public IEnumerable<GetRestItemInfo> OwnedItemInfos { get; set; } = new List<GetRestItemInfo>();
    }
}
