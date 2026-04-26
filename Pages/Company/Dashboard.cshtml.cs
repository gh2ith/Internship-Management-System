using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Pages.Company
{
    [Authorize(Roles = "Company")]
    public class DashboardModel : PageModel
    {
        private readonly MydbContext _context;

        public DashboardModel(MydbContext context)
        {
            _context = context;
        }

        public Models.Company? CompanyInfo { get; set; }
        public List<InternshipOpportunity> Posts { get; set; } = new();
        public List<Application> Applications { get; set; } = new();
        public List<Report> SubmittedReports { get; set; } = new();
        public List<Evaluation> Evaluations { get; set; } = new();

        [TempData]
        public string? ErrorMessage { get; set; }

        [TempData]
        public string? SuccessMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var userIdClaim = User.FindFirst("UserId")?.Value;
            if (userIdClaim == null || !int.TryParse(userIdClaim, out var companyId))
            {
                return RedirectToPage("/Account/Login");
            }

            CompanyInfo = await _context.Companies.FirstOrDefaultAsync(c => c.CompanyId == companyId);

            if (CompanyInfo != null)
            {
                // Load this company's internship posts with their applications
                Posts = await _context.InternshipOpportunities
                    .Include(i => i.Applications)
                    .Where(i => i.CompanyId == CompanyInfo.CompanyId)
                    .ToListAsync();

                // Load all applications for this company's posts
                Applications = await _context.Applications
                    .Include(a => a.Std)
                    .Include(a => a.Internship)
                    .Where(a => a.CompId == CompanyInfo.CompanyId)
                    .ToListAsync();

                // Load submitted reports for this company
                SubmittedReports = await _context.Reports
                    .Include(r => r.Student)
                    .Where(r => r.CompId == CompanyInfo.CompanyId)
                    .ToListAsync();

                // Load evaluations
                Evaluations = await _context.Evaluations
                    .Include(e => e.Student)
                    .Where(e => e.CompId == CompanyInfo.CompanyId)
                    .ToListAsync();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostDeletePostAsync(int internshipId)
        {
            var email = User.Identity?.Name;
            var company = await _context.Companies.FirstOrDefaultAsync(c => c.ContactInfo == email);
            if (company == null) return RedirectToPage();

            var post = await _context.InternshipOpportunities
                .Include(i => i.Applications)
                .FirstOrDefaultAsync(i => i.InternshipId == internshipId && i.CompanyId == company.CompanyId);

            if (post != null)
            {
                var acceptedCount = post.Applications.Count(a => a.Status == 1);
                
                if (acceptedCount > 0)
                {
                    ErrorMessage = "Cannot delete an internship that has accepted students. Please edit the post and mark it as 'Closed' instead, or contact the university administration.";
                    return RedirectToPage();
                }

                // Delete any pending or rejected applications attached to this post to satisfy SQL constraints
                var removableApplications = post.Applications.Where(a => a.Status == 0 || a.Status == 2).ToList();
                if (removableApplications.Any())
                {
                    _context.Applications.RemoveRange(removableApplications);
                }

                _context.InternshipOpportunities.Remove(post);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostSubmitWeeklyReportAsync(int studentId, int weekNumber, string task, string tools, int numberOfHours)
        {
            var email = User.Identity?.Name;
            var company = await _context.Companies.FirstOrDefaultAsync(c => c.ContactInfo == email);
            if (company == null) return RedirectToPage();

            // Get the student's supervisor
            var student = await _context.Students.FirstOrDefaultAsync(s => s.StudentId == studentId);
            if (student == null)
            {
                ErrorMessage = "Student not found.";
                return RedirectToPage();
            }

            // Check if this week's report already exists
            var existing = await _context.Reports.FirstOrDefaultAsync(r =>
                r.CompId == company.CompanyId &&
                r.StudentId == studentId &&
                r.WeekNumber == weekNumber);

            if (existing != null)
            {
                ErrorMessage = $"Week {weekNumber} report for this student has already been submitted.";
                return RedirectToPage();
            }

            // Generate a new report ID
            var maxId = await _context.Reports.MaxAsync(r => (int?)r.ReportId) ?? 0;
            var report = new Report
            {
                ReportId = maxId + 1,
                CompId = company.CompanyId,
                SuperId = student.SuperId,
                StudentId = studentId,
                WeekNumber = weekNumber,
                Task = task,
                Tools = tools,
                NumberOfHours = numberOfHours
            };

            _context.Reports.Add(report);
            await _context.SaveChangesAsync();

            SuccessMessage = $"Week {weekNumber} report submitted successfully!";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostSubmitFinalEvaluationAsync(
            int studentId, string performanceLevel, string responsibility,
            string punctuality, string accuracy, string teamwork,
            string adaptability, string skillAcquisition, string overallCompletion,
            string? comments)
        {
            var email = User.Identity?.Name;
            var company = await _context.Companies.FirstOrDefaultAsync(c => c.ContactInfo == email);
            if (company == null) return RedirectToPage();

            var student = await _context.Students.FirstOrDefaultAsync(s => s.StudentId == studentId);
            if (student == null)
            {
                ErrorMessage = "Student not found.";
                return RedirectToPage();
            }

            // Check for duplicate
            var existing = await _context.Evaluations.FirstOrDefaultAsync(e =>
                e.CompId == company.CompanyId && e.StudentId == studentId);
            if (existing != null)
            {
                ErrorMessage = "Final evaluation for this student has already been submitted.";
                return RedirectToPage();
            }

            var evaluation = new Evaluation
            {
                StudentId = studentId,
                CompId = company.CompanyId,
                SuperId = student.SuperId,
                PerformanceLevel = performanceLevel,
                Responsibility = responsibility,
                Punctuality = punctuality,
                Accuracy = accuracy,
                Teamwork = teamwork,
                Adaptability = adaptability,
                SkillAcquisition = skillAcquisition,
                OverallCompletion = overallCompletion,
                Comments = comments,
                CreatedAt = DateTime.Now
            };

            _context.Evaluations.Add(evaluation);
            await _context.SaveChangesAsync();

            SuccessMessage = "Final evaluation submitted successfully!";
            return RedirectToPage();
        }
    }
}
