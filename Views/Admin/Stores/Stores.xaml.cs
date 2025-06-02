using HindApp.Models;
using HindApp.Services;
using System.Collections.ObjectModel;

namespace HindApp.Views.Admin;

public partial class StoresPage : ContentPage
{
    private readonly DatabaseService _dbService;
    private ObservableCollection<Store> _stores = new();
    private readonly NavigationDataService _navigationDataService;

    public StoresPage(DatabaseService dbService, NavigationDataService navigationDataService)
    {
        InitializeComponent();
        _dbService = dbService;
        LoadStores();
        _navigationDataService = navigationDataService;
    }

    private async void LoadStores()
    {
        var list = await _dbService.GetAllStoresAsync();
        _stores = new ObservableCollection<Store>(list);
        StoreList.ItemsSource = _stores;
    }

    private async void OnAddStoreClicked(object sender, EventArgs e)
    {
        string name = StoreNameEntry.Text?.Trim();
        string location = StoreLocationEntry.Text?.Trim();

        if (string.IsNullOrEmpty(name))
        {
            await DisplayAlert("Viga", "Sisesta poe nimi", "Œ ");
            return;
        }

        var newStore = new Store { Name = name, Location = location };
        await _dbService.AddStoreAsync(newStore);
        _stores.Add(newStore);
        StoreNameEntry.Text = "";
        StoreLocationEntry.Text = "";
    }

    private async void OnDeleteStoreClicked(object sender, EventArgs e)
    {
        if ((sender as Button)?.BindingContext is Store store)
        {
            bool confirm = await DisplayAlert("Kinnitus", $"Kas te tahate kustuta pood '{store.Name}'?", "Jah", "Ei");
            if (confirm)
            {
                await _dbService.DeleteStoreAsync(store);
                _stores.Remove(store);
            }
        }
    }

    private async void OnStoreSelected(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is Store selectedStore)
        {
            _navigationDataService.SelectedStore = selectedStore;
            await Shell.Current.GoToAsync("storeproducts");
            StoreList.SelectedItem = null;
        }
    }
}
