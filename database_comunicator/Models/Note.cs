namespace database_communicator.Models;

public partial class Note
{
    public int NoteId { get; set; }

    public string NoteDescription { get; set; } = null!;

    public DateTime NoteDate { get; set; }

    public int UsersId { get; set; }

    public virtual AppUser Users { get; set; } = null!;

    public virtual ICollection<Delivery> Deliveries { get; set; } = new List<Delivery>();

    public virtual ICollection<Request> Requests { get; set; } = new List<Request>();
}
