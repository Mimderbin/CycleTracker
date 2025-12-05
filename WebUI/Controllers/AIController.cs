using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Microsoft.Extensions.Options;
using WebUI.Models;

namespace WebUI.Controllers;

[Route("AI")]
public class AIController : Controller
{
    private readonly HttpClient _client;

    public AIController(IHttpClientFactory factory, IOptions<OpenRouterConfig> config)
    {
        _client = factory.CreateClient();
        _client.BaseAddress = new Uri("https://openrouter.ai/api/v1/");
        _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {config.Value.ApiKey}");
    }


    [HttpPost("Ask")]
    public async Task<IActionResult> Ask([FromBody] AiRequest req)
    {
        var payload = new
        {
            model = "deepseek/deepseek-chat",
            messages = new[]
            {
                new { role = "user", content = req.Message }
            }
        };

        var response = await _client.PostAsJsonAsync("chat/completions", payload);
        var json = await response.Content.ReadAsStringAsync();

        var doc = JsonDocument.Parse(json);
        var content = doc.RootElement
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString();

        return Json(new { reply = content });
    }
}

public class AiRequest
{
    public string Message { get; set; } = "";
}