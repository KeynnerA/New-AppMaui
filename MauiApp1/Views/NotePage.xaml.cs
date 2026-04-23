namespace MauiApp1.Views;

public partial class NotePage : ContentPage
{
    public NotePage()
    {
        InitializeComponent();
        BindingContext = MauiApp1.Services.ViewModelLocator.NotasViewModel;
    }

    private async void EditButton_Clicked(object sender, EventArgs e)
    {
        var action = await DisplayActionSheet("Opción", "Cancelar", null, "Editar nota", "Eliminar nota");

        if (action == "Editar nota")
        {
            if (BindingContext is MauiApp1.Models.NotasViewModel vm &&
                sender is Button button &&
                button.BindingContext is MauiApp1.Models.Nota nota)
            {
                vm.CargarNotaParaEdicion(nota);
                // Cambia al tab existente de CreateNotes sin apilar una nueva pagina
                await Shell.Current.GoToAsync("//CreateNotes");
            }
        }
        else if (action == "Eliminar nota")
        {
            if (BindingContext is MauiApp1.Models.NotasViewModel vm &&
                sender is Button button &&
                button.BindingContext is MauiApp1.Models.Nota nota)
            {
                vm.EliminarNota(nota);
            }
        }
    }

    private async void SortToolbarItem_Clicked(object sender, EventArgs e)
    {
        var action = await DisplayActionSheet(
            "Ordenar notas",
            "Cancelar",
            null,
            "Por prioridad",
            "Por fecha de creacion");

        if (BindingContext is not MauiApp1.Models.NotasViewModel vm)
        {
            return;
        }

        if (action == "Por prioridad")
        {
            vm.OrdenarNotasPorPrioridad();
        }
        else if (action == "Por fecha de creacion")
        {
            vm.OrdenarNotasPorFechaCreacion();
        }
    }

}