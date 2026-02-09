using QrGenerator.Application.Auth;

namespace QrGenerator.Application.Services;

public interface IAuthService
{
    Task<AuthResponse> LoginByPhoneAsync(string phoneNumber, CancellationToken ct);
}

