using System.ComponentModel.DataAnnotations;

namespace ProductManagement_API.Models.DTOs
{
    public class CreateProductDto
    {
        [Required(ErrorMessage = "Tên sản phẩm không được để trống")]
        [StringLength(200, ErrorMessage = "Tên sản phẩm không được quá 200 ký tự")]
        public string ProductName { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "Mô tả không được quá 1000 ký tự")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Giá sản phẩm không được để trống")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Giá phải lớn hơn 0")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Danh mục không được để trống")]
        public int CategoryId { get; set; }

        [StringLength(50)]
        public string ProductType { get; set; } = string.Empty;

        [StringLength(100)]
        public string Origin { get; set; } = string.Empty;

        [Range(1, 10000, ErrorMessage = "Trọng lượng phải từ 1-10000 gram")]
        public int Weight { get; set; }

        [StringLength(10)]
        public string Grade { get; set; } = string.Empty;

        [Url(ErrorMessage = "URL hình ảnh không hợp lệ")]
        [StringLength(500)]
        public string ImageUrl { get; set; } = string.Empty;

        public int InitialStock { get; set; } = 0;
    }
}
