using FrontendMonitoring.Models;
using FrontendMonitoring.Services;

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
}
