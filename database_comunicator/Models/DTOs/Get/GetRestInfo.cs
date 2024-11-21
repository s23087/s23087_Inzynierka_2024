namespace database_communicator.Models.DTOs.Get
{
    public class GetRestInfo
    {
        public IEnumerable<GetRestInfoOutsideItems> OutsideItemInfos { get; set; } = new List<GetRestInfoOutsideItems>();
        public IEnumerable<GetRestInfoOwnedItems> OwnedItemInfos { get; set; } = new List<GetRestInfoOwnedItems>();
    }
}
