using FrontendMonitoring.Models;

namespace FrontendMonitoring.Services;

public class AuthenticationService
{
    private readonly ApiClient _apiClient;

    public AuthenticationService(ApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<bool> RegisterAsync(RegisterModel model)
    {
        var result = await _apiClient.PostAsync<RegisterModel, string>("account/register", model);
        return result != null;
    }

    public async Task<string?> LoginAsync(LoginModel model)
    {
        return await _apiClient.PostAsync<LoginModel, string>("account/login", model);
    }
}
