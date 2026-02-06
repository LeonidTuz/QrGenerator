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

    public QrController(IQrCodeService qrCodeService)
    {
        _qrCodeService = qrCodeService;
    }

    [HttpPost]
    [ProducesResponseType(typeof(QrCodeResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<QrCodeResponse>> CreateQrCode(
        [FromBody] CreateQrCodeRequest request,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var userId = GetCurrentUserId();
        var result = await _qrCodeService.CreateQrCode(userId, request, cancellationToken);

        return CreatedAtAction(nameof(GetQrCode), new { id = result.QrCodeId }, result);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(QrCodeResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<QrCodeResponse>> GetQrCode(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _qrCodeService.GetQrCodeById(id, cancellationToken);

        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [HttpGet("history")]
    [ProducesResponseType(typeof(IEnumerable<QrCodeResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<QrCodeResponse>>> GetUserQrCodes(
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var result = await _qrCodeService.GetUserQrCodes(userId, cancellationToken);

        return Ok(result);
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