using DMAdvantage.Data;
using DMAdvantage.Server;
using DMAdvantage.Shared.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace TestEngineering
{
    public class TestServerFactory
    {
        private readonly Guid _databaseId = Guid.NewGuid();

        public static readonly string CurrentUser = "testuser@email.com";
        public static readonly string CurrentPassword = "P@ssw0rd";

        public TestServer Create()
        {
            var builder = new WebHostBuilder().UseStartup<Startup>();

            var projectDir = Directory.GetCurrentDirectory();
            var configPath = Path.Combine(projectDir, "appsettings.json");

            builder.ConfigureAppConfiguration((_, conf) =>
            {
                conf.AddJsonFile(configPath);
            });

            builder.ConfigureTestServices(services =>
            {
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<DMContext>));

                if (descriptor != null)
                    services.Remove(descriptor);

                services.AddDbContext<DMContext>(options =>
                {
                    options.UseInMemoryDatabase(_databaseId.ToString());
                });

                var sp = services.BuildServiceProvider();

                using var scope = sp.CreateScope();
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<DMContext>();
                db.Database.EnsureCreated();

                var userManager = scopedServices.GetRequiredService<UserManager<User>>();
                var result = Task.Run(async () => await userManager.CreateAsync(new User
                    {
                        UserName = CurrentUser,
                        Email = CurrentUser
                    },
                    CurrentPassword))
                    .Result;
                if (result != IdentityResult.Success)
                {
                    throw new InvalidOperationException("Could not create new user in seeder");
                }
            });

            return new TestServer(builder);
        }
    }
}
