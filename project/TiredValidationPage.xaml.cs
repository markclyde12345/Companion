namespace project;

public partial class TiredValidationPage : ContentPage
{
    public TiredValidationPage()
    {
        InitializeComponent();
    }

    private async void OnBackTapped(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

    private async void OnSubmitActivityClicked(object sender, EventArgs e)
    {
        string tension = TensionEntry.Text ?? "";
        string drain = DrainEntry.Text ?? "";
        string rest = RestEntry.Text ?? "";

        if (string.IsNullOrWhiteSpace(tension) &&
            string.IsNullOrWhiteSpace(drain) &&
            string.IsNullOrWhiteSpace(rest))
        {
            await DisplayAlert("Reminder", "Please fill in at least one field before submitting.", "OK");
            return;
        }

        await Navigation.PushAsync(new finalChecks("tired"));
    }
}