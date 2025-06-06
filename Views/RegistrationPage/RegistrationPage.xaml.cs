using HindApp.Models;

namespace HindApp.Views;

public partial class RegistrationPage : ContentPage
{
	private readonly DatabaseService _database;
    private readonly LoginView _loginView;
    private readonly SessionService _sessionService;

        public RegistrationPage(DatabaseService database, LoginView loginView, SessionService sessionService)
        {
            InitializeComponent();
            _database = database;
            _loginView = loginView;
        _sessionService = sessionService;
        }

        private async void OnRegisterClicked(object sender, EventArgs e)
        {
            string username = UsernameEntry.Text?.Trim();
            string password = PasswordEntry.Text?.Trim();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                StatusLabel.Text = "Kõik väljad on kohustuslikud.";
                return;
            }

            var existingUser = await _database.GetConnection()
                .Table<User>()
                .Where(u => u.Username == username)
                .FirstOrDefaultAsync();

            if (existingUser != null)
            {
                StatusLabel.Text = "Kasutaja on juba olemas.";
                return;
            }

            var newUser = new User
            {
                Username = username,
                PasswordHash = password,
                IsAdmin = 0 
            };

            await _database.GetConnection().InsertAsync(newUser);
            await DisplayAlert("Edu", "Kasutaja registreeritud!", "OK");
            _sessionService.SetUser(newUser);
            Application.Current.MainPage = new AppShell();
        }

        private async void OnLoginClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
}