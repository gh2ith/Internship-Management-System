using System;
using System.Collections.Generic;

namespace WebApplication1.Models;

public partial class College
{
    public int CollegeId { get; set; }

    public string? CollegeName { get; set; }

    public string? UniName { get; set; }

    public virtual ICollection<Student> Students { get; set; } = new List<Student>();

    public virtual ICollection<Supervisor> Supervisors { get; set; } = new List<Supervisor>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
