namespace QrGenerator.Domain.Entities;

public class User
{
    public Guid UserId { get; set; }
    public string PhoneNumber { get; set; } = null!;
    public int UserTelegramId { get; set; }
}