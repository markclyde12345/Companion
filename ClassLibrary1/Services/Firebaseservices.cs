using Firebase.Database;
using Firebase.Database.Query;
using Firebase.Auth;
using Firebase.Auth.Providers;
using System.Text;
using System.Net.Http;
using System.Text.Json;

namespace project
{
    public class UserProfile
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string UserId { get; set; }
        public string CreatedAt { get; set; }
        public bool IsEmailVerified { get; set; }
    }

    public static class FirebaseService
    {
        private const string FirebaseApiKey = "AIzaSyCoA1ekJ-b_dEIb3mFSofFDC0SAeOli3wE";
        private const string FirebaseDatabaseUrl = "https://mindbloom-d0622-default-rtdb.firebaseio.com/";
        private const string FirebaseAuthDomain = "mindbloom-d0622.firebaseapp.com";

        public static string CurrentUserId { get; private set; }
        public static string CurrentUserEmail { get; private set; }
        public static string CurrentToken { get; private set; }

        private static FirebaseClient _db;
        private static FirebaseAuthClient _auth;
        private static UserCredential _currentCredential;

        public static void Initialize()
        {
            var authConfig = new FirebaseAuthConfig
            {
                ApiKey = FirebaseApiKey,
                AuthDomain = FirebaseAuthDomain,
                Providers = new FirebaseAuthProvider[]
                {
                    new EmailProvider()
                }
            };
            _auth = new FirebaseAuthClient(authConfig);
            _db = new FirebaseClient(FirebaseDatabaseUrl);
        }

        public static async Task<bool> RegisterAsync(
            string firstName, string lastName, string email, string password)
        {
            UserCredential result = null;
            try
            {
                result = await _auth.CreateUserWithEmailAndPasswordAsync(email, password, firstName);
                _currentCredential = result;
                CurrentUserId = result.User.Uid;
                CurrentUserEmail = email;
                CurrentToken = await result.User.GetIdTokenAsync();

                await SendEmailVerificationAsync(CurrentToken);

                var authenticatedDb = new FirebaseClient(
                    FirebaseDatabaseUrl,
                    new FirebaseOptions
                    {
                        AuthTokenAsyncFactory = () => Task.FromResult(CurrentToken)
                    });

                await authenticatedDb
                    .Child("users")
                    .Child(CurrentUserId)
                    .PutAsync(new UserProfile
                    {
                        FirstName = firstName,
                        LastName = lastName,
                        Email = email,
                        UserId = CurrentUserId,
                        CreatedAt = DateTime.UtcNow.ToString("o"),
                        IsEmailVerified = false
                    });

                _db = authenticatedDb;
                return true;
            }
            catch (Exception)
            {
                if (result != null)
                {
                    try { await result.User.DeleteAsync(); } catch { }
                }
                CurrentUserId = null;
                CurrentUserEmail = null;
                CurrentToken = null;
                _currentCredential = null;
                throw;
            }
        }

        public static async Task<(UserProfile profile, bool isVerified)> LoginAsync(
            string email, string password)
        {
            var result = await _auth.SignInWithEmailAndPasswordAsync(email, password);
            _currentCredential = result;
            CurrentUserId = result.User.Uid;
            CurrentUserEmail = email;
            CurrentToken = await result.User.GetIdTokenAsync();

            _db = new FirebaseClient(
                FirebaseDatabaseUrl,
                new FirebaseOptions
                {
                    AuthTokenAsyncFactory = () => Task.FromResult(CurrentToken)
                });

            var isVerified = result.User.Info.IsEmailVerified;

            if (isVerified)
            {
                await _db.Child("users").Child(CurrentUserId)
                    .Child("IsEmailVerified").PutAsync(true);
            }

            var profile = await _db.Child("users").Child(CurrentUserId)
                .OnceSingleAsync<UserProfile>();

            return (profile, isVerified);
        }

        private static async Task SendEmailVerificationAsync(string idToken)
        {
            using var client = new HttpClient();
            var payload = new { requestType = "VERIFY_EMAIL", idToken };
            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            await client.PostAsync(
                $"https://identitytoolkit.googleapis.com/v1/accounts:sendOobCode?key={FirebaseApiKey}",
                content);
        }

        public static async Task ResendVerificationEmailAsync()
        {
            if (!string.IsNullOrEmpty(CurrentToken))
                await SendEmailVerificationAsync(CurrentToken);
        }

        public static void Logout()
        {
            CurrentUserId = null;
            CurrentUserEmail = null;
            CurrentToken = null;
            _currentCredential = null;
            _db = new FirebaseClient(FirebaseDatabaseUrl);
        }

        private static void EnsureUser()
        {
            if (string.IsNullOrEmpty(CurrentUserId))
                throw new Exception("User not logged in.");
        }

        // ─── ATTEMPTS ─────────────────────────────────────────
        public static async Task<int> IncrementAttemptsAsync()
        {
            EnsureUser();
            var refDb = _db.Child("users").Child(CurrentUserId).Child("totalAttempts");
            var current = await refDb.OnceSingleAsync<int?>();
            int newValue = (current ?? 0) + 1;
            await refDb.PutAsync(newValue);
            return newValue;
        }

        public static async Task<int> GetTotalAttemptsAsync()
        {
            EnsureUser();
            var result = await _db.Child("users").Child(CurrentUserId)
                .Child("totalAttempts").OnceSingleAsync<int?>();
            return result ?? 0;
        }

        // ─── LOGS ─────────────────────────────────────────────
        public static async Task<int> IncrementLogsAsync()
        {
            EnsureUser();
            var refDb = _db.Child("users").Child(CurrentUserId).Child("totalLogs");
            var current = await refDb.OnceSingleAsync<int?>();
            int newValue = (current ?? 0) + 1;
            await refDb.PutAsync(newValue);
            return newValue;
        }

        public static async Task<int> GetTotalLogsAsync()
        {
            EnsureUser();
            var result = await _db.Child("users").Child(CurrentUserId)
                .Child("totalLogs").OnceSingleAsync<int?>();
            return result ?? 0;
        }

        // ─── STREAK ───────────────────────────────────────────
        public static async Task<int> GetStreakAsync()
        {
            EnsureUser();
            var result = await _db.Child("users").Child(CurrentUserId)
                .Child("streak").OnceSingleAsync<int?>();
            return result ?? 0;
        }

        public static async Task<string> GetLastMoodDateAsync()
        {
            EnsureUser();
            var raw = await _db.Child("users").Child(CurrentUserId)
                .Child("lastMoodDate").OnceSingleAsync<dynamic>();
            return raw?.ToString().Trim('"');
        }

        public static async Task<int> GetGraceAsync()
        {
            EnsureUser();
            var result = await _db.Child("users").Child(CurrentUserId)
                .Child("streakGraceRemaining").OnceSingleAsync<int?>();
            return result ?? 1;
        }

        public static async Task RestoreGraceAsync()
        {
            EnsureUser();
            await _db.Child("users").Child(CurrentUserId)
                .Child("streakGraceRemaining").PutAsync(1);
        }

        public static async Task UpdateStreakAsync()
        {
            EnsureUser();
            var today = DateTime.Now.Date;
            string lastDateStr = await GetLastMoodDateAsync();
            int currentStreak = await GetStreakAsync();
            int grace = await GetGraceAsync();

            if (DateTime.TryParse(lastDateStr, out DateTime lastDate))
            {
                var diff = (today - lastDate.Date).Days;
                if (diff == 0)
                {
                    return;
                }
                else if (diff == 1)
                {
                    currentStreak += 1;
                }
                else if (diff > 1)
                {
                    if (grace > 0)
                    {
                        grace -= 1;
                        await _db.Child("users").Child(CurrentUserId)
                            .Child("streakGraceRemaining").PutAsync(grace);
                        currentStreak += 1;
                    }
                    else
                    {
                        currentStreak = 1;
                    }
                }
            }
            else
            {
                currentStreak = 1;
            }

            await _db.Child("users").Child(CurrentUserId)
                .Child("streak").PutAsync(currentStreak);
            await _db.Child("users").Child(CurrentUserId)
                .Child("lastMoodDate")
                .PutAsync("\"" + today.ToString("yyyy-MM-dd") + "\"");
        }

        // ─── MOOD LOGGING ─────────────────────────────────────
        public static async Task<bool> HasLoggedMoodTodayAsync()
        {
            EnsureUser();
            var today = DateTime.Now.Date;
            var entries = await _db.Child("moods").Child(CurrentUserId)
                .OnceAsync<MoodEntry>();
            return entries.Any(e =>
            {
                if (DateTime.TryParse(e.Object.CreatedAt, out var date))
                    return date.ToLocalTime().Date == today;
                return false;
            });
        }

        public static async Task<bool> HasLoggedMainMoodTodayAsync()
        {
            EnsureUser();
            var entries = await _db.Child("moods").Child(CurrentUserId)
                .Child("mainLogs").OnceAsync<MoodEntry>();
            return entries.Any(e =>
                DateTime.TryParse(e.Object.CreatedAt, out var date) &&
                date.ToLocalTime().Date == DateTime.Now.Date);
        }

        public static async Task<int> GetWeeklyMoodCountAsync()
        {
            EnsureUser();
            var startOfWeek = DateTime.Now.Date.AddDays(-(int)DateTime.Now.DayOfWeek + (int)DayOfWeek.Monday);
            var endOfWeek = startOfWeek.AddDays(7);
            var entries = await _db.Child("moods").Child(CurrentUserId)
                .OnceAsync<MoodEntry>();
            return entries.Count(e =>
            {
                if (DateTime.TryParse(e.Object.CreatedAt, out var date))
                {
                    var localDate = date.ToLocalTime().Date;
                    return localDate >= startOfWeek && localDate < endOfWeek;
                }
                return false;
            });
        }

        public static async Task<int> GetWeeklyMainMoodCountAsync()
        {
            EnsureUser();
            var startOfWeek = DateTime.Now.Date.AddDays(-(int)DateTime.Now.DayOfWeek + (int)DayOfWeek.Monday);
            var endOfWeek = startOfWeek.AddDays(7);
            var entries = await _db.Child("moods").Child(CurrentUserId)
                .Child("mainLogs").OnceAsync<MoodEntry>();
            return entries.Count(e =>
            {
                if (DateTime.TryParse(e.Object.CreatedAt, out var date))
                {
                    var local = date.ToLocalTime().Date;
                    return local >= startOfWeek && local < endOfWeek;
                }
                return false;
            });
        }

        public static async Task SaveMoodAsync(
            string mood, int score, string notes, List<int> answers)
        {
            EnsureUser();
            int attemptNumber = await IncrementAttemptsAsync();
            await _db.Child("moods").Child(CurrentUserId)
                .PostAsync(new MoodEntry
                {
                    UserId = CurrentUserId,
                    Mood = mood,
                    MoodScore = score,
                    Answers = answers,
                    AttemptNumber = attemptNumber,
                    Notes = notes,
                    CreatedAt = DateTime.UtcNow.ToString("o")
                });
            try { await UpdateStreakAsync(); }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Streak update failed: {ex.Message}");
            }
        }

        public static async Task SaveMainMoodAsync(
            string mood, int score, string notes, List<int> answers)
        {
            EnsureUser();
            if (await HasLoggedMainMoodTodayAsync()) return;
            await _db.Child("moods").Child(CurrentUserId).Child("mainLogs")
                .PostAsync(new MoodEntry
                {
                    UserId = CurrentUserId,
                    Mood = mood,
                    MoodScore = score,
                    Answers = answers,
                    Notes = notes,
                    CreatedAt = DateTime.UtcNow.ToString("o")
                });
            await UpdateStreakAsync();
        }

        public static async Task SaveExtraMoodAsync(
            string mood, int score, string notes)
        {
            EnsureUser();
            await _db.Child("moods").Child(CurrentUserId).Child("extraLogs")
                .PostAsync(new MoodEntry
                {
                    UserId = CurrentUserId,
                    Mood = mood,
                    MoodScore = score,
                    Notes = notes,
                    CreatedAt = DateTime.UtcNow.ToString("o")
                });
        }

        public static async Task<List<MoodEntry>> GetMoodsAsync()
        {
            EnsureUser();
            var entries = await _db.Child("moods").Child(CurrentUserId)
                .OnceAsync<MoodEntry>();
            return entries
                .Select(e => e.Object)
                .OrderByDescending(e => e.CreatedAt)
                .ToList();
        }

        public static async Task ResetCopingProgressIfNewWeekAsync()
        {
            EnsureUser();

            int daysSinceMonday = ((int)DateTime.Today.DayOfWeek + 6) % 7;
            var weekStart = DateTime.Today.AddDays(-daysSinceMonday);
            string weekKey = weekStart.ToString("yyyy-MM-dd");

            var lastReset = await _db.Child("users").Child(CurrentUserId)
                .Child("lastCopingReset").OnceSingleAsync<string>();

            if (lastReset?.Trim('"') != weekKey)
            {
                await SaveCopingProgressAsync(0, 4);
                await _db.Child("users").Child(CurrentUserId)
                    .Child("lastCopingReset").PutAsync($"\"{weekKey}\"");
            }
        }

        // ─── COPING PROGRESS ──────────────────────────────────
        public static async Task SaveCopingProgressAsync(int completed, int total)
        {
            EnsureUser();
            await _db.Child("users").Child(CurrentUserId)
                .Child("copingProgress")
                .PutAsync(new { completed, total });
        }

        public static async Task<(int completed, int total)> GetCopingProgressAsync()
        {
            EnsureUser();
            var result = await _db.Child("users").Child(CurrentUserId)
                .Child("copingProgress").OnceSingleAsync<dynamic>();
            if (result == null) return (0, 4);
            return ((int)result.completed, (int)result.total);
        }

        // ─── USER PROFILE ─────────────────────────────────────
        public static async Task<UserProfile> GetUserProfileAsync()
        {
            EnsureUser();
            return await _db.Child("users").Child(CurrentUserId)
                .OnceSingleAsync<UserProfile>();
        }

        public static async Task SendPasswordResetAsync(string email)
        {
            await _auth.ResetEmailPasswordAsync(email);
        }

        public static async Task ChangePasswordAsync(
            string currentPassword, string newPassword)
        {
            EnsureUser();
            var result = await _auth.SignInWithEmailAndPasswordAsync(
                CurrentUserEmail, currentPassword);
            await result.User.ChangePasswordAsync(newPassword);
        }

        public static async Task RefreshTodayState()
        {
            var result = await HasLoggedMoodTodayAsync();
            System.Diagnostics.Debug.WriteLine($"HasLoggedToday: {result}");
        }
    }
}