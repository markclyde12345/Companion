namespace project;

public partial class HeavyExhaustedValidationPage : ContentPage
{
    public HeavyExhaustedValidationPage()
    {
        InitializeComponent();
    }

    private async void OnBackTapped(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

    private async void OnSubmitActivityClicked(object sender, EventArgs e)
    {
        string body = BodyEntry.Text ?? "";
        string duration = DurationEntry.Text ?? "";
        string gentle = GentleEntry.Text ?? "";

        if (string.IsNullOrWhiteSpace(body) &&
            string.IsNullOrWhiteSpace(duration) &&
            string.IsNullOrWhiteSpace(gentle))
        {
            await DisplayAlert("Reminder", "Please fill in at least one field before submitting.", "OK");
            return;
        }

        await Navigation.PushAsync(new finalChecks("heavy"));
    }
}