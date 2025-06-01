using HindApp.Services;
using Microsoft.Maui.Controls;

namespace HindApp.Views
{
    public partial class LoginView : ContentPage
    {
        private readonly DatabaseService _databaseService;

        public LoginView(DatabaseService databaseService)
        {
            InitializeComponent();
            _databaseService = databaseService;
        }

        private async void OnLoginClicked(object sender, EventArgs e)
        {
            //string username = UsernameEntry.Text?.Trim();
            //string password = PasswordEntry.Text;

            //if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            //{
            //    await DisplayAlert("������", "������� ��� ������������ � ������", "OK");
            //    return;
            //}

            //// ����� ����� ���� �������� ������������ �� ��
            //var conn = _databaseService.GetConnection();
            //var users = await conn.Table<Models.User>()
            //                      .Where(u => u.Username == username && u.PasswordHash == password)
            //                      .ToListAsync();

            //if (users.Count > 0)
            //{
            //    // �������� ����
            //    await Shell.Current.GoToAsync("//main");
            //}
            //else
            //{
            //    await DisplayAlert("������", "�������� ������", "OK");
            //}
            await Shell.Current.GoToAsync("//main");
        }
    }
}
