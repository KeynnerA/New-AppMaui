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
            string fileName = Path.Combine(FileSystem.AppDataDirectory, $"{Guid.NewGuid()}.txt");
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

    private void EditButton_Clicked(object sender, EventArgs e) 
    {
        
    }
}