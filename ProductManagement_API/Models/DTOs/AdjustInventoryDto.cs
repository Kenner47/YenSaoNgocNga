using System.ComponentModel.DataAnnotations;

namespace ProductManagement_API.Models.DTOs
{
    public class AdjustInventoryDto
    {
        [Required]
        public int Adjustment { get; set; } // Có thể âm (giảm) hoặc dương (tăng)

        [Required]
        [StringLength(1000)]
        public string Reason { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string UpdatedBy { get; set; } = string.Empty;
    }
}
