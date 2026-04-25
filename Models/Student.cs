using System;
using System.Collections.Generic;

namespace WebApplication1.Models;

public partial class Student
{
    public int StudentId { get; set; }

    public int? SuperId { get; set; }

    public int? ReportId { get; set; }

    public int? CollegeId { get; set; }

    public string? StDepartment { get; set; }

    public string? FullName { get; set; }

    public float? Gpa { get; set; }

    public int? Resume { get; set; }

    public virtual ICollection<Application> Applications { get; set; } = new List<Application>();

    public virtual College? College { get; set; }

    public virtual ICollection<InternshipOpportunity> InternshipOpportunities { get; set; } = new List<InternshipOpportunity>();

    public virtual Report? Report { get; set; }

    public virtual Supervisor? Super { get; set; }
}
