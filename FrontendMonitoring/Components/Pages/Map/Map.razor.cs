using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FrontendMonitoring.Models;
using FrontendMonitoring.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;

namespace FrontendMonitoring.Components.Pages.Map
{
    public partial class Map : ComponentBase
    {
        [Inject] private IJSRuntime JS { get; set; }
        [Inject] private NavigationManager Nav { get; set; }
        [Inject] private AfvalApiClient AfvalClient { get; set; }
        [Inject] private ITokenStorage TokenStorage { get; set; }
        [Inject] private AuthenticationStateProvider AuthProvider { get; set; }

        private bool _isLoggedIn = false;
        private bool _isLoading = true;
        private bool _isRefreshing = false;
        private bool showOnlyCleaned = false;
        private bool useClusterView = false;

        FilterOption period = new FilterOption { Name = "Laatste 7 dagen" };
        FilterOption wasteType = new FilterOption { Name = "Alle types" };
        FilterOption location = new FilterOption { Name = "Alle locaties" };

        private LitterApiResponse? apiData;
        List<AfvalModel> detections = new();

        IEnumerable<AfvalModel> FilteredDetections =>
            detections.Where(d =>
                (wasteType.Name == "Alle types" || d.TrashType == wasteType.Name) &&
                (location.Name == "Alle locaties" || d.Location == location.Name) &&
                (!showOnlyCleaned || d.Cleaned) &&
                DateInSelectedPeriod(d.Time) &&
                d.Latitude.HasValue && d.Longitude.HasValue
            );

        private bool DateInSelectedPeriod(DateTime? date)
        {
            if (!date.HasValue) return false;
            var now = DateTime.Now.Date;
            var d = date.Value.Date;
            return period.Name switch
            {
                "Alle tijd" => true,
                "Laatste 1 dag" => d >= now.AddDays(-1),
                "Laatste 7 dagen" => d >= now.AddDays(-7),
                "Laatste 30 dagen" => d >= now.AddDays(-30),
                "Dit jaar" => d.Year == now.Year,
                _ => true
            };
        }

        private IEnumerable<string> TrashTypes => detections.Select(d => d.TrashType).Where(t => !string.IsNullOrEmpty(t)).Cast<string>().Distinct().OrderBy(x => x);
        private IEnumerable<string> UniqueLocations => detections.Select(d => GetShortLocation(d.Location)).Where(l => !string.IsNullOrEmpty(l)).Cast<string>().Distinct().OrderBy(x => x);

        public class FilterOption
        {
            public string Name { get; set; } = string.Empty;
            public override bool Equals(object? o) => (o as FilterOption)?.Name == Name;
            public override int GetHashCode() => Name?.GetHashCode() ?? 0;
        }

        Func<FilterOption, string> converter = p => p?.Name ?? string.Empty;

        protected override async Task OnInitializedAsync()
        {
            _isLoading = true;
            try
            {
                _isLoggedIn = await JS.InvokeAsync<bool>("hasAuthToken");
                if (!_isLoggedIn)
                {
                    Nav.NavigateTo("/login");
                    return;
                }
            }
            catch
            {
                Nav.NavigateTo("/login");
                return;
            }
            await LoadData();
        }

        private async Task LoadData()
        {
            try
            {
                try
                {
                    apiData = await AfvalClient.GetLitterAndWeatherAsync();
                    detections = apiData?.Litter ?? new List<AfvalModel>();
                }
                catch (Exception structuredException)
                {
                    Console.WriteLine($"Structured API call failed: {structuredException.Message}");
                    try
                    {
                        var litterResponse = await AfvalClient.GetLitterDataAsync();
                        detections = litterResponse ?? new List<AfvalModel>();
                    }
                    catch (Exception fallbackException)
                    {
                        Console.WriteLine($"Fallback API call failed: {fallbackException.Message}");
                        detections = new List<AfvalModel>();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"All API calls failed: {ex.Message}");
                detections = new List<AfvalModel>();
            }
            finally
            {
                _isLoading = false;
                StateHasChanged();
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                try
                {
                    await UpdateMap();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error initializing map: {ex.Message}");
                }
            }
        }

        private async Task RefreshData()
        {
            _isRefreshing = true;
            StateHasChanged();
            await LoadData();
            await UpdateMap();
            _isRefreshing = false;
            StateHasChanged();
        }

        private async Task UpdateMap()
        {
            try
            {
                var mapData = FilteredDetections.Select(d => new
                {
                    id = d.Id?.ToString() ?? Guid.NewGuid().ToString(),
                    lat = d.Latitude!.Value,
                    lng = d.Longitude!.Value,
                    type = d.TrashType ?? "Onbekend",
                    location = d.Location ?? "Onbekende locatie",
                    confidence = d.Confidence,
                    verified = d.Verified,
                    cleaned = d.Cleaned,
                    time = d.Time?.ToString("dd-MM-yyyy HH:mm") ?? "Onbekend",
                    temperature = d.Temperature?.ToString("F1") ?? "Onbekend",
                    cleanedTime = d.CleanedTime?.ToString("dd-MM-yyyy HH:mm")
                }).ToArray();
                await JS.InvokeVoidAsync("updateAdvancedMapMarkers", mapData, useClusterView);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Map update failed: {ex.Message}");
            }
        }

        private async Task FitMapToMarkers()
        {
            try
            {
                await JS.InvokeVoidAsync("fitMapToMarkers");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fit map to markers failed: {ex.Message}");
            }
        }

        private async Task ToggleClusterView()
        {
            useClusterView = !useClusterView;
            await UpdateMap();
        }

        private async Task ExportMapData()
        {
            var csv = new StringBuilder();
            csv.AppendLine("ID,Type,Location,Latitude,Longitude,Timestamp,Temperature,Confidence,Verified,Cleaned,CleanedTime");
            foreach (var d in FilteredDetections)
            {
                string id = EscapeCsv(d.Id?.ToString());
                string type = EscapeCsv(d.TrashType);
                string locationStr = EscapeCsv(d.Location);
                string latitude = d.Latitude?.ToString("F6", CultureInfo.InvariantCulture) ?? "";
                string longitude = d.Longitude?.ToString("F6", CultureInfo.InvariantCulture) ?? "";
                string timestamp = d.Time.HasValue ? d.Time.Value.ToString("u") : "";
                string temperature = d.Temperature.HasValue ? d.Temperature.Value.ToString("0.0", CultureInfo.InvariantCulture) : "";
                string confidence = d.Confidence.ToString("0.00", CultureInfo.InvariantCulture);
                string verified = d.Verified.ToString();
                string cleaned = d.Cleaned.ToString();
                string cleanedTime = d.CleanedTime.HasValue ? d.CleanedTime.Value.ToString("u") : "";
                csv.AppendLine($"{id},{type},{locationStr},{latitude},{longitude},{timestamp},{temperature},{confidence},{verified},{cleaned},{cleanedTime}");
            }
            await JS.InvokeVoidAsync("downloadCsv", "map-detections.csv", csv.ToString());
        }

        private string EscapeCsv(string? field)
        {
            if (string.IsNullOrEmpty(field)) return "";
            if (field.Contains("\""))
                field = field.Replace("\"", "\"\"");
            if (field.Contains(",") || field.Contains("\"") || field.Contains("\n"))
                return $"\"{field}\"";
            return field;
        }

        private string GetShortLocation(string? location)
        {
            if (string.IsNullOrWhiteSpace(location))
                return "Onbekend";
            var parts = location.Split(',');
            return parts[0].Trim();
        }

        private async Task OnPeriodChanged(FilterOption newPeriod)
        {
            period = newPeriod;
            await UpdateMap();
            StateHasChanged();
        }

        private async Task OnWasteTypeChanged(FilterOption newWasteType)
        {
            wasteType = newWasteType;
            await UpdateMap();
            StateHasChanged();
        }

        private async Task OnLocationChanged(FilterOption newLocation)
        {
            location = newLocation;
            await UpdateMap();
            StateHasChanged();
        }

        private async Task OnShowOnlyCleanedChanged(bool newValue)
        {
            showOnlyCleaned = newValue;
            await UpdateMap();
            StateHasChanged();
        }

        protected override async Task OnParametersSetAsync()
        {
            await UpdateMap();
        }
    }
}
