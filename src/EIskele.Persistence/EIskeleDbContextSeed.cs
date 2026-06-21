using Microsoft.EntityFrameworkCore;
using EIskele.Domain.Entities;
using EIskele.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace EIskele.Persistence;

public static class EIskeleDbContextSeed
{
    public static async Task SeedRolesAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();

        var roles = new[] { "SuperAdmin", "Admin", "Captain", "Customer" };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new ApplicationRole { Name = role });
            }
        }

        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var superAdminEmail = "admin@e-iskele.com";

        if (!await userManager.Users.IgnoreQueryFilters().AnyAsync(u => u.NormalizedUserName == userManager.NormalizeName(superAdminEmail)))
        {
            var superAdmin = new ApplicationUser
            {
                UserName = superAdminEmail,
                Email = superAdminEmail,
                FirstName = "Sistem",
                LastName = "Yöneticisi",
                EmailConfirmed = true,
                Status = UserStatus.Active,
                CreatedAt = DateTime.UtcNow
            };

            var result = await userManager.CreateAsync(superAdmin, "Admin123!");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(superAdmin, "SuperAdmin");
            }
        }

        var captainEmail = "kaptan@e-iskele.com";
        if (!await userManager.Users.IgnoreQueryFilters().AnyAsync(u => u.NormalizedUserName == userManager.NormalizeName(captainEmail)))
        {
            var captainUser = new ApplicationUser
            {
                UserName = captainEmail,
                Email = captainEmail,
                FirstName = "Test",
                LastName = "Kaptan",
                EmailConfirmed = true,
                Status = UserStatus.Active,
                CreatedAt = DateTime.UtcNow
            };

            var result = await userManager.CreateAsync(captainUser, "Kaptan123!");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(captainUser, "Captain");
            }
        }
    }

    public static async Task SeedNotificationTemplatesAsync(EIskeleDbContext context)
    {
        if (!await context.NotificationTemplates.AnyAsync(t => t.Code == "CAPTAIN_APPLICATION_RECEIVED"))
        {
            context.NotificationTemplates.Add(new NotificationTemplate
            {
                Id = Guid.NewGuid(),
                Code = "CAPTAIN_APPLICATION_RECEIVED",
                Channel = "Email",
                SubjectTemplate = "Kaptan Başvurunuz Alındı - e-iskele",
                BodyTemplate = "Sayın {{CaptainName}}, {{ApplicationNo}} numaralı başvurunuz başarıyla alınmıştır. İnceleme süreci sonrası size bilgi verilecektir.",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            });

            await context.SaveChangesAsync();
        }
    }
}
