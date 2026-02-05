using System.ComponentModel.DataAnnotations;

namespace QrGenerator.Application.Auth;

public class LoginByPhoneRequest
{
    [Required]
    [Phone]
    public string PhoneNumber { get; set; } = null!;
}