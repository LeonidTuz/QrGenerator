using System.Text.Json;
using Microsoft.Extensions.Options;
using QrGenerator.Application.Interfaces;
using QrGenerator.Infrastructure.Telegram;

namespace QrGenerator.BackgroundServices;

public class TelegramPollingService : BackgroundService
{
    private readonly ILogger<TelegramPollingService> _logger;
    private readonly HttpClient _httpClient;
    private readonly IUserRepository _userRepository;
    private readonly ITelegramSender _telegramSender;
    private readonly TelegramOptions _options;

    private long _offset;

    public TelegramPollingService(
        ILogger<TelegramPollingService> logger,
        IHttpClientFactory httpClientFactory,
        IUserRepository userRepository,
        ITelegramSender telegramSender,
        IOptions<TelegramOptions> telegramOptions)
    {
        _logger = logger;
        _httpClient = httpClientFactory.CreateClient("telegram");
        _userRepository = userRepository;
        _telegramSender = telegramSender;
        _options = telegramOptions.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(_options.BotToken))
        {
            _logger.LogInformation("Telegram BotToken is not configured, polling disabled");
            return;
        }

        _logger.LogInformation("Telegram polling started");

        while (!ct.IsCancellationRequested)
        {
            try
            {
                var response = await _httpClient.GetAsync(
                    $"https://api.telegram.org/bot{_options.BotToken}/getUpdates?offset={_offset + 1}&timeout=30",
                    ct);

                var content = await response.Content.ReadAsStringAsync(ct);

                using var doc = JsonDocument.Parse(content);
                if (!doc.RootElement.TryGetProperty("result", out var updates))
                {
                    await Task.Delay(1000, ct);
                    continue;
                }

                foreach (var update in updates.EnumerateArray())
                {
                    _offset = update.GetProperty("update_id").GetInt64();

                    if (!update.TryGetProperty("message", out var message))
                        continue;

                    var text = message.TryGetProperty("text", out var textProp)
                        ? textProp.GetString()
                        : null;

                    if (!message.TryGetProperty("chat", out var chat) ||
                        !chat.TryGetProperty("id", out var chatIdProp))
                        continue;

                    var chatId = chatIdProp.GetInt64();

                    if (string.IsNullOrWhiteSpace(text) || !text.StartsWith("/start", StringComparison.Ordinal))
                        continue;

                    var parts = text.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length != 2)
                        continue;

                    var payload = parts[1].Trim();
                    if (!Guid.TryParse(payload, out var userId))
                        continue;

                    _logger.LogInformation(
                        "User connected telegram chat {ChatId} with payload {Payload}",
                        chatId, payload);

                    var user = await _userRepository.GetById(userId, ct);
                    if (user == null)
                        continue;

                    user.TelegramId = chatId;
                    await _userRepository.Update(user, ct);

                    await _telegramSender.SendMessage(
                        chatId,
                        "Готово. Теперь можно отправлять QR-коды с сайта.",
                        ct);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Telegram polling error");
            }

            await Task.Delay(1000, ct);
        }
    }
}