using HindApp.Services;
using HindApp.Views;

namespace HindApp;

public partial class App : Application
{
    public App(LoginView loginView)
    {
        InitializeComponent();

        MainPage = new AppShell();
    }
}
