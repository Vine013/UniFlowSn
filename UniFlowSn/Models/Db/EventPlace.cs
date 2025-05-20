using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UniFlowSn.Models.Db;

public partial class EventPlace
{
    [Display(Name = "Código")]
    public int Id { get; set; }

    [Display(Name = "Local")]
    public string Place { get; set; } = null!;

    [Display(Name = "Endereço")]
    public string Address { get; set; } = null!;

    [Display(Name = "Cidade")]
    public string? City { get; set; }

    [Display(Name = "Estado")]
    public string? State { get; set; }

    public virtual ICollection<Event> Events { get; set; } = new List<Event>();
}
