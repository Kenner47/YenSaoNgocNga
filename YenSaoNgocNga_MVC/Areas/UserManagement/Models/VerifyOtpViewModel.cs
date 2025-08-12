using System.ComponentModel.DataAnnotations;

namespace YenSaoNgocNga_MVC.Areas.UserManagement.Models
{
    public class VerifyOtpViewModel
    {
        [Required]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mã OTP không được để trống")]
        [StringLength(6, MinimumLength = 6, ErrorMessage = "Mã OTP phải có 6 số")]
        [Display(Name = "Mã OTP")]
        public string Otp { get; set; } = string.Empty;
    }
}