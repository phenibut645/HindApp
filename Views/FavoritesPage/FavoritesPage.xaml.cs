using HindApp.Models;
using HindApp.Services;
using SQLite;
using System.Collections.ObjectModel;

namespace HindApp.Views;

public partial class FavoritesPage : ContentPage
{
    private readonly DatabaseService _databaseService;
    private readonly SessionService _sessionService;
    private SQLiteAsyncConnection _db;
    private ObservableCollection<FavoriteViewModel> _favorites = new();

    public FavoritesPage(DatabaseService databaseService, SessionService sessionService)
    {
        InitializeComponent();
        _databaseService = databaseService;
        _sessionService = sessionService;
        _db = _databaseService.GetConnection();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadFavoritesAsync();
    }

    private async Task LoadFavoritesAsync()
    {
        var user = _sessionService.CurrentUser;
        if (user == null)
        {
            await DisplayAlert("Viga", "Kasutaja ei ole autoriseeritud", "OK");
            return;
        }

        var favorites = await _db.Table<Favorite>().Where(f => f.UserId == user.Id).ToListAsync();
        var productPriceIds = favorites.Select(f => f.ProductPricesId).ToList();
        var productPrices = await _db.Table<ProductPrice>().Where(pp => productPriceIds.Contains(pp.Id)).ToListAsync();
        var productIds = productPrices.Select(pp => pp.ProductId).Distinct().ToList();
        var storeIds = productPrices.Select(pp => pp.StoreId).Distinct().ToList();
        var products = await _db.Table<Product>().Where(p => productIds.Contains(p.Id)).ToListAsync();
        var stores = await _db.Table<Store>().Where(s => storeIds.Contains(s.Id)).ToListAsync();

        var favoriteViewModels = favorites.Select(fav =>
        {
            var pp = productPrices.FirstOrDefault(p => p.Id == fav.ProductPricesId);
            var product = products.FirstOrDefault(p => p.Id == pp?.ProductId);
            var store = stores.FirstOrDefault(s => s.Id == pp?.StoreId);
            return new FavoriteViewModel
            {
                FavoriteId = fav.Id,
                ProductName = product?.Name ?? string.Empty,
                StoreName = store?.Name ?? string.Empty,
                Price = pp?.Price ?? 0,
                LastUpdated = pp?.LastUpdated ?? DateTime.MinValue
            };
        }).ToList();

        _favorites = new ObservableCollection<FavoriteViewModel>(favoriteViewModels);
        FavoritesList.ItemsSource = _favorites;
    }

    private async void OnRemoveFavoriteClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.BindingContext is FavoriteViewModel fav)
        {
            var favorite = await _db.Table<Favorite>().Where(f => f.Id == fav.FavoriteId).FirstOrDefaultAsync();
            if (favorite != null)
            {
                await _db.DeleteAsync(favorite);
                _favorites.Remove(fav);
            }
        }
    }

    public class FavoriteViewModel
    {
        public int FavoriteId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string StoreName { get; set; } = string.Empty;
        public double Price { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
