using System;
using System.Collections.Generic;

namespace UniFlowSn.Models.Db;

public partial class Comment
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string CommentText { get; set; } = null!;

    public int EventId { get; set; }

    public DateTime CreateDate { get; set; }

    public virtual Event Event { get; set; } = null!;
}
