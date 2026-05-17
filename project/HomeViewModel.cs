using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.Maui.Storage;

namespace project;

public class HomeViewModel : INotifyPropertyChanged
{

    private bool _hasLoggedToday = false;

    public bool HasLoggedToday
    {
        get => _hasLoggedToday;
        set
        {
            _hasLoggedToday = value;
            OnPropertyChanged(nameof(HasLoggedToday));
            OnPropertyChanged(nameof(MoodStatusText));
        }
    }

    public string MoodStatusText
    {
        get
        {
            return HasLoggedToday
                ? "You have done enough for today"
                : "Log Your Mood";
        }
    }

    public HomeViewModel()
    {
        System.Diagnostics.Debug.WriteLine("HomeViewModel CREATED");
        _ = InitializeAsync();

        var savedDate = Preferences.Get("LastMoodLogDate", "");

        if (savedDate == DateTime.Today.ToString())
        {
            HasLoggedToday = true;
        }

    }

    private async Task InitializeAsync()
    {
        var savedDate = Preferences.Get("LastMoodLogDate", "");

        if (savedDate == DateTime.Today.ToString("yyyy-MM-dd"))
            HasLoggedToday = true;

        await LoadStreak();
    }

    public async Task RefreshMoodStatus()
    {
        HasLoggedToday = await FirebaseService.HasLoggedMoodTodayAsync();
    }


    private bool _isHeaderVisible;

    public bool IsHeaderVisible
    {
        get => _isHeaderVisible;
        set
        {
            _isHeaderVisible = value;
            OnPropertyChanged();
        }
    }

    public string TodayDate => DateTime.Now.ToString("dddd, MMMM dd");

    public async Task ShowHeaderTemporarily()
    {
        IsHeaderVisible = true;

        await Task.Delay(2000);

        IsHeaderVisible = false;
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string name = "")
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    private int _streak;

    public int Streak
    {
        get => _streak;
        set
        {
            _streak = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(StreakText));
        }
    }

    public async Task LoadStreak()
    {
        Streak = await FirebaseService.GetStreakAsync();
    }

    public string StreakText =>
    Streak == 1
        ? "1 Day Streak"
        : $"{Streak} Days Streak";

    private int _weeklyMainMoodCount;

    public int WeeklyMainMoodCount
    {
        get => _weeklyMainMoodCount;
        set
        {
            _weeklyMainMoodCount = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(WeeklyMoodText));
            OnPropertyChanged(nameof(WeeklyMoodProgress));
        }
    }

    public string WeeklyMoodText =>
        $"{WeeklyMainMoodCount}/7 Mood Logs This Week";

    public double WeeklyMoodProgress =>
        Math.Clamp(WeeklyMainMoodCount / 7.0, 0, 1);

    public async Task LoadWeeklyMood()
    {
        WeeklyMainMoodCount =
            await FirebaseService.GetWeeklyMainMoodCountAsync();
    }

    private int _weeklyTotalMoodCount;

    public int WeeklyTotalMoodCount
    {
        get => _weeklyTotalMoodCount;
        set
        {
            _weeklyTotalMoodCount = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(WeeklyTotalMoodText));
            OnPropertyChanged(nameof(WeeklyTotalMoodProgress)); // ← added
        }
    }

    public string WeeklyTotalMoodText =>
        $"{WeeklyTotalMoodCount} Total Mood Logs This Week";

    // ← added: progress out of 7 logs per week
    public double WeeklyTotalMoodProgress =>
        Math.Clamp(WeeklyTotalMoodCount / 7.0, 0, 1);

    public async Task LoadWeeklyTotalMood()
    {
        var firebaseMoods = await FirebaseService.GetMoodsAsync();

        int daysSinceMonday = ((int)DateTime.Today.DayOfWeek + 6) % 7;
        var weekStart = DateTime.Today.AddDays(-daysSinceMonday);
        var tomorrow = DateTime.Today.AddDays(1);

        int weeklyCount = firebaseMoods.Count(e =>
            DateTime.TryParse(e.CreatedAt, out var d) &&
            d.ToLocalTime().Date >= weekStart &&
            d.ToLocalTime().Date < tomorrow);

        WeeklyTotalMoodCount = weeklyCount;
    }

    public string CopingProgressText =>
    CopingProgressService.GetProgressText();

    public double CopingProgress =>
        CopingProgressService.GetProgressValue();


}