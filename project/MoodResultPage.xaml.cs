using Microsoft.Maui.Controls;

namespace project;

public partial class MoodResultPage : ContentPage
{
    private readonly int _score;
    private readonly int _maxScore;

    public MoodResultPage(int score, int maxScore)
    {
        InitializeComponent();
        _score = score;
        _maxScore = maxScore;
        ShowResult();
    }

    private void ShowResult()
    {
        ScoreLabel.Text = $"{_score} / {_maxScore}";

        string mood, description, emoji;

        // Score brackets (out of 15 max, or proportional if questions were skipped)
        double percent = _maxScore > 0 ? (double)_score / _maxScore : 0;

        if (percent >= 0.87)       // 13–15
        {
            mood = "Great";
            description = "You're feeling wonderful! Keep up whatever you're doing.";
            emoji = "🌟";
        }
        else if (percent >= 0.67)  // 10–12
        {
            mood = "Good";
            description = "You're doing well overall. A few things on your mind, but manageable.";
            emoji = "😊";
        }
        else if (percent >= 0.47)  // 7–9
        {
            mood = "Okay";
            description = "You're getting by. It might help to take a short break and breathe.";
            emoji = "😐";
        }
        else if (percent >= 0.27)  // 4–6
        {
            mood = "Low";
            description = "Things feel heavy right now. Be kind to yourself — you don't have to push through alone.";
            emoji = "😔";
        }
        else                        // 0–3
        {
            mood = "Overwhelming";
            description = "It sounds like you're going through a really tough time. Please reach out to someone you trust.";
            emoji = "💙";
        }

        MoodLabel.Text = $"{emoji}  {mood}";
        DescriptionLabel.Text = description;
    }

    private async void OnDoneClicked(object sender, EventArgs e)
    {
        await Navigation.PopToRootAsync();
    }
}