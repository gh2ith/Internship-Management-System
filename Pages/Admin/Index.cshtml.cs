using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly MydbContext _context;

        public IndexModel(MydbContext context)
        {
            _context = context;
        }

        // Counts
        public int TotalStudents { get; set; }
        public int TotalCompanies { get; set; }
        public int TotalSupervisors { get; set; }
        public int TotalUsers { get; set; }
        public int TotalPosts { get; set; }
        public int TotalApplications { get; set; }
        public int PendingApplications { get; set; }
        public int ApprovedApplications { get; set; }

        // Detail lists
        public List<User> Users { get; set; } = new();
        public List<Models.Student> Students { get; set; } = new();
        public List<Models.Company> Companies { get; set; } = new();
        public List<Models.Supervisor> Supervisors { get; set; } = new();
        public List<InternshipOpportunity> Posts { get; set; } = new();
        public List<Application> Applications { get; set; } = new();

        public async Task OnGetAsync()
        {
            Users = await _context.Users.Include(u => u.College).ToListAsync();
            Students = await _context.Students.Include(s => s.College).Include(s => s.Super).ToListAsync();
            Companies = await _context.Companies.ToListAsync();
            Supervisors = await _context.Supervisors.Include(s => s.College).ToListAsync();
            Posts = await _context.InternshipOpportunities.Include(i => i.Company).ToListAsync();
            Applications = await _context.Applications
                .Include(a => a.Std).Include(a => a.Comp).Include(a => a.Internship)
                .ToListAsync();

            TotalUsers = Users.Count;
            TotalStudents = Students.Count;
            TotalCompanies = Companies.Count;
            TotalSupervisors = Supervisors.Count;
            TotalPosts = Posts.Count;
            TotalApplications = Applications.Count;
            PendingApplications = Applications.Count(a => a.Status == 0);
            ApprovedApplications = Applications.Count(a => a.Status == 1);
        }
    }
}
