using Microsoft.EntityFrameworkCore;
using QrGenerator.Application.Interfaces;
using QrGenerator.Domain.Entities;

namespace QrGenerator.Infrastructure.Data;

public class UserRepository : IUserRepository
{
    private readonly QrDbContext _context;

    public UserRepository(QrDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByPhoneNumber(string phoneNumber, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber, cancellationToken);
    }

    public async Task<User> Create(User user, CancellationToken cancellationToken = default)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync(cancellationToken);
        return user;
    }
}

