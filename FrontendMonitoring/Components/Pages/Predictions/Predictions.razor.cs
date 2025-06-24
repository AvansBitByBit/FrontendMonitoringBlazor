using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using FrontendMonitoring.Services;

namespace FrontendMonitoring.Components.Pages.Predictions
{
    public partial class Predictions : ComponentBase
    {
        [Inject] private PythonPredictionApiClient PredictionApiClient { get; set; }
        [Inject] private ISnackbar Snackbar { get; set; }

        private DateTime? selectedDate = DateTime.Now; // Default date
        private double temperature = 20.0; // Default temperature
        private bool isLoading = false;
        private PredictionResponse? response;

        private async Task MakePrediction()
        {
            isLoading = true;
            try
            {
                if (selectedDate.HasValue)
                {
                    var request = new PredictionRequest
                    {
                        Date = selectedDate.Value,
                        Temperature = (int)temperature
                    };
                    response = await PredictionApiClient.MakePredictionAsync(request);

                    if (response != null)
                    {
                        Snackbar.Add("Prediction received successfully!", Severity.Success);
                    }
                    else
                    {
                        Snackbar.Add("Failed to get a response from the API.", Severity.Warning);
                    }
                }
                else
                {
                    Snackbar.Add("Please select a valid date.", Severity.Warning);
                }
            }
            catch (Exception ex)
            {
                Snackbar.Add($"Error: {ex.Message}", Severity.Error);
            }
            finally
            {
                isLoading = false;
            }
        }
    }
}
