using System;
using System.Collections.Generic;

namespace WebApplication1.Models;

public partial class Company
{
    public int CompanyId { get; set; }

    public string? CompanyName { get; set; }

    public string? ContactInfo { get; set; }

    public string? Description { get; set; }

    public string? Location { get; set; }

    public virtual ICollection<Application> Applications { get; set; } = new List<Application>();

    public virtual ICollection<InternshipOpportunity> InternshipOpportunities { get; set; } = new List<InternshipOpportunity>();

    public virtual ICollection<Report> Reports { get; set; } = new List<Report>();
}
