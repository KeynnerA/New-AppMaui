namespace MauiApp1.Views;

public partial class Edit : ContentPage
{
	public Edit()
	{
		InitializeComponent();
	}

    private async void LearnMore_Clicked(object sender, EventArgs e)
    {
		await Launcher.Default.OpenAsync("https://thecodercave.com");
    }
}