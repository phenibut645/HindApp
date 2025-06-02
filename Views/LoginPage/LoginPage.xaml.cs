using HindApp.Services;
using Microsoft.Maui.Controls;

namespace HindApp.Views
{
    public partial class LoginView : ContentPage
    {
        private readonly DatabaseService _databaseService;
        private readonly SessionService _sessionService;
        private readonly IServiceProvider _services;

        public LoginView(DatabaseService databaseService, SessionService sessionService, IServiceProvider services)
        {
            InitializeComponent();
            _databaseService = databaseService;
            _sessionService = sessionService;
            _services = services;
        }

        private async void OnLoginClicked(object sender, EventArgs e)
        {
            string username = UsernameEntry.Text.Trim();
            string password = PasswordEntry.Text.Trim();

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                await DisplayAlert("Viga", "Sisesta kasutajanimi ja parool.", "OK");
                return;
            }

            var user = await _databaseService.GetUserByUsernameAndPasswordAsync(username, password);

            if (user == null)
            {
                await DisplayAlert("Viga", "Vale kasutajanimi või parool.", "OK");
                return;
            }

            _sessionService.SetUser(user);

            if (user.IsAdmin == 1)
            {
                Application.Current.MainPage = new AdminShell();
            }
            else
            {
                Application.Current.MainPage = new AppShell();
            }
        }
    }
}
