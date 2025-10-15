using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO;

namespace ProyectoPrueba
{
    public partial class App : Application
    {
        public static IConfiguration? Configuration { get; private set; }

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            // Configurar la configuración aquí
            Configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                try
                {
                    var serviceProvider = Program.GetServiceProvider();
                    var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
                    desktop.MainWindow = new MainWindow(context);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("Error initializing application: " + ex.Message, ex);
                }
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}