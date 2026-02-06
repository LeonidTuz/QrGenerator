namespace QrGenerator.Application.Interfaces;

public interface IQrCodeGenerator
{
    string GenerateQrCodeImageBase64(string data);
}
