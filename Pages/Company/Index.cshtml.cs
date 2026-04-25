using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApplication1.Pages.Company
{
    [Authorize(Roles = "Company")]
    public class IndexModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
