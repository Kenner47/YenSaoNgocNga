namespace YenSaoNgocNga_MVC.Areas.ProductManagement.Models
{
    public class ProductViewModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string ProductType { get; set; } = string.Empty;
        public string Origin { get; set; } = string.Empty;
        public string Grade { get; set; } = string.Empty;
        public int Weight { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        public DateTime CreatedAt { get; set; }

        // Inventory Info
        public int StockQuantity { get; set; }
        public string StockStatus { get; set; } = string.Empty;

        // Formatted Properties
        public string FormattedPrice => $"{Price:N0}₫";
        public string FormattedWeight => $"{Weight}g";
    }
}