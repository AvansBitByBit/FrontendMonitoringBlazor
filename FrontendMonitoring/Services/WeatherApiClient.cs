using FrontendMonitoring.Models;

namespace FrontendMonitoring.Services
{
    public class WeatherApiClient
    {
        private readonly HttpRequesterOnlyUrl.HttpRequesterOnlyUrl _httpRequesterOnlyUrl;

        public WeatherApiClient(HttpRequesterOnlyUrl.HttpRequesterOnlyUrl httpRequesterOnlyUrl)
        {
            _httpRequesterOnlyUrl = httpRequesterOnlyUrl;
        }

        public Task<WeatherModel?> GetDataAsync()
        {
            // Include forecast_days=2 to get today and tomorrow's data
            return _httpRequesterOnlyUrl.GetAsync<WeatherModel>("https://api.open-meteo.com/v1/forecast?latitude=51.571915&longitude=4.768323&hourly=temperature_2m,rain&current=temperature_2m&timezone=auto&past_days=7&forecast_days=2");
        }

        public async Task<(double todayTemp, double tomorrowTemp)> GetTodayAndTomorrowTemperaturesAsync()
        {
            try
            {
                var weatherData = await GetDataAsync();
                if (weatherData?.Current?.Temperature2m != null && weatherData?.Hourly?.Temperature2m != null && weatherData?.Hourly?.Time != null)
                {
                    var todayTemp = weatherData.Current.Temperature2m;
                    
                    // Find tomorrow's average temperature
                    var tomorrow = DateTime.Now.AddDays(1).Date;
                    var tomorrowTemp = GetAverageTemperatureForDate(weatherData, tomorrow);
                    
                    return (todayTemp, tomorrowTemp);
                }
                
                // Fallback to current temperature for both if data is unavailable
                return (weatherData?.Current?.Temperature2m ?? 15.0, weatherData?.Current?.Temperature2m ?? 15.0);
            }
            catch (Exception)
            {
                // Fallback temperatures if API fails
                return (15.0, 15.0);
            }
        }

        private double GetAverageTemperatureForDate(WeatherModel weatherData, DateTime targetDate)
        {
            var temperaturesForDate = new List<double>();
            
            for (int i = 0; i < weatherData.Hourly.Time.Count; i++)
            {
                if (DateTime.TryParse(weatherData.Hourly.Time[i], out var hourlyTime))
                {
                    if (hourlyTime.Date == targetDate && i < weatherData.Hourly.Temperature2m.Count)
                    {
                        temperaturesForDate.Add(weatherData.Hourly.Temperature2m[i]);
                    }
                }
            }
            
            // Return average temperature for the day, or fallback to 15.0
            return temperaturesForDate.Count > 0 ? temperaturesForDate.Average() : 15.0;
        }
    }
}