using Supabase;
using Supabase.Gotrue;
using System.Threading.Tasks;

namespace ClassLibrary1.Services
{
    public static class SupabaseServices
    {
        public static Supabase.Client Client { get; private set; }

        public static async Task InitializeAsync()
        {
            var options = new SupabaseOptions
            {
                AutoConnectRealtime = false  // ✅ Fixes SSL error on Windows
            };

            Client = new Supabase.Client(
                "https://bbphwrlzbwlqdcsopsun.supabase.co",
                "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6ImJicGh3cmx6YndscWRjc29wc3VuIiwicm9sZSI6ImFub24iLCJpYXQiOjE3NzEwOTU0OTgsImV4cCI6MjA4NjY3MTQ5OH0.7CSOKIDWmtC9qoiwgCC2Ne4-HObvaB9CLYmaIWrljvs",
                options  // ✅ Pass options here
            );

            await Client.InitializeAsync();
        }

        public static async Task<Supabase.Gotrue.Session?> Login(string email, string password)
        {
            try
            {
                var session = await Client.Auth.SignIn(email, password);
                return session;
            }
            catch
            {
                return null;
            }
        }
    }
}