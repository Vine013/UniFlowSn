using System;
using System.Collections.Generic;

namespace UniFlowSn.Models.Db;

public partial class EventPlace
{
    public int Id { get; set; }

    public string Place { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string? City { get; set; }

    public string? State { get; set; }

    public virtual ICollection<Event> Events { get; set; } = new List<Event>();
}
