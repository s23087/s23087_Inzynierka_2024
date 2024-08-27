namespace database_comunicator.Models.DTOs
{
    public class GetRestInfoOrg
    {
        public int UserId { get; set; }
        public string Username { get; set; } = null!;
        public IEnumerable<GetRestItemInfo> OutsideItemInfos { get; set; } = new List<GetRestItemInfo>();
        public IEnumerable<GetRestItemInfo> OwnedItemInfos { get; set; } = new List<GetRestItemInfo>();
    }
}
