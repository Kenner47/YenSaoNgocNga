using System.ComponentModel.DataAnnotations;

namespace ProductManagement_API.Models.DTOs
{
    public class UpdateProductDto
    {
        [StringLength(200, ErrorMessage = "Tên sản phẩm không được quá 200 ký tự")]
        public string? ProductName { get; set; }

        [StringLength(1000, ErrorMessage = "Mô tả không được quá 1000 ký tự")]
        public string? Description { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Giá phải lớn hơn 0")]
        public decimal? Price { get; set; }

        public int? CategoryId { get; set; }

        [StringLength(50)]
        public string? ProductType { get; set; }

        [StringLength(100)]
        public string? Origin { get; set; }

        [Range(1, 10000, ErrorMessage = "Trọng lượng phải từ 1-10000 gram")]
        public int? Weight { get; set; }

        [StringLength(10)]
        public string? Grade { get; set; }

        [Url(ErrorMessage = "URL hình ảnh không hợp lệ")]
        [StringLength(500)]
        public string? ImageUrl { get; set; }

        public bool? IsActive { get; set; }
    }
}
