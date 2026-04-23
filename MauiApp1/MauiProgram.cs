using Microsoft.Extensions.Logging;
using Plugin.LocalNotification;

namespace MauiApp1
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseLocalNotification(config =>
                {
                    config.AddCategory(new NotificationCategory(NotificationCategoryType.Reminder)
                    {
                        ActionList = new HashSet<NotificationAction>
                        {
                            new NotificationAction(App.AccionPosponer5MinId)
                            {
                                Title = "Posponer 5 min",
                                Android = { LaunchAppWhenTapped = false }
                            },
                            new NotificationAction(App.AccionMarcarHechaId)
                            {
                                Title = "Marcar como hecha",
                                Android = { LaunchAppWhenTapped = false }
                            }
                        }
                    });
                })
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
