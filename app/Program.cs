using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using ApiDemo;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.Logging;
using Microsoft.AspNetCore.Identity;
using Keycloak.AuthServices.Authentication;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("appIdentityDbContextConnection") ?? throw new InvalidOperationException("Connection string 'appIdentityDbContextConnection' not found.");

builder.Services.AddDbContext<VehicleContext>(options => options.UseSqlServer(connectionString));

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddAuthentication(options =>
        {
            //Sets cookie authentication scheme
            options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
        })

        .AddCookie(cookie =>
        {
            //Sets the cookie name and maxage, so the cookie is invalidated.
            cookie.Cookie.Name = "keycloak.cookie";
            cookie.Cookie.MaxAge = TimeSpan.FromMinutes(60);
            cookie.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
            cookie.SlidingExpiration = true;
        })
        .AddOpenIdConnect(options =>
        {
            //Use default signin scheme
            options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            //Keycloak server
            options.Authority = builder.Configuration["jwt:Authority"];
            //Keycloak client ID
            options.ClientId = builder.Configuration["jwt:Audience"];
            //Keycloak client secret
            options.ClientSecret = builder.Configuration["jwt:Secret"];

            //Require keycloak to use SSL
            options.SaveTokens = true;
            options.RequireHttpsMetadata = false;
            //options.GetClaimsFromUserInfoEndpoint = true;
            options.Scope.Add("openid");
            options.Scope.Add("profile");
            options.Scope.Add("email");

            //Save the token
            options.SaveTokens = true;
            //Token response type, will sometimes need to be changed to IdToken, depending on config.
            options.ResponseType = OpenIdConnectResponseType.Code;
            //SameSite is needed for Chrome/Firefox, as they will give http error 500 back, if not set to unspecified.
            options.NonceCookie.SameSite = SameSiteMode.None;
            options.CorrelationCookie.SameSite = SameSiteMode.None;

            options.TokenValidationParameters = new TokenValidationParameters
            {
                NameClaimType = "name",
                RoleClaimType = "https://schemas.scopic.com/roles"
            };

            //builder.Configuration.Bind("<Json Config Filter>", options);
            options.Events.OnRedirectToIdentityProvider = async context =>
            {
                context.ProtocolMessage.RedirectUri = "https://localhost:7002/";
                await Task.FromResult(0);
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
