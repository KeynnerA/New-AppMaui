using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows.Input;
using Microsoft.Maui.Storage;

namespace MauiApp1.Models;

public class NotasViewModel : INotifyPropertyChanged
{
    private readonly string _filePath =
        Path.Combine(FileSystem.AppDataDirectory, "notas.json");

    // Colección observable que se enlaza a la UI
    public ObservableCollection<Nota> Notas { get; set; } = new ObservableCollection<Nota>();

    // Texto que se enlaza al Editor (contenido de la nota)
    private string _notaEntryText;
    public string NotaEntryText
    {
        get => _notaEntryText;
        set
        {
            if (_notaEntryText != value)
            {
                _notaEntryText = value;
                OnPropertyChanged(nameof(NotaEntryText));
            }
        }
    }

    // Texto que se enlaza al Entry de título
    private string _tituloEntryText;
    public string TituloEntryText
    {
        get => _tituloEntryText;
        set
        {
            if (_tituloEntryText != value)
            {
                _tituloEntryText = value;
                OnPropertyChanged(nameof(TituloEntryText));
            }
        }
    }

    private bool _modoListaCheckboxActivo;
    public bool ModoListaCheckboxActivo
    {
        get => _modoListaCheckboxActivo;
        private set
        {
            if (_modoListaCheckboxActivo != value)
            {
                _modoListaCheckboxActivo = value;
                OnPropertyChanged(nameof(ModoListaCheckboxActivo));
                OnPropertyChanged(nameof(UsaEditorTextoPlano));
            }
        }
    }

    /// <summary>True cuando se muestra el Editor de texto plano (no la lista con checkboxes).</summary>
    public bool UsaEditorTextoPlano => !ModoListaCheckboxActivo;

    /// <summary>El ícono de lista solo aplica al crear una nota nueva, no al editar una guardada.</summary>
    public bool PuedeActivarModoLista => NotaSeleccionada == null;

    public ObservableCollection<NotaItemLista> ItemsListaCheck { get; } = new();

    // Nota actualmente seleccionada para edición
    private Nota _notaSeleccionada;
    public Nota NotaSeleccionada
    {
        get => _notaSeleccionada;
        set
        {
            if (_notaSeleccionada != value)
            {
                _notaSeleccionada = value;
                if (value != null)
                {
                    ModoListaCheckboxActivo = false;
                    ItemsListaCheck.Clear();
                }

                OnPropertyChanged(nameof(NotaSeleccionada));
                OnPropertyChanged(nameof(PuedeActivarModoLista));
                OnPropertyChanged(nameof(UsaEditorTextoPlano));
            }
        }
    }

    // Comandos
    public ICommand GuardarNotaCommand { get; }
    public ICommand EditarNotaCommand { get; }

    public NotasViewModel()
    {
        GuardarNotaCommand = new Command(GuardarNota);
        EditarNotaCommand = new Command<Nota>(EditarNota);

        CargarNotasDesdeArchivo();
    }

    private void GuardarNota()
    {
        var contenido = ModoListaCheckboxActivo
            ? ConstruirTextoDesdeListaCheck()
            : NotaEntryText;

        // No permitir guardar sin título
        if (string.IsNullOrWhiteSpace(TituloEntryText))
        {
            return;
        }

        var tituloBase = TituloEntryText.Trim();

        // Función local para obtener un título único con sufijos (2), (3), ...
        string ObtenerTituloUnico(string baseTitle, Nota notaActual)
        {
            var tituloFinal = baseTitle;
            var contador = 2;

            while (Notas.Any(n => !ReferenceEquals(n, notaActual) && n.Titulo == tituloFinal))
            {
                tituloFinal = $"{baseTitle} ({contador})";
                contador++;
            }

            return tituloFinal;
        }

        if (NotaSeleccionada != null)
        {
            // Si hay una nota seleccionada, actualizamos su contenido y título
            var tituloFinal = ObtenerTituloUnico(tituloBase, NotaSeleccionada);
            NotaSeleccionada.Titulo = tituloFinal;
            NotaSeleccionada.Contenido = contenido;
            NotaSeleccionada.Fecha = DateTime.Now;
            NotaSeleccionada = null; // limpiamos selección
        }
        else
        {
            // Si no hay nota seleccionada, creamos una nueva
            var tituloFinal = ObtenerTituloUnico(tituloBase, null);

            var nuevaNota = new Nota
            {
                Titulo = tituloFinal,
                Contenido = contenido,
                Fecha = DateTime.Now
            };
            Notas.Add(nuevaNota);
        }

        GuardarNotasEnArchivo();

        // Limpiamos los campos de entrada después de guardar
        NotaEntryText = string.Empty;
        TituloEntryText = string.Empty;
        ModoListaCheckboxActivo = false;
        ItemsListaCheck.Clear();
        OnPropertyChanged(nameof(PuedeActivarModoLista));
    }

    public void LimpiarBorradorContenido()
    {
        NotaEntryText = string.Empty;
        ItemsListaCheck.Clear();
        ModoListaCheckboxActivo = false;
    }

    public void AlternarModoListaCheck()
    {
        if (!PuedeActivarModoLista)
            return;

        if (!ModoListaCheckboxActivo)
        {
            ModoListaCheckboxActivo = true;
            ItemsListaCheck.Clear();

            if (!string.IsNullOrWhiteSpace(NotaEntryText))
            {
                foreach (var linea in NotaEntryText.Split(
                             new[] { "\r\n", "\n", "\r" },
                             StringSplitOptions.None))
                {
                    var t = linea.Trim();
                    if (t.Length > 0)
                        ItemsListaCheck.Add(new NotaItemLista { Texto = t });
                }
            }

            if (ItemsListaCheck.Count == 0)
            {
                ItemsListaCheck.Add(new NotaItemLista());
                ItemsListaCheck.Add(new NotaItemLista());
            }
        }
        else
        {
            NotaEntryText = string.Join(
                Environment.NewLine,
                ItemsListaCheck.Select(i => i.Texto?.Trim() ?? string.Empty)
                    .Where(s => s.Length > 0));
            ItemsListaCheck.Clear();
            ModoListaCheckboxActivo = false;
        }
    }

    private string ConstruirTextoDesdeListaCheck()
    {
        var lineas = new List<string>();
        foreach (var i in ItemsListaCheck)
        {
            var texto = i.Texto?.Trim() ?? string.Empty;
            if (string.IsNullOrEmpty(texto))
                continue;
            lineas.Add((i.Marcado ? "☑ " : "☐ ") + texto);
        }

        return string.Join(Environment.NewLine, lineas);
    }

    private void EditarNota(Nota nota)
    {
        if (nota != null)
        {
            NotaSeleccionada = nota;
            NotaEntryText = nota.Contenido; // cargamos el contenido en el Editor
            TituloEntryText = nota.Titulo;  // cargamos el título en el Entry
        }
    }

    public void EliminarNota(Nota nota)
    {
        if (nota != null && Notas.Contains(nota))
        {
            Notas.Remove(nota);
            GuardarNotasEnArchivo();
        }
    }

    private void CargarNotasDesdeArchivo()
    {
        try
        {
            if (File.Exists(_filePath))
            {
                var json = File.ReadAllText(_filePath);
                var lista = JsonSerializer.Deserialize<List<Nota>>(json);
                if (lista != null)
                {
                    Notas = new ObservableCollection<Nota>(lista);
                    OnPropertyChanged(nameof(Notas));
                }
            }
        }
        catch
        {
            // Si algo falla al leer/parsear, simplemente empezamos con lista vacía.
        }
    }

    private void GuardarNotasEnArchivo()
    {
        try
        {
            var opciones = new JsonSerializerOptions { WriteIndented = true };
            var json = JsonSerializer.Serialize(Notas, opciones);
            File.WriteAllText(_filePath, json);
        }
        catch
        {
            // En caso de error al guardar, no rompemos la app.
        }
    }

    // INotifyPropertyChanged
    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}

public class NotaItemLista : INotifyPropertyChanged
{
    private bool _marcado;
    public bool Marcado
    {
        get => _marcado;
        set
        {
            if (_marcado != value)
            {
                _marcado = value;
                OnPropertyChanged(nameof(Marcado));
            }
        }
    }

    private string _texto;
    public string Texto
    {
        get => _texto;
        set
        {
            if (_texto != value)
            {
                _texto = value;
                OnPropertyChanged(nameof(Texto));
            }
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}

public class Nota : INotifyPropertyChanged
{
    private string _titulo;
    public string Titulo
    {
        get => _titulo;
        set
        {
            if (_titulo != value)
            {
                _titulo = value;
                OnPropertyChanged(nameof(Titulo));
            }
        }
    }

    private string _contenido;
    public string Contenido
    {
        get => _contenido;
        set
        {
            if (_contenido != value)
            {
                _contenido = value;
                OnPropertyChanged(nameof(Contenido));
            }
        }
    }

    private DateTime _fecha = DateTime.Now;
    public DateTime Fecha
    {
        get => _fecha;
        set
        {
            if (_fecha != value)
            {
                _fecha = value;
                OnPropertyChanged(nameof(Fecha));
            }
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
