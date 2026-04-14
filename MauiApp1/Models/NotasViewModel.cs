using System.Collections.Generic;
using System.Collections.Specialized;
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
    private string _notaEntryText = string.Empty;
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
    private string _tituloEntryText = string.Empty;
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

    /// <summary>Permite alternar a modo lista con checkboxes tanto al crear como al editar.</summary>
    public bool PuedeActivarModoLista => true;

    public ObservableCollection<NotaItemLista> ItemsListaCheck { get; } = new();

    // Nota actualmente seleccionada para edición
    private Nota? _notaSeleccionada;
    public Nota? NotaSeleccionada
    {
        get => _notaSeleccionada;
        set
        {
            if (_notaSeleccionada != value)
            {
                _notaSeleccionada = value;
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
        ConfigurarSincronizacionNotas();
    }

    private void ConfigurarSincronizacionNotas()
    {
        foreach (var nota in Notas)
        {
            SuscribirNota(nota);
        }

        Notas.CollectionChanged += Notas_CollectionChanged;
    }

    private void Notas_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.NewItems != null)
        {
            foreach (var item in e.NewItems.OfType<Nota>())
            {
                SuscribirNota(item);
            }
        }

        if (e.OldItems != null)
        {
            foreach (var item in e.OldItems.OfType<Nota>())
            {
                item.PropertyChanged -= Nota_PropertyChanged;
            }
        }
    }

    private void SuscribirNota(Nota nota)
    {
        nota.PropertyChanged -= Nota_PropertyChanged;
        nota.PropertyChanged += Nota_PropertyChanged;
    }

    private void Nota_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(Nota.Contenido) || e.PropertyName == nameof(Nota.Titulo))
        {
            GuardarNotasEnArchivo();
        }
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
        string ObtenerTituloUnico(string baseTitle, Nota? notaActual)
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
            var notaEnEdicion = NotaSeleccionada;
            var tituloFinal = ObtenerTituloUnico(tituloBase, notaEnEdicion);
            notaEnEdicion.Titulo = tituloFinal;
            notaEnEdicion.Contenido = contenido;
            notaEnEdicion.Fecha = DateTime.Now;
            NotaSeleccionada = null; // limpiamos selección
            OnPropertyChanged(nameof(Notas)); // fuerza refresco visual cuando el contenido queda vacío
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

    private void EditarNota(Nota? nota)
    {
        if (nota != null)
        {
            CargarNotaParaEdicion(nota);
        }
    }

    public void CargarNotaParaEdicion(Nota nota)
    {
        NotaSeleccionada = nota;
        TituloEntryText = nota.Titulo;

        if (IntentarCargarContenidoComoLista(nota.Contenido))
        {
            // Ya quedó cargado en ItemsListaCheck.
            return;
        }

        ModoListaCheckboxActivo = false;
        ItemsListaCheck.Clear();
        NotaEntryText = nota.Contenido;
    }

    private bool IntentarCargarContenidoComoLista(string contenido)
    {
        var lineas = contenido.Split(new[] { "\r\n", "\n", "\r" }, StringSplitOptions.None);
        var items = new List<NotaItemLista>();
        var tieneMarcadores = false;

        foreach (var linea in lineas)
        {
            var t = linea.Trim();
            if (t.Length == 0)
            {
                continue;
            }

            if (t.StartsWith("☑ "))
            {
                tieneMarcadores = true;
                items.Add(new NotaItemLista { Marcado = true, Texto = t[2..].Trim() });
            }
            else if (t.StartsWith("☐ "))
            {
                tieneMarcadores = true;
                items.Add(new NotaItemLista { Marcado = false, Texto = t[2..].Trim() });
            }
            else
            {
                items.Add(new NotaItemLista { Marcado = false, Texto = t });
            }
        }

        if (!tieneMarcadores)
        {
            return false;
        }

        ItemsListaCheck.Clear();
        foreach (var item in items)
        {
            ItemsListaCheck.Add(item);
        }

        if (ItemsListaCheck.Count == 0)
        {
            ItemsListaCheck.Add(new NotaItemLista());
        }

        NotaEntryText = string.Empty;
        ModoListaCheckboxActivo = true;
        return true;
    }

    public void EliminarNota(Nota? nota)
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
    public event PropertyChangedEventHandler? PropertyChanged;
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

    private string _texto = string.Empty;
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

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}

public class Nota : INotifyPropertyChanged
{
    private bool _sincronizandoContenido;
    private bool _sincronizandoLineas;

    private string _titulo = string.Empty;
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

    private string _contenido = string.Empty;
    public string Contenido
    {
        get => _contenido;
        set
        {
            if (_contenido != value)
            {
                _contenido = value;
                if (!_sincronizandoLineas)
                {
                    SincronizarLineasDesdeContenido();
                }
                OnPropertyChanged(nameof(Contenido));
            }
        }
    }

    private bool _tieneFormatoChecklist;
    public bool TieneFormatoChecklist
    {
        get => _tieneFormatoChecklist;
        private set
        {
            if (_tieneFormatoChecklist != value)
            {
                _tieneFormatoChecklist = value;
                OnPropertyChanged(nameof(TieneFormatoChecklist));
                OnPropertyChanged(nameof(MostrarTextoPlano));
            }
        }
    }

    public bool MostrarTextoPlano => !TieneFormatoChecklist;

    public ObservableCollection<NotaLineaVisual> LineasVisuales { get; } = new();

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

    public Nota()
    {
        SincronizarLineasDesdeContenido();
    }

    private void SincronizarLineasDesdeContenido()
    {
        if (_sincronizandoContenido)
            return;

        _sincronizandoContenido = true;
        try
        {
            var lineas = _contenido.Split(new[] { "\r\n", "\n", "\r" }, StringSplitOptions.None);
            var items = new List<NotaLineaVisual>();
            var tieneMarcadores = false;

            foreach (var linea in lineas)
            {
                var t = linea.Trim();
                if (string.IsNullOrEmpty(t))
                    continue;

                if (t.StartsWith("☑ "))
                {
                    tieneMarcadores = true;
                    items.Add(new NotaLineaVisual(t[2..].Trim(), true, ReconstruirContenidoDesdeLineas));
                }
                else if (t.StartsWith("☐ "))
                {
                    tieneMarcadores = true;
                    items.Add(new NotaLineaVisual(t[2..].Trim(), false, ReconstruirContenidoDesdeLineas));
                }
                else
                {
                    items.Add(new NotaLineaVisual(t, false, ReconstruirContenidoDesdeLineas));
                }
            }

            LineasVisuales.Clear();
            if (tieneMarcadores)
            {
                foreach (var item in items)
                {
                    LineasVisuales.Add(item);
                }
            }

            TieneFormatoChecklist = tieneMarcadores;
        }
        finally
        {
            _sincronizandoContenido = false;
        }
    }

    private void ReconstruirContenidoDesdeLineas()
    {
        if (_sincronizandoContenido)
            return;

        _sincronizandoLineas = true;
        try
        {
            Contenido = string.Join(
                Environment.NewLine,
                LineasVisuales
                    .Where(l => !string.IsNullOrWhiteSpace(l.Texto))
                    .Select(l => (l.Marcado ? "☑ " : "☐ ") + l.Texto.Trim()));
        }
        finally
        {
            _sincronizandoLineas = false;
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}

public class NotaLineaVisual : INotifyPropertyChanged
{
    private readonly Action _onChanged;
    private bool _marcado;
    private string _texto;

    public NotaLineaVisual(string texto, bool marcado, Action onChanged)
    {
        _texto = texto;
        _marcado = marcado;
        _onChanged = onChanged;
    }

    public bool Marcado
    {
        get => _marcado;
        set
        {
            if (_marcado != value)
            {
                _marcado = value;
                OnPropertyChanged(nameof(Marcado));
                _onChanged();
            }
        }
    }

    public string Texto
    {
        get => _texto;
        set
        {
            if (_texto != value)
            {
                _texto = value;
                OnPropertyChanged(nameof(Texto));
                _onChanged();
            }
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
