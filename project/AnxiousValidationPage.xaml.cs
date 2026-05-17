namespace project;

public partial class AnxiousValidationPage : ContentPage
{
    public AnxiousValidationPage()
    {
        InitializeComponent();
    }

    private async void OnBackTapped(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

    private async void OnSubmitActivityClicked(object sender, EventArgs e)
    {
        string trigger = TriggerEntry.Text ?? "";
        string control = ControlEntry.Text ?? "";
        string calm = CalmEntry.Text ?? "";

        if (string.IsNullOrWhiteSpace(trigger) &&
            string.IsNullOrWhiteSpace(control) &&
            string.IsNullOrWhiteSpace(calm))
        {
            await DisplayAlert("Reminder", "Please fill in at least one field before submitting.", "OK");
            return;
        }

        await Navigation.PushAsync(new finalChecks("anxious"));
    }
}