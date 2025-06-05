namespace FrontendMonitoring.Services;

using Microsoft.AspNetCore.Components.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;

public class JwtAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly ITokenStorage _tokenStorage;

    public JwtAuthenticationStateProvider(ITokenStorage tokenStorage)
    {
        _tokenStorage = tokenStorage;
    }    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            var token = await _tokenStorage.GetTokenAsync();
            var identity = new ClaimsIdentity();

            if (!string.IsNullOrWhiteSpace(token))
            {
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);

                if (jwtToken.ValidTo > DateTime.UtcNow)
                {
                    identity = new ClaimsIdentity(jwtToken.Claims, "jwt");
                }
            }

            var user = new ClaimsPrincipal(identity);
            return new AuthenticationState(user);
        }
        catch
        {
            // Return unauthenticated state if any errors occur
            var identity = new ClaimsIdentity();
            var user = new ClaimsPrincipal(identity);
            return new AuthenticationState(user);
        }
    }

    public void NotifyAuthenticationStateChanged() =>
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
}