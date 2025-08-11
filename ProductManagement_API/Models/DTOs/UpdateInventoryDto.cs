using System.ComponentModel.DataAnnotations;

namespace ProductManagement_API.Models.DTOs
{
    public class UpdateInventoryDto
    {
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Số lượng không được âm")]
        public int Quantity { get; set; }

        [Range(0, 1000, ErrorMessage = "Ngưỡng cảnh báo phải từ 0-1000")]
        public int? MinStockLevel { get; set; }

        [Required]
        [StringLength(1000)]
        public string Notes { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string UpdatedBy { get; set; } = string.Empty;
    }
}
