using Microsoft.Extensions.Logging;
using HindApp.Services;
using HindApp.Views;
using System.Diagnostics;
using HindApp.Views.Admin;
using ZXing.Net.Maui.Controls;

namespace HindApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            builder
                .UseMauiApp<App>()
                .UseBarcodeReader()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            string dbPath = Path.Combine(FileSystem.AppDataDirectory, "app.db");
            if (false)
            {
                File.Delete(dbPath);
                Debug.WriteLine("deleted");
            }
            Debug.WriteLine(FileSystem.AppDataDirectory);
            var databaseService = new DatabaseService(dbPath);
            _ = databaseService.InitializeAsync();

            var sessionService = new SessionService();

            builder.Services.AddSingleton(databaseService);
            builder.Services.AddSingleton(sessionService);
            builder.Services.AddSingleton<NavigationDataService>();
            builder.Services.AddSingleton<LoginView>();
            builder.Services.AddTransient<ProductSearchPage>();
            builder.Services.AddTransient<ProductDetailsPage>();
            builder.Services.AddSingleton<AppShell>();
            builder.Services.AddSingleton<AdminShell>();
            builder.Services.AddTransient<FavoritesPage>();
            builder.Services.AddTransient<ProductsPage>();
            builder.Services.AddTransient<StoresPage>();
            builder.Services.AddTransient<StoreProductsPage>();
            builder.Services.AddTransient<CategoriesPage>();
            builder.Services.AddTransient<LogoutPage>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
