using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QrGenerator.Application.Interfaces;
using QrGenerator.Application.QrCodes;
using System.Security.Claims;

namespace QrGenerator.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class QrController : ControllerBase
{
    private readonly IQrCodeService _qrCodeService;
    private readonly IUserRepository _userRepository;
    private readonly ITelegramSender _telegramSender;

    public QrController(IQrCodeService qrCodeService, IUserRepository userRepository, ITelegramSender telegramSender)
    {
        _qrCodeService = qrCodeService;
        _userRepository = userRepository;
        _telegramSender = telegramSender;
    }

    [HttpPost]
    [ProducesResponseType(typeof(QrCodeResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<QrCodeResponse>> CreateQrCode(
        [FromBody] CreateQrCodeRequest request,
        CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var userId = GetCurrentUserId();
        var result = await _qrCodeService.CreateQrCode(userId, request, ct);

        var user = await _userRepository.GetById(userId, ct);
        if (user?.TelegramId != null)
        {
            await _telegramSender.SendQr(user.TelegramId.Value, result, ct);
        }

        return CreatedAtAction(nameof(GetQrCode), new { id = result.QrCodeId }, result);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(QrCodeResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<QrCodeResponse>> GetQrCode(
        Guid id,
        CancellationToken ct)
    {
        var userId = GetCurrentUserId();
        var result = await _qrCodeService.GetQrCodeById(id, userId, ct);

        if (result == null) return NotFound();

        return Ok(result);
    }

    [HttpGet("history")]
    [ProducesResponseType(typeof(IEnumerable<QrCodeResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<QrCodeResponse>>> GetUserQrCodes(
        CancellationToken ct)
    {
        var userId = GetCurrentUserId();
        var result = await _qrCodeService.GetUserQrCodes(userId, ct);

        return Ok(result);
    }

    [HttpPost("{id}/send-telegram")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SendToTelegram(Guid id, CancellationToken ct)
    {
        var userId = GetCurrentUserId();
        var qr = await _qrCodeService.GetQrCodeById(id, userId, ct);
        if (qr == null) return NotFound();
        var user = await _userRepository.GetById(userId, ct);
        if (user?.TelegramId == null) return BadRequest("Telegram not linked.");
        await _telegramSender.SendQr(user.TelegramId.Value, qr, ct);
        return NoContent();
    }

    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            throw new UnauthorizedAccessException("Invalid user identifier.");
        }

        return userId;
    }
}