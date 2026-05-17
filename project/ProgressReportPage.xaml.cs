using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Layouts;
using System;
using System.Collections.Generic;
using System.Linq;

// Aliases to avoid ambiguity between the two MoodEntry classes
using LocalMoodEntry = project.MoodEntry;
using FirebaseMoodEntry = MoodEntry;

namespace project
{
    public class MoodEntryDisplay
    {
        public string Emoji { get; set; }
        public string MoodLabel { get; set; }
        public string DateText { get; set; }
        public string ScoreText { get; set; }
    }

    public partial class ProgressReportPage : ContentPage
    {
        private int _currentDays = 7;
        private const int MaxScorePerSession = 15;

        // ── Firebase mood cache ────────────────────────────────
        private List<FirebaseMoodEntry> _cachedFirebaseMoods = new();

        public ProgressReportPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _ = LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            try
            {
                var allEntries = project.MoodEntry.MoodScoreStorage.GetAllEntries();

                var firebaseMoods = await FirebaseService.GetMoodsAsync();
                _cachedFirebaseMoods = firebaseMoods;

                // ── Mood Log Count (this week) ──────────────────────
                int daysSinceMonday = ((int)DateTime.Today.DayOfWeek + 6) % 7;
                var weekStart = DateTime.Today.AddDays(-daysSinceMonday);
                var tomorrow = DateTime.Today.AddDays(1);

                int weeklyMoodCount = firebaseMoods.Count(e =>
                {
                    if (!DateTime.TryParse(e.CreatedAt, out var d)) return false;
                    var local = d.ToLocalTime().Date;
                    return local >= weekStart && local < tomorrow;
                });
                MoodLogCountLabel.Text = weeklyMoodCount.ToString();

                // ── Activities ──────────────────────────────────────
                int attempts = await FirebaseService.GetTotalAttemptsAsync();
                ActivitiesLabel.Text = attempts.ToString();

                // ── Avg Mood ────────────────────────────────────────
                var emojiLabel = this.FindByName<Label>("AvgMoodEmoji");
                var avgLabel = this.FindByName<Label>("AvgMoodLabel");
                var subLabel = this.FindByName<Label>("AvgMoodSubLabel");

                if (firebaseMoods.Any())
                {
                    double avgRating = firebaseMoods
                        .Average(e => (double)e.MoodScore / MaxScorePerSession * 5.0);
                    avgRating = Math.Round(avgRating, 1);

                    string emoji, moodLabel;
                    if (avgRating >= 4.5) { emoji = "🌟"; moodLabel = "Great"; }
                    else if (avgRating >= 3.5) { emoji = "😊"; moodLabel = "Good"; }
                    else if (avgRating >= 2.5) { emoji = "😐"; moodLabel = "Okay"; }
                    else if (avgRating >= 1.5) { emoji = "😔"; moodLabel = "Low"; }
                    else { emoji = "💙"; moodLabel = "Hard"; }

                    if (emojiLabel != null) emojiLabel.Text = emoji;
                    if (avgLabel != null) avgLabel.Text = $"{avgRating:F1}/5";
                    if (subLabel != null) subLabel.Text = moodLabel;
                }
                else
                {
                    if (emojiLabel != null) emojiLabel.Text = "😊";
                    if (avgLabel != null) avgLabel.Text = "—";
                    if (subLabel != null) subLabel.Text = "";
                }

                GenerateInsight(allEntries);
                DrawChart(_currentDays);
                DrawMonthlyChart();
                LoadRecentEntries(allEntries);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ProgressReportPage] LoadDataAsync error: {ex}");
            }
        }

        private void GenerateInsight(List<LocalMoodEntry> allEntries)
        {
            if (!allEntries.Any())
            {
                InsightLabel.Text = "Complete your first check-in to see insights!";
                return;
            }
            if (allEntries.Count == 1)
            {
                InsightLabel.Text = "Great start! Keep checking in daily to track your mood trends.";
                return;
            }

            var last3 = allEntries.Where(e => e.Date >= DateTime.Now.AddDays(-3)).ToList();
            var prev3 = allEntries.Where(e => e.Date >= DateTime.Now.AddDays(-6)
                                           && e.Date < DateTime.Now.AddDays(-3)).ToList();

            if (last3.Any() && prev3.Any())
            {
                double avgLast = last3.Average(e => (double)e.Score / e.MaxScore * 5);
                double avgPrev = prev3.Average(e => (double)e.Score / e.MaxScore * 5);
                double diff = avgLast - avgPrev;
                double pct = Math.Abs(diff / avgPrev * 100);

                if (diff > 0)
                    InsightLabel.Text = $"Your mood improved by {pct:F0}% in the last 3 days! Keep it up 🌟";
                else if (diff < 0)
                    InsightLabel.Text = $"Your mood dipped {pct:F0}% recently. Try a short walk 💙";
                else
                    InsightLabel.Text = "Your mood has been steady. Consistency is great!";
            }
            else if (last3.Any())
            {
                double avgScore = last3.Average(e => (double)e.Score / e.MaxScore * 5);
                string lbl = avgScore >= 4 ? "great" : avgScore >= 3 ? "okay" : "challenging";
                InsightLabel.Text = $"Your average mood is {avgScore:F1}/5 — feeling {lbl}. Keep checking in!";
            }
            else
            {
                InsightLabel.Text = "Keep checking in daily to unlock mood insights!";
            }
        }

        private void DrawChart(int days)
        {
            while (ChartCanvas.Children.Count > 6)
                ChartCanvas.Children.RemoveAt(6);

            ChartCanvas.GestureRecognizers.Clear();

            // ── Use UTC cutoff to avoid timezone filtering issues ──
            var cutoff = DateTime.UtcNow.AddDays(-days);
            var entries = _cachedFirebaseMoods
                .Where(e =>
                {
                    if (!DateTime.TryParse(e.CreatedAt, out var d)) return false;
                    return d.ToUniversalTime() >= cutoff;
                })
                .OrderBy(e => e.CreatedAt)
                .ToList();

            var avgBadge = this.FindByName<Border>("ChartAvgBadge");
            var avgEmoji = this.FindByName<Label>("ChartAvgEmoji");
            var avgTxt = this.FindByName<Label>("ChartAvgLabel");

            if (!entries.Any())
            {
                EmptyChartLabel.IsVisible = true;
                ChartTooltip.IsVisible = false;
                if (avgBadge != null) avgBadge.IsVisible = false;
                return;
            }

            EmptyChartLabel.IsVisible = false;

            double sevenDayAvg = entries.Average(e => (double)e.MoodScore / MaxScorePerSession * 5.0);
            sevenDayAvg = Math.Round(sevenDayAvg, 1);

            string badgeEmoji = sevenDayAvg >= 4.5 ? "🌟"
                              : sevenDayAvg >= 3.5 ? "😊"
                              : sevenDayAvg >= 2.5 ? "😐"
                              : sevenDayAvg >= 1.5 ? "😔" : "💙";

            if (avgBadge != null) avgBadge.IsVisible = true;
            if (avgEmoji != null) avgEmoji.Text = badgeEmoji;
            if (avgTxt != null) avgTxt.Text = $"{sevenDayAvg:F1}/5";

            // ── Dashed avg line ────────────────────────────────────
            double avgPct = (sevenDayAvg - 1.0) / 4.0 * 100.0;
            double avgLineY = (100 - avgPct) / 100.0 * 160;

            const double segOn = 8, segOff = 5, cw = 300;
            double x = 0;
            while (x < cw)
            {
                double x2 = Math.Min(x + segOn, cw);
                var seg = new Line
                {
                    X1 = x,
                    Y1 = avgLineY,
                    X2 = x2,
                    Y2 = avgLineY,
                    Stroke = new SolidColorBrush(Color.FromArgb("#FF9500")),
                    StrokeThickness = 1.5
                };
                AbsoluteLayout.SetLayoutBounds(seg, new Rect(0, 0, AbsoluteLayout.AutoSize, AbsoluteLayout.AutoSize));
                AbsoluteLayout.SetLayoutFlags(seg, AbsoluteLayoutFlags.None);
                ChartCanvas.Children.Insert(ChartCanvas.Children.Count - 1, seg);
                x += segOn + segOff;
            }

            // ── Daily slots ────────────────────────────────────────
            var slots = new List<(string label, double? score)>();
            for (int i = days - 1; i >= 0; i--)
            {
                var day = DateTime.Now.AddDays(-i).Date;
                var dayEntries = entries
                    .Where(e =>
                    {
                        if (!DateTime.TryParse(e.CreatedAt, out var d)) return false;
                        return d.ToLocalTime().Date == day;
                    })
                    .ToList();

                double? avg = dayEntries.Any()
                    ? dayEntries.Average(e => (double)e.MoodScore / MaxScorePerSession * 5.0)
                    : (double?)null;

                slots.Add((day.ToString("ddd"), avg));
            }

            UpdateXAxisLabels(slots.Select(s => s.label).ToList());

            int total = slots.Count;
            const double chartHeight = 160;
            const double chartWidth = 300;

            // ── Only include slots that have data ─────────────────
            var pointsWithIndex = slots
                .Select((s, i) => new { s.score, index = i })
                .Where(p => p.score != null)
                .ToList();

            // ── Connecting lines (spans across empty days) ────────
            for (int p = 0; p < pointsWithIndex.Count - 1; p++)
            {
                var a = pointsWithIndex[p];
                var b = pointsWithIndex[p + 1];

                double xA = total == 1 ? 0.5 : (double)a.index / (total - 1);
                double xB = total == 1 ? 0.5 : (double)b.index / (total - 1);
                double yA = (5 - a.score.Value) / 4.0 * chartHeight;
                double yB = (5 - b.score.Value) / 4.0 * chartHeight;

                var line = new Line
                {
                    X1 = xA * chartWidth,
                    Y1 = yA,
                    X2 = xB * chartWidth,
                    Y2 = yB,
                    Stroke = new SolidColorBrush(Color.FromArgb("#5BC8D0")),
                    StrokeThickness = 2
                };
                AbsoluteLayout.SetLayoutBounds(line, new Rect(0, 0, AbsoluteLayout.AutoSize, AbsoluteLayout.AutoSize));
                AbsoluteLayout.SetLayoutFlags(line, AbsoluteLayoutFlags.None);
                ChartCanvas.Children.Insert(ChartCanvas.Children.Count - 1, line);
            }

            // ── Dots ──────────────────────────────────────────────
            for (int i = 0; i < total; i++)
            {
                if (slots[i].score == null) continue;

                double score = slots[i].score.Value;
                double y = (5 - score) / 4.0 * chartHeight - 6;
                double xProp = total == 1 ? 0.5 : (double)i / (total - 1);
                bool isHigh = score >= 4.5;
                double size = isHigh ? 14 : 11;
                string dayLabel = slots[i].label;

                double capturedScore = score;
                double capturedX = xProp;
                double capturedY = y;

                var dot = new Ellipse
                {
                    Fill = new SolidColorBrush(Color.FromArgb("#5BC8D0")),
                    WidthRequest = size,
                    HeightRequest = size,
                    Stroke = new SolidColorBrush(Colors.White),
                    StrokeThickness = 2
                };

                var tap = new TapGestureRecognizer();
                tap.Tapped += (s, e) =>
                {
                    ChartTooltipLabel.Text = $"{dayLabel}: {capturedScore:F1}/5";
                    double tooltipX = Math.Clamp(capturedX * chartWidth - 40, 0, chartWidth - 90);
                    double tooltipY = Math.Max(capturedY - 36, 0);
                    AbsoluteLayout.SetLayoutBounds(ChartTooltip, new Rect(tooltipX, tooltipY, 90, 28));
                    AbsoluteLayout.SetLayoutFlags(ChartTooltip, AbsoluteLayoutFlags.None);
                    ChartTooltip.IsVisible = true;
                };
                dot.GestureRecognizers.Add(tap);

                AbsoluteLayout.SetLayoutBounds(dot, new Rect(xProp, y, size, size));
                AbsoluteLayout.SetLayoutFlags(dot, AbsoluteLayoutFlags.XProportional);
                ChartCanvas.Children.Insert(ChartCanvas.Children.Count - 1, dot);
            }

            var canvasTap = new TapGestureRecognizer();
            canvasTap.Tapped += (s, e) => ChartTooltip.IsVisible = false;
            ChartCanvas.GestureRecognizers.Add(canvasTap);
        }

        private void DrawMonthlyChart()
        {
            MonthlyChartGrid.Children.Clear();
            MonthlyChartGrid.ColumnDefinitions.Clear();
            MonthlyTooltip.IsVisible = false;

            var entries = _cachedFirebaseMoods
                .Where(e =>
                {
                    if (!DateTime.TryParse(e.CreatedAt, out var d)) return false;
                    return d.ToLocalTime() >= DateTime.Now.AddDays(-28);
                })
                .ToList();

            const double maxBarHeight = 120.0;

            var weeks = new[]
            {
                ("Week 1", DateTime.Now.AddDays(-28), DateTime.Now.AddDays(-21)),
                ("Week 2", DateTime.Now.AddDays(-21), DateTime.Now.AddDays(-14)),
                ("Week 3", DateTime.Now.AddDays(-14), DateTime.Now.AddDays(-7)),
                ("Week 4", DateTime.Now.AddDays(-7),  DateTime.Now)
            };

            for (int w = 0; w < weeks.Length; w++)
            {
                MonthlyChartGrid.ColumnDefinitions.Add(
                    new ColumnDefinition { Width = GridLength.Star });

                var (label, from, to) = weeks[w];
                var weekEntries = entries
                    .Where(e =>
                    {
                        if (!DateTime.TryParse(e.CreatedAt, out var d)) return false;
                        var local = d.ToLocalTime();
                        return local >= from && local < to;
                    })
                    .ToList();

                double weekTotal = weekEntries.Any()
                    ? weekEntries.Sum(e => (double)e.MoodScore / MaxScorePerSession * 5)
                    : 0;

                double barHeight = (weekTotal / 35.0) * maxBarHeight;
                string capturedLabel = label;
                double capturedTotal = weekTotal;

                var col = new VerticalStackLayout
                {
                    VerticalOptions = LayoutOptions.End,
                    HorizontalOptions = LayoutOptions.Center,
                    Spacing = 4
                };

                col.Children.Add(new BoxView
                {
                    HeightRequest = maxBarHeight - barHeight,
                    Color = Colors.Transparent
                });

                if (weekTotal > 0)
                {
                    var bar = new Border
                    {
                        HeightRequest = barHeight,
                        WidthRequest = 32,
                        Background = new SolidColorBrush(Color.FromArgb("#5BC8D0")),
                        StrokeThickness = 0,
                        StrokeShape = new RoundRectangle { CornerRadius = new CornerRadius(6, 6, 0, 0) }
                    };

                    var tap = new TapGestureRecognizer();
                    tap.Tapped += (s, e) =>
                    {
                        MonthlyTooltipLabel.Text = $"{capturedLabel}: {capturedTotal:F1} avg";
                        MonthlyTooltip.IsVisible = true;
                    };
                    bar.GestureRecognizers.Add(tap);
                    col.Children.Add(bar);
                }

                col.Children.Add(new Label
                {
                    Text = label,
                    FontSize = 10,
                    TextColor = Color.FromArgb("#AAAAAA"),
                    HorizontalOptions = LayoutOptions.Center
                });

                Grid.SetColumn(col, w);
                MonthlyChartGrid.Children.Add(col);
            }
        }

        private void UpdateXAxisLabels(List<string> labels)
        {
            XAxisLabels.Children.Clear();
            XAxisLabels.ColumnDefinitions.Clear();

            for (int i = 0; i < labels.Count; i++)
            {
                XAxisLabels.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
                var lbl = new Label
                {
                    Text = labels[i],
                    FontSize = 10,
                    TextColor = Color.FromArgb("#AAAAAA"),
                    HorizontalOptions = LayoutOptions.Center
                };
                Grid.SetColumn(lbl, i);
                XAxisLabels.Children.Add(lbl);
            }
        }

        private async void OnBackbuttonClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//HomePage");
        }

        private void LoadRecentEntries(List<LocalMoodEntry> allEntries)
        {
            var collection = this.FindByName<CollectionView>("RecentEntriesCollection");
            var emptyLabel = this.FindByName<Label>("EmptyEntriesLabel");

            var recent = allEntries
                .OrderByDescending(e => e.Date)
                .Take(10)
                .Select(e =>
                {
                    double score = (double)e.Score / e.MaxScore * 5;
                    string emoji, label;

                    if (score >= 4.5) { emoji = "🌟"; label = "Feeling Great"; }
                    else if (score >= 3.5) { emoji = "😊"; label = "Feeling Good"; }
                    else if (score >= 2.5) { emoji = "😐"; label = "Feeling Okay"; }
                    else if (score >= 1.5) { emoji = "😔"; label = "Feeling Low"; }
                    else { emoji = "💙"; label = "Overwhelming"; }

                    return new MoodEntryDisplay
                    {
                        Emoji = emoji,
                        MoodLabel = label,
                        DateText = e.Date.ToString("MMM dd, yyyy  h:mm tt"),
                        ScoreText = $"{score:F1}/5"
                    };
                })
                .ToList();

            if (recent.Any())
            {
                if (collection != null) collection.ItemsSource = recent;
                if (emptyLabel != null) emptyLabel.IsVisible = false;
            }
            else
            {
                if (collection != null) collection.ItemsSource = null;
                if (emptyLabel != null) emptyLabel.IsVisible = true;
            }
        }

        private async void OnBackClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}