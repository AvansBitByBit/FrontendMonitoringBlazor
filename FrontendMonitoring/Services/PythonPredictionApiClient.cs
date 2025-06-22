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
        }

        public async Task<string> CheckApiStatusAsync()
        {
            try
            {
                var response = await _httpClient.GetStringAsync($"{API_BASE_URL}/");
                return response;
            }
            catch (Exception ex)
            {
                return $"API niet bereikbaar: {ex.Message}";
            }
        }

        public async Task<PredictionResponse?> MakeFuturePredictionAsync(FuturePredictionRequest request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"{API_BASE_URL}/predictFuture", request);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<PredictionResponse>();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<PredictionResponse?> MakeCustomPredictionAsync(CustomPredictionRequest request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"{API_BASE_URL}/predict/", request);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<PredictionResponse>();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<HotspotPredictionResponse?> PredictHotspotsAsync(HotspotPredictionRequest request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"{API_BASE_URL}/predict_trash_hotspots/", request);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<HotspotPredictionResponse>();
            }
            catch (Exception)
            {
                return null;
            }
        }
    }

    // Request/Response models
    public class FuturePredictionRequest
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [JsonPropertyName("time")]
        public string Time { get; set; } = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");

        [JsonPropertyName("trashType")]
        public string TrashType { get; set; } = "unknown";

        [JsonPropertyName("location")]
        public string Location { get; set; } = "unknown";

        [JsonPropertyName("confidence")]
        public double Confidence { get; set; }

        [JsonPropertyName("celcius")]
        public double Celcius { get; set; }
    }

    public class CustomPredictionRequest
    {
        [JsonPropertyName("features")]
        public List<double> Features { get; set; } = new();
    }

    public class HotspotPredictionRequest
    {
        [JsonPropertyName("days")]
        public int Days { get; set; }
    }

    public class PredictionResponse
    {
        [JsonPropertyName("prediction")]
        public List<double>? Prediction { get; set; }
    }

    public class HotspotPredictionResponse
    {
        [JsonPropertyName("hotspots")]
        public List<string>? Hotspots { get; set; }

        [JsonPropertyName("location_predictions")]
        public Dictionary<string, double>? LocationPredictions { get; set; }
    }
}
