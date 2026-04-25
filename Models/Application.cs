using System;
using System.Collections.Generic;

namespace WebApplication1.Models;

public partial class Application
{
    public int ApplicationId { get; set; }

    public int? SuperId { get; set; }

    public int? InternshipId { get; set; }

    public int? StdId { get; set; }

    public int? CompId { get; set; }

    public sbyte? Status { get; set; }

    public virtual Company? Comp { get; set; }

    public virtual InternshipOpportunity? Internship { get; set; }

    public virtual Student? Std { get; set; }

    public virtual Supervisor? Super { get; set; }
}
