namespace MentalWellness.Shared.Models;

public class MoodEntry
{
    public string Id { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string MoodLevel { get; set; } = string.Empty; // Happy, Sad, Stressed
    public string Notes { get; set; } = string.Empty;
    public int MoodScore { get; set; } // 1-5 scale
    public DateTime CreatedAt { get; set; }
    public DateTime DateRecorded { get; set; }
}