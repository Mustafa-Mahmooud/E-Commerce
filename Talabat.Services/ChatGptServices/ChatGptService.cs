using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;

public class ChatGptService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ChatGptService> _logger;
    private readonly string _apiKey;

    public ChatGptService(HttpClient httpClient, ILogger<ChatGptService> logger, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _logger = logger;
        _apiKey = configuration["GitHub:ApiKey"]; // Use the GitHub API key from appsettings.json
    }

    public async Task<string> GetChatGptResponse(string prompt)
    {
        try
        {
            // GitHub API endpoint (example: create an issue in a repository)
            var url = "https://api.github.com/repos/your-username/your-repo/issues";

            var requestBody = new
            {
                title = "ChatGPT Response",
                body = prompt // Use the prompt as the issue body
            };

            var json = JsonConvert.SerializeObject(requestBody);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            // Create a request message and set headers for the request
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = content
            };

            // Add headers to the HttpRequestMessage
            requestMessage.Headers.Add("Accept", "application/vnd.github.v3+json"); // GitHub API version
            requestMessage.Headers.Add("User-Agent", "MyApp"); // GitHub requires a User-Agent header
            requestMessage.Headers.Add("Authorization", $"Bearer {_apiKey}"); // Use the GitHub API key

            // Send the request using HttpClient
            var response = await _httpClient.SendAsync(requestMessage);

            // Log the full response for debugging
            var responseContent = await response.Content.ReadAsStringAsync();
            _logger.LogInformation("GitHub API Response - Status Code: {StatusCode}, Content: {Content}", response.StatusCode, responseContent);

            // Check if the request was successful
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("GitHub API request failed with status code {StatusCode}: {Response}", response.StatusCode, responseContent);
                return "GitHub API request failed.";
            }

            // Read and parse the response
            dynamic responseJson = JsonConvert.DeserializeObject(responseContent);

            // Return the issue URL or other relevant data
            return responseJson?.html_url?.ToString() ?? "No response content.";
        }
        catch (Exception ex)
        {
            // Log the exception
            _logger.LogError(ex, "An error occurred while calling GitHub API.");
            return "An error occurred while calling the GitHub API.";
        }
    
    }
}