

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize]
public class AccountController : Controller
{

    
    public IActionResult Login() {
        return Redirect("/");
    }

    
    public IActionResult Logout()
    {
        return new SignOutResult(new[] { "oidc", "Cookies" });
    }
}