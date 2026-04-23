namespace MauiApp1.Views;

public partial class CreateNotes : ContentPage
{
    public CreateNotes()
    {
        InitializeComponent();
        BindingContext = MauiApp1.Services.ViewModelLocator.NotasViewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        
        // Si hay una nota seleccionada para edición, enfocar el campo apropiado
        if (BindingContext is MauiApp1.Models.NotasViewModel vm && vm.NotaSeleccionada != null)
        {
            if (vm.UsaEditorTextoPlano)
            {
                TextEditor.Focus();
            }
        }
    }

    private async void SaveButton_Clicked(object sender, EventArgs e)
    {
        if (BindingContext is MauiApp1.Models.NotasViewModel vm)
        {
            var estabaEditando = vm.NotaSeleccionada != null;

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
                AlarmPanel.IsVisible = false;

                if (estabaEditando)
                {
                    await Shell.Current.GoToAsync("//Notes");
                }
            }
        }
    }



    private void ClearButton_Clicked(object sender, EventArgs e)
    {
        if (BindingContext is MauiApp1.Models.NotasViewModel vm)
        {
            vm.LimpiarBorradorContenido();
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

    private void MostrarAlarmaToolbarItem_Clicked(object sender, EventArgs e)
    {
        if (BindingContext is MauiApp1.Models.NotasViewModel vm)
        {
            AlarmTimePicker.Time = vm.HoraAlarmaBorrador;
            AlarmDatePicker.Date = vm.FechaAlarmaBorrador;
        }

        AlarmPanel.IsVisible = !AlarmPanel.IsVisible;
    }

    private async void GuardarAlarma_Clicked(object sender, EventArgs e)
    {
        if (BindingContext is MauiApp1.Models.NotasViewModel vm)
        {
            vm.HoraAlarmaBorrador = AlarmTimePicker.Time;
            vm.FechaAlarmaBorrador = AlarmDatePicker.Date;
        }

        var estado = (BindingContext as MauiApp1.Models.NotasViewModel)?.AlarmaActivaBorrador == true
            ? $"Alarma colocada para {AlarmDatePicker.Date:dd/MM/yyyy} {AlarmDatePicker.Date.Add(AlarmTimePicker.Time):hh:mm tt}"
            : "Alarma desactivada para esta nota";

        await DisplayAlert("Alarma", estado, "OK");
    }
}