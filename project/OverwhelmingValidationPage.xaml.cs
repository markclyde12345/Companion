namespace project;

public partial class OverwhelmingValidationPage : ContentPage
{
    public OverwhelmingValidationPage()
    {
        InitializeComponent();
    }

    private async void OnBackTapped(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

    private async void OnSubmitActivityClicked(object sender, EventArgs e)
    {
        string overwhelm = OverwhelmEntry.Text ?? "";
        string support = SupportEntry.Text ?? "";
        string step = StepEntry.Text ?? "";

        if (string.IsNullOrWhiteSpace(overwhelm) &&
            string.IsNullOrWhiteSpace(support) &&
            string.IsNullOrWhiteSpace(step))
        {
            await DisplayAlert("Reminder", "Please fill in at least one field before submitting.", "OK");
            return;
        }

        await Navigation.PushAsync(new finalChecks("overwhelming"));
    }
}