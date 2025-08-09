namespace ProductManagement_API.Models.DTOs
{
    public class ProductResponseDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string ProductType { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public int StockQuantity { get; set; }
        public string StockStatus { get; set; } = string.Empty;
    }
}
