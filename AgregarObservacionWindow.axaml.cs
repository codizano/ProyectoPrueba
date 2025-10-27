using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Microsoft.EntityFrameworkCore;
using ProyectoPrueba.Models;


namespace ProyectoPrueba
{
    public partial class AgregarObservacionWindow : Window
    {
        private readonly ApplicationDbContext _context;
        private readonly Student _estudiante;
        private List<Sheet>? _hojas;

        // Referencias a controles UI
        private TextBlock? _estadoText;
        private ComboBox? _hojasComboBox;
        private TextBox? _observacionTextBox;

        public AgregarObservacionWindow(ApplicationDbContext context, Student estudiante)
        {
            InitializeComponent();
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _estudiante = estudiante ?? throw new ArgumentNullException(nameof(estudiante));

            InicializarControles();
            _ = InicializarAsync();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void InicializarControles()
        {
            // Cachear referencias a controles
            _estadoText = this.FindControl<TextBlock>("EstadoText");
            _hojasComboBox = this.FindControl<ComboBox>("HojasComboBox");
            _observacionTextBox = this.FindControl<TextBox>("ObservacionTextBox");

            // Mostrar nombre del estudiante
            if (this.FindControl<TextBlock>("NombreEstudianteText") is TextBlock nombreEstudianteText)
            {
                nombreEstudianteText.Text = _estudiante.Name;
            }
        }

        private async Task InicializarAsync()
        {
            await CargarHojasEstudianteAsync();
        }

        private async Task CargarHojasEstudianteAsync()
        {
            try
            {
                _hojas = await _context.Sheets
                    .Where(s => s.StudentId == _estudiante._id)
                    .OrderBy(s => s.SheetName)
                    .ToListAsync();

                if (_hojas.Count > 0)
                {
                    CargarComboBoxHojas();
                    MostrarMensajeEstado($"Partituras cargadas: {_hojas.Count}");
                }
                else
                {
                    MostrarMensajeEstado("No hay partituras disponibles para este estudiante");
                }
            }
            catch (Exception ex)
            {
                var mensajeError = ObtenerMensajeErrorMasInterno(ex);
                Console.WriteLine($"Error al cargar las partituras: {ex}");
                MostrarMensajeEstado($"Error al cargar partituras: {mensajeError}");
            }
        }

        private void CargarComboBoxHojas()
        {
            if (_hojasComboBox != null && _hojas != null && _hojas.Count > 0)
            {
                _hojasComboBox.ItemsSource = _hojas.Select(h => h.SheetName).ToList();
                _hojasComboBox.SelectedIndex = 0;
            }
        }

        private void OnGuardarClick(object sender, RoutedEventArgs e)
        {
            if (!ValidarFormulario())
            {
                return;
            }

            _ = GuardarObservacionAsync();
        }

        private bool ValidarFormulario()
        {
            if (_observacionTextBox == null || string.IsNullOrWhiteSpace(_observacionTextBox.Text))
            {
                MostrarMensajeEstado("Por favor, ingresa una observación");
                return false;
            }

            if (_hojasComboBox == null || _hojasComboBox.SelectedItem == null)
            {
                MostrarMensajeEstado("Por favor, selecciona una partitura");
                return false;
            }

            if (_hojas == null || _hojas.Count == 0)
            {
                MostrarMensajeEstado("No hay partituras disponibles");
                return false;
            }

            return true;
        }

        private async Task GuardarObservacionAsync()
        {
            try
            {
                if (_hojasComboBox == null || _observacionTextBox == null || _hojas == null)
                {
                    return;
                }

                // Copiar referencias de campo a variables locales para que el compilador
                // reconozca que no son nulas y evitar advertencias de nullability.
                var hojasLocal = _hojas;
                var hojasComboBoxLocal = _hojasComboBox;
                var observacionTextBoxLocal = _observacionTextBox;

                var indiceSeleccionado = hojasComboBoxLocal.SelectedIndex;
                if (indiceSeleccionado < 0 || indiceSeleccionado >= hojasLocal.Count)
                {
                    MostrarMensajeEstado("Selección de partitura inválida");
                    return;
                }

                var hojaSeleccionada = hojasLocal[indiceSeleccionado];
                // Usar accesso null-conditional por si acaso y luego validar con IsNullOrWhiteSpace.
                var textoObservacion = observacionTextBoxLocal.Text?.Trim();

                if (string.IsNullOrWhiteSpace(textoObservacion))
                {
                    MostrarMensajeEstado("Por favor, ingresa una observación");
                    return;
                }

                var observacion = new SheetObservation
                {
                    SheetId = hojaSeleccionada.Id,
                    Observation = textoObservacion,
                    ObservationDate = DateTime.SpecifyKind(DateTime.Now.Date, DateTimeKind.Utc)
                };

                _context.SheetObservations.Add(observacion);
                await _context.SaveChangesAsync();

                MostrarMensajeEstado("Observación guardada exitosamente");
                LimpiarFormulario();
            }
            catch (Exception ex)
            {
                var mensajeError = ObtenerMensajeErrorMasInterno(ex);
                Console.WriteLine($"Error al guardar la observación: {ex}");
                MostrarMensajeEstado($"Error al guardar la observación: {mensajeError}");
            }
        }

        private void OnCancelarClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void LimpiarFormulario()
        {
            if (_observacionTextBox != null)
            {
                _observacionTextBox.Text = string.Empty;
            }
        }

        private void MostrarMensajeEstado(string mensaje)
        {
            if (_estadoText != null)
            {
                _estadoText.Text = mensaje;
            }
        }

        private void LimpiarMensajeEstado()
        {
            MostrarMensajeEstado(string.Empty);
        }

        private static string ObtenerMensajeErrorMasInterno(Exception ex)
        {
            var innerException = ex;
            while (innerException.InnerException != null)
            {
                innerException = innerException.InnerException;
            }
            return innerException.Message;
        }
    }
}