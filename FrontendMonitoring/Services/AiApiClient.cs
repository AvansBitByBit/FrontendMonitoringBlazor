using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace FrontendMonitoring.Services
{
    public class AiApiClient
    {
        private readonly HttpClient _httpClient;
        public AiApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<float>> PredictAsync(List<float> features)
        {
            var response = await _httpClient.PostAsJsonAsync("http://<FASTAPI_URL>/predict/", new { features });
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<PredictionResponse>();
            return result?.prediction ?? new List<float>();
        }

        public class PredictionResponse
        {
            public List<float> prediction { get; set; }
        }
    }
}
