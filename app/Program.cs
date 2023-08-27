using Microsoft.EntityFrameworkCore;
using app.Areas.Identity.Data;
using IdentityManagerUI.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("appIdentityDbContextConnection") ?? throw new InvalidOperationException("Connection string 'appIdentityDbContextConnection' not found.");

builder.Services.AddDbContext<appIdentityDbContext>(options => options.UseSqlServer(connectionString));

builder.Services
    .AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<ApplicationRole>()
    .AddEntityFrameworkStores<appIdentityDbContext>();

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddAuthorization()
    .AddAuthorization(options =>
{
    options.AddPolicy("OnlyAdmins", policy =>
        policy.RequireClaim(ClaimTypes.Role, "Admin") 
    );
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// extra mapping for management area
app.MapControllerRoute(
    name: "areas",
    pattern: "{area=}/{controller=access}/{action=users}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// we are using the default UI for auth
app.MapRazorPages();

// making sure the database is there!
await app.Services.EnsureIdentityDatabaseIsUpToDate();

app.Run();
