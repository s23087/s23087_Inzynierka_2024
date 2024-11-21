namespace database_communicator.Models.DTOs.Get
{
    public class GetUserBasicInfo
    {
        public string Username { get; set; } = null!;

        public string Surname { get; set; } = null!;
        public string OrgName { get; set; } = null!;
    }
}
