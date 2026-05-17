using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Microsoft.Maui.Storage;

namespace project
{
    public class MoodEntry
    {
        public DateTime Date { get; set; }
        public int Score { get; set; }
        public int MaxScore
        {
            get; set;
        }
        public DateTime CreatedAt { get; internal set; }

        public static class MoodScoreStorage
        {
            private const string Key = "mood_entries";

            public static void SaveEntry(int score, int maxScore)
            {
                var entries = GetAllEntries();
                entries.Add(new MoodEntry
                {
                    Date = DateTime.Now,
                    Score = score,
                    MaxScore = maxScore
                });

                entries = entries
                    .Where(e => e.Date >= DateTime.Now.AddDays(-30))
                    .ToList();

                Preferences.Set(Key, JsonSerializer.Serialize(entries));
            }

            public static List<MoodEntry> GetAllEntries()
            {
                var json = Preferences.Get(Key, "[]");
                return JsonSerializer.Deserialize<List<MoodEntry>>(json) ?? new List<MoodEntry>();
            }

            public static double GetAverageScore(int days = 3)
            {
                var cutoff = DateTime.Now.AddDays(-days);
                var entries = GetAllEntries().Where(e => e.Date >= cutoff).ToList();
                if (!entries.Any()) return 0;
                return entries.Average(e => (double)e.Score / e.MaxScore * 5);
            }

            public static int GetTotalLogs() => GetAllEntries().Count;
        }
    }
}