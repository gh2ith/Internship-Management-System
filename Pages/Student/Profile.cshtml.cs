using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;

namespace WebApplication1.Pages.Student
{
    [Authorize(Roles = "Student")]
    public class ProfileModel : PageModel
    {
        private readonly MydbContext _context;

        public ProfileModel(MydbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public string FullName { get; set; } = string.Empty;

        public string Department { get; set; } = string.Empty;
        public string Gpa { get; set; } = string.Empty;
        public string SupervisorName { get; set; } = "None";

        [TempData]
        public string? SuccessMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var userIdStr = User.FindFirstValue("UserId");
            if (!int.TryParse(userIdStr, out int userId)) return RedirectToPage("/Index");

            var student = await _context.Students
                .Include(s => s.Super)
                .FirstOrDefaultAsync(s => s.StudentId == userId);

            if (student == null) return NotFound();

            FullName = student.FullName ?? "";
            Department = student.StDepartment ?? "N/A";
            Gpa = student.Gpa?.ToString("0.0") ?? "N/A";
            SupervisorName = student.Super?.FullName ?? "None Assigned";

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var userIdStr = User.FindFirstValue("UserId");
            if (!int.TryParse(userIdStr, out int userId)) return RedirectToPage("/Index");

            var student = await _context.Students
                .Include(s => s.Super)
                .FirstOrDefaultAsync(s => s.StudentId == userId);

            if (student == null) return NotFound();

            student.FullName = FullName;
            await _context.SaveChangesAsync();

            SuccessMessage = "Profile updated successfully!";
            
            // Re-populate readonly fields
            Department = student.StDepartment ?? "N/A";
            Gpa = student.Gpa?.ToString("0.0") ?? "N/A";
            SupervisorName = student.Super?.FullName ?? "None Assigned";

            return Page();
        }
    }
}
