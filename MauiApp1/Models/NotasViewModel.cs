using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace MauiApp1.Models;

public class NotasViewModel : INotifyPropertyChanged
{
    // Colección observable que se enlaza a la UI
    public ObservableCollection<Nota> Notas { get; set; } = new ObservableCollection<Nota>();

    // Texto que se enlaza al Entry
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
                OnPropertyChanged(nameof(NotaSeleccionada));
            }
        }
    }

    // Comandos
    public ICommand GuardarNotaCommand { get; }
    public ICommand EditarNotaCommand { get; }

    public NotasViewModel()
    {
        GuardarNotaCommand = new Command<string>(GuardarNota);
        EditarNotaCommand = new Command<Nota>(EditarNota);
    }

    private void GuardarNota(string contenido)
    {
        if (NotaSeleccionada != null)
        {
            // Si hay una nota seleccionada, actualizamos su contenido
            NotaSeleccionada.Contenido = contenido;
            NotaSeleccionada.Fecha = DateTime.Now;
            NotaSeleccionada = null; // limpiamos selección
        }
        else
        {
            // Si no hay nota seleccionada, creamos una nueva
            var nuevaNota = new Nota
            {
                Titulo = "Nueva Nota",
                Contenido = contenido,
                Fecha = DateTime.Now
            };
            Notas.Add(nuevaNota);
        }
    }

    private void EditarNota(Nota nota)
    {
        if (nota != null)
        {
            NotaSeleccionada = nota;
            NotaEntryText = nota.Contenido; // cargamos el contenido en el Entry
        }
    }

    // INotifyPropertyChanged
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
