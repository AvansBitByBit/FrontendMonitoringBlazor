using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace FrontendMonitoring.Services
{
    public class PythonPredictionApiClient
    {
        private readonly HttpClient _httpClient;

        public PythonPredictionApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.Timeout = TimeSpan.FromSeconds(30); // Set timeout
        }

        public async Task<PredictionResponse?> MakePredictionAsync(PredictionRequest request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("https://pythonbitbybit.orangecliff-c30465b7.northeurope.azurecontainerapps.io/Predict", request);
                Console.WriteLine("Response Status Code: " + response.StatusCode);

                if (response.StatusCode == System.Net.HttpStatusCode.TemporaryRedirect && response.Headers.Location != null)
                {
                    // Follow the redirect manually
                    var redirectUrl = response.Headers.Location.ToString();
                    Console.WriteLine("Redirecting to: " + redirectUrl);

                    // Resend the POST request to the redirected URL
                    var jsonContent = JsonContent.Create(request);
                    response = await _httpClient.PostAsync(redirectUrl, jsonContent);
                    Console.WriteLine("Response Status Code after redirect: " + response.StatusCode);
                }

                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<PredictionResponse>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred while making prediction: {ex.Message}");
                return null;
            }
        }
    }

    public class PredictionRequest
    {
        [JsonPropertyName("features")]
        public List<double> Features { get; set; } = new();
    }

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