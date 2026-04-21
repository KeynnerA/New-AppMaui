namespace MauiApp1.Views;

public partial class CreateNotes : ContentPage
{
	public CreateNotes()
	{
		InitializeComponent();
	}

    private async void LearnMore_Clicked(object sender, EventArgs e)
    {
		await Launcher.Default.OpenAsync("https://thecodercave.com");
    }
}