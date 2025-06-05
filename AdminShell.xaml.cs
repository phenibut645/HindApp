using HindApp.Views;
using HindApp.Views.Admin;
using System.Diagnostics;

namespace HindApp
{
    
	public partial class AdminShell : Shell
	{
		public AdminShell()
		{
			InitializeComponent();
			Debug.WriteLine("deleted12");
			Routing.RegisterRoute("stores", typeof(StoresPage));
			Debug.WriteLine("deleted12");
			Routing.RegisterRoute("products", typeof(ProductsPage));
			Debug.WriteLine("deleted12");
			Routing.RegisterRoute("storeproducts", typeof(StoreProductsPage));
			Debug.WriteLine("deleted12");
			Routing.RegisterRoute("categories", typeof(CategoriesPage));
			Routing.RegisterRoute("logout", typeof(LogoutPage));
			Routing.RegisterRoute("users", typeof(UsersPage));
		}
	}
}
