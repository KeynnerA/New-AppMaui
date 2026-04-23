using Plugin.LocalNotification;
using Plugin.LocalNotification.EventArgs;
using MauiApp1.Services;

namespace MauiApp1
{
    public partial class App : Application
    {
        public const int AccionPosponer5MinId = 5001;
        public const int AccionMarcarHechaId = 5002;

        public App()
        {
            InitializeComponent();
            LocalNotificationCenter.Current.NotificationActionTapped += OnNotificationActionTapped;
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }

        private async void OnNotificationActionTapped(NotificationActionEventArgs e)
        {
            var notaId = e.Request?.ReturningData;
            if (string.IsNullOrWhiteSpace(notaId))
            {
                return;
            }

            var vm = ViewModelLocator.NotasViewModel;

            if (e.ActionId == AccionPosponer5MinId)
            {
                vm.PosponerAlarma(notaId, 5);
                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    var page = Current?.Windows.FirstOrDefault()?.Page;
                    if (page != null)
                    {
                        await page.DisplayAlert("Alarma", "Alarma pospuesta 5 minutos.", "OK");
                    }
                });
            }
            else if (e.ActionId == AccionMarcarHechaId)
            {
                vm.MarcarNotaComoHecha(notaId);
                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    var page = Current?.Windows.FirstOrDefault()?.Page;
                    if (page != null)
                    {
                        await page.DisplayAlert("Alarma", "Nota marcada como hecha.", "OK");
                    }
                });
            }
        }
    }
}