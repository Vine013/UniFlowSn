using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UniFlowSn.Models.Db;

public partial class Order
{
    public int Id { get; set; }

    [Display(Name ="Código Usuário")]
    public int UserId { get; set; }

    [Display(Name = "Código Evento")]
    public int EventId { get; set; }

    [Display(Name = "Data")]
    public DateTime CreateDate { get; set; }

    public string Status { get; set; } = null!;

    [Display(Name = "Evento")]
    public virtual Event Event { get; set; } = null!;

    [Display(Name = "Usuário")]
    public virtual User User { get; set; } = null!;
}
