namespace MauiApp1.Views;

public partial class NotePage : ContentPage
{
    public NotePage()
    {
        InitializeComponent();
    }

    private async void SaveButton_Clicked(object sender, EventArgs e)
    {
        if (BindingContext is MauiApp1.Models.NotasViewModel vm)
        {
            if (string.IsNullOrWhiteSpace(vm.TituloEntryText))
            {
                await DisplayAlert("Advertencia",
                                   "No hay título, coloque un título y podrá guardar la nota",
                                   "OK");
                return;
            }

            if (vm.GuardarNotaCommand.CanExecute(null))
            {
                vm.GuardarNotaCommand.Execute(null);
                await DisplayAlert("Aviso", "Nota guardada", "OK");
            }
        }
    }



    private void DeleteButton_Clicked(object sender, EventArgs e)
    {
        if (BindingContext is MauiApp1.Models.NotasViewModel vm)
        {
            vm.LimpiarBorradorContenido();
        }
    }

    private async void EditButton_Clicked(object sender, EventArgs e)
    {
        var action = await DisplayActionSheet("Opcion", "Cancelar", null, "Editar nota", "Eliminar nota");

        if (action == "Editar nota")
        {
            if (BindingContext is MauiApp1.Models.NotasViewModel vm &&
                sender is Button button &&
                button.BindingContext is MauiApp1.Models.Nota nota)
            {
                vm.CargarNotaParaEdicion(nota);
            }

            if (BindingContext is MauiApp1.Models.NotasViewModel vmEdicion && vmEdicion.UsaEditorTextoPlano)
            {
                TextEditor.Focus();
            }
        }
        else if (action == "Cancelar")
        {

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

    private void ToggleEditorCheckboxToolbarItem_Clicked(object sender, EventArgs e)
    {
        if (BindingContext is MauiApp1.Models.NotasViewModel vm)
        {
            vm.AlternarModoListaCheck();
        }
    }

    private void AnadirLineaLista_Clicked(object sender, EventArgs e)
    {
        if (BindingContext is MauiApp1.Models.NotasViewModel vm)
        {
            vm.ItemsListaCheck.Add(new MauiApp1.Models.NotaItemLista());
        }
    }

    private void EliminarLineaLista_Clicked(object sender, EventArgs e)
    {
        if (BindingContext is MauiApp1.Models.NotasViewModel vm &&
            vm.ItemsListaCheck.Count > 0)
        {
            vm.ItemsListaCheck.RemoveAt(vm.ItemsListaCheck.Count - 1);
        }
    }
}