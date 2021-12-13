using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using BookStore.Services;

namespace BookStore.Shared
{
    public partial class MainNavigation : ComponentBase
    {
        [Inject] public AppService AppService { get; set; }

        [Inject] public NavigationManager NavigationManager { get; set; }

        public string Path { get; set; } = string.Empty;

        protected override void OnInitialized()
        {
            AppService.OnSettingsUpdated += SettingsUpdated;
            NavigationManager.LocationChanged += NavigationManager_LocationChanged;

            Path = NavigationManager.Uri.Replace(NavigationManager.BaseUri, string.Empty);
        }

        private void NavigationManager_LocationChanged(object? sender, LocationChangedEventArgs e)
        {
            Path = e.Location.Replace(NavigationManager.BaseUri, string.Empty);
            InvokeAsync(() => StateHasChanged());
        }

        private void SettingsUpdated()
        {
            InvokeAsync(() => StateHasChanged());
        }

        public void Dispose()
        {
            AppService.OnSettingsUpdated -= SettingsUpdated;
            NavigationManager.LocationChanged -= NavigationManager_LocationChanged;
        }
    }
}
