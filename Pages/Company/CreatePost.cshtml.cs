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
    public class CreatePostModel : PageModel
    {
        private readonly MydbContext _context;

        public CreatePostModel(MydbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public string Title { get; set; } = string.Empty;

        [BindProperty]
        public string Description { get; set; } = string.Empty;

        [BindProperty]
        public string? Requirements { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Capacity is required.")]
        [Range(1, 1000, ErrorMessage = "Capacity must be at least 1.")]
        public int Capacity { get; set; } = 1;

        [BindProperty]
        [Required(ErrorMessage = "Deadline is required.")]
        public DateTime? Deadline { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            if (Deadline.HasValue && Deadline.Value.Date < DateTime.Now.Date)
            {
                ModelState.AddModelError("Deadline", "Deadline cannot be in the past.");
                return Page();
            }

            var email = User.Identity?.Name;
            var company = await _context.Companies
                .FirstOrDefaultAsync(c => c.ContactInfo == email);

            if (company == null)
            {
                ModelState.AddModelError(string.Empty, "Company not found.");
                return Page();
            }

            // Get the next available ID
            var maxId = await _context.InternshipOpportunities
                .MaxAsync(i => (int?)i.InternshipId) ?? 0;

            var post = new InternshipOpportunity
            {
                InternshipId = maxId + 1,
                CompanyId = company.CompanyId,
                Title = Title,
                Description = Description,
                Requirments = Requirements,
                Deadline = Deadline,
                Capacity = Capacity,
                IsClosed = false
            };

            _context.InternshipOpportunities.Add(post);
            await _context.SaveChangesAsync();

            return RedirectToPage("/Company/Dashboard");
        }
    }
}
