using System.Windows.Input;

namespace project;

public partial class GetHelpPage : ContentPage
{
	public GetHelpPage()
	{
		InitializeComponent();
        BindingContext = this;
    }

    private async void OnBackTapped(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//HomePage");
    }

    public ICommand OpenUrlCommand => new Command<string>(async (url) =>
    {
        await Launcher.Default.OpenAsync(url);
    });

    private async void OnCallEmergencyClicked(object sender, EventArgs e)
    {
        bool confirm = await DisplayAlert(
            "Emergency Call",
            "Do you want to call 911?",
            "Call",
            "Cancel");

        if (!confirm) return;

        try
        {
            if (PhoneDialer.Default.IsSupported)
                PhoneDialer.Default.Open("911");
            else
                await DisplayAlert("Not supported", "Dialer not available", "OK");
        }
        catch
        {
            await DisplayAlert("Error", "Unable to make call", "OK");
        }
    }
}