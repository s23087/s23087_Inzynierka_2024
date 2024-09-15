namespace database_comunicator.Models.DTOs
{
    public class GetRestInfo
    {
        public IEnumerable<GetRestInfoOutsideItems> OutsideItemInfos { get; set; } = new List<GetRestInfoOutsideItems>();
        public IEnumerable<GetRestInfoOwnedItems> OwnedItemInfos { get; set; } = new List<GetRestInfoOwnedItems>();
    }
}
