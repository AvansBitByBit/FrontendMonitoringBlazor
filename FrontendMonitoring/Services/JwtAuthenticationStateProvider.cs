namespace FrontendMonitoring.Services;

using Microsoft.AspNetCore.Components.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;

public class JwtAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly ITokenStorage _tokenStorage; // Je eigen service om het JWT uit storage te halen

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
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

    public void NotifyAuthenticationStateChanged() =>
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
}