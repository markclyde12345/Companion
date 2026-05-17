public class MoodEntry
{
    public string UserId { get; set; }
    public string Mood { get; set; }
    public int MoodScore { get; set; }
    public List<int> Answers { get; set; }
    public int AttemptNumber { get; set; }
    public string Notes { get; set; }

    // ✅ FIXED: use DateTime instead of string
    public string CreatedAt { get; set; }
}