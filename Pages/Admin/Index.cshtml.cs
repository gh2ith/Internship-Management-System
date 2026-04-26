using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        public List<College> Colleges { get; set; } = new();

        [TempData]
        public string? SuccessMessage { get; set; }

        [TempData]
        public string? ErrorMessage { get; set; }

        public async Task OnGetAsync()
        {
            await LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            Users = await _context.Users.Include(u => u.College).ToListAsync();
            Students = await _context.Students.Include(s => s.College).Include(s => s.Super).ToListAsync();
            Companies = await _context.Companies.ToListAsync();
            Supervisors = await _context.Supervisors.Include(s => s.College).ToListAsync();
            Posts = await _context.InternshipOpportunities
                .Include(i => i.Company)
                .Include(i => i.Applications)
                .ToListAsync();
            Applications = await _context.Applications
                .Include(a => a.Std).Include(a => a.Comp).Include(a => a.Internship)
                .ToListAsync();
            Colleges = await _context.Colleges.ToListAsync();

            TotalUsers = Users.Count;
            TotalStudents = Students.Count;
            TotalCompanies = Companies.Count;
            TotalSupervisors = Supervisors.Count;
            TotalPosts = Posts.Count;
            TotalApplications = Applications.Count;
            PendingApplications = Applications.Count(a => a.Status == 0);
            ApprovedApplications = Applications.Count(a => a.Status == 1);
        }

        private async Task<int> GetNextUserIdAsync()
        {
            var maxUserId = await _context.Users.MaxAsync(u => (int?)u.UserId) ?? 0;
            var maxStudentId = await _context.Students.MaxAsync(s => (int?)s.StudentId) ?? 0;
            var maxCompanyId = await _context.Companies.MaxAsync(c => (int?)c.CompanyId) ?? 0;
            var maxSupervisorId = await _context.Supervisors.MaxAsync(s => (int?)s.SupervisorId) ?? 0;
            return Math.Max(Math.Max(maxUserId, maxStudentId), Math.Max(maxCompanyId, maxSupervisorId)) + 1;
        }

        // ──────── CREATE STUDENT ────────
        public async Task<IActionResult> OnPostCreateStudentAsync(string fullName, string department, float? gpa, int? collegeId, string email, string password)
        {
            if (string.IsNullOrWhiteSpace(fullName) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            { ErrorMessage = "Name, email, and password are required."; return RedirectToPage(); }

            if (await _context.Users.AnyAsync(u => u.Email == email))
            { ErrorMessage = $"Email '{email}' is already in use."; return RedirectToPage(); }

            var id = await GetNextUserIdAsync();
            var user = new User { UserId = id, Email = email, Password = DbSeeder.HashPassword(password), Role = "Student", CollegeId = collegeId };
            var student = new Models.Student { StudentId = id, FullName = fullName, StDepartment = department, Gpa = gpa, CollegeId = collegeId };

            _context.Users.Add(user);
            _context.Students.Add(student);
            await _context.SaveChangesAsync();

            SuccessMessage = $"Student \"{fullName}\" created successfully (ID: {id}).";
            return RedirectToPage();
        }

        // ──────── CREATE COMPANY ────────
        public async Task<IActionResult> OnPostCreateCompanyAsync(string companyName, string contactEmail, string? location, string? description, string password)
        {
            if (string.IsNullOrWhiteSpace(companyName) || string.IsNullOrWhiteSpace(contactEmail) || string.IsNullOrWhiteSpace(password))
            { ErrorMessage = "Company name, email, and password are required."; return RedirectToPage(); }

            if (await _context.Users.AnyAsync(u => u.Email == contactEmail))
            { ErrorMessage = $"Email '{contactEmail}' is already in use."; return RedirectToPage(); }

            var id = await GetNextUserIdAsync();
            var user = new User { UserId = id, Email = contactEmail, Password = DbSeeder.HashPassword(password), Role = "Company" };
            var company = new Models.Company { CompanyId = id, CompanyName = companyName, ContactInfo = contactEmail, Location = location, Description = description };

            _context.Users.Add(user);
            _context.Companies.Add(company);
            await _context.SaveChangesAsync();

            SuccessMessage = $"Company \"{companyName}\" created successfully (ID: {id}).";
            return RedirectToPage();
        }

        // ──────── CREATE SUPERVISOR ────────
        public async Task<IActionResult> OnPostCreateSupervisorAsync(string fullName, string department, int? collegeId, string email, string password)
        {
            if (string.IsNullOrWhiteSpace(fullName) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            { ErrorMessage = "Name, email, and password are required."; return RedirectToPage(); }

            if (await _context.Users.AnyAsync(u => u.Email == email))
            { ErrorMessage = $"Email '{email}' is already in use."; return RedirectToPage(); }

            var id = await GetNextUserIdAsync();
            var user = new User { UserId = id, Email = email, Password = DbSeeder.HashPassword(password), Role = "Supervisor", CollegeId = collegeId };
            var supervisor = new Models.Supervisor { SupervisorId = id, FullName = fullName, SuperDepartment = department, CollegeId = collegeId };

            _context.Users.Add(user);
            _context.Supervisors.Add(supervisor);
            await _context.SaveChangesAsync();

            SuccessMessage = $"Supervisor \"{fullName}\" created successfully (ID: {id}).";
            return RedirectToPage();
        }

        // ──────── ASSIGN SUPERVISOR ────────
        public async Task<IActionResult> OnPostAssignSupervisorAsync(int studentId, int supervisorId)
        {
            Console.WriteLine($"[DEBUG] OnPostAssignSupervisorAsync called with studentId={studentId}, supervisorId={supervisorId}");
            var student = await _context.Students.FindAsync(studentId);
            if (student == null) { ErrorMessage = "Student not found."; return RedirectToPage(); }

            student.SuperId = supervisorId;
            
            // Also assign existing reports to the new supervisor
            var reports = await _context.Reports.Where(r => r.StudentId == studentId).ToListAsync();
            foreach (var r in reports) r.SuperId = supervisorId;

            await _context.SaveChangesAsync();

            SuccessMessage = $"Supervisor assigned to {student.FullName} successfully.";
            return RedirectToPage();
        }

        // ──────── UNASSIGN SUPERVISOR ────────
        public async Task<IActionResult> OnPostUnassignSupervisorAsync(int studentId)
        {
            var student = await _context.Students.FindAsync(studentId);
            if (student == null) { ErrorMessage = "Student not found."; return RedirectToPage(); }

            student.SuperId = null;

            // Also unassign reports so they don't show up in the supervisor's dashboard
            var reports = await _context.Reports.Where(r => r.StudentId == studentId).ToListAsync();
            foreach (var r in reports) r.SuperId = null;

            await _context.SaveChangesAsync();

            SuccessMessage = $"Supervisor unassigned from {student.FullName}.";
            return RedirectToPage();
        }

        // ──────── DELETE STUDENT ────────
        public async Task<IActionResult> OnPostDeleteStudentAsync(int studentId)
        {
            var student = await _context.Students.Include(s => s.Applications).FirstOrDefaultAsync(s => s.StudentId == studentId);
            if (student == null) { ErrorMessage = "Student not found."; return RedirectToPage(); }

            // Remove related applications
            if (student.Applications.Any())
                _context.Applications.RemoveRange(student.Applications);

            // Remove related reports to ensure they don't show up in supervisor dashboards
            var reports = await _context.Reports.Where(r => r.StudentId == studentId).ToListAsync();
            if (reports.Any())
                _context.Reports.RemoveRange(reports);

            // Unassign from any internship posts
            var internships = await _context.InternshipOpportunities.Where(i => i.StdId == studentId).ToListAsync();
            foreach (var i in internships) i.StdId = null;

            _context.Students.Remove(student);

            // Remove user account
            var user = await _context.Users.FindAsync(studentId);
            if (user != null) _context.Users.Remove(user);

            await _context.SaveChangesAsync();
            SuccessMessage = $"Student \"{student.FullName}\" deleted and unassigned.";
            return RedirectToPage();
        }

        // ──────── DELETE COMPANY ────────
        public async Task<IActionResult> OnPostDeleteCompanyAsync(int companyId)
        {
            var company = await _context.Companies
                .Include(c => c.Applications)
                .Include(c => c.InternshipOpportunities).ThenInclude(i => i.Applications)
                .FirstOrDefaultAsync(c => c.CompanyId == companyId);
            if (company == null) { ErrorMessage = "Company not found."; return RedirectToPage(); }

            // Remove all applications for the company's internships
            foreach (var post in company.InternshipOpportunities)
                if (post.Applications.Any()) _context.Applications.RemoveRange(post.Applications);

            // Remove company applications
            if (company.Applications.Any())
                _context.Applications.RemoveRange(company.Applications);

            _context.InternshipOpportunities.RemoveRange(company.InternshipOpportunities);
            _context.Companies.Remove(company);

            var user = await _context.Users.FindAsync(companyId);
            if (user != null) _context.Users.Remove(user);

            await _context.SaveChangesAsync();
            SuccessMessage = $"Company \"{company.CompanyName}\" and all related data deleted.";
            return RedirectToPage();
        }

        // ──────── DELETE SUPERVISOR ────────
        public async Task<IActionResult> OnPostDeleteSupervisorAsync(int supervisorId)
        {
            var supervisor = await _context.Supervisors.FirstOrDefaultAsync(s => s.SupervisorId == supervisorId);
            if (supervisor == null) { ErrorMessage = "Supervisor not found."; return RedirectToPage(); }

            // Unassign from students
            var students = await _context.Students.Where(s => s.SuperId == supervisorId).ToListAsync();
            foreach (var s in students) s.SuperId = null;

            _context.Supervisors.Remove(supervisor);

            var user = await _context.Users.FindAsync(supervisorId);
            if (user != null) _context.Users.Remove(user);

            await _context.SaveChangesAsync();
            SuccessMessage = $"Supervisor \"{supervisor.FullName}\" deleted.";
            return RedirectToPage();
        }

        // ──────── DELETE INTERNSHIP POST ────────
        public async Task<IActionResult> OnPostDeletePostAsync(int internshipId)
        {
            var post = await _context.InternshipOpportunities
                .Include(i => i.Applications)
                .FirstOrDefaultAsync(i => i.InternshipId == internshipId);
            if (post == null) { ErrorMessage = "Post not found."; return RedirectToPage(); }

            if (post.Applications.Any(a => a.Status == 1))
            { ErrorMessage = "Cannot delete — this post has active accepted students. Terminate them first."; return RedirectToPage(); }

            if (post.Applications.Any())
                _context.Applications.RemoveRange(post.Applications);

            _context.InternshipOpportunities.Remove(post);
            await _context.SaveChangesAsync();

            SuccessMessage = $"Internship \"{post.Title}\" deleted.";
            return RedirectToPage();
        }

        // ──────── TERMINATE INTERNSHIP ────────
        public async Task<IActionResult> OnPostTerminateAsync(int applicationId)
        {
            var application = await _context.Applications
                .Include(a => a.Std).Include(a => a.Internship)
                .FirstOrDefaultAsync(a => a.ApplicationId == applicationId);
            if (application == null) { ErrorMessage = "Application not found."; return RedirectToPage(); }
            if (application.Status != 1) { ErrorMessage = "Only active internships can be terminated."; return RedirectToPage(); }

            application.Status = 3;
            
            // Unassign the supervisor from the student and their reports
            if (application.Std != null)
            {
                application.Std.SuperId = null;
                var reports = await _context.Reports.Where(r => r.StudentId == application.Std.StudentId).ToListAsync();
                foreach (var r in reports) r.SuperId = null;
            }

            await _context.SaveChangesAsync();

            SuccessMessage = $"Internship for {application.Std?.FullName ?? "student"} terminated.";
            return RedirectToPage();
        }

        // ──────── APPROVE APPLICATION ────────
        public async Task<IActionResult> OnPostApproveApplicationAsync(int applicationId)
        {
            var application = await _context.Applications
                .Include(a => a.Std).Include(a => a.Internship)
                .FirstOrDefaultAsync(a => a.ApplicationId == applicationId);
            if (application == null) { ErrorMessage = "Application not found."; return RedirectToPage(); }

            application.Status = 1;
            await _context.SaveChangesAsync();

            SuccessMessage = $"Application for {application.Std?.FullName ?? "student"} approved.";
            return RedirectToPage();
        }

        // ──────── REJECT APPLICATION ────────
        public async Task<IActionResult> OnPostRejectApplicationAsync(int applicationId)
        {
            var application = await _context.Applications
                .Include(a => a.Std).Include(a => a.Internship)
                .FirstOrDefaultAsync(a => a.ApplicationId == applicationId);
            if (application == null) { ErrorMessage = "Application not found."; return RedirectToPage(); }

            application.Status = 2;
            await _context.SaveChangesAsync();

            SuccessMessage = $"Application for {application.Std?.FullName ?? "student"} rejected.";
            return RedirectToPage();
        }
    }
}
