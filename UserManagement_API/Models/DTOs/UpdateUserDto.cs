using System.ComponentModel.DataAnnotations;

namespace UserManagement_API.Models.DTOs
{
    public class UpdateUserDto
    {
        [Required]
        [StringLength(100)]
        public string FullName { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        public string? PhoneNumber { get; set; }

        public string? Address { get; set; }

        public DateOnly? DateOfBirth { get; set; }

        public string? Sex { get; set; }
    }
}
