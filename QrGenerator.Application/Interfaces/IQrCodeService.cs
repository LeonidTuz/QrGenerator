using QrGenerator.Application.QrCodes;

namespace QrGenerator.Application.Interfaces;

public interface IQrCodeService
{
    Task<QrCodeResponse> CreateQrCode(Guid userId, CreateQrCodeRequest request, CancellationToken cancellationToken = default);
    Task<QrCodeResponse?> GetQrCodeById(Guid qrCodeId, CancellationToken cancellationToken = default);
    Task<IEnumerable<QrCodeResponse>> GetUserQrCodes(Guid userId, CancellationToken cancellationToken = default);
}
