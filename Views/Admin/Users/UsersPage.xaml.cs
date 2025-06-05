using HindApp.Models;
using SQLite;
using System.Collections.ObjectModel;

namespace HindApp.Views.Admin;

public partial class UsersPage : ContentPage
{
	private readonly DatabaseService _db;
    public ObservableCollection<User> Users { get; set; } = new();

    public UsersPage(DatabaseService dbService)
    {
        InitializeComponent();
        _db = dbService;
        BindingContext = this;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadUsersAsync();
    }

    private async Task LoadUsersAsync()
    {
        var users = await _db.GetAllUsersAsync();
        Users.Clear();
        foreach (var user in users)
            Users.Add(user);
    }

    private async void OnAddUserClicked(object sender, EventArgs e)
    {
        var user = new User
        {
            Username = UsernameEntry.Text,
            PasswordHash = PasswordEntry.Text, // Заменить на хеш!
            IsAdmin = IsAdminSwitch.IsToggled ? 1 : 0
        };

        try
        {
            await _db.AddUserAsync(user);
            await LoadUsersAsync();
            UsernameEntry.Text = string.Empty;
            PasswordEntry.Text = string.Empty;
            IsAdminSwitch.IsToggled = false;
        }
        catch (SQLiteException ex)
        {
            await DisplayAlert("Ошибка", ex.Message, "OK");
        }
    }

    private async void OnDeleteUser(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is User user)
        {
            await _db.DeleteUserAsync(user);
            await LoadUsersAsync();
        }
    }
}