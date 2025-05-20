using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UniFlowSn.Models.Db;

public partial class Menu
{
    public int Id { get; set; }

    [Display(Name ="Título")]
    public string? MenuTitle { get; set; }

    public string? Link { get; set; }

    [Display(Name = "Tipo")]
    public string? Type { get; set; }
}
