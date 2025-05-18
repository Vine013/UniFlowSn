using System;
using System.Collections.Generic;

namespace UniFlowSn.Models.Db;

public partial class Event
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string? FullDescription { get; set; }

    public string? ImageName { get; set; }

    public int? Qty { get; set; }

    public string? Tags { get; set; }

    public string? VideoUrl { get; set; }

    public DateTime? DtStart { get; set; }

    public DateTime? DtEnd { get; set; }

    public int? PlaceId { get; set; }

    public int? TypeId { get; set; }

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual ICollection<EventGallery> EventGalleries { get; set; } = new List<EventGallery>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual EventPlace? Place { get; set; }

    public virtual EventType? Type { get; set; }
}
