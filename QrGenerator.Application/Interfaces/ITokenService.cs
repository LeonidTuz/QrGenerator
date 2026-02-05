using QrGenerator.Domain.Entities;

namespace QrGenerator.Application.Interfaces;

public interface ITokenService
{
    string GenerateToken(User user);
}