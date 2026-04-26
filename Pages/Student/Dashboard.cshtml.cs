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

        public bool HasActiveApplication { get; set; }

        [TempData]
        public string? ErrorMessage { get; set; }

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
                .Include(i => i.Applications)
                .ToListAsync();

            // Load this student's applications
            if (StudentInfo != null)
            {
                MyApplications = await _context.Applications
                    .Include(a => a.Internship)
                    .Include(a => a.Comp)
                    .Where(a => a.StdId == StudentInfo.StudentId)
                    .ToListAsync();
                    
                HasActiveApplication = MyApplications.Any(a => a.Status == 0 || a.Status == 1);
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
                .Include(i => i.Applications)
                .FirstOrDefaultAsync(i => i.InternshipId == internshipId);
            if (post == null) return RedirectToPage();

            var acceptedCount = post.Applications.Count(a => a.Status == 1);
            var capacity = post.Capacity ?? 1;
            if (post.IsClosed == true || acceptedCount >= capacity)
            {
                ErrorMessage = "This internship post is no longer accepting applications (Closed or Full).";
                return RedirectToPage();
            }

            // Check if student already has a pending or accepted application
            var hasActiveApplication = await _context.Applications
                .AnyAsync(a => a.StdId == student.StudentId && (a.Status == 0 || a.Status == 1));
            
            if (hasActiveApplication) 
            {
                ErrorMessage = "You already have an active application. You must wait for the supervisor's decision before applying to another internship.";
                return RedirectToPage();
            }

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
