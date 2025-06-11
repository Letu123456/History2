using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DataAccess.Service
{
    public class GPTService
    {
        private readonly IConfiguration _config;
        private readonly HttpClient _httpClient;

        public GPTService(IConfiguration config)
        {
            _config = config;
            _httpClient = new HttpClient();
        }

        public async Task<bool> IsBlogContentAppropriateAsync(string content)
        {
            var apiKey = _config["OpenAI:ApiKey"];
            var endpoint = "https://openrouter.ai/api/v1/chat/completions";

            var requestBody = new
            {
                model = "openai/gpt-3.5-turbo",
                messages = new[]
                {
                new { role = "system", content = "You are a content moderation assistant. If the input contains hate speech, violence, sexual content, or anything inappropriate, respond 'NO'. Otherwise, respond 'YES'." },
                new { role = "user", content = $"Please check this blog content:\n\n{content}" }
            }
            };

            var json = JsonSerializer.Serialize(requestBody);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

            var response = await _httpClient.PostAsync(endpoint, httpContent);
            var responseBody = await response.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(responseBody);
            var messageContent = doc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString()
                ?.Trim()
                .ToUpper();

            return messageContent == "YES";
        }
    }
}
