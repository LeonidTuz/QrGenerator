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

    public async Task<User?> GetByPhoneNumber(string phoneNumber, CancellationToken ct)
    {
        return await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber, ct);
    }

    public async Task<User?> GetById(Guid userId, CancellationToken ct)
    {
        return await _context.Users.FindAsync(new object[] { userId }, ct);
    }

    public async Task<User> Create(User user, CancellationToken ct)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync(ct);
        return user;
    }

    public async Task Update(User user, CancellationToken ct)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync(ct);
    }
}

