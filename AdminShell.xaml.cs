using HindApp.Views;
using HindApp.Views.Admin;

namespace HindApp
{
    
	public partial class AdminShell : Shell
	{
		public AdminShell()
		{
			InitializeComponent();

			Routing.RegisterRoute("stores", typeof(StoresPage));
			Routing.RegisterRoute("products", typeof(ProductsPage));
			Routing.RegisterRoute("storeproducts", typeof(StoreProductsPage));
			Routing.RegisterRoute("categories", typeof(CategoriesPage));
			Routing.RegisterRoute("logout", typeof(LogoutPage));
		}
	}
}
