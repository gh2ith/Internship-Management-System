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
        public List<Models.Student> AssignedStudents { get; set; } = new();
        public List<Application> PendingApplications { get; set; } = new();
        public List<Application> ApprovedApplications { get; set; } = new();
        public List<Application> AllApplications { get; set; } = new();
        public List<Report> Reports { get; set; } = new();

        // Computed stats
        public int AssignedStudentCount => AssignedStudents.Count;
        public int ReportsSubmittedCount => Reports.Count;
        public int ReportsMissingCount => AssignedStudents.Count(s => s.ReportId == null);
        public int TotalEvaluationsCount => Reports.Sum(r => r.NumberOfHours ?? 0);

        [TempData]
        public string? SuccessMessage { get; set; }

        [TempData]
        public string? ErrorMessage { get; set; }

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

            if (SupervisorInfo != null)
            {
                // Load students assigned to this supervisor
                AssignedStudents = await _context.Students
                    .Include(s => s.College)
                    .Include(s => s.Report)
                    .Where(s => s.SuperId == SupervisorInfo.SupervisorId)
                    .ToListAsync();

                // Load reports for this supervisor
                Reports = await _context.Reports
                    .Include(r => r.Comp)
                    .Include(r => r.Student)
                    .Where(r => r.SuperId == SupervisorInfo.SupervisorId)
                    .ToListAsync();
            }

            // Load pending applications (supervisor can review)
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

            // All applications for the applications tab
            AllApplications = await _context.Applications
                .Include(a => a.Std)
                .Include(a => a.Comp)
                .Include(a => a.Internship)
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostAcceptAsync(int applicationId)
        {
            var app = await _context.Applications.FindAsync(applicationId);
            if (app != null)
            {
                app.Status = 1; // Approved

                // Also assign the student to this supervisor
                var email = User.Identity?.Name;
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
                if (user != null && app.StdId.HasValue)
                {
                    var student = await _context.Students.FindAsync(app.StdId.Value);
                    if (student != null)
                    {
                        student.SuperId = user.UserId;
                    }
                    app.SuperId = user.UserId;
                }

                await _context.SaveChangesAsync();
                SuccessMessage = "Application approved successfully.";
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
                SuccessMessage = "Application rejected.";
            }
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostUnassignStudentAsync(int studentId)
        {
            var student = await _context.Students.FindAsync(studentId);
            if (student != null)
            {
                student.SuperId = null;
                
                var reports = await _context.Reports.Where(r => r.StudentId == studentId).ToListAsync();
                foreach (var r in reports) r.SuperId = null;

                await _context.SaveChangesAsync();
                SuccessMessage = $"{student.FullName} has been unassigned.";
            }
            return RedirectToPage();
        }
    }
}
