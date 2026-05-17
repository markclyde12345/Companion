using Microsoft.Maui.Controls;

namespace project
{
    public partial class moodvalidate : ContentPage
    {
        public string Mood { get; private set; }

        public moodvalidate(string mood)
        {
            InitializeComponent();
            Mood = mood;
            SetMood(mood);
        }

        private void SetMood(string mood)
        {
            if (mood == "great")
            {
                MoodTitle.Text = "You're feeling great!";
                MoodEmoji.Text = "😊";
                MoodCircle.BackgroundColor = Colors.Green;
            }
            else if (mood == "okay")
            {
                MoodTitle.Text = "You're feeling okay";
                MoodEmoji.Text = "😐";
                MoodCircle.BackgroundColor = Colors.Orange;
            }
            else
            {
                MoodTitle.Text = "You're struggling";
                MoodEmoji.Text = "😟";
                MoodCircle.BackgroundColor = Colors.Red;
            }
        }

        private async void OnBackTapped(object sender, EventArgs e)
            => await Navigation.PopAsync();

        private async void OnSaveMemoryClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Joy1Entry.Text) ||
                string.IsNullOrWhiteSpace(Joy2Entry.Text) ||
                string.IsNullOrWhiteSpace(Joy3Entry.Text))
            {
                HighlightEmptyEntries();
                await DisplayAlert("Incomplete", "Please fill in all 3 joy moments.", "OK");
                return;
            }

            HighlightEmptyEntries();
            SaveButton.IsEnabled = false;

            try
            {
                var notes = $"Joy 1: {Joy1Entry.Text.Trim()}\n" +
                            $"Joy 2: {Joy2Entry.Text.Trim()}\n" +
                            $"Joy 3: {Joy3Entry.Text.Trim()}";

                if (!string.IsNullOrWhiteSpace(MemoryEditor.Text))
                    notes += $"\nReflection: {MemoryEditor.Text.Trim()}";

                // Save the mood entry to moods/{userId}
                await FirebaseService.SaveMoodAsync(
                    mood: Mood,
                    score: Mood == "great" ? 5 : Mood == "okay" ? 3 : 1,
                    notes: notes,
                    answers: new List<int>()
                );

                // +1 to users/{userId}/totalLogs in Firebase
                await FirebaseService.IncrementLogsAsync();

                await DisplayAlert("Saved", "Your happy memory has been saved!", "OK");
                await Navigation.PushAsync(new CompletionPage());
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[moodvalidate] Save error: {ex.Message}");
                await DisplayAlert("Error", "Something went wrong. Please try again.", "OK");
            }
            finally
            {
                SaveButton.IsEnabled = true;
            }
        }

        private void HighlightEmptyEntries()
        {
            Joy1Entry.BackgroundColor = string.IsNullOrWhiteSpace(Joy1Entry.Text)
                ? Color.FromArgb("#F8D7DA") : Colors.Transparent;
            Joy2Entry.BackgroundColor = string.IsNullOrWhiteSpace(Joy2Entry.Text)
                ? Color.FromArgb("#F8D7DA") : Colors.Transparent;
            Joy3Entry.BackgroundColor = string.IsNullOrWhiteSpace(Joy3Entry.Text)
                ? Color.FromArgb("#F8D7DA") : Colors.Transparent;
        }
    }
}