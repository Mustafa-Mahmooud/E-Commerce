using Microsoft.AspNetCore.Mvc;

public class ChatGptAssistantController : ControllerBase
{
    private readonly ChatGptService _chatGptService;

    public ChatGptAssistantController(ChatGptService chatGptService)
    {
        _chatGptService = chatGptService;
    }

    [HttpGet]
    public async Task<IActionResult> GetResponse(string prompt)
    {
        if (string.IsNullOrEmpty(prompt))
        {
            return BadRequest("Prompt is required.");
        }

        var response = await _chatGptService.GetChatGptResponse(prompt);
        return Ok(response);
    }
}