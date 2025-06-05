using HindApp.Services;
using HindApp.Views;
using System.Diagnostics;

namespace HindApp;

public partial class App : Application
{
    public App(LoginView loginView)
    {
        Debug.WriteLine("deleted11");
        InitializeComponent();

        Debug.WriteLine("deleted12");
        MainPage = new NavigationPage(loginView);
        Debug.WriteLine("deleted13");
    }
}
