using QrGenerator.Application.Interfaces;
using QRCoder;

namespace QrGenerator.Infrastructure.QrCodes;

public class QrCodeGenerator : IQrCodeGenerator
{
    public string GenerateQrCodeImageBase64(string data)
    {
        using var qrGenerator = new QRCodeGenerator();
        var qrCodeData = qrGenerator.CreateQrCode(data, QRCodeGenerator.ECCLevel.Q);
        using var qrCode = new PngByteQRCode(qrCodeData);
        var qrCodeBytes = qrCode.GetGraphic(20);
        return Convert.ToBase64String(qrCodeBytes);
    }
}
