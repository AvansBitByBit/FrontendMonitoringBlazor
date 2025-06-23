using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace FrontendMonitoring.Services
{
    public class PythonPredictionApiClient
    {
        private readonly HttpClient _httpClient;
        private const string API_BASE_URL = "https://pythonbitbybit.orangecliff-c30465b7.northeurope.azurecontainerapps.io";

        public PythonPredictionApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.Timeout = TimeSpan.FromSeconds(30); // Set timeout
        }

        public async Task<PredictionResponse?> MakePredictionAsync(PredictionRequest request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"{API_BASE_URL}/prediction", request);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<PredictionResponse>();
            }
            catch (Exception)
            {
                return null;
            }
        }
    }

    // Request Model
    public class PredictionRequest
    {
        [JsonPropertyName("features")]
        public List<double> Features { get; set; } = new();
    }

    // Response Model
    public class PredictionResponse
    {
        [JsonPropertyName("AdresPredection")]
        public string AdresPrediction { get; set; } = "";

        [JsonPropertyName("CountOfPossibleLitter")]
        public int CountOfPossibleLitter { get; set; }

        [JsonPropertyName("MatchWithModel")]
        public double MatchWithModel { get; set; }

        [JsonPropertyName("confidence")]
        public double Confidence { get; set; }

        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; }
    }
}