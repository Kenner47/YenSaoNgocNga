using System.ComponentModel.DataAnnotations;

namespace UserManagement_API.Models.DTOs
{
    public class LogoutDto
    {
        [Required]
        public int UserId { get; set; }
    }
}
