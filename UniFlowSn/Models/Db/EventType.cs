using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UniFlowSn.Models.Db;

public partial class EventType
{
    public int Id { get; set; }

    [Display(Name = "Tipo")]
    public string Type { get; set; } = null!;

    [Display(Name = "Icone")]
    public string? Icon { get; set; }

    public virtual ICollection<Event> Events { get; set; } = new List<Event>();
}
