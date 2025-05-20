using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UniFlowSn.Models.Db;

public partial class EventGallery
{
    public int Id { get; set; }

    public int EventId { get; set; }

    [Display(Name ="Imagem")]
    public string? ImageName { get; set; }

    [Display(Name = "Evento")]
    public virtual Event Event { get; set; } = null!;
}
