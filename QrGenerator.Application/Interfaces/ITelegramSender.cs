using QrGenerator.Application.QrCodes;

namespace QrGenerator.Application.Interfaces;

public interface ITelegramSender
{
    Task SendQr(long chatId, QrCodeResponse qr, CancellationToken ct );
    Task SendMessage(long chatId, string text, CancellationToken ct);
}