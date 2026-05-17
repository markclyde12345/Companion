namespace project;

public partial class BreathingPage : ContentPage
{
    bool running = false;
    public BreathingPage()
	{
		InitializeComponent();
        StartAmbient();
    }

    async void StartAmbient()
    {
        while (true)
        {
            await Task.WhenAll(
                star1.TranslateTo(0, -12, 4000),
                star2.TranslateTo(0, 10, 5000),
                star3.TranslateTo(0, -8, 4500)
            );

            await Task.WhenAll(
                star1.TranslateTo(0, 0, 4000),
                star2.TranslateTo(0, 0, 5000),
                star3.TranslateTo(0, 0, 4500)
            );
        }
    }

    private async void OnStartClicked(object sender, EventArgs e)
    {
        if (running) return;

        running = true;

        await startButton.FadeTo(0, 400);
        startButton.IsVisible = false;

        for (int round = 0; round < 4; round++)
        {
            await Inhale();
            await Hold();
            await Exhale();
        }

        phaseLabel.Text = "Peace";
        countLabel.Text = "You are calm now";

        running = false;
    }

    async Task Inhale()
    {
        phaseLabel.Text = "Inhale";
        countLabel.Text = "4";

        await Task.WhenAll(
            orb.ScaleTo(1.22, 4000, Easing.CubicInOut),
            glow.ScaleTo(1.35, 4000, Easing.CubicInOut),
            glow.FadeTo(0.9, 4000)
        );
    }

    async Task Hold()
    {
        phaseLabel.Text = "Hold";
        countLabel.Text = "4";
        await Task.Delay(4000);
    }

    async Task Exhale()
    {
        phaseLabel.Text = "Exhale";
        countLabel.Text = "4";

        await Task.WhenAll(
            orb.ScaleTo(1.0, 4000, Easing.CubicInOut),
            glow.ScaleTo(1.0, 4000, Easing.CubicInOut),
            glow.FadeTo(0.45, 4000)
        );
    }

    private async void OnCloseTapped(object sender, TappedEventArgs e)
    {
        if (running)
        {
            bool exit = await DisplayAlert(
                "Leave session?",
                "Your breathing session is still in progress.",
                "Leave",
                "Stay");

            if (!exit)
                return;
        }

        await Navigation.PopAsync();
    }
}