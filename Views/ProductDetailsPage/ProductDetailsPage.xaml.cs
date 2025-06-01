using HindApp.Models;
using HindApp.Services;

namespace HindApp.Views;

public partial class ProductDetailsPage : ContentPage
{
    private readonly Product _product;
    private readonly ProductComparisonService _comparisonService;

    public ProductDetailsPage(Product product, DatabaseService databaseService)
    {
        InitializeComponent();
        _product = product;
        ProductTitle.Text = _product.Name;

        var db = databaseService;
        _comparisonService = new ProductComparisonService(db.GetConnection());

        LoadData();
    }

    private async void LoadData(bool ascending = true)
    {
        var prices = await _comparisonService.GetProductPricesAsync(_product.Id, ascending);
        PriceList.ItemsSource = prices;
    }

    private void OnSortChanged(object sender, CheckedChangedEventArgs e)
    {
        LoadData(ascending: e.Value);
    }
}
