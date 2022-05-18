using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.JSInterop;

namespace DMAdvantage.Client.Services.Implementations
{
    public class KafkaConsumer : IKafkaConsumer
    {
        private readonly IJSRuntime _js;
        private IJSObjectReference _jsModule;
        private readonly NavigationManager _navigationManager;

        public KafkaConsumer(IJSRuntime js,
            NavigationManager navigationManager)
        {
            _js = js;
            _navigationManager = navigationManager;
            _navigationManager.LocationChanged += NavigationManager_LocationChanged;
        }

        public async Task ConnectAsync(string topic)
        {
            var baseUri = _navigationManager.BaseUri;
            baseUri = baseUri.Replace("http:", "ws:");
            baseUri = baseUri.Replace("https:", "wss:");
            _jsModule = await _js.InvokeAsync<IJSObjectReference>("import", "./socket.js");
            await _jsModule.InvokeVoidAsync("connect", DotNetObjectReference.Create(this), $"{baseUri}api/ws/{topic}");
        }

        [JSInvokable]
        public void MessageReceived(string? data)
        {
            OnMessageReceived?.Invoke(this, data);
        }

        public event EventHandler<string?> OnMessageReceived;

        private async void NavigationManager_LocationChanged(object? sender, LocationChangedEventArgs e)
        {
            if (_jsModule != null)
            {
                await _jsModule.InvokeVoidAsync("close");
                await _jsModule.DisposeAsync();
            }
            _navigationManager.LocationChanged -= NavigationManager_LocationChanged;
        }
    }
}
