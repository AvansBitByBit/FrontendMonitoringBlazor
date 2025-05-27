using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace HttpRequesterOnlyUrl
{
    public class HttpRequesterOnlyUrl
    {
        private readonly HttpClient _httpClient;

        public HttpRequesterOnlyUrl(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        /// <summary>
        /// Voert een GET-request uit naar een volledige URL en deserialiseert het antwoord naar het opgegeven type.
        /// </summary>
        public async Task<T?> GetAsync<T>(string url)
        {
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<T>();
        }

        /// <summary>
        /// Voert een POST-request uit naar een volledige URL met een payload en deserialiseert het antwoord naar het opgegeven type.
        /// </summary>
        public async Task<TResponse?> PostAsync<TRequest, TResponse>(string url, TRequest payload)
        {
            var response = await _httpClient.PostAsJsonAsync(url, payload);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<TResponse>();
        }

        /// <summary>
        /// Voert een DELETE-request uit naar een volledige URL en retourneert of het succesvol was.
        /// </summary>
        public async Task<bool> DeleteAsync(string url)
        {
            var response = await _httpClient.DeleteAsync(url);
            return response.IsSuccessStatusCode;
        }

        /// <summary>
        /// Voert een PUT-request uit naar een volledige URL met een payload en deserialiseert het antwoord naar het opgegeven type.
        /// </summary>
        public async Task<TResponse?> PutAsync<TRequest, TResponse>(string url, TRequest payload)
        {
            var response = await _httpClient.PutAsJsonAsync(url, payload);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<TResponse>();
        }
    }
}