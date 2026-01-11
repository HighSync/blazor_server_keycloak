using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

namespace MudBlazorAuthExample;

public static class OpenIdConnectScopeExtensions
{
    //Configure custom OIDC scopes
    public static void UseCustomScope(this ICollection<string> scope)
    {
        scope.Clear();
        scope.Add("openid");
        scope.Add("profile");
        scope.Add("email");
        scope.Add("roles");
    }

    //Configure custom OIDC scopes
    public static void UseCustomScope(this ICollection<string> scope, params string[] claims)
    {
        scope.Clear();
        foreach (var claim in claims)
        {
            scope.Add(claim);
        }
    }
}


// TODO: Feature
// [Flags]
// public enum OpenIdConnectConfigurationPath
// {
//     AppSettings,
//     Environment
// }

public static class KeycloakIdentityMapper
{
    /// <summary>
    /// Map keycloak data to user identity information
    /// </summary>
    /// <param name="context"></param>
    public static void Map(this UserInformationReceivedContext context)
    {
        if (context.Principal.Identity is not ClaimsIdentity claimsIdentity) return;

        if (context.User.RootElement.TryGetProperty("preferred_username", out var username))
        {
            claimsIdentity.AddClaim(new Claim(ClaimTypes.Name, username.ToString()));
        }

        if (context.User.RootElement.TryGetProperty("realm_access", out var realmAccess)
            && realmAccess.TryGetProperty("roles", out var globalRoles))
        {
            foreach (var role in globalRoles.EnumerateArray())
            {
                claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, role.ToString()));
            }
        }

        if (context.User.RootElement.TryGetProperty("resource_access", out var clientAccess)
            && clientAccess.TryGetProperty(context.Options.ClientId, out var client)
            && client.TryGetProperty("roles", out var clientRoles))
        {
            foreach (var role in clientRoles.EnumerateArray())
            {
                claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, role.ToString()));
            }
        }
    }
}