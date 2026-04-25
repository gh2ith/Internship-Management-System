using System;
using System.Collections.Generic;

namespace WebApplication1.Models;

public partial class InternshipOpportunity
{
    public int InternshipId { get; set; }

    public int? CompanyId { get; set; }

    public int? SuperId { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }

    public DateTime? Deadline { get; set; }

    public string? Requirments { get; set; }

    public int? StdId { get; set; }

    public bool? IsClosed { get; set; }

    public int? Capacity { get; set; }

    public virtual ICollection<Application> Applications { get; set; } = new List<Application>();

    public virtual Company? Company { get; set; }

    public virtual Student? Std { get; set; }

    public virtual Supervisor? Super { get; set; }
}
