using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Pages.Supervisor
{
    [Authorize(Roles = "Supervisor")]
    public class DashboardModel : PageModel
    {
        private readonly MydbContext _context;

        public DashboardModel(MydbContext context)
        {
            _context = context;
        }

        public Models.Supervisor? SupervisorInfo { get; set; }
        public List<Application> PendingApplications { get; set; } = new();
        public List<Application> ApprovedApplications { get; set; } = new();
        public List<Report> Reports { get; set; } = new();

        public async Task OnGetAsync()
        {
            var email = User.Identity?.Name;
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (user != null)
            {
                SupervisorInfo = await _context.Supervisors
                    .Include(s => s.College)
                    .FirstOrDefaultAsync(s => s.SupervisorId == user.UserId);
            }

            // Load all pending applications (status = 0)
            PendingApplications = await _context.Applications
                .Include(a => a.Std)
                .Include(a => a.Comp)
                .Include(a => a.Internship)
                .Where(a => a.Status == 0)
                .ToListAsync();

            // Load approved applications
            ApprovedApplications = await _context.Applications
                .Include(a => a.Std)
                .Include(a => a.Comp)
                .Include(a => a.Internship)
                .Where(a => a.Status == 1)
                .ToListAsync();

            // Load reports
            if (SupervisorInfo != null)
            {
                Reports = await _context.Reports
                    .Include(r => r.Comp)
                    .Where(r => r.SuperId == SupervisorInfo.SupervisorId)
                    .ToListAsync();
            }
        }

        public async Task<IActionResult> OnPostAcceptAsync(int applicationId)
        {
            var app = await _context.Applications.FindAsync(applicationId);
            if (app != null)
            {
                app.Status = 1; // Approved
                await _context.SaveChangesAsync();
            }
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostRejectAsync(int applicationId)
        {
            var app = await _context.Applications.FindAsync(applicationId);
            if (app != null)
            {
                app.Status = 2; // Rejected
                await _context.SaveChangesAsync();
            }
            return RedirectToPage();
        }
    }
}
