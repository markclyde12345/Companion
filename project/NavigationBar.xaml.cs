using Microsoft.Maui.Controls;
namespace project;


public partial class NavigationBar : ContentView
{
	public NavigationBar()
	{
		InitializeComponent();
	}

    private async void OnHomeTapped(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//HomePage");
    }

    private async void OnCheckInTapped(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//MoodCheckInPage");
    }

    private async void OnProgressTapped(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//ProgressPage");
    }

    private async void OnProfileTapped(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//ProfilePage");
    }
}