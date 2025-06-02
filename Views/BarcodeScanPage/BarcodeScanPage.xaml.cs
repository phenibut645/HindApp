using ZXing.Net.Maui;

namespace HindApp.Views;

public partial class BarcodeScannerPage : ContentPage
{
    public BarcodeScannerPage()
    {
        InitializeComponent();
    }

    private void OnBarcodeDetected(object sender, BarcodeDetectionEventArgs e)
    {
        var result = e.Results.FirstOrDefault()?.Value;

        if (!string.IsNullOrEmpty(result))
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await DisplayAlert("Skännitud", $"Kood: {result}", "OK");

            });
        }
    }
}
