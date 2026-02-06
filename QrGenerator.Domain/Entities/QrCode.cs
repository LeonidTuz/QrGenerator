namespace QrGenerator.Domain.Entities;

public class QrCode
{
    public Guid QrCodeId { get; set; }
    public Guid UserId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int GuestCount { get; set; }
    public string DoorLockPassword { get; set; } = null!;
    public string QrCodeData { get; set; } = null!;
    public string QrCodeImageBase64 { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public string DataType { get; set; } = null!;
    public User User { get; set; } = null!;
}
