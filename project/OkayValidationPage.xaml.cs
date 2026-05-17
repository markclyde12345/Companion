using Microsoft.Maui.Controls;

namespace project;

public partial class OkayValidationPage : ContentPage
{
    public OkayValidationPage()
    {
        InitializeComponent();
    }

    private async void OnSubmitActivityClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(SeeEntry.Text) ||
            string.IsNullOrWhiteSpace(HearEntry.Text) ||
            string.IsNullOrWhiteSpace(FeelEntry.Text))
        {
            await DisplayAlert("Incomplete", "Please fill in all three observations.", "OK");
            return;
        }

        var activity = new
        {
            See = SeeEntry.Text,
            Hear = HearEntry.Text,
            Feel = FeelEntry.Text
        };

        SeeEntry.Text = string.Empty;
        HearEntry.Text = string.Empty;
        FeelEntry.Text = string.Empty;

        // Navigate back to home/mood check-in after saving
        await Navigation.PushAsync(new finalChecks("okay"));
    }

    
}
