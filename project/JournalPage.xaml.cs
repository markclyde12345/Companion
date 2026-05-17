namespace project;

public partial class JournalPage : ContentPage
{
	public JournalPage()
	{
		InitializeComponent();
	}

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        await DisplayAlert("Saved", "Your thoughts have been recorded.", "OK");
        await Navigation.PopAsync();
    }

    private async void OnCloseTapped(object sender, TappedEventArgs e)
    {
        await Navigation.PopAsync();
    }
}