namespace database_communicator.Models.DTOs.Modify
{
    public class SetUserClientBindings
    {
        public required int OrgId { get; set; }
        public IEnumerable<int> UsersId { get; set; } = new List<int>();
    }
}
