using HindApp.Views;

namespace HindApp
{
   public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute("main", typeof(ProductSearchPage));
            Routing.RegisterRoute("favorites", typeof(FavoritesPage));
            Routing.RegisterRoute("barcode", typeof(BarcodeScannerPage));
            Routing.RegisterRoute("ProductDetailsPage", typeof(ProductDetailsPage));
            Routing.RegisterRoute("logout", typeof(LogoutPage));
        }
    }

}
