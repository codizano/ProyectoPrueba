using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Microsoft.EntityFrameworkCore;
using ProyectoPrueba.Models;
using System;
using System.Threading.Tasks;

namespace ProyectoPrueba
{
    public partial class AgregarPartituraWindow : Window
    {
    private readonly ApplicationDbContext? _context;
    private readonly Student? _estudiante;

        public AgregarPartituraWindow()
        {
            InitializeComponent();
        }

        public AgregarPartituraWindow(ApplicationDbContext context, Student estudiante) : this()
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _estudiante = estudiante ?? throw new ArgumentNullException(nameof(estudiante));

            // Wire up events
            if (this.FindControl<Button>("BtnGuardar") is Button btnGuardar)
                btnGuardar.Click += OnGuardarClick;

            if (this.FindControl<Button>("BtnCancelar") is Button btnCancelar)
                btnCancelar.Click += OnCancelarClick;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private async void OnGuardarClick(object? sender, RoutedEventArgs e)
        {
            var nombreBox = this.FindControl<TextBox>("NombreTextBox");
            var objetivoBox = this.FindControl<TextBox>("ObjetivoTextBox");
            var opinionBox = this.FindControl<TextBox>("OpinionTextBox");
            var estadoText = this.FindControl<TextBlock>("EstadoText");

            if (nombreBox == null)
                return;

            if (string.IsNullOrWhiteSpace(nombreBox.Text))
            {
                if (estadoText != null) estadoText.Text = "El nombre de la partitura es requerido.";
                return;
            }

            if (_context == null || _estudiante == null)
            {
                if (estadoText != null) estadoText.Text = "Contexto o estudiante no disponible.";
                return;
            }

            var sheet = new Sheet
            {
                SheetName = nombreBox.Text.Trim(),
                Objective = objetivoBox?.Text?.Trim(),
                Opinion = opinionBox?.Text?.Trim(),
                StudentId = _estudiante._id
            };

            try
            {
                _context.Sheets.Add(sheet);
                await _context.SaveChangesAsync();

                if (estadoText != null) estadoText.Foreground = Avalonia.Media.Brushes.Green;
                if (estadoText != null) estadoText.Text = "Partitura guardada correctamente.";

                // Opcional: cerrar ventana
                Close();
            }
            catch (Exception ex)
            {
                if (estadoText != null) estadoText.Text = "Error al guardar la partitura: " + ex.Message;
            }
        }

        private void OnCancelarClick(object? sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
