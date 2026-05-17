namespace project;

public partial class StrugglingValidationPage : ContentPage
{
    public StrugglingValidationPage()
    {
        InitializeComponent();
    }

    private async void OnBackTapped(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

    private async void OnSubmitReflectionClicked(object sender, EventArgs e)
    {
        string smallHelp = SmallHelpEntry.Text ?? "";
        string important = ImportantEditor.Text ?? "";

        if (string.IsNullOrWhiteSpace(smallHelp) &&
            string.IsNullOrWhiteSpace(important))
        {
            await DisplayAlert("Reminder", "Please fill in at least one field before submitting.", "OK");
            return;
        }

        await Navigation.PushAsync(new finalChecks("struggling"));
    }
}