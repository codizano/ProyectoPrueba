using Avalonia;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;

namespace ProyectoPrueba
{
    class Program
    {
        private static IServiceProvider? _serviceProvider;

        public static void Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var services = new ServiceCollection();
            
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("PostgreSQL")));

            services.AddSingleton<IConfiguration>(configuration);

            _serviceProvider = services.BuildServiceProvider();

            BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
        }

        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .LogToTrace();

        public static IServiceProvider GetServiceProvider()
        {
            if (_serviceProvider == null)
            {
                throw new InvalidOperationException("Service provider not initialized");
            }
            return _serviceProvider;
        }
    }
}