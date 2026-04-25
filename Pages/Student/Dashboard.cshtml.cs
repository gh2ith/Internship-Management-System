using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Pages.Student
{
    [Authorize(Roles = "Student")]
    public class DashboardModel : PageModel
    {
        private readonly MydbContext _context;

        public DashboardModel(MydbContext context)
        {
            _context = context;
        }

        public Models.Student? StudentInfo { get; set; }
        public List<InternshipOpportunity> AvailablePosts { get; set; } = new();
        public List<Application> MyApplications { get; set; } = new();

        public async Task OnGetAsync()
        {
            var email = User.Identity?.Name;

            // Find the student's user record, then find the student profile
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user != null)
            {
                StudentInfo = await _context.Students
                    .Include(s => s.College)
                    .FirstOrDefaultAsync(s => s.StudentId == user.UserId);
            }

            // Load all available internship posts with their company info
            AvailablePosts = await _context.InternshipOpportunities
                .Include(i => i.Company)
                .ToListAsync();

            // Load this student's applications
            if (StudentInfo != null)
            {
                MyApplications = await _context.Applications
                    .Include(a => a.Internship)
                    .Include(a => a.Comp)
                    .Where(a => a.StdId == StudentInfo.StudentId)
                    .ToListAsync();
            }
        }

        public async Task<IActionResult> OnPostApplyAsync(int internshipId)
        {
            var email = User.Identity?.Name;
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null) return RedirectToPage();

            var student = await _context.Students.FirstOrDefaultAsync(s => s.StudentId == user.UserId);
            if (student == null) return RedirectToPage();

            var post = await _context.InternshipOpportunities
                .FirstOrDefaultAsync(i => i.InternshipId == internshipId);
            if (post == null) return RedirectToPage();

            // Check if already applied
            var alreadyApplied = await _context.Applications
                .AnyAsync(a => a.StdId == student.StudentId && a.InternshipId == internshipId);
            if (alreadyApplied) return RedirectToPage();

            // Get next ID
            var maxId = await _context.Applications
                .MaxAsync(a => (int?)a.ApplicationId) ?? 0;

            var application = new Application
            {
                ApplicationId = maxId + 1,
                StdId = student.StudentId,
                InternshipId = internshipId,
                CompId = post.CompanyId,
                Status = 0 // Pending
            };

            _context.Applications.Add(application);
            await _context.SaveChangesAsync();

            return RedirectToPage();
        }
    }
}
