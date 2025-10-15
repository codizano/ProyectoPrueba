using Avalonia.Controls;
using Avalonia.Interactivity;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace ProyectoPrueba
{
    public partial class BuscarAlumnoWindow : Window
    {
        private readonly ApplicationDbContext _context;

        public BuscarAlumnoWindow(ApplicationDbContext context)
        {
            InitializeComponent();
            _context = context;
        }
        
        private async void OnBuscarClick(object sender, RoutedEventArgs e)
        {
            string nombre = BuscarTextBox?.Text ?? string.Empty;
            if (!string.IsNullOrWhiteSpace(nombre))
            {
                try
                {
                    var observaciones = await _context.SheetObservations
                        .Include(so => so.Sheet)
                            .ThenInclude(s => s!.Student)
                        .Where(so => so.Sheet != null && 
                                   so.Sheet.Student != null && 
                                   so.Sheet.Student.Name != null && 
                                   EF.Functions.ILike(so.Sheet.Student.Name, $"%{nombre}%"))
                        .OrderByDescending(so => so.ObservationDate) // Ordenar por fecha más reciente
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
                        sb.AppendLine();
                        
                        foreach (var grupo in observacionesPorEstudiante)
                        {
                            sb.AppendLine($"Estudiante: {grupo.Key.StudentName}");
                            sb.AppendLine("Últimas observaciones:");
                            
                            // Tomar las 3 observaciones más recientes de cada estudiante
                            foreach (var obs in grupo.Take(3))
                            {
                                sb.AppendLine($"  • {obs.ObservationDate:dd/MM/yyyy}: {obs.Observation}");
                            }
                            sb.AppendLine(); // Línea en blanco entre estudiantes
                        }

                        ResultadoText.Text = sb.ToString();
                    }
                    else
                    {
                        ResultadoText.Text = $"No se encontraron observaciones para alumnos con nombre '{nombre}'";
                    }
                }
                catch (Exception ex)
                {
                    ResultadoText.Text = $"Error al buscar: {ex.Message}";
                }
            }
            else
            {
                ResultadoText.Text = "Por favor ingresa un nombre";
            }
        }
    }
}