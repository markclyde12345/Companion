namespace project;

public partial class SadValidationPage : ContentPage
{
    public SadValidationPage()
    {
        InitializeComponent();
    }

    private async void OnBackTapped(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

    private async void OnSubmitActivityClicked(object sender, EventArgs e)
    {
        string cause = CauseEntry.Text ?? "";
        string gratitude = GratitudeEntry.Text ?? "";
        string kindness = KindnessEntry.Text ?? "";

        if (string.IsNullOrWhiteSpace(cause) &&
            string.IsNullOrWhiteSpace(gratitude) &&
            string.IsNullOrWhiteSpace(kindness))
        {
            await DisplayAlert("Reminder", "Please fill in at least one field before submitting.", "OK");
            return;
        }

        await Navigation.PushAsync(new finalChecks("sad"));
    }
}