using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components.RenderTree;

namespace FrontendMonitoring.Components.Pages.home
{
    public partial class Home : ComponentBase
    {
        [Inject] private IJSRuntime JS { get; set; }
        [Inject] private NavigationManager Nav { get; set; }

        private bool _isLoggedIn = false;
        private bool _introDismissed = false;
        private bool _isInteractive = false;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                _isInteractive = true;
                try
                {
                    _isLoggedIn = await JS.InvokeAsync<bool>("hasAuthToken");
                    StateHasChanged();
                }
                catch
                {
                    _isLoggedIn = false;
                    StateHasChanged();
                }
            }
        }

        private void GoToLogin()
        {
            try
            {
                Nav.NavigateTo("/login");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Navigation failed: {ex.Message}");
            }
        }

        private void GoToDashboard()
        {
            Nav.NavigateTo("/dashboard");
        }

        private async Task Logout()
        {
            try
            {
                await JS.InvokeVoidAsync("clearAuthToken");
                _isLoggedIn = false;
                StateHasChanged();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Logout failed: {ex.Message}");
            }
        }

        private void DismissIntro()
        {
            _introDismissed = true;
            StateHasChanged();
        }
    }
}
