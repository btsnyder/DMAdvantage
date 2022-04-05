using DMAdvantage.Client.Helpers;
using DMAdvantage.Client.Services;
using Microsoft.AspNetCore.Components;

namespace DMAdvantage.Client.Shared
{
    public class ComponentBaseWithDeviceChange : ComponentBase
    {
        [Inject] IDeviceSizeService DeviceSizeService { get; set; }
        [Inject] NavigationManager NavigationManager { get; set; }

        private void UpdatedBrowserWidth(object? sender, DeviceSize size)
        {
            StateHasChanged();
        }

        protected override void OnInitialized()
        {
            DeviceSizeService.Resize += UpdatedBrowserWidth;
            NavigationManager.LocationChanged += (sender, e) => { DeviceSizeService.Resize -= UpdatedBrowserWidth; };
            base.OnInitialized();
        }
    }
}
