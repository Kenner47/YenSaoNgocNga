namespace ProductManagement_API.Models.DTOs
{
    public class CategoryResponseDto
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public int ProductCount { get; set; }
    }
}
