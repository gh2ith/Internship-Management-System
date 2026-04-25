using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Pages.Company
{
    [Authorize(Roles = "Company")]
    public class EditPostModel : PageModel
    {
        private readonly MydbContext _context;

        public EditPostModel(MydbContext context)
        {
            _context = context;
        }

        [BindProperty(SupportsGet = true)]
        public int Id { get; set; }

        [BindProperty]
        public string Title { get; set; } = string.Empty;

        [BindProperty]
        public string Description { get; set; } = string.Empty;

        [BindProperty]
        public string? Requirements { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Capacity is required.")]
        [Range(1, 1000, ErrorMessage = "Capacity must be at least 1.")]
        public int Capacity { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Deadline is required.")]
        public DateTime? Deadline { get; set; }

        [BindProperty]
        public bool IsClosed { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var email = User.Identity?.Name;
            var company = await _context.Companies.FirstOrDefaultAsync(c => c.ContactInfo == email);
            if (company == null) return RedirectToPage("/Company/Dashboard");

            var post = await _context.InternshipOpportunities
                .FirstOrDefaultAsync(i => i.InternshipId == Id && i.CompanyId == company.CompanyId);

            if (post == null) return RedirectToPage("/Company/Dashboard");

            Title = post.Title ?? "";
            Description = post.Description ?? "";
            Requirements = post.Requirments;
            Capacity = post.Capacity ?? 1;
            Deadline = post.Deadline;
            IsClosed = post.IsClosed ?? false;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var email = User.Identity?.Name;
            var company = await _context.Companies.FirstOrDefaultAsync(c => c.ContactInfo == email);
            if (company == null) return RedirectToPage("/Company/Dashboard");

            var post = await _context.InternshipOpportunities
                .FirstOrDefaultAsync(i => i.InternshipId == Id && i.CompanyId == company.CompanyId);

            if (post == null) return RedirectToPage("/Company/Dashboard");

            post.Title = Title;
            post.Description = Description;
            post.Requirments = Requirements;
            post.Capacity = Capacity;
            post.Deadline = Deadline;
            post.IsClosed = IsClosed;

            await _context.SaveChangesAsync();

            return RedirectToPage("/Company/Dashboard");
        }
    }
}
