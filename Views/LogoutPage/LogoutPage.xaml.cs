using HindApp.Views;

namespace HindApp.Views;

public partial class LogoutPage : ContentPage
{
    private readonly SessionService _sessionService;
    private readonly LoginView _loginView;
    public LogoutPage(SessionService sessionService, LoginView loginView)
    {
        InitializeComponent();
        _sessionService = sessionService;
        _loginView = loginView;
    }

    private async void OnLogoutClicked(object sender, EventArgs e)
    {
        _sessionService.ClearUser();
        Application.Current.MainPage = new NavigationPage(_loginView);
    }
}
