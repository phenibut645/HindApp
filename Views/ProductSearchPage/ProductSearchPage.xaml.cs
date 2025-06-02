using HindApp.Services;
using HindApp.Models;
using System.Diagnostics;

namespace HindApp.Views;

public partial class ProductSearchPage : ContentPage
{
    private readonly ProductSearchService _searchService;
    private readonly DatabaseService _databaseService;
    private readonly NavigationDataService _navigationDataService;

    public ProductSearchPage(DatabaseService databaseService, NavigationDataService navigationDataService)
    {
        InitializeComponent();
        _databaseService = databaseService;
        _searchService = new ProductSearchService(_databaseService.GetConnection());
        _navigationDataService = navigationDataService;
    }

    private async void OnSearchClicked(object sender, EventArgs e)
    {
        var query = SearchEntry.Text;
        int limit = int.TryParse(LimitEntry.Text, out var l) ? l : 50;
        var selectedCategory = CategoryPicker.SelectedItem as Category;
        int? categoryId = selectedCategory?.Id;

        var results = await _searchService.SearchProductsAsync(query, limit, categoryId);
        ProductsList.ItemsSource = results;
    }


    private async void OnProductSelected(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is Product selectedProduct)
        {
            _navigationDataService.SelectedProduct = selectedProduct;
            try
            {
                await Shell.Current.GoToAsync("ProductDetailsPage");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Navigation error: {ex.Message}", "OK");
            }
            ProductsList.SelectedItem = null;
        }
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        var categories = await _databaseService.GetAllCategoriesAsync();
        CategoryPicker.ItemsSource = categories;
    }

}
