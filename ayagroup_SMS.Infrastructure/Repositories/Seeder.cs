
using ayagroup_SMS.Core.conts;
using ayagroup_SMS.Core.Entities;
using ayagroup_SMS.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


namespace ayagroup_SMS.Infrastructure.Repositories
{
    public class Seeder
    {
        public static async Task SeedRolesAndAdminAsync(
            UserManager<User> userManager,
            RoleManager<IdentityRole<Guid>> roleManager,
            ApplicationDbContext dbContext)
        {
            await SeedRoles(roleManager);
            await SeedAdminUser(userManager);
            await SeedCategories(dbContext);
            await SeedTasks(userManager, dbContext);
        }

        private static async Task SeedRoles(RoleManager<IdentityRole<Guid>> roleManager)
        {
            var roles = new[] { Roles.Guest.ToString(), Roles.Owner.ToString() };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole<Guid>(role));
                }
            }
        }

        private static async Task SeedAdminUser(UserManager<User> userManager)
        {
            var adminEmail = "admin@domain.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                var admin = new User
                {
                    Email = adminEmail,
                    EmailConfirmed = true,
                    PhoneNumber = "+20 123 123 123",
                    UserName = "Admin",
                    PhoneNumberConfirmed = true,
                    CreatedAt = DateTime.UtcNow
                };
                await userManager.CreateAsync(admin, "Aa@112233@");
                await userManager.AddToRoleAsync(admin, Roles.Owner.ToString());
            }
        }

        private static async Task SeedCategories(ApplicationDbContext dbContext)
        {
            if (!await dbContext.Category.AnyAsync())
            {
                var categories = new List<Category>
                {
                    new Category { Name = "work" },
                    new Category { Name = "personal" },
                    new Category { Name = "marketing" },
                    new Category { Name = "health" }
                };

                await dbContext.Category.AddRangeAsync(categories);
                await dbContext.SaveChangesAsync();
            }
        }

        private static async Task SeedTasks(
            UserManager<User> userManager,
            ApplicationDbContext dbContext)
        {
            if (!await dbContext.Tasks.AnyAsync())
            {
                var admin = await userManager.FindByEmailAsync("admin@domain.com");
                var categories = await dbContext.Category.ToListAsync();

                var guest = new User
                {
                    Email = "guest@domain.com",
                    EmailConfirmed = true,
                    UserName = "GuestUser",
                    CreatedAt = DateTime.UtcNow
                };
                await userManager.CreateAsync(guest, "Guest@123");
                await userManager.AddToRoleAsync(guest, Roles.Guest.ToString());

                var tasks = new List<Tasks>
                {
                    new Tasks
                    {
                        Title = "create users interface",
                        Description = "design interface",
                        UserId = admin.Id,
                        Priority = Priority.High,
                        CategoryId = categories.First(c => c.Name == "work").Id
                    },
                    new Tasks
                    {
                        Title = "buy needs for office",
                        DueDate = DateTime.UtcNow.AddDays(3),
                        UserId = admin.Id,
                        Priority = Priority.Medium,
                        CategoryId = categories.First(c => c.Name == "marketing").Id
                    },
                    new Tasks
                    {
                        Title = "meeting work team",
                        Description = "check project progress ",
                        UserId = admin.Id,
                        Priority = Priority.High,
                        CategoryId = categories.First(c => c.Name == "work").Id
                    },

                    new Tasks
                    {
                        Title = "read new book",
                        Description = "end chapter one",
                        UserId = guest.Id,
                        Priority = Priority.Low,
                        CategoryId = categories.First(c => c.Name == "personal").Id
                    },
                    new Tasks
                    {
                        Title = "do sprot",
                        DueDate = DateTime.UtcNow.AddDays(1),
                        UserId = guest.Id,
                        Priority = Priority.Medium,
                        CategoryId = categories.First(c => c.Name == "health").Id,
                        IsCompleted = true
                    }
                };

                await dbContext.Tasks.AddRangeAsync(tasks);
                await dbContext.SaveChangesAsync();
            }
        }
    }
}