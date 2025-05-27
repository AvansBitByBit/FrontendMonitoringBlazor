using FrontendMonitoring.Models;
public class WeatherApiClient
{
    private readonly HttpRequesterOnlyUrl.HttpRequesterOnlyUrl _httpRequesterOnlyUrl;

    public WeatherApiClient(HttpRequesterOnlyUrl.HttpRequesterOnlyUrl httpRequesterOnlyUrl)
    {
        _httpRequesterOnlyUrl = httpRequesterOnlyUrl;
    }

    public Task<WeatherModel?> GetDataAsync()
    {
        return _httpRequesterOnlyUrl.GetAsync<WeatherModel>("https://api.open-meteo.com/v1/forecast?latitude=51.571915&longitude=4.768323&hourly=temperature_2m,rain&current=temperature_2m&timezone=auto&past_days=7");
    }
}