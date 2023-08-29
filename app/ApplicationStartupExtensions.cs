using Microsoft.EntityFrameworkCore;
using app.Areas.Identity.Data;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.AspNetCore.Identity;
using IdentityManagerUI.Models;
using System.Security.Claims;

public static class ApplicationStartupExtensions
{
    private const string Email = "admin@owaspdemo.org";

    public static async Task EnsureIdentityDatabaseIsUpToDate(this IServiceProvider services)
    {
        using var scope = services.CreateScope();

        var dataContext = scope.ServiceProvider.GetRequiredService<appIdentityDbContext>();
        dataContext.Database.Migrate();

        var manager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        var user = await manager.FindByEmailAsync(Email);
        if (user != null)
        {
            // default admin exists
            return; 
        }

        var result = await manager.CreateAsync(new ApplicationUser()
        {
            UserName = Email,
            Email = Email,
            EmailConfirmed = true
        });
        if (!result.Succeeded)
            throw new InvalidProgramException("Could not create default admin");

        user = await manager.FindByEmailAsync(Email);
        if (user == null)
            throw new InvalidProgramException("Default admin could not be found in storage");

        await manager.AddPasswordAsync(user, "P@ssw0rd");
        await manager.AddClaimAsync(user, new System.Security.Claims.Claim(ClaimTypes.Role, "Admin"));
    }
}