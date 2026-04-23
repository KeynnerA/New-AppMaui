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
                // Navegar a CreateNotes para editar
                await Navigation.PushAsync(new MauiApp1.Views.CreateNotes());
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

}