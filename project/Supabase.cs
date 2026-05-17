using Supabase;

namespace project;

public static class SupabaseService
{
    public static Client Client { get; private set; }

    public static async Task Initialize()
    {
        string supabaseUrl = "https://bbphwrlzbwlqdcsopsun.supabase.co";
        string supabaseKey = "sb_publishable_Szk4c9FCZE400in71NPnoA_5n-rIcFn";

        // Ensure TLS 1.2+ is used (important for HTTPS)
        System.Net.ServicePointManager.SecurityProtocol =
            System.Net.SecurityProtocolType.Tls12 | System.Net.SecurityProtocolType.Tls13;

        // Create Supabase client with options if needed
        var options = new Supabase.SupabaseOptions
        {
            AutoRefreshToken = true,
            AutoConnectRealtime = false // turn on if you need realtime
        };

        Client = new Client(supabaseUrl, supabaseKey, options);

        try
        {
            await Client.InitializeAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Supabase initialize error: {ex.Message}");
            throw;
        }
    }
}