using QrGenerator.Domain.Entities;

namespace QrGenerator.Application.Interfaces;

public interface IQrCodeRepository
{
    Task<QrCode> Create(QrCode qrCode, CancellationToken cancellationToken = default);
    Task<QrCode?> GetById(Guid qrCodeId, CancellationToken cancellationToken = default);
    Task<IEnumerable<QrCode>> GetByUserId(Guid userId, CancellationToken cancellationToken = default);
}
