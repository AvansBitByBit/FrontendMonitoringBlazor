namespace FrontendMonitoring.Services;

using Microsoft.JSInterop;
using System.Threading.Tasks;

public class LocalStorageTokenStorage : ITokenStorage
{
    private readonly IJSRuntime _jsRuntime;
    private const string TokenKey = "authToken";

    public LocalStorageTokenStorage(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }    public async Task<string> GetTokenAsync()
    {
        try
        {
            return await _jsRuntime.InvokeAsync<string>("localStorage.getItem", TokenKey) ?? string.Empty;
        }
        catch
        {
            return string.Empty;
        }
    }    public async Task SetTokenAsync(string token)
    {
        try
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", TokenKey, token);
        }
        catch
        {
            // Ignore errors during server-side rendering
        }
    }

    public async Task RemoveTokenAsync()
    {
        try
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", TokenKey);
        }
        catch
        {
            // Ignore errors during server-side rendering
        }
    }
}