namespace MauiApp1.Views;

public partial class NotePage : ContentPage
{
    string _fileName = Path.Combine(FileSystem.AppDataDirectory, $"notes.txt");
    public NotePage()
    {
        InitializeComponent();

        if (File.Exists(_fileName))
            TextEditor.Text = File.ReadAllText(_fileName);

    }

    private async void SaveButton_Clicked(object sender, EventArgs e)
    {
        if (!string.IsNullOrWhiteSpace(TextEditor.Text))
        {
            //string fileName = Path.Combine(FileSystem.AppDataDirectory, $"{Guid.NewGuid()}.txt");
            string fileName = Path.Combine(FileSystem.AppDataDirectory, $"Notas_{DateTime.Now:yyyyMMdd_HHmmss}.txt");
            File.WriteAllText(fileName, TextEditor.Text);

            TextEditor.Text = string.Empty;
            await DisplayAlert("Aviso!", "Nota guardada", "OK");
        }
    }



    private void DeleteButton_Clicked(object sender, EventArgs e)
    {
        if (File.Exists(_fileName))
            File.Delete(_fileName);

        TextEditor.Text = string.Empty;
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
                vm.NotaSeleccionada = nota;
                vm.NotaEntryText = nota.Contenido;
            }
            TextEditor.Focus();
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
                vm.Notas.Remove(nota);
            }
        }


    }
}