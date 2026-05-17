namespace MentalWellness.Shared.Models
{
    public class CopingStrategy
    {
        public string Id { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty; // Breathing, Exercise, Journaling
    }
}