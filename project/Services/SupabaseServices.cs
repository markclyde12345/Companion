using Supabase;
using Supabase.Gotrue;
using System.Threading.Tasks;

namespace MentalWellness.Shared.Services
{
    public class SupabaseService
    {
        private readonly Supabase.Client _client;

        public static Supabase.Client Client { get; private set; }

        public static async Task Initialize()
        {
            Client = new Supabase.Client(
                "https://bbphwrlzbwlqdcsopsun.supabase.co",
                "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6ImJicGh3cmx6YndscWRjc29wc3VuIiwicm9sZSI6ImFub24iLCJpYXQiOjE3NzEwOTU0OTgsImV4cCI6MjA4NjY3MTQ5OH0.7CSOKIDWmtC9qoiwgCC2Ne4-HObvaB9CLYmaIWrljvs"
            );

            await Client.InitializeAsync();
        }

        public async Task<Session?> Login(string email, string password)
        {
            try
            {
                var session = await _client.Auth.SignIn(email, password);
                return session;
            }
            catch
            {
                return null;
            }
        }
    }
}