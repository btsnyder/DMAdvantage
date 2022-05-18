using DMAdvantage.Client.Services;
using DMAdvantage.Client.Services.Implementations;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.JSInterop;
using MudBlazor;
using MudBlazor.Services;

namespace DMAdvantage.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            builder.Services
                .AddScoped<IApiService, ApiService>()
                .AddScoped<IAccountService, AccountService>()
                .AddScoped<IHttpService, HttpService>()
                .AddSingleton<ILocalStorageService, LocalStorageService>()
                .AddSingleton<IDeviceSizeService, DeviceSizeService>()
                .AddTransient<IKafkaConsumer, KafkaConsumer>()
                .AddMudServices(config =>
                {
                    config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomLeft;
                    config.SnackbarConfiguration.VisibleStateDuration = 3000;
                })
                .AddScoped(_ => new HttpClient {BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)});

            var host = builder.Build();

            var accountService = host.Services.GetRequiredService<IAccountService>();
            await accountService.InitializeAsync();

            var js = host.Services.GetRequiredService<IJSRuntime>();
            var deviceSizeService = host.Services.GetRequiredService<IDeviceSizeService>();
            await deviceSizeService.InitializeAsync(js);

            await host.RunAsync();
        }
    }
}
