using FrontendMonitoring.Shared;
using System.Text;

namespace FrontendMonitoring.Services;

public class DetectionService
{
    private readonly ApiClient _apiClient;

    public DetectionService(ApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public Task<List<DetectionResult>?> GetDetectionsAsync()
    {
        return _apiClient.GetAsync<List<DetectionResult>>("detections");
    }

    public Task<bool> VerifyAsync(int id)
    {
        return _apiClient.PutAsync<object, bool>($"detections/{id}/verify", new { });
    }

    public async Task<string> GenerateCsvAsync()
    {
        var data = await GetDetectionsAsync() ?? new List<DetectionResult>();
        var sb = new StringBuilder();
        sb.AppendLine("Id,Type,Location,TimeStamp,IsVerified");
        foreach (var d in data)
        {
            sb.AppendLine($"{d.Id},{d.Type},{d.Location},{d.TimeStamp:o},{d.IsVerified}");
        }
        return sb.ToString();
    }
}

