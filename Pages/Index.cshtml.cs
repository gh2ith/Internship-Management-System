using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApplication1.Pages
{
    public class IndexModel : PageModel
    {
        public IActionResult OnGet()
        {
            // If user is already logged in, redirect to their dashboard
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                var role = User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Role)?.Value;
                return role switch
                {
                    "Admin" => RedirectToPage("/Admin/Index"),
                    "Student" => RedirectToPage("/Student/Dashboard"),
                    "Company" => RedirectToPage("/Company/Dashboard"),
                    "Supervisor" => RedirectToPage("/Supervisor/Dashboard"),
                    _ => Page()
                };
            }

            // Show the landing page for unauthenticated users
            return Page();
        }
    }
}
