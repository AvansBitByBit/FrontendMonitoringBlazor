using FrontendMonitoring.Models;
using FrontendMonitoring.Services;

public class WeatherApiClient
{
    private readonly ApiClient _apiClient;

    public WeatherApiClient(ApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public Task<WeatherModel?> GetDataAsync()
    {
        return _apiClient.GetAsync<WeatherModel>(
            "https://api.open-meteo.com/v1/forecast?latitude=51.571915&longitude=4.768323&hourly=temperature_2m,rain&current=temperature_2m&timezone=auto&past_days=7");
    }
}
