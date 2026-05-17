using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;

namespace project;

public static class AIService
{
    private static readonly HttpClient client = new HttpClient();

    private const string ApiKey = "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";

    public static async Task<string> GetReply(string userMessage)
{
    try
    {
        var client = new HttpClient();

        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.Add("Authorization", "Bearer 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");
        client.DefaultRequestHeaders.Add("HTTP-Referer", "https://localhost");
        client.DefaultRequestHeaders.Add("X-Title", "DrAria");

        var body = new
        {
            model = "meta-llama/llama-3.1-8b-instruct",
            messages = new[]
            {
                new { role = "system", content = "You are Dr. Aria, a supportive therapist AI." },
                new { role = "user", content = userMessage }
            }
        };

        var json = JsonConvert.SerializeObject(body);

        var response = await client.PostAsync(
            "https://openrouter.ai/api/v1/chat/completions",
            new StringContent(json, Encoding.UTF8, "application/json"));

        var result = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            return $"ERROR {response.StatusCode}: {result}";

        dynamic data = JsonConvert.DeserializeObject(result);

        return data.choices[0].message.content.ToString();
    }
    catch (Exception ex)
    {
        return "Exception: " + ex.Message;
    }
}
}