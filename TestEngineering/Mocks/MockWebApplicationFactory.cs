using DMAdvantage.Data;
using DMAdvantage.Shared.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Headers;

namespace TestEngineering.Mocks
{
    public class MockWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> 
        where TStartup : class
    {
        public MockWebApplicationFactory()
        {
            
        }

        public string DatabaseId = Guid.NewGuid().ToString();

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            var projectDir = Directory.GetCurrentDirectory();
            var configPath = Path.Combine(projectDir, "appsettings.json");

            builder.ConfigureAppConfiguration((_, conf) =>
            {
                conf.AddJsonFile(configPath);
            });

            builder.ConfigureServices(services =>
            {
                var descriptor = services.Single(
                    d => d.ServiceType == typeof(DbContextOptions<DMContext>));

                services.Remove(descriptor);

                services.AddDbContext<DMContext>(options =>
                {
                    options.UseInMemoryDatabase(DatabaseId);
                });

                var sp = services.BuildServiceProvider();

                using var scope = sp.CreateScope();
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<DMContext>();
                db.Database.EnsureCreated();

                var userManager = scopedServices.GetRequiredService<UserManager<User>>();
                var result = Task.Run(async () => await userManager.CreateAsync(MockHttpContext.CurrentUser, MockSigninManagerFactory.CurrentPassword)).Result;
                if (result != IdentityResult.Success)
                {
                    throw new InvalidOperationException("Could not create new user in seeder");
                }
            });
        }

        public async Task<HttpClient> CreateAuthenticatedClientAsync()
        {
            var client = CreateClient();
            var token = await client.CreateToken();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return client;
        }
    }
}
