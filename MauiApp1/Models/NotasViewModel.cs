using System.Collections.ObjectModel;
using System.Windows.Input;

namespace MauiApp1.Models;

public class NotasViewModel
{
    // Colección observable que se enlaza a la UI
    public ObservableCollection<Nota> Notas { get; set; } = new ObservableCollection<Nota>();

    // Comando para guardar una nueva nota
    public ICommand GuardarNotaCommand { get; }

    public NotasViewModel()
    {
        GuardarNotaCommand = new Command<string>(GuardarNota);
    }

    private void GuardarNota(string contenido)
    {
        var nuevaNota = new Nota
        {
            Titulo = "Nueva Nota",
            Contenido = contenido
        };

        Notas.Add(nuevaNota);
    }
}
    public class Nota
    {
        public string Titulo { get; set; }
        public string Contenido { get; set; }
        public DateTime Fecha { get; set; } = DateTime.Now;
    }

