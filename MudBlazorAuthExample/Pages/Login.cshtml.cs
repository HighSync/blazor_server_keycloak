using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MudBlazorAuthExample.Components.Pages;

public class Login : PageModel
{
    private readonly ILogger<Login> _logger;

    public Login(ILogger<Login> logger)
    {
        _logger = logger;
    }
    
    public async Task OnGet(string redirectUri)
    {
        _logger.LogInformation($"Redirecting after login to {redirectUri}");
        
        //TODO: AWAIT WORKS AS UNEXPECTED
        await HttpContext.ChallengeAsync(OpenIdConnectDefaults.AuthenticationScheme, new AuthenticationProperties
        {
            RedirectUri = redirectUri ?? "/"
        });
        
        // throw new Exception("Something went wrong");
        //
        //
        // if (this.HttpContext.User.Identity?.IsAuthenticated ?? false)
        //     _logger.LogInformation(this.HttpContext.User.Identity.Name);
        // else _logger.LogWarning("User Not Authenticated");
    }
}