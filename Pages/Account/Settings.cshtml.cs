using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;

namespace WebApplication1.Pages.Account
{
    [Authorize]
    public class SettingsModel : PageModel
    {
        private readonly MydbContext _context;

        public SettingsModel(MydbContext context)
        {
            _context = context;
        }

        public string UserEmail { get; set; } = string.Empty;
        public string UserRole { get; set; } = string.Empty;

        [BindProperty]
        public PasswordInput PasswordForm { get; set; } = default!;

        [TempData]
        public string? SuccessMessage { get; set; }

        [TempData]
        public string? ErrorMessage { get; set; }

        public class PasswordInput
        {
            public string CurrentPassword { get; set; } = string.Empty;
            public string NewPassword { get; set; } = string.Empty;
            public string ConfirmPassword { get; set; } = string.Empty;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(email))
                return RedirectToPage("/Account/Login");

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
                return RedirectToPage("/Account/Login");

            UserEmail = user.Email ?? string.Empty;
            UserRole = user.Role ?? string.Empty;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(email))
                return RedirectToPage("/Account/Login");

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
                return RedirectToPage("/Account/Login");

            UserEmail = user.Email ?? string.Empty;
            UserRole = user.Role ?? string.Empty;

            // Validate current password
            if (!DbSeeder.VerifyPassword(PasswordForm.CurrentPassword, user.Password ?? ""))
            {
                ErrorMessage = "Current password is incorrect.";
                return Page();
            }

            // Validate new password
            if (string.IsNullOrWhiteSpace(PasswordForm.NewPassword) || PasswordForm.NewPassword.Length < 6)
            {
                ErrorMessage = "New password must be at least 6 characters.";
                return Page();
            }

            if (PasswordForm.NewPassword != PasswordForm.ConfirmPassword)
            {
                ErrorMessage = "New password and confirmation do not match.";
                return Page();
            }

            // Update password
            user.Password = DbSeeder.HashPassword(PasswordForm.NewPassword);
            await _context.SaveChangesAsync();

            SuccessMessage = "Password updated successfully!";
            return RedirectToPage();
        }
    }
}
