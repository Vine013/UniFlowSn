using System;
using System.Collections.Generic;

namespace UniFlowSn.Models.Db;

public partial class EventGallery
{
    public int Id { get; set; }

    public int EventId { get; set; }

    public string? ImageName { get; set; }

    public virtual Event Event { get; set; } = null!;
}
