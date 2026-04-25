using System;
using System.Collections.Generic;

namespace WebApplication1.Models;

public partial class Report
{
    public int ReportId { get; set; }

    public int? CompId { get; set; }

    public int? SuperId { get; set; }

    public string? Task { get; set; }

    public string? Tools { get; set; }

    public int? NumberOfHours { get; set; }

    public virtual Company? Comp { get; set; }

    public virtual ICollection<Student> Students { get; set; } = new List<Student>();

    public virtual Supervisor? Super { get; set; }
}
