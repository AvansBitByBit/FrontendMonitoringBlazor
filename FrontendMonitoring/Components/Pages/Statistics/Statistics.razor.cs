using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FrontendMonitoring.Models;
using FrontendMonitoring.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace FrontendMonitoring.Components.Pages.Statistics
{
    public partial class Statistics : ComponentBase
    {
        [Inject] public AfvalApiClient AfvalClient { get; set; } = null!;
        private LitterApiResponse? apiData;
        private List<AfvalModel> detections = new();
        private string? weather;
        private bool _isLoading = true;
        private bool _isRefreshing = false;

        protected override async Task OnInitializedAsync()
        {
            await LoadData();
        }

        private async Task LoadData()
        {
            _isLoading = true;
            StateHasChanged();
            try
            {
                apiData = await AfvalClient.GetLitterAndWeatherAsync();
                detections = apiData?.Litter ?? new List<AfvalModel>();
                weather = apiData?.Weather;
            }
            catch (Exception)
            {
                detections = new List<AfvalModel>();
            }
            finally
            {
                _isLoading = false;
                StateHasChanged();
            }
        }

        private async Task RefreshData()
        {
            _isRefreshing = true;
            StateHasChanged();
            try
            {
                apiData = await AfvalClient.GetLitterAndWeatherAsync();
                detections = apiData?.Litter ?? new List<AfvalModel>();
                weather = apiData?.Weather;
            }
            catch (Exception)
            {
                detections = new List<AfvalModel>();
            }
            finally
            {
                _isRefreshing = false;
                StateHasChanged();
            }
        }

        private string MostCommonTrashType => detections
            .GroupBy(d => d.TrashType)
            .OrderByDescending(g => g.Count())
            .Select(g => g.Key)
            .FirstOrDefault() ?? "-";

        private string TopLocation => detections
            .GroupBy(d => d.Location)
            .OrderByDescending(g => g.Count())
            .Select(g => g.Key)
            .FirstOrDefault() ?? "-";

        private double AverageConfidence => detections.Count == 0 ? 0 : detections.Average(d => d.Confidence);

        private string ShortLocation => GetStreetName(TopLocation);

        private string GetStreetName(string location)
        {
            if (string.IsNullOrWhiteSpace(location)) return "-";
            var idx = location.IndexOf(',');
            return idx > 0 ? location.Substring(0, idx) : location;
        }

        private IEnumerable<(string Type, int Count, double AvgConfidence)> TrashTypeStats =>
            detections.GroupBy(d => d.TrashType)
                .Select(g => (g.Key ?? "-", g.Count(), g.Average(x => x.Confidence)));

        private IEnumerable<(string Location, int Count, double AvgConfidence)> LocationStats =>
            detections.GroupBy(d => d.Location)
                .Select(g => (g.Key ?? "-", g.Count(), g.Average(x => x.Confidence)));

        private IEnumerable<(DateTime Date, int Count)> DailyStats =>
            detections.Where(d => d.Time.HasValue)
                .GroupBy(d => d.Time.Value.Date)
                .OrderByDescending(g => g.Key)
                .Select(g => (g.Key, g.Count()));

        private IEnumerable<(DateTime Date, double AvgConfidence)> DailyConfidenceStats =>
            detections.Where(d => d.Time.HasValue)
                .GroupBy(d => d.Time.Value.Date)
                .OrderByDescending(g => g.Key)
                .Select(g => (g.Key, g.Average(x => x.Confidence)));

        private IEnumerable<(DateTime Date, string Type, int Count)> TypePerDayStats =>
            detections.Where(d => d.Time.HasValue)
                .GroupBy(d => new { d.Time.Value.Date, d.TrashType })
                .OrderByDescending(g => g.Key.Date)
                .ThenBy(g => g.Key.TrashType)
                .Select(g => (g.Key.Date, g.Key.TrashType ?? "-", g.Count()));

        private double CleanupPercentage => detections.Count == 0 ? 0 : detections.Count(d => d.Cleaned) / (double)detections.Count;
        private double AverageResponseTime => detections.Count == 0 ? 0 : detections.Where(d => d.Cleaned && d.Time.HasValue && d.CleanedTime.HasValue).Select(d => (d.CleanedTime.Value - d.Time.Value).TotalHours).DefaultIfEmpty(0).Average();
        private int ActiveZones => detections.Select(d => d.Location).Distinct().Count();
        private int MaxDailyCount => DailyStats.Any() ? DailyStats.Max(x => x.Count) : 1;
        private int MaxTypeCount => TypePerDayStats.Any() ? TypePerDayStats.Max(x => x.Count) : 1;

        private Color GetConfidenceColor(double confidence)
        {
            return confidence switch
            {
                >= 0.8 => Color.Success,
                >= 0.6 => Color.Warning,
                _ => Color.Error
            };
        }
    }
}
