using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using QrGenerator.Application.Telegram;
using QrGenerator.Infrastructure.Telegram;
using System.Security.Claims;

namespace QrGenerator.Controllers;

[ApiController]
[Route("api/telegram")]
public class TelegramController : ControllerBase
{
    private readonly TelegramOptions _telegramOptions;

    public TelegramController(IOptions<TelegramOptions> telegramOptions)
    {
        _telegramOptions = telegramOptions.Value;
    }

    [HttpGet("link-url")]
    [Authorize]
    [ProducesResponseType(typeof(TelegramLinkUrlResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult GetLinkUrl()
    {
        if (string.IsNullOrWhiteSpace(_telegramOptions.BotUsername))
            return BadRequest("Telegram bot not configured.");

        var userId = GetCurrentUserId();
        var payload = userId.ToString("N");
        var url = $"https://t.me/{_telegramOptions.BotUsername.TrimStart('@')}?start={payload}";

        return Ok(new TelegramLinkUrlResponse { Url = url });
    }

    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            throw new UnauthorizedAccessException("Invalid user identifier.");
        return userId;
    }
}
