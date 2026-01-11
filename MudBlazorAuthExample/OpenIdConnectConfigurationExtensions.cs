using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace MudBlazorAuthExample;

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
        // options.PushedAuthorizationBehavior = PushedAuthorizationBehavior.Disable;
        options.ResponseType = OpenIdConnectResponseType.Code;
        options.ConfigureConnection();
        options.GetClaimsFromUserInfoEndpoint = true;
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