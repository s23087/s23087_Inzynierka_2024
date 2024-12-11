﻿namespace database_communicator.Models;

public partial class RequestStatus
{
    public int RequestStatusId { get; set; }

    public string StatusName { get; set; } = null!;

    public virtual ICollection<Request> Requests { get; set; } = new List<Request>();
}
