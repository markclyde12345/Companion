using MentalWellness.Shared.Models;

namespace MentalWellness.Shared.Services
{
    public class ApiService : IDataService
    {
        public async Task<List<MoodEntry>> GetMoodEntriesAsync(string userId)
        {
            // Replace with real Supabase API call
            return new List<MoodEntry>();
        }

        public async Task SaveMoodEntryAsync(MoodEntry entry)
        {
            // Save to Supabase
        }

        public async Task<List<CopingStrategy>> GetCopingStrategiesAsync()
        {
            return new List<CopingStrategy>
            {
                new CopingStrategy
                {
                    Id = Guid.NewGuid().ToString(),
                    Title = "Deep Breathing",
                    Description = "Take slow deep breaths for 5 minutes.",
                    Category = "Breathing"
                }
            };
        }
    }
}