using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StudentConferenceApp.DAL.Entities;
using StudentConferenceApp.DataAccess.Seeding;

namespace Presentation.Infrastructure;

/// <summary>Host-level startup tasks (presentation layer): Identity roles and reference data.</summary>
public static class ApplicationBootstrap
{
    public static async Task InitializeAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        foreach (var role in new[] { "Participant", "Admin", "Manager" })
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
        }

        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await SectionSeeding.EnsureDefaultSectionsAsync(db);
        await ManagerSeeding.EnsureSampleManagerAsync(db);

        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        const string managerEmail = "manager@studentconference.local";
        const string managerPassword = "Manager123!";

        var managerUser = await userManager.FindByEmailAsync(managerEmail);
        if (managerUser == null)
        {
            managerUser = new ApplicationUser
            {
                UserName = managerEmail,
                Email = managerEmail,
                EmailConfirmed = true,
                FullName = "Demo supervisor account",
                UserRole = "Manager"
            };
            await userManager.CreateAsync(managerUser, managerPassword);
            managerUser = await userManager.FindByEmailAsync(managerEmail);
        }

        if (managerUser != null)
        {
            if (!await userManager.IsInRoleAsync(managerUser, "Manager"))
                await userManager.AddToRoleAsync(managerUser, "Manager");

            var unlinked = await db.Managers
                .OrderBy(m => m.Id)
                .FirstOrDefaultAsync(m => m.LinkedUserId == null);

            if (unlinked != null)
            {
                unlinked.LinkedUserId = managerUser.Id;
                await db.SaveChangesAsync();
            }
        }
    }
}
