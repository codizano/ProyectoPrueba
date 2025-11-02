using Avalonia.Controls;
using Avalonia.Interactivity;
using Microsoft.EntityFrameworkCore;
using ProyectoPrueba.Models;
using System.Text;

namespace ProyectoPrueba
{
    public partial class BuscarAlumnoWindow : Window
    {
    private readonly ApplicationDbContext? _context;
        private Student? _estudianteActual;

        // Parameterless constructor for XAML loader
        public BuscarAlumnoWindow()
        {
            InitializeComponent();
        }

        public BuscarAlumnoWindow(ApplicationDbContext context) : this()
        {
            _context = context;
        }

        private void OnAgregarObservacionClick(object sender, RoutedEventArgs e)
        {
            if (_estudianteActual != null && _context != null)
            {
                var ventanaObservacion = new AgregarObservacionWindow(_context, _estudianteActual);
                ventanaObservacion.Show();
            }
        }

        private void OnAgregarPartituraClick(object sender, RoutedEventArgs e)
        {
            if (_estudianteActual != null && _context != null)
            {
                var ventanaPartitura = new AgregarPartituraWindow(_context, _estudianteActual);
                ventanaPartitura.Show();
            }
        }
        
        private async void OnBuscarClick(object sender, RoutedEventArgs e)
        {
            string nombre = BuscarTextBox?.Text ?? string.Empty;
            if (!string.IsNullOrWhiteSpace(nombre))
            {
                if (_context == null)
                {
                    ResultadoText.Text = "Contexto de base de datos no disponible.";
                    AgregarObservacionButton.IsVisible = false;
                    _estudianteActual = null;
                    return;
                }
                try
                {
                    var observaciones = await _context.SheetObservations
                        .Include(so => so.Sheet)
                            .ThenInclude(s => s!.Student)
                        .Where(so => so.Sheet != null && 
                                   so.Sheet.Student != null && 
                                   so.Sheet.Student.Name != null && 
                                   EF.Functions.ILike(so.Sheet.Student.Name, $"%{nombre}%"))
                        .OrderByDescending(so => so.ObservationDate)
                        .ToListAsync();

                    if (observaciones.Any())
                    {
                        var observacionesPorEstudiante = observaciones
                            .GroupBy(o => new { 
                                StudentId = o.Sheet?.Student?._id, 
                                StudentName = o.Sheet?.Student?.Name 
                            })
                            .Where(g => g.Key.StudentId != null && g.Key.StudentName != null)
                            .OrderBy(g => g.Key.StudentName)
                            .ToList();

                        var sb = new StringBuilder();
                        sb.AppendLine($"Se encontraron {observaciones.Count} observaciones para {observacionesPorEstudiante.Count} estudiante(s):");
                        sb.AppendLine("(⚠️ Aquí solo se muestran las últimas 3 observaciones por estudiante)");
                        sb.AppendLine();
                        
                        foreach (var grupo in observacionesPorEstudiante)
                        {
                            sb.AppendLine($"Estudiante: {grupo.Key.StudentName}");
                            sb.AppendLine("Últimas observaciones:");
                            
                            // Tomar las 3 observaciones más recientes de cada estudiante
                            foreach (var obs in grupo.Take(3))
                            {
                                string sheetName = obs.Sheet?.SheetName ?? "Sin partitura";
                                sb.AppendLine($"  • {obs.ObservationDate:dd/MM/yyyy} - [{sheetName}]: {obs.Observation}");
                            }
                            sb.AppendLine(); // Línea en blanco entre estudiantes
                        }

                        ResultadoText.Text = sb.ToString();
                        
                        // Si solo hay un estudiante, mostrar los botones para agregar observación y partitura
                        if (observacionesPorEstudiante.Count == 1)
                        {
                            AgregarObservacionButton.IsVisible = true;
                            AgregarPartituraButton.IsVisible = true;
                            // Guardar el estudiante actual para usarlo al agregar observación/partitura
                            _estudianteActual = await _context.Students
                                .FirstOrDefaultAsync(s => s._id == observacionesPorEstudiante.First().Key.StudentId);
                        }
                        else
                        {
                            AgregarObservacionButton.IsVisible = false;
                            AgregarPartituraButton.IsVisible = false;
                            _estudianteActual = null;
                        }
                    }
                    else
                    {
                        ResultadoText.Text = $"No se encontraron observaciones para alumnos con nombre '{nombre}'";
                        AgregarObservacionButton.IsVisible = false;
                        AgregarPartituraButton.IsVisible = false;
                        _estudianteActual = null;
                    }
                }
                catch (Exception ex)
                {
                    ResultadoText.Text = $"Error al buscar: {ex.Message}";
                    AgregarObservacionButton.IsVisible = false;
                    AgregarPartituraButton.IsVisible = false;
                    _estudianteActual = null;
                }
            }
            else
            {
                ResultadoText.Text = "Por favor ingresa un nombre";
                AgregarObservacionButton.IsVisible = false;
                AgregarPartituraButton.IsVisible = false;
                _estudianteActual = null;
            }
        }
    }
}