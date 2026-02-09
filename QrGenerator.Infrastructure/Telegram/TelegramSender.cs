using System.Net.Http.Headers;
using Microsoft.Extensions.Options;
using QrGenerator.Application.Interfaces;
using QrGenerator.Application.QrCodes;

namespace QrGenerator.Infrastructure.Telegram;

public class TelegramSender : ITelegramSender
{
    private readonly TelegramOptions _options;
    private static readonly HttpClient HttpClient = new();

    public TelegramSender(IOptions<TelegramOptions> options)
    {
        _options = options.Value;
    }

    public async Task SendQr(long chatId, QrCodeResponse qr, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(_options.BotToken))
            throw new InvalidOperationException("Telegram BotToken is not configured.");

        var bytes = Convert.FromBase64String(qr.QrCodeImageBase64);
        var caption = $"Заезд: {qr.StartDate:dd.MM.yyyy} - {qr.EndDate:dd.MM.yyyy}, гостей: {qr.GuestCount}, пароль: {qr.DoorLockPassword}";
        var url = $"https://api.telegram.org/bot{_options.BotToken}/sendPhoto";
        using var content = new MultipartFormDataContent();
        content.Add(new StringContent(chatId.ToString()), "chat_id");
        content.Add(new StringContent(caption), "caption");
        var streamContent = new StreamContent(new MemoryStream(bytes));
        streamContent.Headers.ContentType = new MediaTypeHeaderValue("image/png");
        content.Add(streamContent, "photo", "qr.png");

        var response = await HttpClient.PostAsync(url, content, ct);
        response.EnsureSuccessStatusCode();
    }

    public async Task SendMessage(long chatId, string text, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(_options.BotToken))
            throw new InvalidOperationException("Telegram BotToken is not configured.");
        var url = $"https://api.telegram.org/bot{_options.BotToken}/sendMessage";
        using var content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["chat_id"] = chatId.ToString(),
            ["text"] = text
        });
        var response = await HttpClient.PostAsync(url, content, ct);
        response.EnsureSuccessStatusCode();
    }
}
