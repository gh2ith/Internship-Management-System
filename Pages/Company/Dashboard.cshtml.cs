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

        public async Task OnGetAsync()
        {
            var email = User.Identity?.Name;

            // Find the company by matching the logged-in user's email to ContactInfo
            CompanyInfo = await _context.Companies
                .FirstOrDefaultAsync(c => c.ContactInfo == email);

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
            }
        }
    }
}
