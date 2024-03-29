﻿using DMAdvantage.Data;
using DMAdvantage.Shared.Services.Kafka;

namespace DMAdvantage.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            RunSeeding(host);

            host.Run();
        }

        private static void RunSeeding(IHost host)
        {
            var scopeFactory = host.Services.GetService<IServiceScopeFactory>();
            if (scopeFactory == null)
                return;
            using var scope = scopeFactory.CreateScope();
            var seeder = scope.ServiceProvider.GetService<Seeder>();
            seeder?.SeedAsync().Wait();

            var producer = scope.ServiceProvider.GetService<KafkaProducer>();
            producer?.Start();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
