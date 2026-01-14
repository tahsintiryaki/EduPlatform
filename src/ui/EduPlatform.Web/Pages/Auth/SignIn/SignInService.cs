#region

using System.Security.Claims;
using Duende.IdentityModel.Client;
using EduPlatform.Web.Options;
using EduPlatform.Web.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization.Infrastructure;

#endregion

namespace EduPlatform.Web.Pages.Auth.SignIn;

public class SignInService(
    IHttpContextAccessor contextAccessor,
    TokenService tokenService,
    IdentityOption identityOption,
    HttpClient client,
    ILogger<SignInService> logger)
{
    public async Task<ServiceResult> AuthenticateAsync(SignInViewModel signInViewModel)
    {
        var tokenResponse = await GetAccessToken(signInViewModel);
        if (tokenResponse.IsError) return ServiceResult.Error(tokenResponse.Error!, tokenResponse.ErrorDescription!);
        var userClaims = tokenService.ExtractClaims(tokenResponse.AccessToken!);
        var authenticationProperties = tokenService.CreateAuthenticationProperties(tokenResponse);

        #region Keycloak settings
        // Keycloakpanel=>clientScopes>roles=>mapper=>realm roles=>Token Claim Name has been changed as http://schemas\.microsoft\.com/ws/2008/06/identity/claims/role
        // to catch roles value with ClaimsType.Role 
        // Keycloakpanel => clientScopes=> profile=> mappers=>username=>Token Claim Name has been changed http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name
        // to catch username value with ClaimsType.Name 
        #endregion
        var claimIdentity = new ClaimsIdentity(userClaims, CookieAuthenticationDefaults.AuthenticationScheme,
            ClaimTypes.Name, ClaimTypes.Role);
        var claimsPrincipal = new ClaimsPrincipal(claimIdentity);
        await contextAccessor.HttpContext!.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
            claimsPrincipal, authenticationProperties);

        return ServiceResult.Success();
    }


    private async Task<TokenResponse> GetAccessToken(SignInViewModel signInViewModel)
    {
        var discoveryRequest = new DiscoveryDocumentRequest
        {
            Address = identityOption.Address,
            Policy = { RequireHttps = false }
        };

        client.BaseAddress = new Uri(identityOption.Address);
        var discoveryResponse = await client.GetDiscoveryDocumentAsync(discoveryRequest);

        if (discoveryResponse.IsError)
            throw new Exception($"Discovery document request failed: {discoveryResponse.Error}");


        var tokenResponse = await client.RequestPasswordTokenAsync(new PasswordTokenRequest
        {
            Address = discoveryResponse.TokenEndpoint,
            ClientId = identityOption.Web.ClientId,
            ClientSecret = identityOption.Web.ClientSecret,
            UserName = signInViewModel.Email,
            Password = signInViewModel.Password,
            Scope = "offline_access"
        });

        return tokenResponse;
    }
}