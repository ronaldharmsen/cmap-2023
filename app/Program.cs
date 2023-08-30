using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using ApiDemo;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.Logging;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("appIdentityDbContextConnection") ?? throw new InvalidOperationException("Connection string 'appIdentityDbContextConnection' not found.");

builder.Services.AddDbContext<VehicleContext>(options => options.UseSqlServer(connectionString));

// Add services to the container.
builder.Services.AddControllersWithViews();

//JwtSecurityTokenHandler.DefaultMapInboundClaims = false;

builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = "Cookies";
        options.DefaultChallengeScheme = "oidc";
    })
    .AddCookie("Cookies")
    .AddOpenIdConnect("oidc", options =>
    {
        options.RequireHttpsMetadata = false;
        options.Authority = builder.Configuration["jwt:Authority"];
        options.ClientId = builder.Configuration["jwt:Audience"];
        options.ClientSecret = builder.Configuration["jwt:Secret"];

        options.MetadataAddress = builder.Configuration["jwt:MetaData"];

        options.ResponseType = "code";

        options.Scope.Clear();
        options.Scope.Add("openid");
        options.Scope.Add("profile");

        options.SaveTokens = true;

        options.ClaimActions.MapUniqueJsonKey(ClaimTypes.Role, "role");

        options.TokenValidationParameters = new TokenValidationParameters
        {
            NameClaimType = ClaimTypes.Email,
            RoleClaimType = "role"
        };
    });

builder.Services.AddHttpContextAccessor();

builder.Services.AddAuthorization()
    .AddAuthorization(options =>
{
    options.AddPolicy("OnlyAdmins", policy =>
        policy.RequireClaim(ClaimTypes.Role, "Admin") 
    );

    options.AddPolicy("AuthenticatedUser", policy =>
        policy.RequireAuthenticatedUser()
    );
});


var app = builder.Build();

IdentityModelEventSource.ShowPII = true;
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//app.UseHttpsRedirection(); // in the template, but turned off by dev
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


// making sure the database is there!
await app.Services.EnsureVehicleDatabaseIsUpToDate();

app.Run();
