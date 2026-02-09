using QrGenerator.Application.QrCodes;

namespace QrGenerator.Application.Interfaces;

public interface IQrCodeService
{
    Task<QrCodeResponse> CreateQrCode(Guid userId, CreateQrCodeRequest request, CancellationToken ct);
    Task<QrCodeResponse?> GetQrCodeById(Guid qrCodeId, Guid userId, CancellationToken ct);
    Task<IEnumerable<QrCodeResponse>> GetUserQrCodes(Guid userId, CancellationToken ct);
}
