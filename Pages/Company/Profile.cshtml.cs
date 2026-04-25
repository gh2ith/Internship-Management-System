using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using WebApplication1.Data;

namespace WebApplication1.Pages.Company
{
    [Authorize(Roles = "Company")]
    public class ProfileModel : PageModel
    {
        private readonly MydbContext _context;

        public ProfileModel(MydbContext context)
        {
            _context = context;
        }

        public Models.Company? CompanyInfo { get; set; }

        [BindProperty]
        public CompanyProfileInput Input { get; set; } = new();

        public string? SuccessMessage { get; set; }

        public class CompanyProfileInput
        {
            [Required(ErrorMessage = "Company name is required")]
            [Display(Name = "Company Name")]
            public string CompanyName { get; set; } = string.Empty;

            [Display(Name = "Location")]
            public string? Location { get; set; }

            [Display(Name = "Contact Information")]
            public string? ContactInfo { get; set; }

            [Display(Name = "Company Description")]
            public string? Description { get; set; }
        }

        public async Task OnGetAsync()
        {
            var email = User.Identity?.Name;
            CompanyInfo = await _context.Companies
                .FirstOrDefaultAsync(c => c.ContactInfo == email);

            if (CompanyInfo != null)
            {
                Input = new CompanyProfileInput
                {
                    CompanyName = CompanyInfo.CompanyName ?? "",
                    Location = CompanyInfo.Location,
                    ContactInfo = CompanyInfo.ContactInfo,
                    Description = CompanyInfo.Description
                };
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var email = User.Identity?.Name;
            CompanyInfo = await _context.Companies
                .FirstOrDefaultAsync(c => c.ContactInfo == email);

            if (CompanyInfo == null)
                return NotFound();

            if (!ModelState.IsValid)
                return Page();

            CompanyInfo.CompanyName = Input.CompanyName;
            CompanyInfo.Location = Input.Location;
            CompanyInfo.Description = Input.Description;
            // Note: ContactInfo is the login email, so we don't change it
            // to avoid breaking the login association

            await _context.SaveChangesAsync();

            SuccessMessage = "Profile updated successfully!";
            return Page();
        }
    }
}
