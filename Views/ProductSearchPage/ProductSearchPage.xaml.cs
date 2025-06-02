using HindApp.Services;
using HindApp.Models;
using System.Diagnostics;

namespace HindApp.Views;

public partial class ProductSearchPage : ContentPage
{
    private readonly ProductSearchService _searchService;
    private readonly DatabaseService _databaseService;

    public ProductSearchPage(DatabaseService databaseService)
    {
        InitializeComponent();
        _databaseService = databaseService;
        _searchService = new ProductSearchService(_databaseService.GetConnection());
    }

    private async void OnSearchClicked(object sender, EventArgs e)
    {
        var query = SearchEntry.Text?.Trim() ?? "";
        int limit = int.TryParse(LimitEntry.Text, out var l) ? l : 10;
        
        var results = await _searchService.SearchProductsAsync(query, limit);
        Debug.WriteLine("==============Result===============");
        foreach(var result in results)
        {
            Debug.WriteLine(result.Name);
        }
        Debug.WriteLine("===================================");
        ProductsList.ItemsSource = results;

        List<Product> products = await _databaseService.GetAllProductsAsync();
    }

    private async void OnProductSelected(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is Product selectedProduct)
        {
            await Navigation.PushAsync(new ProductDetailsPage(selectedProduct, _databaseService));
            ProductsList.SelectedItem = null;
        }
    }
}
