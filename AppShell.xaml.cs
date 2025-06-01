using HindApp.Views;

namespace HindApp
{
   public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute("search", typeof(ProductSearchPage));
        Routing.RegisterRoute("favorites", typeof(FavoritesPage));
        Routing.RegisterRoute("barcode", typeof(BarcodeScanPage));
    }
}

}
