using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

public static class OpenIdConnectScopeExtensions
{
    //Configure custom OIDC scopes
    public static void UseCustomScope(this ICollection<string> scope)
    {
        scope.Clear();
        scope.Add("openid");
        scope.Add("profile");
        scope.Add("email");
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



public static class OpenIdConnectConfigurationExtensions
{
    private static void ConfigureConnection(this OpenIdConnectOptions options)
    {
        options.Authority = "http://localhost:8085/realms/codeheap";
        options.ClientId = "blazor-client";
        options.CallbackPath = "/signin-oidc"; 
        options.ClientSecret = "RImGOWdZz3HsNih8rvxGVZYvCj4DSOkW";
    }
    
    public static void Configure(this OpenIdConnectOptions options)
    {
        //require https when built as not dev
        #if DEBUG
        options.RequireHttpsMetadata = false;
        #else        
        options.RequireHttpsMetadata = true;
        #endif
        options.ReturnUrlParameter = "/";
        // options.PushedAuthorizationBehavior = PushedAuthorizationBehavior.Disable;
        options.ResponseType = OpenIdConnectResponseType.Code;
        options.ConfigureConnection();
        options.Scope.UseCustomScope();

        options.Events = new OpenIdConnectEvents()
        {
            OnUserInformationReceived = context =>
            {
                //Add identity information mapping
                context.Map();
                return Task.CompletedTask;
            }
        };
    }
}


public static class KeycloakIdentityMapper
{
    //map keycloak data to user identity information mapping
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

