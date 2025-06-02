using HindApp.Models;
using HindApp.Services;
using System.Collections.ObjectModel;
using System.Globalization;

namespace HindApp.Views.Admin;

public partial class StoreProductsPage : ContentPage
{
    private readonly Store _store;
    private readonly DatabaseService _dbService;
    private List<Product> _allProducts;
    private ObservableCollection<ProductPriceDisplay> _storeProducts;
    private readonly NavigationDataService _navigationDataService;

    public StoreProductsPage(DatabaseService dbService, NavigationDataService navigationDataService)
    {
        InitializeComponent();
        _navigationDataService = navigationDataService;
        _store = _navigationDataService.SelectedStore;
        _dbService = dbService;
        StoreTitle.Text = $"Kauplus: {_store.Name}";
        LoadProducts();
        
    }

    private async void LoadProducts()
    {
        _allProducts = await _dbService.GetAllProductsAsync();
        ProductPicker.ItemsSource = _allProducts;
        ProductPicker.ItemDisplayBinding = new Binding("Name");

        var prices = await _dbService.GetStoreProductPricesAsync(_store.Id);
        _storeProducts = new ObservableCollection<ProductPriceDisplay>(
            prices.Select(p => new ProductPriceDisplay
            {
                Id = p.Id,
                ProductName = _allProducts.FirstOrDefault(x => x.Id == p.ProductId)?.Name ?? "Неизвестно",
                Price = p.Price,
                LastUpdated = p.LastUpdated
            }));
        StoreProductList.ItemsSource = _storeProducts;
    }

    private async void OnAddProductToStoreClicked(object sender, EventArgs e)
    {
        if (ProductPicker.SelectedItem is not Product selectedProduct)
        {
            await DisplayAlert("Viga", "Vali toode", "OK");
            return;
        }

        string input = PriceEntry.Text.Trim().Replace(',', '.');

        if (!double.TryParse(input, NumberStyles.Any, CultureInfo.InvariantCulture, out double price))
        {
            await DisplayAlert("Viga", "Sisesta korrektne hind (nt 1.99)", "OK");
            return;
        }

        var productPrice = new ProductPrice
        {
            ProductId = selectedProduct.Id,
            StoreId = _store.Id,
            Price = price,
            LastUpdated = DateTime.Now
        };

        await _dbService.AddOrUpdateProductPriceAsync(productPrice);
        LoadProducts();
        ProductPicker.SelectedItem = null;
        PriceEntry.Text = "";
    }

    private async void OnRemoveProductClicked(object sender, EventArgs e)
    {
        if ((sender as Button)?.BindingContext is ProductPriceDisplay priceDisplay)
        {
            bool confirm = await DisplayAlert("Kinnitus", $"Kas tahate kustuta '{priceDisplay.ProductName}' poest?", "Jah", "Ei");
            if (confirm)
            {
                await _dbService.DeleteProductPriceAsync(priceDisplay.Id);
                LoadProducts();
            }
        }
    }

    class ProductPriceDisplay
    {
        public int Id { get; set; }
        public string ProductName { get; set; } = "";
        public double Price { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
