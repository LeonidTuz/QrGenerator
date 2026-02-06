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

    public async Task<QrCode> Create(QrCode qrCode, CancellationToken cancellationToken = default)
    {
        _context.QrCodes.Add(qrCode);
        await _context.SaveChangesAsync(cancellationToken);
        return qrCode;
    }

    public async Task<QrCode?> GetById(Guid qrCodeId, CancellationToken cancellationToken = default)
    {
        return await _context.QrCodes
            .AsNoTracking()
            .FirstOrDefaultAsync(q => q.QrCodeId == qrCodeId, cancellationToken);
    }

    public async Task<IEnumerable<QrCode>> GetByUserId(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.QrCodes
            .AsNoTracking()
            .Where(q => q.UserId == userId)
            .OrderByDescending(q => q.CreatedAt)
            .ToListAsync(cancellationToken);
    }
}
