namespace project;

public partial class finalChecks : ContentPage
{
    private readonly string _mood;

    public finalChecks(string mood)
    {
        InitializeComponent();
        _mood = mood;
        SetMoodContent(mood);
    }

    void SetMoodContent(string mood)
    {
        switch (mood)
        {
            case "great":
                IconCircle.BackgroundColor = Colors.Green;
                TitleLabel.Text = "You're shining today!";
                SubtitleLabel.Text = "You've captured what made today special. Keep that energy going!";
                InsightCard.BackgroundColor = Color.FromArgb("#D6F0D6");
                InsightCard.BorderColor = Color.FromArgb("#A0D8A0");
                InsightTitle.Text = "Happiness is worth celebrating.";
                InsightMessage.Text = "Reflecting on good moments helps your brain remember them more vividly.";
                CompleteButton.BackgroundColor = Colors.Green;
                break;

            case "okay":
                IconCircle.BackgroundColor = Color.FromArgb("#D4A017");
                TitleLabel.Text = "Great work!";
                SubtitleLabel.Text = "You've completed your mood validation activity.";
                InsightCard.BackgroundColor = Color.FromArgb("#FFF8E7");
                InsightCard.BorderColor = Color.FromArgb("#F0D080");
                InsightTitle.Text = "Okay days are still good days.";
                InsightMessage.Text = "Being present with a neutral mood is mindfulness in action.";
                CompleteButton.BackgroundColor = Color.FromArgb("#D4A017");
                break;

            case "anxious":
                IconCircle.BackgroundColor = Color.FromArgb("#E05555");
                TitleLabel.Text = "You did something brave.";
                SubtitleLabel.Text = "Facing your anxiety takes real courage. Be proud of yourself.";
                InsightCard.BackgroundColor = Color.FromArgb("#FDE8E8");
                InsightCard.BorderColor = Color.FromArgb("#F0B0B0");
                InsightTitle.Text = "Grounding helps more than you know.";
                InsightMessage.Text = "Each time you practice grounding, you train your mind to find calm faster.";
                CompleteButton.BackgroundColor = Color.FromArgb("#E05555");
                break;

            case "sad":
                IconCircle.BackgroundColor = Color.FromArgb("#5588CC");
                TitleLabel.Text = "You showed up for yourself.";
                SubtitleLabel.Text = "Expressing sadness is healthy and healing. You are not alone.";
                InsightCard.BackgroundColor = Color.FromArgb("#D6E8F5");
                InsightCard.BorderColor = Color.FromArgb("#A0C4E0");
                InsightTitle.Text = "Your feelings deserve to be heard.";
                InsightMessage.Text = "Writing about sadness can reduce its intensity and bring clarity.";
                CompleteButton.BackgroundColor = Color.FromArgb("#5588CC");
                break;

            case "tired":
                IconCircle.BackgroundColor = Color.FromArgb("#8888BB");
                TitleLabel.Text = "Rest is an achievement too.";
                SubtitleLabel.Text = "Acknowledging your fatigue is the first step to recovery.";
                InsightCard.BackgroundColor = Color.FromArgb("#EEEEF8");
                InsightCard.BorderColor = Color.FromArgb("#C0C0E0");
                InsightTitle.Text = "Your body deserves rest.";
                InsightMessage.Text = "Honoring your need to rest makes you more resilient in the long run.";
                CompleteButton.BackgroundColor = Color.FromArgb("#8888BB");
                break;

            case "heavy":
                IconCircle.BackgroundColor = Color.FromArgb("#9966BB");
                TitleLabel.Text = "You listened to your body.";
                SubtitleLabel.Text = "That took strength. Give yourself the gentleness you deserve.";
                InsightCard.BackgroundColor = Color.FromArgb("#E8D6F5");
                InsightCard.BorderColor = Color.FromArgb("#C8A8E8");
                InsightTitle.Text = "Healing starts with awareness.";
                InsightMessage.Text = "Noticing where your body holds heaviness helps you release it over time.";
                CompleteButton.BackgroundColor = Color.FromArgb("#9966BB");
                break;

            case "overwhelming":
                IconCircle.BackgroundColor = Color.FromArgb("#6655AA");
                TitleLabel.Text = "You took one small step.";
                SubtitleLabel.Text = "That is more than enough. One step at a time is all it takes.";
                InsightCard.BackgroundColor = Color.FromArgb("#D6DCF5");
                InsightCard.BorderColor = Color.FromArgb("#A8B0E8");
                InsightTitle.Text = "You are not alone in this.";
                InsightMessage.Text = "Reaching out or reflecting when overwhelmed is a powerful act of self-care.";
                CompleteButton.BackgroundColor = Color.FromArgb("#6655AA");
                break;

            default:
                // fallback
                IconCircle.BackgroundColor = Color.FromArgb("#3A8A3A");
                MoodEmoji.Text = "✓";
                TitleLabel.Text = "Great work!";
                SubtitleLabel.Text = "You've completed your mood validation activity.";
                InsightIcon.Text = "💚";
                InsightTitle.Text = "You did something meaningful today.";
                InsightMessage.Text = "Taking time to check in with yourself is an act of self-care.";
                break;
        }
    }

    private async void OnBackTapped(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

    private async void OnCompleteClicked(object sender, EventArgs e)
    {
        string thought = FinalThoughtEditor.Text ?? "";

        await DisplayAlert( 
            "Check-In Complete",
            "Your reflection has been saved. Take good care of yourself!",
            "OK");

        // TODO: Navigate to dashboard/history page when ready
        await Navigation.PushAsync(new CompletionPage());
    }
}