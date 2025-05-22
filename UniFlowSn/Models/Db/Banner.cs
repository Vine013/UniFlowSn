using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UniFlowSn.Models.Db;

public partial class Banner
{
    public int Id { get; set; }

    [Display(Name ="Título")]
    public string? Title { get; set; }

    [Display(Name = "Subtítulo")]
    public string? SubTitle { get; set; }

    [Display(Name = "Imagem")]
    public string? ImageName { get; set; }

    [Display(Name = "Prioridade")]
    public short? Priority { get; set; }

    public string? Link { get; set; }

    [Display(Name = "Posição")]
    public string Position { get; set; } = null!;
}
