using System.ComponentModel.DataAnnotations;

namespace UserManagement_API.Models.DTOs
{
    public class ChangePasswordDto
    {
        [Required]
        public string CurrentPassword { get; set; }

        [Required]
        [StringLength(255, MinimumLength = 6)]
        public string NewPassword { get; set; }

        [Required]
        [Compare("NewPassword")]
        public string ConfirmPassword { get; set; }
    }
}
