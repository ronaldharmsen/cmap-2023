using IdentityManagerUI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace app.Areas.Identity.Data;

public class appIdentityDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
{
    public appIdentityDbContext(DbContextOptions<appIdentityDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<ApplicationUser>().HasMany(p => p.Roles).WithOne().HasForeignKey(p => p.UserId).IsRequired().OnDelete(DeleteBehavior.Cascade);
        builder.Entity<ApplicationUser>().HasMany(e => e.Claims).WithOne().HasForeignKey(e => e.UserId).IsRequired().OnDelete(DeleteBehavior.Cascade);
        builder.Entity<ApplicationRole>().HasMany(r => r.Claims).WithOne().HasForeignKey(r => r.RoleId).IsRequired().OnDelete(DeleteBehavior.Cascade);

        
    }
}
