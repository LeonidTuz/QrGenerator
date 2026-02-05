using QrGenerator.Application.Auth;
using QrGenerator.Application.Interfaces;
using QrGenerator.Domain.Entities;

namespace QrGenerator.Application.Services;

public class AuthService : IAuthService
{
    private readonly ITokenService _tokenService;

    public AuthService(ITokenService tokenService)
    {
        _tokenService = tokenService;
    }

    public Task<AuthResponse> LoginByPhoneAsync(string phoneNumber, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
        {
            throw new ArgumentException("Телефон не заполнен или некорректен", nameof(phoneNumber));
        }
        
        var user = new User
        {
            UserId = Guid.NewGuid(),
            PhoneNumber = phoneNumber
        };

        var token = _tokenService.GenerateToken(user);

        var response = new AuthResponse
        {
            AccessToken = token,
            TokenType = "Bearer"
        };

        return Task.FromResult(response);
    }
}