using FrontendMonitoring.Models;
using FrontendMonitoring.Services;
using FrontendMonitoring.Shared;

public class AfvalApiClient
{
    private readonly ApiClient _apiClient;

    public AfvalApiClient(ApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public Task<AfvalModel?> GetDataAsync()
    {
        return _apiClient.GetAsync<AfvalModel>("afval/afval");
    }

    public Task<List<DetectionResult>?> GetDetectionsAsync()
    {
        return _apiClient.GetAsync<List<DetectionResult>>("afval/detections");
    }
}
