using QrGenerator.Domain.Entities;

namespace QrGenerator.Application.Interfaces;

public interface IQrCodeRepository
{
    Task<QrCode> Create(QrCode qrCode, CancellationToken ct);
    Task<QrCode?> GetById(Guid qrCodeId, CancellationToken ct);
    Task<IEnumerable<QrCode>> GetByUserId(Guid userId, CancellationToken ct);
}
