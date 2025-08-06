using System.ComponentModel.DataAnnotations;

namespace UserManagement_API.Models.DTOs
{
    public class VerifyOtpDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(6, MinimumLength = 6, ErrorMessage = "OTP phải có đúng 6 chữ số")]
        [RegularExpression("^[0-9]{6}$", ErrorMessage = "OTP chỉ được chứa số")]
        public string Otp { get; set; }
    }
}