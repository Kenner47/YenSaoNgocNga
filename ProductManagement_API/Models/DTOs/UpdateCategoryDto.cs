using System.ComponentModel.DataAnnotations;

namespace ProductManagement_API.Models.DTOs
{
    public class UpdateCategoryDto
    {
        [StringLength(100, ErrorMessage = "Tên danh mục không được quá 100 ký tự")]
        public string? CategoryName { get; set; }

        [StringLength(500, ErrorMessage = "Mô tả không được quá 500 ký tự")]
        public string? Description { get; set; }

        public bool? IsActive { get; set; }
    }
}
