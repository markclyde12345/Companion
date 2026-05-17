using project;

namespace project;

public partial class CompletionPage : ContentPage
{
    public CompletionPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Bounce the icon
        await CompletionIcon.ScaleTo(0.5, 1);
        await CompletionIcon.ScaleTo(1.2, 350, Easing.CubicOut);
        await CompletionIcon.ScaleTo(1.0, 150, Easing.CubicIn);

        // Fade in text
        await CompletionText.FadeTo(1, 500);
        await CompletionSubText.FadeTo(1, 400);

        // Wait then go to MainPage
        await Task.Delay(2500);

        await Navigation.PushAsync(new HomePage());
    }
}
