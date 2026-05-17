using MentalWellness.Shared.Models;

namespace MentalWellness.Shared.Services
{
    public interface IDataService
    {
        Task<List<MoodEntry>> GetMoodEntriesAsync(string userId);
        Task SaveMoodEntryAsync(MoodEntry entry);
        Task<List<CopingStrategy>> GetCopingStrategiesAsync();
    }
}