using System.Threading.Tasks;

namespace project;

public partial class AnalyzingPage : ContentPage
{
    public AnalyzingPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Simulate loading / analyzing
        await Task.Delay(3000);

        await DisplayAlert("Analysis Complete", "Your mood has been analyzed.", "OK");

        // Navigate to MoodCheckInPage after loading
        await Navigation.PushAsync(new moodvalidate("great"));
    }
}