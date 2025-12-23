namespace MauiApp1.Views;

public partial class NotePage : ContentPage
{
	string _fileName = Path.Combine(FileSystem.AppDataDirectory, "notes.txt");
	public NotePage()
	{
		InitializeComponent();

		if (File.Exists(_fileName))
			TextEditor.Text = File.ReadAllText(_fileName);
		
	}

    public class Nota
    {
        public string Titulo { get; set; }
        public string Contenido { get; set; }
        public DateTime Fecha { get; set; } = DateTime.Now;
    }


    private async void SaveButton_Clicked(object sender, EventArgs e)
    {
		File.WriteAllText(_fileName, TextEditor.Text);
        TextEditor.Text = string.Empty;
        await DisplayAlert("Aviso!", "Nota guardada", "OK");
    }
	

    private void DeleteButton_Clicked(object sender, EventArgs e)
    {
		if (File.Exists(_fileName))
			File.Delete(_fileName);

		TextEditor.Text = string.Empty;
    }
}