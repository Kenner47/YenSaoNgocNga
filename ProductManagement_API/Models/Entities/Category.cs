using System.ComponentModel.DataAnnotations;

namespace ProductManagement_API.Models.Entities
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Tên danh mục không được để trống")]
        [StringLength(100, ErrorMessage = "Tên danh mục không được quá 100 ký tự")]
        public string CategoryName { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Mô tả không được quá 500 ký tự")]
        public string Description { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public List<Product> Products { get; set; } = new();

        // Helper Properties
        public int ProductCount => Products?.Count(p => p.IsActive) ?? 0;
        public bool HasProducts => ProductCount > 0;
    }
}