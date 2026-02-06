using System.ComponentModel.DataAnnotations;

namespace QrGenerator.Application.QrCodes;

public class CreateQrCodeRequest
{
    [Required]
    public DateTime StartDate { get; set; }

    [Required]
    public DateTime EndDate { get; set; }

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Количество гостей должно быть больше 0")]
    public int GuestCount { get; set; }

    [Required]
    [MinLength(4, ErrorMessage = "Пароль должен содержать минимум 4 символа")]
    public string DoorLockPassword { get; set; } = null!;
}
