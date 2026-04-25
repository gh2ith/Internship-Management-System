using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Pages.Company
{
    [Authorize(Roles = "Company")]
    public class ViewApplicantsModel : PageModel
    {
        private readonly MydbContext _context;

        public ViewApplicantsModel(MydbContext context)
        {
            _context = context;
        }

        public InternshipOpportunity? Post { get; set; }
        public List<Application> Applicants { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var email = User.Identity?.Name;
            var company = await _context.Companies
                .FirstOrDefaultAsync(c => c.ContactInfo == email);

            if (company == null)
                return RedirectToPage("/Company/Dashboard");

            // Load the post and verify it belongs to this company
            Post = await _context.InternshipOpportunities
                .FirstOrDefaultAsync(i => i.InternshipId == id && i.CompanyId == company.CompanyId);

            if (Post == null)
                return RedirectToPage("/Company/Dashboard");

            // Load applicants for this post
            Applicants = await _context.Applications
                .Include(a => a.Std)
                .Where(a => a.InternshipId == id)
                .ToListAsync();

            return Page();
        }
    }
}
