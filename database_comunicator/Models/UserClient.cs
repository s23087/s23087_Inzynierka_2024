namespace database_comunicator.Models
{
    public partial class UserClient
    {
        public int IdUser { get; set; }
        public int OrganizationId { get; set; }
        public virtual AppUser AppUser { get; set; } = new AppUser();
        public virtual Organization Organization { get; set; } = new Organization();
    }
}
