using Firebase.Database;

namespace project;

public partial class CopingToolsPage : ContentPage
{

    
private int _totalExercises = 4;

public string CopingProgressText { get; set; } = "0/4 completed";
public string CopingLog { get; set; } = "";
    private string selectedActivity = "";
    public CopingToolsPage()
	{
		InitializeComponent();
        BindingContext = this;
    }

    private async void OnBackTapped(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//HomePage");
    }

    private async Task ShowPopup(string image, string title, string desc, string duration)
    {
        // Fill content first
        PopupImage.Source = image;
        PopupTitle.Text = title;
        PopupDescription.Text = desc;
        PopupDuration.Text = duration;

        // Reset state EVERY OPEN
        ActivityPopup.IsVisible = true;

        ActivityPopup.Opacity = 0;

        PopupCard.Opacity = 1;
        PopupCard.Scale = 0.82;
        PopupCard.TranslationY = 25;

        // Animate
        await Task.WhenAll(
            ActivityPopup.FadeTo(1, 180, Easing.CubicOut),

            PopupCard.ScaleTo(1, 220, Easing.SpringOut),
            PopupCard.TranslateTo(0, 0, 220, Easing.CubicOut)
        );
    }

    private async void OnMeditationTapped(object sender, EventArgs e)
    {
        selectedActivity = "Meditation";

        await ShowPopup(
            "meditation_banner.jpg",
            "Guided Meditation",
            "10-minute calming session",
            "Duration: 10 mins");
    }

    private async void OnBreathingTapped(object sender, EventArgs e)
    {
        selectedActivity = "Breathing";

        await ShowPopup(
            "breathing.jpg",
            "Breathing Exercise",
            "Box breathing technique",
            "Duration: 3 mins");
    }

    private async void OnJournalingTapped(object sender, EventArgs e)
    {
        selectedActivity = "Journal";

        await ShowPopup(
            "journal.jpg",
            "Journaling Prompt",
            "Reflect on your thoughts",
            "Duration: 5 mins");
    }

    private async void OnAffirmationTapped(object sender, EventArgs e)
    {
        selectedActivity = "Affirmation";

        await ShowPopup(
            "confidence.jpg",
            "Positive Affirmations",
            "Boost your confidence",
            "Duration: 2 mins");
    }

    private async void ClosePopup(object sender, EventArgs e)
    {
        await Task.WhenAll(
            ActivityPopup.FadeTo(0, 150),
            PopupCard.ScaleTo(0.85, 150),
            PopupCard.TranslateTo(0, 20, 150)
        );

        ActivityPopup.IsVisible = false;
    }

    private async void StartActivity(object sender, EventArgs e)
    {
        await StartButton.ScaleTo(0.96, 70);
        await StartButton.ScaleTo(1, 70);

        await ClosePopupNow();

        CompleteExercise(selectedActivity);

        await FirebaseService.SaveCopingProgressAsync(
            CopingProgressService.CompletedExercises,
            CopingProgressService.TotalExercises
        );

        switch (selectedActivity)
        {
            case "Meditation":
                await Navigation.PushAsync(new VideoPage());
                break;

            case "Breathing":
                await Navigation.PushAsync(new BreathingPage());
                break;

            case "Journal":
                await Navigation.PushAsync(new JournalPage());
                break;

            case "Affirmation":
                await Navigation.PushAsync(new AffirmationPage());
                break;
        }
    }
    private void CompleteExercise(string activityName)
    {
        CopingProgressService.CompletedExercises++;

        CopingProgressText =
            $"{CopingProgressService.CompletedExercises}/{CopingProgressService.TotalExercises} completed";

        OnPropertyChanged(nameof(CopingProgressText));

        CopingLog += $"{activityName} completed\n";
    }



    private async Task ClosePopupNow()
    {
        await ActivityPopup.FadeTo(0, 150);
        ActivityPopup.IsVisible = false;
    }
}