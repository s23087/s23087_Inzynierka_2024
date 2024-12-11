namespace database_communicator.Models;

public partial class ActionLog
{
    public int LogId { get; set; }

    public string LogDescription { get; set; } = null!;

    public DateTime LogDate { get; set; }

    public int UsersId { get; set; }

    public int LogTypeId { get; set; }

    public virtual LogType LogType { get; set; } = null!;

    public virtual AppUser Users { get; set; } = null!;
}
