using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UniFlowSn.Models.Db;

public partial class Comment
{
    public int Id { get; set; }

    [Display(Name = "Autor")]
    public string Name { get; set; } = null!;

    [Display(Name = "E-mail")]
    public string Email { get; set; } = null!;

    [Display(Name = "Comentário")]
    public string CommentText { get; set; } = null!;

    public int EventId { get; set; }

    [Display(Name = "Data de Criação")]
    public DateTime CreateDate { get; set; }

    [Display(Name = "Evento")]
    public virtual Event Event { get; set; } = null!;
}
