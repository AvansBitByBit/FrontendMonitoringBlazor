using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace HttpRequester;
public class HttpRequester
{
    private readonly HttpClient _httpClient;

    public HttpRequester(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<T?> GetAsync<T>(string endpoint)
    {
        var response = await _httpClient.GetAsync($"https://bitbybit-api--0000005.orangecliff-c30465b7.northeurope.azurecontainerapps.io/{endpoint}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<T>();
    }

    public async Task<TResponse?> PostAsync<TRequest, TResponse>(string baseUrl, string endpoint, TRequest payload)
    {
        var response = await _httpClient.PostAsJsonAsync($"{baseUrl}/{endpoint}", payload);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<TResponse>();
    }

    // Voeg indien nodig ook PutAsync, DeleteAsync, etc. toe
}