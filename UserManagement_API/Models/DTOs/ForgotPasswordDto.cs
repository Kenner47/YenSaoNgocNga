using System.ComponentModel.DataAnnotations;

namespace UserManagement_API.Models.DTOs
{
    public class ForgotPasswordDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
