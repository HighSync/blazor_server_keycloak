using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MudBlazorWebApp1.Components.Pages;

public class Login : PageModel
{
    public async Task OnGet(string redirectUri)
    {
        await this.HttpContext.ChallengeAsync(OpenIdConnectDefaults.AuthenticationScheme,
            new AuthenticationProperties { RedirectUri = redirectUri });
    }
}