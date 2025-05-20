using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UniFlowSn.Models.Db;

public partial class Event
{
    public int Id { get; set; }

    [Display(Name = "Título")]
    public string Title { get; set; } = null!;

    [Display(Name = "Descrição")]
    public string Description { get; set; } = null!;

    [Display(Name = "Descrição Completa")]
    public string? FullDescription { get; set; }

    [Display(Name = "Imagem")]
    public string? ImageName { get; set; }

    [Display(Name = "Ingressos Disponíveis")]
    public int? Qty { get; set; }

    public string? Tags { get; set; }

    [Display(Name = "URL do Vídeo")]
    public string? VideoUrl { get; set; }

    [Display(Name = "Início")]
    public DateTime? DtStart { get; set; }

    [Display(Name = "Fim")]
    public DateTime? DtEnd { get; set; }

    [Display(Name = "Local")]
    public int? PlaceId { get; set; }

    [Display(Name = "Tipo")]
    public int? TypeId { get; set; }

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual ICollection<EventGallery> EventGalleries { get; set; } = new List<EventGallery>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    [Display(Name = "Local")]
    public virtual EventPlace? Place { get; set; }

    [Display(Name = "Tipo")]
    public virtual EventType? Type { get; set; }
}
