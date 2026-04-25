using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApplication1.Pages.Supervisor
{
    [Authorize(Roles = "Supervisor")]
    public class IndexModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
