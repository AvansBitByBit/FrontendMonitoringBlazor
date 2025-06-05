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
    }

    public async Task<string> GetTokenAsync()
    {
        return await _jsRuntime.InvokeAsync<string>("localStorage.getItem", TokenKey);
    }

    public async Task SetTokenAsync(string token)
    {
        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", TokenKey, token);
    }

    public async Task RemoveTokenAsync()
    {
        await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", TokenKey);
    }
}