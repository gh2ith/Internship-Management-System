using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<MydbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/Login";
    });

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

// Initialize the database with seeder
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<MydbContext>();
        context.Database.EnsureCreated();

        // One-time fix: clear old data with mismatched IDs and re-seed
        // Check if student with ID=4 exists (correct alignment)
        var studentAligned = context.Students.Any(s => s.StudentId == 4);
        if (!studentAligned)
        {
            // Clear in FK-safe order
            context.Applications.RemoveRange(context.Applications);
            context.InternshipOpportunities.RemoveRange(context.InternshipOpportunities);
            context.Reports.RemoveRange(context.Reports);
            context.Students.RemoveRange(context.Students);
            context.Supervisors.RemoveRange(context.Supervisors);
            context.Companies.RemoveRange(context.Companies);
            context.Users.RemoveRange(context.Users);
            context.Colleges.RemoveRange(context.Colleges);
            await context.SaveChangesAsync();
        }

        await DbSeeder.SeedDefaultDataAsync(context);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred setting up the database.");
    }
}

app.Run();
