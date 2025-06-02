using Microsoft.Extensions.Logging;
using HindApp.Services;
using HindApp.Views;
using System.Diagnostics;

namespace HindApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            string dbPath = Path.Combine(FileSystem.AppDataDirectory, "app.db");
            Debug.WriteLine(FileSystem.AppDataDirectory);
            var databaseService = new DatabaseService(dbPath);
            _ = databaseService.InitializeAsync();

            builder.Services.AddSingleton(databaseService);

            builder.Services.AddSingleton<LoginView>();
            builder.Services.AddTransient<ProductSearchPage>();
            builder.Services.AddSingleton<AppShell>();
            builder.Services.AddSingleton<App>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
