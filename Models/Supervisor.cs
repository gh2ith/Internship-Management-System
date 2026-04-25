using System;
using System.Collections.Generic;

namespace WebApplication1.Models;

public partial class Supervisor
{
    public int SupervisorId { get; set; }

    public string? FullName { get; set; }

    public string? SuperDepartment { get; set; }

    public int? CollegeId { get; set; }

    public virtual ICollection<Application> Applications { get; set; } = new List<Application>();

    public virtual College? College { get; set; }

    public virtual ICollection<InternshipOpportunity> InternshipOpportunities { get; set; } = new List<InternshipOpportunity>();

    public virtual ICollection<Report> Reports { get; set; } = new List<Report>();

    public virtual ICollection<Student> Students { get; set; } = new List<Student>();
}
