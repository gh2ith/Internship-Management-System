using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;

namespace WebApplication1.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly MydbContext _context;

        public LoginModel(MydbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public InputModel Input { get; set; } = default!;

        public string? ReturnUrl { get; set; }

        [TempData]
        public string? ErrorMessage { get; set; }

        public class InputModel
        {
            public required string Email { get; set; }
            public required string Password { get; set; }
        }

        public IActionResult OnGet(string? returnUrl = null)
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                var role = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                if (role == "Admin") return RedirectToPage("/Admin/Index");
                if (role == "Supervisor") return RedirectToPage("/Supervisor/Dashboard");
                if (role == "Company") return RedirectToPage("/Company/Dashboard");
                if (role == "Student") return RedirectToPage("/Student/Dashboard");
                return RedirectToPage("/Index");
            }

            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }
            ReturnUrl = returnUrl ?? Url.Content("~/");
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            if (ModelState.IsValid)
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email == Input.Email);

                if (user != null && DbSeeder.VerifyPassword(Input.Password, user.Password ?? ""))
                {
                    var claims = new List<Claim>();
                    claims.Add(new Claim(ClaimTypes.Name, user.Email ?? string.Empty));
                    claims.Add(new Claim(ClaimTypes.Email, user.Email ?? string.Empty));
                    claims.Add(new Claim(ClaimTypes.Role, user.Role ?? string.Empty));
                    claims.Add(new Claim("UserId", user.UserId.ToString()));

                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var principal = new ClaimsPrincipal(identity);

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        principal);

                    // Redirect based on role
                    var role = user.Role ?? "";
                    if (role == "Admin") return RedirectToPage("/Admin/Index");
                    if (role == "Supervisor") return RedirectToPage("/Supervisor/Dashboard");
                    if (role == "Company") return RedirectToPage("/Company/Dashboard");
                    if (role == "Student") return RedirectToPage("/Student/Dashboard");

                    return LocalRedirect(returnUrl);
                }

                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return Page();
            }

            return Page();
        }
    }
}
