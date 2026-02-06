using QrGenerator.Application.Auth;
using QrGenerator.Application.Interfaces;
using QrGenerator.Domain.Entities;

namespace QrGenerator.Application.Services;

public class AuthService : IAuthService
{
    private readonly ITokenService _tokenService;
    private readonly IUserRepository _userRepository;

    public AuthService(ITokenService tokenService, IUserRepository userRepository)
    {
        _tokenService = tokenService;
        _userRepository = userRepository;
    }

    public async Task<AuthResponse> LoginByPhoneAsync(string phoneNumber, CancellationToken cancellationToken = default)
    {
        var normalizedPhoneNumber = NormalizePhoneNumber(phoneNumber);

        var user = await _userRepository.GetByPhoneNumber(normalizedPhoneNumber, cancellationToken);

        if (user == null)
        {
            user = new User
            {
                UserId = Guid.NewGuid(),
                PhoneNumber = normalizedPhoneNumber
            };

            await _userRepository.Create(user, cancellationToken);
        }

        var token = _tokenService.GenerateToken(user);

        return new AuthResponse
        {
            AccessToken = token,
            TokenType = "Bearer"
        };
    }

    private static string NormalizePhoneNumber(string phoneNumber)
    {
        var normalized = new string(phoneNumber.Where(c => char.IsDigit(c) || c == '+').ToArray());
        if (string.IsNullOrWhiteSpace(normalized))
        {
            throw new ArgumentException("Телефон не заполнен или некорректен", nameof(phoneNumber));
        }

        return normalized;
    }
}