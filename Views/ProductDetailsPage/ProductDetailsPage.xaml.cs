using HindApp.Models;
using HindApp.Services;
using System.Diagnostics;
using System.Globalization;

namespace HindApp.Views;

public partial class ProductDetailsPage : ContentPage
{
    private readonly Product _product;
    private readonly DatabaseService _databaseService;
    private readonly ProductComparisonService _comparisonService;
    private readonly NavigationDataService _navigationDataService;
    private readonly SessionService _sessionService;


    public ProductDetailsPage(NavigationDataService navigationDataService, DatabaseService databaseService, SessionService sessionService)
    {
        InitializeComponent();
        _navigationDataService = navigationDataService;
        _databaseService = databaseService;
        _product = _navigationDataService.SelectedProduct;
        _sessionService = sessionService;
        ProductTitle.Text = _product.Name;
        var db = databaseService;
        _comparisonService = new ProductComparisonService(db.GetConnection());
        LoadData();
    }

    private async void OnAddToFavoritesClicked(object sender, EventArgs e)
    {
        if (_sessionService.CurrentUser == null)
        {
            await DisplayAlert("Viga", "Te peate olema autoriseeritud", "ОК");
            return;
        }

        if (sender is not Button { CommandParameter: ProductPriceDisplayModel price }) return;

        var userId = _sessionService.CurrentUser.Id;
        var conn = _databaseService.GetConnection();

        try
        {
            var exists = await conn.ExecuteScalarAsync<int>(
                "SELECT COUNT(1) FROM Favorites WHERE UserId = ? AND ProductPricesId = ?", userId, price.Id);

            if (exists > 0)
            {
                await conn.ExecuteAsync("DELETE FROM Favorites WHERE UserId = ? AND ProductPricesId = ?", userId, price.Id);
                price.IsFavorite = false;
                await DisplayAlert("Info", "Kustutatud", "ОК");
            }
            else
            {
                var newFavorite = new Favorite
                {
                    UserId = userId,
                    ProductPricesId = price.Id,
                    AddedAt = DateTime.UtcNow
                };
                await conn.InsertAsync(newFavorite);
                price.IsFavorite = true;
                await DisplayAlert("Valmis", "Kaup on lisatud valitud", "ОК");
            }
        }
        catch
        {
            await DisplayAlert("Viga", "Väljavalitutesse ei õnnestunud lisada", "ОК");
        }
    }

    private async void LoadData(bool ascending = true)
    {
        var prices = await _comparisonService.GetProductPricesAsync(_product.Id, ascending);
        var conn = _databaseService.GetConnection();
        var userId = _sessionService.CurrentUser?.Id ?? -1;

        var favoriteIds = (await conn.Table<Favorite>()
            .Where(f => f.UserId == userId)
            .ToListAsync())
            .Select(f => f.ProductPricesId)
            .ToHashSet();

        var priceModels = prices.Select(p => new ProductPriceDisplayModel
        {
            Id = p.Id,
            StoreName = p.StoreName,
            Price = p.Price,
            LastUpdated = p.LastUpdated,
            IsFavorite = favoriteIds.Contains(p.Id)
        }).ToList();

        PriceList.ItemsSource = priceModels;
    }


    private void OnSortChanged(object sender, CheckedChangedEventArgs e)
    {
        LoadData(ascending: e.Value);
    }

}
public class FavoriteTextConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return (value is bool isFav && isFav) ? "Juba valitud" : "Vali";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
}
public class InverseBoolConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is bool b ? !b : false;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
}
