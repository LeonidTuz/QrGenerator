using QrGenerator.Domain.Entities;

namespace QrGenerator.Application.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByPhoneNumber(string phoneNumber, CancellationToken cancellationToken = default);
    Task<User> Create(User user, CancellationToken cancellationToken = default);
}

