using DMAdvantage.Client.Helpers;
using Microsoft.JSInterop;

namespace DMAdvantage.Client.Services
{
    public interface IDeviceSizeService
    {
        public event EventHandler<DeviceSize> Resize;
        public DeviceSize Current { get; }

        public void UpdateDeviceSize();
        public Task InitializeAsync(IJSRuntime js);
    }
}
