using Avalonia.Controls;
using Avalonia.Input;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace ProyectoPrueba
{
    public partial class MainWindow : Window
    {
        private readonly ApplicationDbContext _context;
        
        public MainWindow(ApplicationDbContext context)
        {
            InitializeComponent();
            _context = context;
        }
        
        // Método para el evento Click del botón "Probar Conexión"
        private void OnTestConnectionClick(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            TestConnection();
        }
        
        // Método para el evento Click del botón "Buscar Alumno"
        private void OnBuscarAlumnoClick(object sender, PointerPressedEventArgs e)
        {
            AbrirVentanaBuscarAlumno();
        }
        
        private void AbrirVentanaBuscarAlumno()
        {
            try
            {
                var ventanaBuscar = new BuscarAlumnoWindow(_context);
                ventanaBuscar.Show();
            }
            catch (Exception ex)
            {
                // Aquí podrías mostrar un diálogo de error
                Console.WriteLine($"Error al abrir la ventana de búsqueda: {ex.Message}");
            }
        }
        
        private async void TestConnection()
        {
            try
            {
                var connectionString = App.Configuration?.GetConnectionString("PostgreSQL");
                Console.WriteLine($"Usando cadena de conexión: {connectionString}");
                
                var canConnect = await _context.Database.CanConnectAsync();
                Console.WriteLine($"¿Puede conectar?: {canConnect}");
                
                // Si llegamos aquí, la conexión fue exitosa
                var messageBoxStandardWindow = new Window
                {
                    Width = 200,
                    Height = 100,
                    Content = new TextBlock
                    {
                        Text = canConnect ? "¡Conexión exitosa!" : "No se pudo conectar a la base de datos",
                        HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                        VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center
                    }
                };
                
                messageBoxStandardWindow.Show();
            }
            catch (Exception ex)
            {
                // Mostrar error en una ventana
                var messageBoxStandardWindow = new Window
                {
                    Width = 300,
                    Height = 150,
                    Content = new TextBlock
                    {
                        Text = $"Error de conexión: {ex.Message}",
                        TextWrapping = Avalonia.Media.TextWrapping.Wrap,
                        HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                        VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center
                    }
                };
                
                messageBoxStandardWindow.Show();
            }
        }
    }
}