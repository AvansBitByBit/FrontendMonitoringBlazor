using Microsoft.AspNetCore.Identity;

namespace FrontendMonitoring.Data;

public class RolesIdentityUser
{
    public static async Task EnsureRoles(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        string[] roles = { "Beheerder", "Gebruiker" };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
        }
    }
}