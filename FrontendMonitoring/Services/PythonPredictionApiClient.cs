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
<<<<<<< HEAD
            _httpClient.Timeout = TimeSpan.FromSeconds(30); // Set timeout
=======
>>>>>>> 493fae7f1ea8ea1c618b728c0093797dd3758028
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

<<<<<<< HEAD
        public async Task<ApiHealthResponse?> GetHealthAsync()
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<ApiHealthResponse>($"{API_BASE_URL}/health");
                return response;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<ModelInfoResponse?> GetModelInfoAsync()
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<ModelInfoResponse>($"{API_BASE_URL}/model/info");
                return response;
            }
            catch (Exception)
            {
                return null;
            }
        }

=======
>>>>>>> 493fae7f1ea8ea1c618b728c0093797dd3758028
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
<<<<<<< HEAD

        public async Task<string?> ReloadModelAsync()
        {
            try
            {
                var response = await _httpClient.PostAsync($"{API_BASE_URL}/model/reload", null);
                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadAsStringAsync();
                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }

    // Enhanced Request/Response models to match the Python API
=======
    }

    // Request/Response models
>>>>>>> 493fae7f1ea8ea1c618b728c0093797dd3758028
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
<<<<<<< HEAD

        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; }

        [JsonPropertyName("model_version")]
        public string? ModelVersion { get; set; }

        [JsonPropertyName("confidence_interval")]
        public ConfidenceInterval? ConfidenceInterval { get; set; }
    }

    public class ConfidenceInterval
    {
        [JsonPropertyName("lower")]
        public double Lower { get; set; }

        [JsonPropertyName("upper")]
        public double Upper { get; set; }
=======
>>>>>>> 493fae7f1ea8ea1c618b728c0093797dd3758028
    }

    public class HotspotPredictionResponse
    {
        [JsonPropertyName("hotspots")]
        public List<string>? Hotspots { get; set; }

        [JsonPropertyName("location_predictions")]
        public Dictionary<string, double>? LocationPredictions { get; set; }
<<<<<<< HEAD

        [JsonPropertyName("total_locations")]
        public int TotalLocations { get; set; }

        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; }
    }

    public class ApiHealthResponse
    {
        [JsonPropertyName("status")]
        public string? Status { get; set; }

        [JsonPropertyName("message")]
        public string? Message { get; set; }

        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; }

        [JsonPropertyName("model_loaded")]
        public bool ModelLoaded { get; set; }

        [JsonPropertyName("uptime_seconds")]
        public double UptimeSeconds { get; set; }
    }

    public class ModelInfoResponse
    {
        [JsonPropertyName("model_loaded")]
        public bool ModelLoaded { get; set; }

        [JsonPropertyName("model_version")]
        public string? ModelVersion { get; set; }

        [JsonPropertyName("last_loaded")]
        public DateTime? LastLoaded { get; set; }

        [JsonPropertyName("features_count")]
        public int FeaturesCount { get; set; }

        [JsonPropertyName("endpoints_available")]
        public List<string>? EndpointsAvailable { get; set; }
=======
>>>>>>> 493fae7f1ea8ea1c618b728c0093797dd3758028
    }
}
