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
        // Ofrece al usuario elegir entre editar en el mismo control o abrir la página "Edit"
        var action = await DisplayActionSheet("¿Editar nota?", "Cancelar", null, "Editar aquí", "Editar en nueva página");

        if (action == "Editar aquí")
        {
            // Lleva el foco al Editor para editar en el lugar
            TextEditor.Focus();
        }
        else if (action == "Editar en nueva página")
        {
            // Crea la página de edición y pasa el texto actual como BindingContext
            var editPage = new Edit
            {
                BindingContext = TextEditor.Text ?? string.Empty
            };

            await Navigation.PushAsync(editPage);
        }
    }


}