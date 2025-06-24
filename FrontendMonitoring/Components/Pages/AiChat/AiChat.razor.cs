using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;

namespace FrontendMonitoring.Components.Pages.AiChat
{
    public partial class AiChat : ComponentBase
    {
        [Inject] private HttpClient Http { get; set; }
        [Inject] private FrontendMonitoring.Services.AfvalApiClient AfvalClient { get; set; }
        [Inject] private ISnackbar Snackbar { get; set; }
        [Inject] private NavigationManager Nav { get; set; }

        private FilterOption selectedPeriodOption = new FilterOption { Name = "Alle tijd", Value = "all" };
        private string userPrompt = string.Empty;
        private string aiResponse = string.Empty;
        private bool isLoading = false;
        private List<FrontendMonitoring.Models.AfvalModel> trashData = new();

        public class FilterOption
        {
            public string Name { get; set; } = string.Empty;
            public string Value { get; set; } = string.Empty;
            public override bool Equals(object? o) => (o as FilterOption)?.Value == Value;
            public override int GetHashCode() => Value?.GetHashCode() ?? 0;
        }

        Func<FilterOption, string> converter = p => p?.Name ?? string.Empty;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                var apiData = await AfvalClient.GetLitterAndWeatherAsync();
                trashData = apiData?.Litter ?? new();
            }
            catch
            {
                Snackbar.Add("Kon afvaldata niet laden.", Severity.Error);
            }
        }

        private async Task SendPrompt()
        {
            isLoading = true;
            aiResponse = string.Empty;
            StateHasChanged();
            var dataForPrompt = GetDataForPeriod(selectedPeriodOption.Value);
            var prompt = ComposePrompt(userPrompt, selectedPeriodOption.Value, dataForPrompt);
            var request = new
            {
                model = "llama3:latest",
                prompt = prompt,
                stream = true
            };
            try
            {
                using var requestMessage = new HttpRequestMessage(HttpMethod.Post, "https://llm.coderize.nl/api/generate")
                {
                    Content = JsonContent.Create(request)
                };
                using var response = await Http.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead);
                response.EnsureSuccessStatusCode();
                using var stream = await response.Content.ReadAsStreamAsync();
                using var reader = new System.IO.StreamReader(stream, Encoding.UTF8);
                var sb = new StringBuilder();
                while (!reader.EndOfStream)
                {
                    var line = await reader.ReadLineAsync();
                    if (string.IsNullOrWhiteSpace(line)) continue;
                    try
                    {
                        var json = JsonDocument.Parse(line);
                        if (json.RootElement.TryGetProperty("response", out var resp))
                        {
                            sb.Append(resp.GetString());
                            aiResponse = sb.ToString();
                            StateHasChanged();
                        }
                        if (json.RootElement.TryGetProperty("done", out var done) && done.GetBoolean())
                            break;
                    }
                    catch { }
                }
            }
            catch (Exception ex)
            {
                aiResponse = $"Fout bij AI-analyse: {ex.Message}";
            }
            isLoading = false;
            StateHasChanged();
        }

        private string ComposePrompt(string userPrompt, string period, List<FrontendMonitoring.Models.AfvalModel> data)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Analyseer de volgende afvaldata, in het nederlands voor periode: {PeriodToText(period)}.");
            sb.AppendLine("Data:");
            foreach (var d in data.Take(100))
            {
                sb.AppendLine($"Type: {d.TrashType}, Locatie: {d.Location}, Datum: {d.Time:yyyy-MM-dd}, Temperatuur: {d.Temperature}, Confidence: {d.Confidence:0.00}");
            }
            sb.AppendLine($"Vraag: {userPrompt}");
            return sb.ToString();
        }

        private List<FrontendMonitoring.Models.AfvalModel> GetDataForPeriod(string period)
        {
            var now = DateTime.Now;
            return period switch
            {
                "all" => trashData,
                "30days" => trashData.Where(d => d.Time.HasValue && d.Time.Value >= now.AddDays(-30)).ToList(),
                "7days" => trashData.Where(d => d.Time.HasValue && d.Time.Value >= now.AddDays(-7)).ToList(),
                "prediction" => trashData,
                _ => trashData
            };
        }

        private string PeriodToText(string period) => period switch
        {
            "all" => "Alle tijd",
            "30days" => "Laatste 30 dagen",
            "7days" => "Laatste 7 dagen",
            "prediction" => "Voorspel morgen",
            _ => period
        };

        private async Task OnPromptKeyDown(KeyboardEventArgs e)
        {
            if (e.Key == "Enter" && !isLoading && !string.IsNullOrWhiteSpace(userPrompt))
            {
                await SendPrompt();
            }
        }
    }
}
