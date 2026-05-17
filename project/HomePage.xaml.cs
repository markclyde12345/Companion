using System.Text.Json;
namespace project;

public partial class HomePage : ContentPage
{
    private bool _isMiniChatOpen = false;
    private readonly HttpClient _httpClient = new HttpClient();
    private CancellationTokenSource _quoteCts;
    public HomePage()
	{
		InitializeComponent();
        BindingContext = new HomeViewModel();
    }



    public class ZenQuote
    {
        public string q { get; set; }
        public string a { get; set; }
    }


    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is HomeViewModel vm)
        {
            await vm.RefreshMoodStatus();
            await vm.LoadStreak();
            await vm.LoadWeeklyMood();
            await vm.LoadWeeklyTotalMood();

            await FirebaseService.ResetCopingProgressIfNewWeekAsync(); // ← added

            var (completed, total) = await FirebaseService.GetCopingProgressAsync();
            CopingProgressService.CompletedExercises = completed;
            CopingProgressService.TotalExercises = total;
            CopingProgressBar.Progress = (double)completed / total;
            CopingProgressLabel.Text = $"{completed}/{total} completed";
        }

        // Header animation — separate concern, early return won't affect data above
        if (string.IsNullOrEmpty(FirebaseService.CurrentUserId))
        {
            HeaderCard.IsVisible = false;
            // DO NOT return here anymore
        }
        else
        {
            await Task.Delay(50);
            HeaderCard.TranslationY = -HeaderCard.Height - 20;
            await HeaderCard.TranslateTo(0, 0, 600, Easing.CubicOut);
            await Task.Delay(3000);
            await HeaderCard.TranslateTo(0, -HeaderCard.Height, 600, Easing.CubicIn);
            HeaderCard.IsVisible = false;
        }

        _quoteCts = new CancellationTokenSource();
        _ = StartQuoteLoop(_quoteCts.Token);
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        _quoteCts?.Cancel();
        _quoteCts?.Dispose();
        _quoteCts = null;
    }
    private async Task StartQuoteLoop(CancellationToken token)
{
    while (!token.IsCancellationRequested)
    {
        await LoadQuoteAsync();

        try
        {
            await Task.Delay(TimeSpan.FromSeconds(30), token);
        }
        catch (TaskCanceledException)
        {
            break;
        }
    }
}

    private async Task LoadQuoteAsync()
    {
        try
        {
            var response = await _httpClient.GetStringAsync("https://zenquotes.io/api/random");

            var quoteData = JsonSerializer.Deserialize<List<ZenQuote>>(response);

            if (quoteData != null && quoteData.Count > 0)
            {
                QuoteLabel.Text = $"“{quoteData[0].q}”\n— {quoteData[0].a}";
            }
        }
        catch
        {
            QuoteLabel.Text = "You are capable of amazing things.";
        }
    }

    private async void OnPointerEntered(object sender, PointerEventArgs e)
    {
        if (sender is Border border &&
            (DeviceInfo.Current.Platform == DevicePlatform.WinUI ||
             DeviceInfo.Current.Platform == DevicePlatform.MacCatalyst))
        {
            border.AbortAnimation("hover");
            await border.ScaleTo(1.05, 120, Easing.CubicOut);
            border.Stroke = Color.FromArgb("#1560bd");
            border.BackgroundColor = Color.FromArgb("#F5F7FA");
        }
    }

    private async void OnPointerExited(object sender, PointerEventArgs e)
    {
        if (sender is Border border &&
            (DeviceInfo.Current.Platform == DevicePlatform.WinUI ||
             DeviceInfo.Current.Platform == DevicePlatform.MacCatalyst))
        {
            border.AbortAnimation("hover");
            await border.ScaleTo(1.0, 120, Easing.CubicIn);
            border.Stroke = Colors.LightGray;
            border.BackgroundColor = Colors.White;
        }
    }

    private async void OnPressed(object sender, EventArgs e)
    {
        if (sender is Border border)
        {
            border.AbortAnimation("press");
            await border.ScaleTo(0.96, 80, Easing.CubicOut);
        }
    }

    private async void OnReleased(object sender, EventArgs e)
    {
        if (sender is Border border)
        {
            await border.ScaleTo(1.0, 80, Easing.CubicIn);
        }
    }

    private async void OnCopingTapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new CopingToolsPage());
    }

    private async void OnGoToCopingToolsClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new GetHelpPage());
    }

    private async void OnChatBubbleTapped(object sender, EventArgs e)
    {
        await Navigation.PushModalAsync(new ChatBot());
    }

    private async void OnCheckInClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new MoodCheckInPage());
    }

}
