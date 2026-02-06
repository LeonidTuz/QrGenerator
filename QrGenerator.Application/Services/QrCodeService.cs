using System.Text.Json;
using QrGenerator.Application.Interfaces;
using QrGenerator.Application.QrCodes;
using QrGenerator.Domain.Entities;

namespace QrGenerator.Application.Services;

public class QrCodeService : IQrCodeService
{
    private readonly IQrCodeRepository _qrCodeRepository;
    private readonly IQrCodeGenerator _qrCodeGenerator;

    public QrCodeService(IQrCodeRepository qrCodeRepository, IQrCodeGenerator qrCodeGenerator)
    {
        _qrCodeRepository = qrCodeRepository;
        _qrCodeGenerator = qrCodeGenerator;
    }

    public async Task<QrCodeResponse> CreateQrCode(Guid userId, CreateQrCodeRequest request, CancellationToken cancellationToken = default)
    {
        if (request.StartDate >= request.EndDate)
        {
            throw new ArgumentException("Дата начала должна быть раньше даты окончания.");
        }

        var qrData = new
        {
            StartDate = request.StartDate.ToString("yyyy-MM-dd HH:mm:ss"),
            EndDate = request.EndDate.ToString("yyyy-MM-dd HH:mm:ss"),
            GuestCount = request.GuestCount,
            DoorLockPassword = request.DoorLockPassword
        };

        var jsonData = JsonSerializer.Serialize(qrData);
        var qrCodeImageBase64 = _qrCodeGenerator.GenerateQrCodeImageBase64(jsonData);

        var qrCode = new QrCode
        {
            QrCodeId = Guid.NewGuid(),
            UserId = userId,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            GuestCount = request.GuestCount,
            DoorLockPassword = request.DoorLockPassword,
            QrCodeData = jsonData,
            QrCodeImageBase64 = qrCodeImageBase64,
            CreatedAt = DateTime.UtcNow,
            DataType = "CheckInData"
        };

        var savedQrCode = await _qrCodeRepository.Create(qrCode, cancellationToken);

        return MapToResponse(savedQrCode);
    }

    public async Task<QrCodeResponse?> GetQrCodeById(Guid qrCodeId, CancellationToken cancellationToken = default)
    {
        var qrCode = await _qrCodeRepository.GetById(qrCodeId, cancellationToken);
        return qrCode == null ? null : MapToResponse(qrCode);
    }

    public async Task<IEnumerable<QrCodeResponse>> GetUserQrCodes(Guid userId, CancellationToken cancellationToken = default)
    {
        var qrCodes = await _qrCodeRepository.GetByUserId(userId, cancellationToken);
        return qrCodes.Select(MapToResponse);
    }

    private static QrCodeResponse MapToResponse(QrCode qrCode)
    {
        return new QrCodeResponse
        {
            QrCodeId = qrCode.QrCodeId,
            StartDate = qrCode.StartDate,
            EndDate = qrCode.EndDate,
            GuestCount = qrCode.GuestCount,
            DoorLockPassword = qrCode.DoorLockPassword,
            QrCodeImageBase64 = qrCode.QrCodeImageBase64,
            CreatedAt = qrCode.CreatedAt,
            DataType = qrCode.DataType
        };
    }
}
