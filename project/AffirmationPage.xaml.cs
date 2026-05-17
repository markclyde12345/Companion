using System.Text.Json;
namespace project;

public partial class AffirmationPage : ContentPage
{
    readonly HttpClient client = new();
    readonly Random random = new();

    List<Quote> apiQuotes = new();

    readonly List<string> localBackup = new()
    {
        "You are enough.",
        "You are safe right now.",
        "You are growing at your own pace.",
        "You deserve peace.",
        "You are stronger than you think.",
        "Breathe. You are okay."
    };

    int index = 0;
    public AffirmationPage()
	{
		InitializeComponent();
        _ = LoadQuotesFromApi();
    }

    private async Task LoadQuotesFromApi()
    {
        try
        {
            string json = await client.GetStringAsync("https://type.fit/api/quotes");

            apiQuotes = JsonSerializer.Deserialize<List<Quote>>(json);

            ShowRandomFromApi();
        }
        catch
        {
            // fallback immediately
            ShowLocal();
        }
    }

    // 🌐 API MODE
    private void ShowRandomFromApi()
    {
        if (apiQuotes != null && apiQuotes.Count > 0)
        {
            var quote = apiQuotes[random.Next(apiQuotes.Count)];

            affirmationLabel.Text = quote.text;
        }
        else
        {
            ShowLocal();
        }
    }

    // 🏠 LOCAL MODE
    private void ShowLocal()
    {
        affirmationLabel.Text =
            localBackup[random.Next(localBackup.Count)];
    }

    // 🔁 NEXT BUTTON
    private async void OnNextClicked(object sender, EventArgs e)
    {
        await FadeOutIn();

        if (apiQuotes != null && apiQuotes.Count > 0)
            ShowRandomFromApi();
        else
            ShowLocal();
    }

    // ✨ CALM FADE ANIMATION
    private async Task FadeOutIn()
    {
        await card.FadeTo(0, 300);
        await card.ScaleTo(0.97, 300);

        await card.FadeTo(1, 300);
        await card.ScaleTo(1, 300);
    }

    // 🚪 CLOSE
    private async void OnCloseTapped(object sender, TappedEventArgs e)
    {
        await Navigation.PopAsync();
    }
}