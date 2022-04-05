using DMAdvantage.Client.Helpers;
using Microsoft.JSInterop;

namespace DMAdvantage.Client.Services.Implementations
{
    public class DeviceSizeService : IDeviceSizeService
    {
        private IJSRuntime? JS = null;
        public event EventHandler<DeviceSize> Resize;
        private int _browserWidth;
        private IJSObjectReference _jsModule;
        public DeviceSize Current { get; private set; }

        public async Task InitializeAsync(IJSRuntime js)
        {
            // enforce single invocation            
            if (JS == null)
            {
                JS = js;
                _jsModule = await JS.InvokeAsync<IJSObjectReference>("import", "./listener.js");
                await _jsModule.InvokeVoidAsync("resizeListener", DotNetObjectReference.Create(this));
                var currentWidth = await _jsModule.InvokeAsync<int>("currentWidth");
                UpdateWidth(currentWidth);
            }
        }

        [JSInvokable]
        public void UpdateWidth(int jsBrowserWidth)
        {
            _browserWidth = jsBrowserWidth;
            UpdateDeviceSize();
            Resize?.Invoke(this, Current);
        }

        public void UpdateDeviceSize()
        {
            Current = _browserWidth switch
            {
                < 600 => DeviceSize.ExtraSmall,
                < 960 => DeviceSize.Small,
                < 1280 => DeviceSize.Medium,
                < 1920 => DeviceSize.Large,
                < 2560 => DeviceSize.ExtraLarge,
                _ => DeviceSize.ExtraExtraLarge
            };
        }
    }
}
