using Microsoft.EntityFrameworkCore;
using QrGenerator.Application.Interfaces;
using QrGenerator.Domain.Entities;

namespace QrGenerator.Infrastructure.Data;

public class QrCodeRepository : IQrCodeRepository
{
    private readonly QrDbContext _context;

    public QrCodeRepository(QrDbContext context)
    {
        _context = context;
    }

    public async Task<QrCode> Create(QrCode qrCode, CancellationToken ct)
    {
        _context.QrCodes.Add(qrCode);
        await _context.SaveChangesAsync(ct);
        return qrCode;
    }

    public async Task<QrCode?> GetById(Guid qrCodeId, CancellationToken ct)
    {
        return await _context.QrCodes
            .AsNoTracking()
            .FirstOrDefaultAsync(q => q.QrCodeId == qrCodeId, ct);
    }

    public async Task<IEnumerable<QrCode>> GetByUserId(Guid userId, CancellationToken ct)
    {
        return await _context.QrCodes
            .AsNoTracking()
            .Where(q => q.UserId == userId)
            .OrderByDescending(q => q.CreatedAt)
            .ToListAsync(ct);
    }
}
