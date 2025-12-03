namespace MauiApp1.Views;

public partial class Thanks : ContentPage
{
	public Thanks()
	{
		InitializeComponent();
	}

    private async void LearnMore_Clicked(object sender, EventArgs e)
    {
		await Launcher.Default.OpenAsync("https://thecodercave.com");
    }
}