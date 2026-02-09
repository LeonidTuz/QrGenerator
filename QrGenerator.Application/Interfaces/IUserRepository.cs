using QrGenerator.Domain.Entities;

namespace QrGenerator.Application.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByPhoneNumber(string phoneNumber, CancellationToken ct);
    Task<User?> GetById(Guid userId, CancellationToken ct);
    Task<User> Create(User user, CancellationToken ct);
    Task Update(User user, CancellationToken ct);
}

