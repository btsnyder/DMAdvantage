using DMAdvantage.Client.Services;
using DMAdvantage.Client.Services.Implementations;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

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
                .AddScoped<IAlertService, AlertService>()
                .AddScoped<IHttpService, HttpService>()
                .AddScoped<ILocalStorageService, LocalStorageService>()
                .AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            await builder.Build().RunAsync();
        }
    }
}
