namespace QrGenerator.Application.QrCodes;

public class QrCodeResponse
{
    public Guid QrCodeId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int GuestCount { get; set; }
    public string DoorLockPassword { get; set; } = null!;
    public string QrCodeImageBase64 { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public string DataType { get; set; } = null!;
}
