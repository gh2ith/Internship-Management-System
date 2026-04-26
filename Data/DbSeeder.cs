using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;

namespace WebApplication1.Data
{
    public static class DbSeeder
    {
        public static async Task SeedDefaultDataAsync(MydbContext context)
        {
            // Seed College
            if (!context.Colleges.Any())
            {
                context.Colleges.Add(new College
                {
                    CollegeId = 1,
                    CollegeName = "College of IT",
                    UniName = "Jordan University"
                });
                await context.SaveChangesAsync();
            }

            // Seed Users - IDs must match profile table IDs
            if (!context.Users.Any())
            {
                var users = new List<User>
                {
                    new User { UserId = 1, CollegeId = 1, Email = "admin@uni.edu", Password = HashPassword("Password123!"), Role = "Admin" },
                    new User { UserId = 2, CollegeId = 1, Email = "dr.smith@uni.edu", Password = HashPassword("Password123!"), Role = "Supervisor" },
                    new User { UserId = 3, CollegeId = 1, Email = "hr@techcorp.com", Password = HashPassword("Password123!"), Role = "Company" },
                    new User { UserId = 4, CollegeId = 1, Email = "student1@uni.edu", Password = HashPassword("Password123!"), Role = "Student" }
                };
                context.Users.AddRange(users);
                await context.SaveChangesAsync();
            }

            // Seed Supervisor - supervisor_id = 2 matches user_id = 2
            if (!context.Supervisors.Any())
            {
                context.Supervisors.Add(new Supervisor
                {
                    SupervisorId = 2,
                    FullName = "Dr. John Smith",
                    SuperDepartment = "Computer Science",
                    CollegeId = 1
                });
                await context.SaveChangesAsync();
            }

            // Seed Company - company_id = 3 matches user_id = 3
            if (!context.Companies.Any())
            {
                context.Companies.Add(new Company
                {
                    CompanyId = 3,
                    CompanyName = "Tech Corp Inc.",
                    ContactInfo = "hr@techcorp.com",
                    Description = "Leading tech firm",
                    Location = "Amman"
                });
                await context.SaveChangesAsync();
            }

            // Seed Student - student_id = 4 matches user_id = 4
            if (!context.Students.Any())
            {
                context.Students.Add(new Student
                {
                    StudentId = 4,
                    SuperId = null,
                    CollegeId = 1,
                    StDepartment = "Software Engineering",
                    FullName = "Alice Johnson",
                    Gpa = 3.5f
                });
                await context.SaveChangesAsync();
            }
        }

        public static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }

        public static bool VerifyPassword(string password, string hashedPassword)
        {
            return HashPassword(password) == hashedPassword;
        }
    }
}
