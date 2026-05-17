using Microsoft.Maui.Controls;

namespace project;

public partial class GoodValidationPage : ContentPage
{
    public GoodValidationPage()
    {
        InitializeComponent();
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        string reflection = ReflectionEditor.Text;

        if (string.IsNullOrWhiteSpace(reflection))
        {
            await DisplayAlert("Almost there!", "Please write something you're grateful for.", "OK");
            return;
        }
        await Navigation.PushAsync(new finalChecks("okay"));

    }

    
}