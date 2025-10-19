using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Microsoft.EntityFrameworkCore;
using ProyectoPrueba.Models;
using System;

namespace ProyectoPrueba
{
    public partial class AgregarObservacionWindow : Window
    {
        private readonly ApplicationDbContext _context;
        private readonly Student _estudiante;
        private List<Sheet>? _hojas;

        public AgregarObservacionWindow(ApplicationDbContext context, Student estudiante)
        {
            InitializeComponent();
            _context = context;
            _estudiante = estudiante;

            // Inicializar la ventana
            if (this.FindControl<TextBlock>("NombreEstudianteText") is TextBlock nombreEstudianteText)
            {
                nombreEstudianteText.Text = estudiante.Name;
            }

            // Cargar las hojas del estudiante
            _ = InicializarAsync();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private async Task CargarHojasEstudianteAsync()
        {
            try
            {
                try
                {
                    _hojas = await _context.Sheets
                        .Where(s => s.StudentId == _estudiante._id)
                        .ToListAsync();

                    var estadoTextLoad = this.FindControl<TextBlock>("EstadoText");
                    if (estadoTextLoad != null)
                    {
                        estadoTextLoad.Text = $"Partituras cargadas: {_hojas.Count}";
                    }
                }
                catch (Exception ex){
                    // Mostrar error en consola
                    Console.WriteLine($"Error interno al cargar partituras: {ex}"); 

                    var estadoTextLoadError = this.FindControl<TextBlock>("EstadoText");
                    if (estadoTextLoadError != null)
                    {
                        var innerException = ex;
                        while (innerException.InnerException != null)
                        {
                            innerException = innerException.InnerException;
                        }
                        estadoTextLoadError.Text = $"Error al cargar partituras: {innerException.Message}";
                    }
                    throw; // Re-lanzar la excepción para manejarla en el nivel superior
                }

                if (_hojas != null && _hojas.Count > 0)
                {
                    var nombresHojas = _hojas.Select(h => h.SheetName).ToList();

                    if (this.FindControl<ComboBox>("HojasComboBox") is ComboBox hojasComboBox)
                    {
                        hojasComboBox.ItemsSource = nombresHojas;
                        hojasComboBox.SelectedIndex = 0;
                    }
                }
                else
                {
                    var estadoTextNoSheets = this.FindControl<TextBlock>("EstadoText");
                    if (estadoTextNoSheets != null)
                    {
                        estadoTextNoSheets.Text = "No hay partituras disponibles para este estudiante";
                    }
                }
            }
            catch (Exception ex)
            {
                // Mostrar error por console
                Console.WriteLine($"Error al cargar las partituras: {ex}");
                var estadoTextCatch = this.FindControl<TextBlock>("EstadoText");
                if (estadoTextCatch != null)
                {
                    estadoTextCatch.Text = $"Error al cargar las partituras: {ex.Message}";
                }
            }
        }

        private void OnGuardarClick(object sender, RoutedEventArgs e)
        {
            if (this.FindControl<TextBox>("ObservacionTextBox") is TextBox observacionTextBox &&
                string.IsNullOrWhiteSpace(observacionTextBox.Text))
            {
                var estadoTextValidacionObs = this.FindControl<TextBlock>("EstadoText");
                if (estadoTextValidacionObs != null)
                {
                    estadoTextValidacionObs.Text = "Por favor, ingresa una observación";
                }
                return;
            }

            if (this.FindControl<ComboBox>("HojasComboBox") is ComboBox hojasComboBox &&
                hojasComboBox.SelectedItem == null)
            {
                var estadoTextValidacionPart = this.FindControl<TextBlock>("EstadoText");
                if (estadoTextValidacionPart != null)
                {
                    estadoTextValidacionPart.Text = "Por favor, selecciona una partitura";
                }
                return;
            }

            _ = GuardarObservacionAsync();
        }

        private async Task GuardarObservacionAsync()
        {
            try
            {
                if (this.FindControl<ComboBox>("HojasComboBox") is ComboBox hojasComboBox &&
                    this.FindControl<TextBox>("ObservacionTextBox") is TextBox observacionTextBox &&
                    hojasComboBox.SelectedIndex >= 0 && 
                    _hojas != null && 
                    hojasComboBox.SelectedIndex < _hojas.Count)
                {
                    var hojaSeleccionada = _hojas[hojasComboBox.SelectedIndex];
                    
                    if (string.IsNullOrWhiteSpace(observacionTextBox.Text))
                    {
                        var estadoTextValidacion = this.FindControl<TextBlock>("EstadoText");
                        if (estadoTextValidacion != null)
                        {
                            estadoTextValidacion.Text = "Por favor, ingresa una observación";
                        }
                        return;
                    }

                    var observacion = new SheetObservation
                    {
                        SheetId = hojaSeleccionada.Id,
                        Observation = observacionTextBox.Text,
                        ObservationDate = DateTime.SpecifyKind(DateTime.Now.Date, DateTimeKind.Utc)
                    };

                    _context.SheetObservations.Add(observacion);
                    await _context.SaveChangesAsync();

                    var estadoTextExito = this.FindControl<TextBlock>("EstadoText");
                    if (estadoTextExito != null)
                    {
                        estadoTextExito.Text = "Observación guardada exitosamente";
                    }

                    // Limpiar campo
                    observacionTextBox.Text = string.Empty;
                }
            }
            catch (Exception ex)
            {
                // Error por console
                Console.WriteLine($"Error al guardar la observación: {ex}");

                var estadoTextError = this.FindControl<TextBlock>("EstadoText");
                if (estadoTextError != null)
                {
                    // Obtener la excepción más interna
                    var innerException = ex;
                    while (innerException.InnerException != null)
                    {
                        innerException = innerException.InnerException;
                    }
                    
                    estadoTextError.Text = $"Error al guardar la observación: {innerException.Message}";
                }
            }
        }

        private void OnCancelarClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void LimpiarMensajeEstado()
        {
            if (this.FindControl<TextBlock>("EstadoText") is TextBlock estadoText)
            {
                estadoText.Text = string.Empty;
            }
        }

        private async Task InicializarAsync()
        {
            await CargarHojasEstudianteAsync();
        }
    }
}