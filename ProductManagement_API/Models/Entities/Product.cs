using System.ComponentModel.DataAnnotations;

namespace ProductManagement_API.Models.Entities
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [Required]
        [StringLength(200)]
        public string ProductName { get; set; } = string.Empty;

        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Giá phải lớn hơn 0")]
        public decimal Price { get; set; }

        [Required]
        [StringLength(100)]
        public string ProductType { get; set; } = string.Empty; // yến Thô, Chưng, Tinh chế...

        [StringLength(100)]
        public string Origin { get; set; } = string.Empty; // Khánh Hòa, Ninh Thuận...

        [StringLength(10)]
        public string Grade { get; set; } = string.Empty; // Hạng: A, B, C...

        [Required]
        [Range(1, 10000, ErrorMessage = "Trọng lượng phải từ 1-10000 gram")]
        public int Weight { get; set; } // gram

        public string ImageUrl { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public Category Category { get; set; } = null!;
        public Inventory? Inventory { get; set; } // 1-1 relationship
    }
}