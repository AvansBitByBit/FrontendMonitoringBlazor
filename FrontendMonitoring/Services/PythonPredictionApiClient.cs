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

        public async Task<TrainingResponse?> TrainModelFromApiAsync()
        {
            try
            {
                var response = await _httpClient.PostAsync($"{API_BASE_URL}/training/train_from_api", null);
                response.EnsureSuccessStatusCode(); 
                return await response.Content.ReadFromJsonAsync<TrainingResponse>();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<TrainingResponse?> TrainModelWithDataAsync(TrainingDataBatch trainingData)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"{API_BASE_URL}/training/train_new_model", trainingData);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<TrainingResponse>();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<ModelMetrics?> GetModelMetricsAsync()
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<ModelMetrics>($"{API_BASE_URL}/training/model_metrics");
                return response;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }

    // Response Models
    public class ApiHealthResponse
    {
        [JsonPropertyName("status")]
        public string Status { get; set; } = "";

        [JsonPropertyName("message")]
        public string Message { get; set; } = "";

        [JsonPropertyName("model_loaded")]
        public bool ModelLoaded { get; set; }

        [JsonPropertyName("uptime_seconds")]
        public double UptimeSeconds { get; set; }

        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; }
    }

    public class ModelInfoResponse
    {
        [JsonPropertyName("model_loaded")]
        public bool ModelLoaded { get; set; }

        [JsonPropertyName("model_version")]
        public string ModelVersion { get; set; } = "";

        [JsonPropertyName("features_count")]
        public int FeaturesCount { get; set; }

        [JsonPropertyName("endpoints_available")]
        public List<string> EndpointsAvailable { get; set; } = new();
    }

    public class PredictionResponse
    {
        [JsonPropertyName("prediction")]
        public List<double> Prediction { get; set; } = new();

        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; }

        [JsonPropertyName("model_version")]
        public string ModelVersion { get; set; } = "";

        [JsonPropertyName("confidence_interval")]
        public ConfidenceInterval? ConfidenceInterval { get; set; }
    }

    public class ConfidenceInterval
    {
        [JsonPropertyName("lower")]
        public double Lower { get; set; }

        [JsonPropertyName("upper")]
        public double Upper { get; set; }
    }

    public class HotspotPredictionResponse
    {
        [JsonPropertyName("hotspots")]
        public List<string> Hotspots { get; set; } = new();

        [JsonPropertyName("location_predictions")]
        public Dictionary<string, double> LocationPredictions { get; set; } = new();

        [JsonPropertyName("total_locations")]
        public int TotalLocations { get; set; }

        [JsonPropertyName("predictions_made")]
        public int PredictionsMade { get; set; }

        [JsonPropertyName("data_points_processed")]
        public int DataPointsProcessed { get; set; }

        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; }

        [JsonPropertyName("message")]
        public string? Message { get; set; }
    }

    public class TrainingResponse
    {
        [JsonPropertyName("status")]
        public string Status { get; set; } = "";

        [JsonPropertyName("message")]
        public string Message { get; set; } = "";

        [JsonPropertyName("model_accuracy")]
        public double? ModelAccuracy { get; set; }

        [JsonPropertyName("training_samples")]
        public int TrainingSamples { get; set; }

        [JsonPropertyName("model_version")]
        public string ModelVersion { get; set; } = "";

        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; }
    }

    public class ModelMetrics
    {
        [JsonPropertyName("accuracy")]
        public double Accuracy { get; set; }

        [JsonPropertyName("mean_squared_error")]
        public double MeanSquaredError { get; set; }

        [JsonPropertyName("feature_importance")]
        public Dictionary<string, double> FeatureImportance { get; set; } = new();

        [JsonPropertyName("training_samples")]
        public int TrainingSamples { get; set; }

        [JsonPropertyName("model_version")]
        public string ModelVersion { get; set; } = "";

        [JsonPropertyName("training_date")]
        public DateTime TrainingDate { get; set; }
    }

    // Request Models
    public class FuturePredictionRequest
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = "";

        [JsonPropertyName("time")]
        public string Time { get; set; } = "";

        [JsonPropertyName("trashType")]
        public string TrashType { get; set; } = "";

        [JsonPropertyName("location")]
        public string Location { get; set; } = "";

        [JsonPropertyName("confidence")]
        public double Confidence { get; set; }

        [JsonPropertyName("celcius")]
        public double Celcius { get; set; }
    }

    public class HotspotPredictionRequest
    {
        [JsonPropertyName("days")]
        public int Days { get; set; }
    }

    public class CustomPredictionRequest
    {
        [JsonPropertyName("features")]
        public List<double> Features { get; set; } = new();
    }

    public class TrainingDataPoint
    {
        [JsonPropertyName("confidence")]
        public double Confidence { get; set; }

        [JsonPropertyName("temperature")]
        public double Temperature { get; set; }

        [JsonPropertyName("hour")]
        public int Hour { get; set; }

        [JsonPropertyName("target")]
        public double Target { get; set; }

        [JsonPropertyName("location")]
        public string? Location { get; set; }

        [JsonPropertyName("trash_type")]
        public string? TrashType { get; set; }
    }

    public class TrainingDataBatch
    {
        [JsonPropertyName("data_points")]
        public List<TrainingDataPoint> DataPoints { get; set; } = new();

        [JsonPropertyName("model_name")]
        public string? ModelName { get; set; }
    }
}
