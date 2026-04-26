using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models;

public partial class Report
{
    public int ReportId { get; set; }

    public int? CompId { get; set; }

    public int? SuperId { get; set; }

    public string? Task { get; set; }

    public string? Tools { get; set; }

    public int? NumberOfHours { get; set; }

    [Column("week_number")]
    public int? WeekNumber { get; set; }

    [Column("student_id")]
    public int? StudentId { get; set; }

    public virtual Company? Comp { get; set; }

    public virtual ICollection<Student> Students { get; set; } = new List<Student>();

    public virtual Supervisor? Super { get; set; }

    [ForeignKey("StudentId")]
    public virtual Student? Student { get; set; }
}
